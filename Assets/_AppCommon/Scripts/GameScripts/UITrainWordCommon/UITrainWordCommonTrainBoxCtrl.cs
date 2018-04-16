using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UITrainWordCommonTrainBoxCtrl : UIBaseSelectCtrl
{
    private UITrainWordCommonItem m_parentItem;
    //Vector2 vc;
    Vector3 m_originalPos;
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        m_isKinematics = false;
        UGUIEventListener.Get(gameObject).onBeginDrag += d =>
        {
            //vc = d.position;
        };
        m_parentItem = GetComponentInParent<UITrainWordCommonItem>();
        m_originalPos = transform.localPosition;
    }

    protected override bool GetMatch()
    {
        TextMeshProUGUI current = GetComponentInChildren<TextMeshProUGUI>(true);
        TextMeshProUGUI matched = m_cacheCollider.GetComponentInChildren<TextMeshProUGUI>(true);
        if (matched && current)
            return current.text.Equals(matched.text) && GetComponent<Image>().sprite.name.Equals(m_cacheCollider.GetComponentInChildren<Image>(true).sprite.name);
        return false;

    }

    protected override void RighMatchAction()
    {
        m_cacheCollider.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.SetActive(false);
        AudioManager.Instance.Play("TrainLoading");//火车装货声音
        m_parentItem.MatchAdd();
        m_parentItem.SetAlpha(m_cacheCollider.gameObject, 1);
    }

    protected override void WrongMatchAction()
    {
        //gameObject.transform.position = vc;
    }

    /// <summary>
    /// 重写错误方法
    /// </summary>
    protected override void WrongRigibodyTween()
    {
        transform.DOLocalMove(m_originalPos, 0.4f).OnComplete(() =>
        {
            GetComponent<Collider2D>().enabled = true;
        });
    }
}
