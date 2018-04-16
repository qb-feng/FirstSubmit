using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

#region 换皮模型定义
public class UISupermanHitMonster : UISharkEatFish { }
#endregion

public class UISharkEatFish : UIBaseLevelGame
{

    #region  字段 属性 变量

    private SharkGame SharkGame { get { return Get<SharkGame>("SharkGame"); } }
    //private BirdGame SharkGame { get { return Get<BirdGame>("SharkGame"); } }
    private Image WordImage { get { return Get<Image>("ImageWord"); } }
    private TextMeshProUGUI WrongWord { get { return Get<TextMeshProUGUI>("TextWrongWord"); } }
    private TextMeshProUGUI RightWord { get { return Get<TextMeshProUGUI>("TextRightWord"); } }


    private RectTransform ImageFishWrong { get { return Get<RectTransform>("ImageFishWrong"); } }
    private RectTransform ImageFishRight { get { return Get<RectTransform>("ImageFishRight"); } }

    /// <summary>
    /// 游戏控制按钮
    /// </summary>
    public Image PlayButton { get { return Get<Image>("PlayButton"); } }

    private string m_wrongWordSound;
    private string m_rightWordSound;
    private GameObject Shark { get { return Get("Shark"); } }
    private GameObject ImageHand1 { get { return Get("ImageHand1"); } }
    private GameObject ImageHand2 { get { return Get("ImageHand2"); } }
    private GameObject HandAnimation { get { return Get("HandAnimation"); } }

    /// <summary>
    /// 背景音效
    /// </summary>
    private AudioSource m_bgSound;
    #endregion
    public static UISharkEatFish Instance { get; set; }

    //修改吃了错误单词时出现多颗星的bug
    private bool isFly = false;//是否正在飞星

    void Awake()
    {
        Instance = this;
        m_bgSound = AudioManager.Instance.Play("UISharkEatFishBgSound");
    }

    void OnDestroy()
    {
        Destroy(m_bgSound);
        Instance = null;
    }

    public override void Refresh()
    {
        StartCoroutine("WaitOneFrameStartLevel");
        UGUIEventListener.Get(WordImage).onPointerEnter = WordImagePointEnter;
        UGUIEventListener.Get(RightWord).onPointerEnter = RightWordPointEnter;
        UGUIEventListener.Get(WrongWord).onPointerEnter = WrongWordPointEnter;
        //UGUIEventListener.Get(ImageFishRight).onPointerEnter = RightWordPointEnter;
        //UGUIEventListener.Get(ImageFishWrong).onPointerEnter = WrongWordPointEnter;
    }
    /// <summary>
    /// 错误读音
    /// </summary>
    /// <param name="arg0"></param>
    private void WrongWordPointEnter(UnityEngine.EventSystems.PointerEventData arg0)
    {
        AudioManager.Instance.Play(m_wrongWordSound, false);
    }
    /// <summary>
    /// 正确读音
    /// </summary>
    /// <param name="arg0"></param>
    private void RightWordPointEnter(UnityEngine.EventSystems.PointerEventData arg0)
    {
        AudioManager.Instance.Play(m_rightWordSound, false);
    }
    /// <summary>
    /// 当前单词读音
    /// </summary>
    /// <param name="arg0"></param>
    private void WordImagePointEnter(UnityEngine.EventSystems.PointerEventData arg0)
    {
        CurrentWord.PlaySound();
    }

    public override void PlayGame()
    {
        if (IsGameEnd)
        {
            SharkGame.PauseLevel();
            return;
        }

        if (IsFirstPlay && m_currentIndex < 1)
        {
            StartCoroutine(ImageHande());
        }
        else
        {
            HandAnimation.SetActive(false);
        }
        ConfigWordLibraryModel model = CurrentWord;
        WordImage.sprite = model.sprite;
        WordImage.transform.DOScale(1.5f, 0.5f).SetLoops(2, LoopType.Yoyo);
        model.PlaySound();

        RightWord.text = model.word;
        m_rightWordSound = model.word;
        int randomWrongIndex = GetRandomCount(m_currentIndex);
        string nextWrongWord = m_randomWordList[randomWrongIndex].word;
        m_wrongWordSound = m_randomWordList[randomWrongIndex].word;
        WrongWord.text = nextWrongWord;
        //DOTween.To(() => WrongWord.text, (next) => WrongWord.text = next, nextWrongWord, 0.1f).SetDelay(2);


        //RandomWordPosition(RightWord.rectTransform);
        //RandomWordPosition(WrongWord.rectTransform);

        if (Random.Range(0, 2) == 1)
        {
            posbool = false;
        }
        RandomWordPosition(ImageFishRight);
        RandomWordPosition(ImageFishWrong);


        //RightWord.gameObject.SetActive(false);
        //WrongWord.gameObject.SetActive(false);
    }

    private bool posbool = true;
    /// <summary>
    /// 随机单词出现的位置
    /// </summary>
    /// <param name="rt"></param>
    
 private void RandomWordPosition(RectTransform rt)
    {
        Vector2 temp = rt.anchoredPosition;
        int random = 0;

        switch (posbool)
        {
            case true:
                random = Random.Range(-180, -130);
                break;
            case false:
                random = Random.Range(140, 301);
                break;
        }
        posbool = !posbool;

        temp.y = random;
        rt.anchoredPosition = temp;
    }
    /// <summary>
    /// 随机出现单词的数量
    /// </summary>
    /// <param name="same"></param>
    /// <returns></returns>
    private int GetRandomCount(int same)
    {
        int random = Random.Range(0, m_randomWordList.Count);
        if (random == same)
            return GetRandomCount(same);
        return random;
    }

    /// <summary>
    ///协程运行游戏
    /// </summary>
    /// <param name="delay"></param>
    public void DelayPlayGame(float delay)
    {
        StartCoroutine("DelayPlayGameInternal", delay);
    }
    /// <summary>
    /// 游戏等待
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator DelayPlayGameInternal(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayGame();
    }
    /// <summary>
    /// 等待一帧开始
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitOneFrameStartLevel()
    {
        yield return new WaitForEndOfFrame();
        SharkGame.StartLevel();
    }
    /// <summary>
    /// 鲨鱼吃错误单词部匹配
    /// </summary>
    /// <param name="other"></param>
    public void SharkEatWrong(Collider2D other)
    {
        if (!isFly)
        {
            isFly = true;
            FlyStar(false).OnComplete += () => { isFly = false; };
        }
        Shark.transform.DOScale(0.6f, 1.5f).SetEase(Ease.OutElastic);
    }
    /// <summary>
    /// 鲨鱼吃单词匹配
    /// </summary>
    /// <param name="other"></param>
    public void SharkEatRight(Collider2D other)
    {
        Utility.ClearChild(Shark.transform);
        CreateFX("CFX3_Hit_SmokePuff", Shark.transform);
        var fly = FlyStar(true);
        fly.OnComplete += () =>
        {
            PlayGame();
        };
        // UISharkEatFish.Instance.DelayPlayGame(0.5f);
        Shark.transform.DOScale(1.0f, 1.5f).SetEase(Ease.OutElastic);
    }

    IEnumerator ImageHande()
    {
        ImageHand1.SetActive(false);
        ImageHand2.SetActive(true);
        yield return new WaitForSeconds(2);
        ImageHand1.SetActive(true);
        ImageHand2.SetActive(false);
        yield return new WaitForSeconds(1);
        if (m_currentIndex < 1)
        {
            StartCoroutine(ImageHande());
        }
    }
}
