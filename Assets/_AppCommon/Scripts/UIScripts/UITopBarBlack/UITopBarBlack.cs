using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public enum LevelType
{
    Game = 1,
    Test = 2,
    Challenge = 3,
}
public class UITopBarBlack : UIBaseInit, IUIOnRefresh
{
    private GameObject BtnBack { get { return Get("ImageBack"); } }

    private TMP_Text TextPreviousUI { get { return Get<TMP_Text>("TextPreviousUI"); } }

    private TMP_Text TextCurrentUI { get { return Get<TMP_Text>("TextCurrentUI"); } }

    private GameObject TweenWrong { get { return Get("TweenWrong"); } }

    private GameObject TweenRight { get { return Get("TweenRight"); } }

    private GameObject TweenRightEndP { get { return Get("Right_EndP"); } }

    private Transform HorizontalLayoutStars { get { return GetT("HorizontalLayoutStars"); } }

    private GameObject PracticeProgress { get { return Get("PracticeProgress"); } }
    private Slider PracticeSlider { get { return Get<Slider>("PracticeSlider"); } }
    private TMP_Text PracticeSliderText { get { return Get<TMP_Text>("PracticeSliderText"); } }

    private int m_starNum = 0;

    private int m_currentStar = 0;
    private LevelType m_currentLevelType;
    public LevelType CurrentLevelType
    {
        get
        {
            return m_currentLevelType;
        }
    }

    private int CurrentStar
    {
        get { return m_currentStar; }
        set
        {
            m_currentStar = value;
            if (m_currentGame != null)
                m_currentGame.CurrentStar = value;
        }
    }

    private int m_nextStar = -1;

    public event Action OnCustomBackEvent;
    public event Action OnCompleteAllStarEvent;
    private float m_delayGameEndTime = 0;

    private UIBaseLevelGame m_currentGame;

    private string m_levelId;

    private static UITopBarBlack s_instance;

    public static UITopBarBlack Instance
    {
        get
        {
            if (s_instance == null)
            {
                GameObject go = CreateUI("UITopBarBlack");
                s_instance = go.AddComponent<UITopBarBlack>();
            }
            return s_instance;
        }
    }

    public void Init()
    {
        UIManager.RegisterObserver(this);
        UGUIEventListener.Get(BtnBack).onPointerClick = BtnBackClick;
        gameObject.SetActive(false);
    }

    #region 得到进入新游戏的对应的关卡信息
    private ConfigLessonLevelModel currentClickLevelModel = null;
    /// <summary>
    /// 关卡地图的游戏关卡点击事件 - 返回值为点击进入的关卡的信息
    /// </summary>
    void UIMainLevelsMap_OnSeleteGameEvent(ConfigLessonLevelModel t)
    {
        currentClickLevelModel = t;
    }
    #endregion

    /// <summary>
    /// 设置当前关卡相关信息
    /// </summary>
    /// <param name="levelId"></param>
    /// <param name="type"></param>
    public void SetLevelInfo(string levelId, LevelType type)
    {
        m_levelId = levelId;
        m_currentLevelType = type;
    }

    public string LevelId { get; set; }

