using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIRiverGearSelectCtrl : UIBaseSelectCtrl
{
    protected override bool PointUpSpeedZero
    {
        get
        {
            return false;
        }
    }

    protected override void Start()
    {
        base.Start();
        m_isKinematics = false;
        
        UGUIEventListener.Get(gameObject).onPointerDown += d =>
        {        
            AudioManager.Instance.Play(GetComponentInChildren<TextMeshProUGUI>().text); 
 
            if (!UIRiver.Instance.GraphEffect(gameObject).IsPlaying())
            {
                UIRiver.Instance.GraphEffect(gameObject);
            }
        };
        // UGUIEventListener.Get(gameObject).on
    }

    protected override void RighMatchAction()
    {
        m_rigidbody2d.constraints = RigidbodyConstraints2D.FreezeAll;
        Destroy(transform.gameObject);//直接销毁
        var cp = m_cacheCollider.GetComponentInParent<UIRiverClamp>();
        cp.SetGearNormal();
        UIRiver.Instance.AddMatchNum();
        AudioManager.Instance.Play("AdsorptionEffect");//吸附声音

    }

    protected override void WrongMatchAction()
    {
    }

    protected override bool GetMatch()
    {
        var letter = m_cacheCollider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        int Index = int.Parse(m_cacheCollider.transform.GetChild(1).GetComponent<Text>().text);//当前的索引

        string s = UIRiver.Instance.CurrentWord.word.Replace(" ", "");
//        string strCurrentSpread = UIConveyor.Instance.GetCurrentWord(s, UIConveyor.Instance.LetterIndex);
//        string strCurrentSpread = UIRiver.Instance.GetCurrentWord(UIRiver.Instance.CurrentWord.word, UIRiver.Instance.LetterIndex);
        string strCurrentSpread = UIRiver.Instance.GetCurrentWord(s, UIRiver.Instance.LetterIndex);

        var currentLetter = GetComponentInChildren<TextMeshProUGUI>();

        if (letter != null && strCurrentSpread != null && strCurrentSpread == letter.text)
        {
            if (currentLetter.text.Equals(letter.text) && Index == UIRiver.Instance.LetterIndex)
            {
                UIRiver.Instance.LetterIndex++;
                UIRiver.Instance.SetCurrentIndx(UIRiver.Instance.LetterIndex);
                return true;
            }
        }
        return false;
    }

    protected override void WrongRigibodyTween()
    {
        //do nothing
    }


    private Tweener MoverMatch(Transform tObj)
    {
        var tween = tObj.DOMove(m_cacheCollider.transform.position, 0, true);
        return tween;
    }
    /// <summary>
    /// 进入吸附效果
    /// </summary>
    /// <param name="other"></param>
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (GetMatch())
        {
            RighMatchAction();
        }
    }

}
