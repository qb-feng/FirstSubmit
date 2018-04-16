using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UITrainWordCommonItem : UIBaseInit
{
    private Image TrainHead { get { return Get<Image>("TrainHead"); } }//车头
    private RectTransform Train { get { return GetR("Train"); } }//火车整体
    private GameObject SmokeFX { get { return Get("SmokeFX"); } }//烟囱
    public static string[] s_trainColor = { "Red", "Green" };//火车的颜色
    private string m_trainColorPrefix = "UITrainWord";//颜色前缀
    private GameObject TrainCarriageHorizontal { get { return Get("TrainCarriageHorizontal"); } }//车厢位置
    /// <summary>
    /// 车厢挂点
    /// </summary>
    private string TrainCarriagePos = "TrainCarriagePos";
    /// <summary>
    /// 车厢单词对象名
    /// </summary>
    private const string TrainWord = "TrainWord";
    /// <summary>
    /// 火车的预制体图片名称
    /// </summary>
    private const string TrainBox = "TrainBox";
    /// <summary>
    /// 火车底盘
    /// </summary>
    private const string TrainWheels = "TrainWheels";

    private RectTransform UpTrainBoxes { get { return GetR("UpTrainBoxes"); } }//上面车厢对象

    private RectTransform DownTrainBoxes { get { return GetR("DownTrainBoxes"); } }//下面面车厢对象

    //private string currentSentence = "";//当前火车对象的句子

    private string[] words = null;//当前句子对应的单词数组

    private int m_trainIndex = 0;
    private int m_matchTrainBoxCount = 0;
    private static int s_matchTrainCount = 0;


    UITrainWordCommonItem trainFrist;
    UITrainWordCommonItem trainSecond;
    /// <summary>
    /// 火车上句子的音频
    /// </summary>
    private string m_audioSound;

    //初始化火车
    /// <summary>
    /// 初始化火车
    /// </summary>
    /// <param name="colorIndex">颜色</param>
    /// <param name="tweenDelay">间隔时间</param>
    /// <param name="currentIndex">当前索引</param>
    /// <param name="sentence">句子</param>
    /// <param name="deviation">矫正位置大小</param>
    public void Init(int colorIndex, float tweenDelay, int currentIndex, string sentence, float deviation = 0)
    {
        s_matchTrainCount = 0;

        m_trainIndex = colorIndex + 1;
        //currentSentence = sentence;//赋值句子
        words = sentence.Split('_');//把当前句子拆分为词组数组
        //打乱顺序上下车厢单词赋值
        string[] rondomWords = Utility.RandomSortList(words);
        string prefix = m_trainColorPrefix + s_trainColor[colorIndex];//火车部件前缀
        TrainHead.sprite = GetS(prefix + 1);//车头的图片名称火车头前缀索引为1
        for (int i = 0; i < words.Length; i++)
        {
            //创建上部的车厢
            GameObject goUpTrainBoxes = CreateUIItem("UITrainWordCommonBoxPrefab", UpTrainBoxes);
            //车厢的图片名称索引为3
            goUpTrainBoxes.transform.Find(TrainBox).GetComponent<Image>().sprite = GetS(prefix + 3);
            //层级
            goUpTrainBoxes.layer = i + 1;
            //给上部火车随机赋值单词
            goUpTrainBoxes.transform.GetChild(0).Find(TrainWord).GetComponent<TextMeshProUGUI>().text = rondomWords[i];
            //创建火车底盘
            GameObject goTrainChassis = CreateUIItem("UITrainWordCommonCarriage", Get(TrainCarriagePos + i).transform);
            //底盘图片名称索引为2
            goTrainChassis.transform.Find(TrainWheels).GetComponent<Image>().sprite = GetS(prefix + 2);
            //底盘上的车厢
            GameObject goTrainBox = CreateUIItem("UITrainWordCommonBoxPrefab", goTrainChassis.transform.transform);
            goTrainBox.transform.Find(TrainBox).GetComponent<Image>().sprite = GetS(prefix + 3);
            //goTrainBox.transform.gameObject.AddComponent<BoxCollider2D>();
            GameObject go = goTrainBox.transform.GetChild(0).GetComponent<BoxCollider2D>().gameObject;
            GameObject go1 = goTrainBox.transform.GetChild(0).GetComponent<Rigidbody2D>().gameObject;
            //Destroy(goTrainBox.transform.GetComponentInChildren<BoxCollider2D>().gameObject);
            //Destroy(goTrainBox.transform.GetComponentInChildren<Rigidbody2D>().gameObject);
            //给车厢赋值单词
            goTrainBox.transform.GetChild(0).Find(TrainWord).GetComponent<TextMeshProUGUI>().text = words[i];
            //创建底部车厢
            GameObject goDownTrainBoxes = CreateUIItem("UITrainWordCommonBoxPrefab", DownTrainBoxes);
            //车厢的图片名称索引为3
            goUpTrainBoxes.transform.Find(TrainBox).GetComponent<Image>().sprite = GetS(prefix + 3);
            goUpTrainBoxes.layer = i + 1;
            goDownTrainBoxes.transform.GetChild(0).Find(TrainWord).GetComponent<TextMeshProUGUI>().text = rondomWords[i];
        }
        UpTrainBoxActive(false);
        DownTrainBoxActive(false);

        //if (currentIndex >= words.Length)
        //{
        //    TrainBoxActive(false);
        //}
        TweenCome(tweenDelay, deviation).OnStart(() =>
        {
            AudioManager.Instance.Play("train_whistle");
        }).OnComplete(() =>
        {
            if (currentIndex < UITrainWordCommon.Instance.currentGamewordCount)
            {
                TweenLeave(4f).OnComplete(() =>
                {
                    TweenCome(2f, deviation).OnStart(() =>
                    {
                        SmokeFX.SetActive(true);
                        AudioManager.Instance.Play("train_whistle");
                    }).OnComplete(() =>
                    {
                        UpTrainBoxActive(m_trainIndex == 1);
                        DownTrainBoxActive(m_trainIndex != 1);
                    });
                    TrainBoxActive(false);
                    SmokeFX.SetActive(false);
                });
                TweenTrainWord(1f).OnStart(() =>
                {
                    UITrainWordCommon.Instance.ImageWord.transform.DOScale(1.5f, 0.5f).SetLoops(2, LoopType.Yoyo);
                    AudioManager.Instance.PlayAppend(m_audioSound);
                });
            }
            else
            {
                UpTrainBoxActive(m_trainIndex == 1);
                DownTrainBoxActive(m_trainIndex != 1);
            }
        });
    }
    /// <summary>
    /// 火车来
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="deviation">偏移量矫正位置</param>
    /// <returns></returns>
    public Tweener TweenCome(float delay, float deviation = 0)
    {
        float y = Train.anchoredPosition.y;
        Train.anchoredPosition = new Vector2(1900, y);
        Tweener tween = Train.DOAnchorPosX(deviation, 2f);
        tween.SetEase(Ease.OutCubic);
        tween.SetDelay(delay);
        return tween;
    }
    /// <summary>
    /// 火车离开
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    public Tweener TweenLeave(float delay)
    {
        Tweener tween = Train.DOAnchorPosX(-1900, 2f);
        tween.SetEase(Ease.InCubic);
        tween.SetDelay(delay);
        return tween;
    }
    /// <summary>
    /// 这只火车车厢的单词及音频
    /// </summary>
    /// <param name="sentence">句子</param>
    /// <param name="punctuation">标点符号</param>
    /// <param name="audio">音频名</param>
    internal void SetTrainWord(string audio)
    {
        m_audioSound = audio;//给当前车厢的音频赋值
    }


    /// <summary>
    /// 火车上的车厢是否可见
    /// </summary>
    /// <param name="active"></param>
    public void TrainBoxActive(bool active)
    {
        for (int i = 0; i < words.Length; i++)
        {
            SetAlpha(Get(TrainCarriagePos + i).transform.GetChild(0).GetChild(2).Find(TrainBox).gameObject, active ? 1 : 0);
        }
    }

    /// <summary>
    /// 设置上部车厢的可见性
    /// </summary>
    /// <param name="active"></param>
    public void UpTrainBoxActive(bool active)
    {
        UpTrainBoxes.gameObject.SetActive(active);
    }

    /// <summary>
    /// 设置下部车厢的可见性
    /// </summary>
    /// <param name="active"></param>
    public void DownTrainBoxActive(bool active)
    {
        DownTrainBoxes.gameObject.SetActive(active);
    }

    /// <summary>
    /// 设置透明
    /// </summary>
    /// <param name="go"></param>
    /// <param name="a"></param>
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
    /// <summary>
    /// 车厢的缩放动画
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private Tweener TweenTrainWord(float delay)
    {
        Tweener tween = null;
        for (int i = 0; i < words.Length; i++)
        {
            tween = Get(TrainCarriagePos + i).transform.GetChild(0).GetChild(2).Find(TrainBox).Find(TrainWord).transform.DOScale(Vector3.one * 1.5f, 0.5f);
            tween.SetDelay(delay);
            tween.SetLoops(2, LoopType.Yoyo);
        }
        return tween;
    }

    public void MatchAdd()
    {
        m_matchTrainBoxCount++;
        if (m_matchTrainBoxCount == words.Length)
        {
            AudioManager.Instance.Play(m_audioSound);
            TweenTrainWord(0).OnComplete(() =>
            {
                UITrainWordCommon.Instance.FlyStar(UITrainWordCommon.Instance.CurrentAsk.id, true);//传句子ID
            });
            s_matchTrainCount++;
            if (s_matchTrainCount == 2)
            {
                trainFrist = UITrainWordCommon.Instance.TrainPosition1.GetChild(0).GetChild(1).GetComponentInParent<UITrainWordCommonItem>();
                trainFrist.TweenLeave(0).OnComplete(() =>
                {
                    trainSecond = UITrainWordCommon.Instance.TrainPosition2.GetChild(0).GetChild(1).GetComponentInParent<UITrainWordCommonItem>();
                    trainSecond.TweenLeave(0).OnComplete(() =>
                    {
                        //trainSecond.TweenLeave(2);
                        UITrainWordCommon.Instance.PlayGame();
                        s_matchTrainCount = 0;
                    });
                });
            }


        }
        Debug.Log("*****Current Match Count is :" + m_matchTrainBoxCount);
    }
}