    public void SetAllStarFinish()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine("WaitSaveProgress");
    }

    public event Action<StatisticsGameDataModel> SendGameDateEvent = null;
    private IEnumerator WaitSaveProgress()
    {
        yield return new WaitForSeconds(m_delayGameEndTime);
        if (CurrentLevelType == LevelType.Game)
        {
            if (SendGameDateEvent != null)
            {
                SendGameDateEvent(m_currentGame.StatisticsData);
            }
            //MessageHandler.Instance.SendGameData(m_currentGame.StatisticsData);
        }
    }

    private void SetStarNum(int num)
    {
        m_starNum = num;
        if (m_starNum == 0)
        {
            HorizontalLayoutStars.gameObject.SetActive(false);
            PracticeProgress.SetActive(false);
        }
        CurrentStar = 0;
        m_nextStar = -1;
        if (CurrentLevelType == LevelType.Game || CurrentLevelType == LevelType.Challenge)
        {
            for (int i = 0; i < HorizontalLayoutStars.childCount; i++)
            {
                if (i < m_starNum)
                    Get("ImageStar" + i).SetActive(true);
                else
                    Get("ImageStar" + i).SetActive(false);
                Get("ImageStarHalf" + i).SetActive(false);
                Get("ImageStarAll" + i).SetActive(false);
            }
        }
        AddHalfStar();
    }

    private void UpdatePracticeSliderText()
    {
        PracticeSliderText.text = string.Format("{0}/{1}", CurrentStar, m_starNum);
        PracticeSlider.value = (CurrentStar) / (float)m_starNum;
    }

    private void AddHalfStar()
    {
        if (CurrentLevelType == LevelType.Test)
        {
            UpdatePracticeSliderText();
        }
        else
        {
            string key = "ImageStarHalf" + CurrentStar;
            if (Get(key) != null)
            {
                Get(key).SetActive(true);
            }
        }
    }

    public UITopBarStarFly AddOneStar()
    {
        if (CurrentStar < m_starNum)
            return RightStarFly();
        return null;
    }

    public void OnBeforeRefresh(Type previous, Type next, UIBaseInit nextUI)
    {
        transform.SetAsLastSibling();
        bool isGame = System.Enum.IsDefined(typeof(UIGameName), next.Name);
        gameObject.SetActive(isGame);
        AudioManager.Instance.FreezeBgSound(isGame);
        if (isGame)
        {

            m_currentGame = nextUI as UIBaseLevelGame;
            HorizontalLayoutStars.gameObject.SetActive(CurrentLevelType == LevelType.Game || CurrentLevelType == LevelType.Challenge);
            PracticeProgress.SetActive(CurrentLevelType == LevelType.Test);
            InitGameEvent();
        }
        else
        {
            m_currentGame = null;
        }
    }

    public void OnAfterRefreshUI(Type previous, Type next, UIBaseInit nextUI)
    {
        if (System.Enum.IsDefined(typeof(UIGameName), next.Name))
        {
            int gameId = (int)System.Enum.Parse(typeof(UIGameName), next.Name);
            var model = ConfigGameLibraryModel.GetModel(gameId);
            TextPreviousUI.text = "";
            if (nextUI is UIBaseLevelGame)
            {
                //TextCurrentUI.text = model.gameName;
            }
        }
    }

    private void InitGameEvent()
    {
        m_currentGame.OnGameRightEvent += AddOneStar;
        m_currentGame.OnGameWrongEvent += WrongStarFly;
        m_currentGame.OnUpdateStarCountEvent += (starCount) => { SetStarNum(starCount); };
        m_currentGame.OnUpdateDelayGameEndEvent += (delay) => { m_delayGameEndTime = delay; };
        if (m_currentGame.CustomGameEndEvent != null)
        {
            OnCompleteAllStarEvent = m_currentGame.CustomGameEndEvent;
        }
        else
        {
            OnCompleteAllStarEvent = SetAllStarFinish;
        }
    }

    private void BtnBackClick(UnityEngine.EventSystems.PointerEventData obj)
    {
        UITopBarStarFly.ClearAllOnComplete();
        if (OnCustomBackEvent != null)
        {
            OnCustomBackEvent();
            OnCustomBackEvent = null;
        }
        else
        {
            UIManager.Instance.Back();
            if (UIManager.Instance.CurrentStatusUI == typeof(UISplash))
            {
                //Application.Quit();
                MessageBridge.Instance.ExitUnity();
            }
        }
    }

    public UITopBarStarFly WrongStarFly()
    {
        AudioManager.Instance.Play("ui_error_1");
        var fly = new UITopBarStarFly(TweenWrong.GetComponent<DrawBezier>(), 2f);
        return fly;
    }

    private UITopBarStarFly RightStarFly()
    {
        m_nextStar = Mathf.Min(m_starNum, ++m_nextStar);
        if (CurrentLevelType == LevelType.Test)
        {
            TweenRightEndP.transform.position = PracticeSlider.handleRect.position;
        }
        else
            TweenRightEndP.transform.position = GetT("ImageStarAll" + m_nextStar).position;
        AudioManager.Instance.Play("bling_01");
        var fly = new UITopBarStarFly(TweenRight.GetComponent<DrawBezier>(), 1.3f);
        fly.RandomTangent(true, true);
        fly.OnComplete += TweenOnComplete;
        return fly;
    }

    private void TweenOnComplete()
    {
        AudioManager.Instance.Play("pop3");
        string key = "ImageStarAll" + CurrentStar;
        if (Get(key) != null)
        {
            Get(key).SetActive(true);

            CurrentStar++;
            if (CurrentStar >= m_starNum)
            {
                CurrentStar = m_starNum;
                if (OnCompleteAllStarEvent != null)
                {
                    OnCompleteAllStarEvent();
                    OnCompleteAllStarEvent = null;
                }
                if (CurrentLevelType == LevelType.Game)
                    return;
            }
        }
        AddHalfStar();
    }
}
