using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIConveyorGearSelectCtrl : UIBaseSelectCtrl
{
    private Tween tween;
    private RectTransform rect;
    private AudioSource temp;
    //private RectTransform rects;
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
            if (temp == null)
            {
                temp = AudioManager.Instance.Play(GetComponentInChildren<TextMeshProUGUI>().text);
            }
            else
            {
                if (!temp.isPlaying)
                {
                    temp.Play();                 
                }
            }
          
          
            if (!UIConveyor.Instance.GraphEffect(gameObject).IsPlaying())
            {
                UIConveyor.Instance.GraphEffect(gameObject);               
            }
        };
        // UGUIEventListener.Get(gameObject).on
    }

    protected override void RighMatchAction()
    {
        m_rigidbody2d.constraints = RigidbodyConstraints2D.FreezeAll;
        Destroy(UIConveyor.Instance.goGuideHand);
        Destroy(transform.gameObject);//直接销毁
        UIConveyor.Instance.Stop();
        if (UIConveyor.Instance.isfirstPlay)
        {
            if (UIConveyor.Instance.LetterIndex == 1)
            {
                UIConveyor.Instance.isfirstPlay = false;
                UIConveyor.Instance.Secondchar();
            }
        }
        //transform.localScale = Vector3.zero;
        var cp = m_cacheCollider.GetComponentInParent<UIConveyorClamp>();
        cp.SetGearNormal();
        UIConveyor.Instance.AddMatchNum();
        AudioManager.Instance.Play("SoundBeltEffect");//吸附声音
    }

    protected override void WrongMatchAction()
    {
    }

    protected override bool GetMatch()
    {
        var letter = m_cacheCollider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        int Index = int.Parse(m_cacheCollider.transform.GetChild(1).GetComponent<Text>().text);//当前的所有

        string s = UIConveyor.Instance.CurrentWord.word.Replace(" ", "");
        string strCurrentSpread = UIConveyor.Instance.GetCurrentWord(s, UIConveyor.Instance.LetterIndex);

        var currentLetter = GetComponentInChildren<TextMeshProUGUI>();

        if (letter != null && strCurrentSpread != null && strCurrentSpread == letter.text)
        {
            if (currentLetter.text.Equals(letter.text) && Index == UIConveyor.Instance.LetterIndex)
            {
                UIConveyor.Instance.LetterIndex++;
                UIConveyor.Instance.SetCurrentIndx(UIConveyor.Instance.LetterIndex);
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

    public void IsFirst(Transform t)
    {
        StartCoroutine(IsFirstIE(t));
    }
    private void TeenGuiderMove(Transform goGuiderParent, Transform t)
    {
        tween = goGuiderParent.DOMove(t.position, 3f).SetEase(Ease.Linear).OnComplete(
            delegate
            {
                tween.Kill();
                rect.anchoredPosition = Vector3.zero;
                TeenGuiderMove(goGuiderParent.transform,t);
            });
    }

    IEnumerator IsFirstIE(Transform t)
    {
        int time = 0;
        if (UIConveyor.Instance.LetterIndex == 0)
        {
            time = 4;
        }
        yield return new WaitForSeconds(time);
        UIConveyor.Instance.goGuideHand = CreateUIItem("GuideHandMoveItem", transform);
        rect = UIConveyor.Instance.goGuideHand.transform as RectTransform;
        //rects = transform as RectTransform;
        rect.anchoredPosition = Vector3.zero;
        UIConveyor.Instance.StartActive(t);
        TeenGuiderMove(UIConveyor.Instance.goGuideHand.transform,t);
    }

}
