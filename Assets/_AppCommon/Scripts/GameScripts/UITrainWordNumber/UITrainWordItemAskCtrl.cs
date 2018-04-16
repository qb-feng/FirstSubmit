using TMPro;
using UnityEngine.UI;

public class UITrainWordItemAskCtrl : UIBaseSelectCtrl
{
    private UITrainWordItemNumAsk m_parentItem;
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        m_isKinematics = false;
        m_parentItem = GetComponentInParent<UITrainWordItemNumAsk>();
    }

    protected override bool GetMatch()
    {
        TextMeshProUGUI current = GetComponentInChildren<TextMeshProUGUI>(true);
        TextMeshProUGUI matched = m_cacheCollider.GetComponentInChildren<TextMeshProUGUI>(true);
        return current.text.Equals(matched.text);
    }

    protected override void RighMatchAction()
    {
        gameObject.SetActive(false);
        AudioManager.Instance.Play("TrainLoading");//火车装货声音
        m_parentItem.MatchAdd();
        m_parentItem.SetAlpha(m_cacheCollider.gameObject, 1);
    }

    protected override void WrongMatchAction()
    {
    }
}
