using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UITrainWordItemNumAsk : UIBaseInit
{
    private string m_trainBoxPrefix = "TrainBox";
    private string m_trainWheelsPrefix = "TrainWheels";
    private string m_trainColorPrefix = "UITrainWord";
    public static string[] s_trainColor = { "Red", "Green" };
    private string m_upTrainBoxPrefix = "UpTrainBox";
    //private string m_downTrainBoxNumPrefix = "DownTrainBoxNum";
    private string m_trainWordPrefix = "TrainWord";
    private string m_upTrainWordPrefix = "UpTrainWord";
    //private string m_downTrainWordPrefix = "DownTrainWord";
    private Image TrainHead { get { return Get<Image>("TrainHead"); } }
    private RectTransform Train { get { return GetR("Train"); } }
    private GameObject SmokeFX { get { return Get("SmokeFX"); } }

    private int m_trainIndex = 0;
    public static int m_matchTrainBoxCount = 0;
    public static int s_matchTrainCount = 0;
    private string m_positiveSound;

    UITrainWordItemNumAsk trainFrist;
    UITrainWordItemNumAnswer trainSecond;
    public void Init(int colorIndex, float tweenDelay, int currentIndex)
    {
        m_trainIndex = colorIndex + 1;
        string prefix = m_trainColorPrefix + s_trainColor[colorIndex];
        string boxPrefix = "UITrainWordNum" + s_trainColor[colorIndex];
        TrainHead.sprite = GetS(prefix + 1);
        for (int i = 1; i < 5; i++)
        {
            Get<Image>(m_trainBoxPrefix + i).sprite = GetS(boxPrefix + 3);
            Get<Image>(m_trainWheelsPrefix + i).sprite = GetS(prefix + 2);
            Get<Image>(m_upTrainBoxPrefix + i).sprite = GetS(boxPrefix + 3);
        }
        UpTrainBoxActive(false);
        if (currentIndex >= 4)
        {
            TrainBoxActive(false);
        }
        TweenCome(tweenDelay).OnStart(() =>
        {
            AudioManager.Instance.Play("train_whistle");
        }).OnComplete(() =>
        {
            if (currentIndex < 4)
            {
                TweenLeave(4f).OnComplete(() =>
                {
                    TweenCome(2f).OnStart(() =>
                    {
                        SmokeFX.SetActive(true);
                        AudioManager.Instance.Play("train_whistle");
                    }).OnComplete(() =>
                    {
                        UpTrainBoxActive(m_trainIndex == 1);
                    });
                    TrainBoxActive(false);
                    SmokeFX.SetActive(false);
                });
                TweenTrainWord(1f).OnStart(() =>
                {
                    UITrainWordNumber.Instance.ImageWord.transform.DOScale(1.5f, 0.5f).SetLoops(2, LoopType.Yoyo);
                    AudioManager.Instance.Play(m_positiveSound);
                });
            }
            else
            {
                UpTrainBoxActive(m_trainIndex == 1);
            }
        });
    }
    public void UpTrainBoxActive(bool active)
    {
        for (int i = 0; i < 4; i++)
        {
            Get(m_upTrainBoxPrefix + (i + 1)).SetActive(active);
        }
    }

    public void SetAlpha(GameObject go, float a)
    {
        Graphic[] cps = go.GetComponentsInChildren<Graphic>();
        foreach (Graphic cp in cps)
        {
            Color color = cp.color;
            color.a = a;
            cp.color = color;
        }
    }

    public void TrainBoxActive(bool active)
    {
        for (int i = 0; i < 4; i++)
        {
            SetAlpha(Get(m_trainBoxPrefix + (i + 1)), active ? 1 : 0);
        }
    }


    public Tweener TweenCome(float delay)
    {
        float y = Train.anchoredPosition.y;
        Train.anchoredPosition = new Vector2(1900, y);
        Tweener tween = Train.DOAnchorPosX(0, 2f);
        tween.SetEase(Ease.OutCubic);
        tween.SetDelay(delay);
        return tween;
    }

    public Tweener TweenLeave(float delay)
    {
        Tweener tween = Train.DOAnchorPosX(-1900, 2f);
        tween.SetEase(Ease.InCubic);
        tween.SetDelay(delay);
        return tween;
    }



    private Tweener TweenTrainWord(float delay)
    {
        Tweener tween = null;
        for (int i = 0; i < 4; i++)
        {
            tween = GetT(m_trainWordPrefix + (i + 1)).DOScale(Vector3.one * 1.5f, 0.5f);
            tween.SetDelay(delay);
            tween.SetLoops(2, LoopType.Yoyo);
        }
        return tween;
    }
    public void SetTrainWordAsk(string sentence, string positive)
    {
        m_positiveSound = positive;
        string[] words = sentence.Split('_');
        for (int i = 0; i < words.Length; i++)
        {
            Get<TextMeshProUGUI>(m_trainWordPrefix + (i + 1)).text = words[i];
        }
        words = Utility.RandomSortList(words);
        for (int i = 0; i < words.Length; i++)
        {
            Get<TextMeshProUGUI>(m_upTrainWordPrefix + (i + 1)).text = words[i];
        }
    }



    /// <summary>
    /// 时间:2016年6月16日15:22:20
    /// 作者:白建新
    /// 描述:修改两列火车的句子都拼写完成后 再开走(问句火车先开走)
    /// </summary>
    public void MatchAdd()
    {
        m_matchTrainBoxCount++;
        if (m_matchTrainBoxCount == 4)
        {
            AudioManager.Instance.Play(m_positiveSound);
            TweenTrainWord(0).OnComplete(() =>
            {
                UITrainWordNumber.Instance.FlyStar(UITrainWordNumber.Instance.CurrentAsk.id, true);
            });
            s_matchTrainCount++;
            if (UITrainWordItemNumAnswer.s_matchTrainCount == 1 && s_matchTrainCount == 1)
            {
                trainFrist = UITrainWordNumber.Instance.TrainPosition1.GetChild(0).GetChild(1).GetComponentInParent<UITrainWordItemNumAsk>();
                trainFrist.TweenLeave(0).OnComplete(() =>
                {
                    trainSecond = UITrainWordNumber.Instance.TrainPosition2.GetChild(0).GetChild(1).GetComponentInParent<UITrainWordItemNumAnswer>();
                    trainSecond.TweenLeave(0).OnComplete(() =>
                    {
                        UITrainWordItemNumAsk.s_matchTrainCount = 0;
                        UITrainWordItemNumAnswer.s_matchTrainCount = 0;

                        UITrainWordItemNumAsk.m_matchTrainBoxCount = 0;
                        UITrainWordItemNumAnswer.m_matchTrainBoxCount = 0;

                        UITrainWordNumber.Instance.PlayGame();
                    });
                });
            }
        }
        Debug.Log("*****Current Match Count is :" + m_matchTrainBoxCount);
    }


}
