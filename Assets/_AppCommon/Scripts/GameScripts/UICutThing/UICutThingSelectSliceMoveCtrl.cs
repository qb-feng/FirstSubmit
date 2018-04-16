using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICutThingSelectSliceMoveCtrl : UIBaseSelectCtrl
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
        Destroy(GetComponent<UGUIEventOnPointerDown>());
        Destroy(GetComponent<UGUIEventOnPointerUp>());
        m_isKinematics = false;
    }
    protected override void RighMatchAction()
    {
        gameObject.SetActive(false);
        var cp = m_cacheCollider.GetComponentInParent<UICutThingSlice>();
        cp.TweenMelonAlpha(1);        
        cp.TweenLetterAlpha(1);
        m_cacheCollider.enabled = false;
        var letter = GetComponentInChildren<TextMeshProUGUI>().text;
        AudioManager.Instance.Play(letter);
        //等单词发音完成
        cp.WaitSecond(() =>
        {
            UICutThing.Instance.AddMatchLetterCount();
        }, 1f);

    }

    protected override void WrongMatchAction()
    {
    }

    protected override bool GetMatch()
    {
        var parentCp = GetComponentInParent<UICutThingSlice>();
        if (parentCp.m_random)
        {
            //TextMeshProUGUI target = m_cacheCollider.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI target = m_cacheCollider.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            int Index = int.Parse(m_cacheCollider.transform.GetChild(2).GetComponent<Text>().text);//当前的所有
            TextMeshProUGUI select = GetComponentInChildren<TextMeshProUGUI>();
            //
            string strCurrentSpread = UICutThing.Instance.GetCurrentWord(UICutThing.Instance.CurrentWord.word, UICutThing.Instance.wallWordIndex);

            if (target != null && select != null && strCurrentSpread == target.text)
            {
                if (target.text.Equals(select.text) && Index == UICutThing.Instance.wallWordIndex)
                {
                    UICutThing.Instance.wallWordIndex++;
                    UICutThing.Instance.SetCurrentIndx(UICutThing.Instance.wallWordIndex);
                    return true;
                }
            }

        }
        return false;
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
