using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBaseLevelGame : UIBaseInit, IUIState, IUIRefresh
{
    /// <summary>
    /// 游戏初始化数据
    /// </summary>
    private StartGameDataModel m_startData;
    public StartGameDataModel StartData { get { return m_startData; } }

    /// <summary>
    /// 游戏分析数据
    /// </summary>
    private StatisticsGameDataModel m_statisticsData;
    public StatisticsGameDataModel StatisticsData { get { return m_statisticsData; } }

    protected List<ConfigWordLibraryModel> m_originalWordList;
    protected List<ConfigWordLibraryModel> m_randomWordList;
    protected List<ConfigAskLibraryModel> m_originalAskList;
    protected List<ConfigAskLibraryModel> m_randomAskList;
    //防止m_AskList表仅为一句无法在该表获取干扰项，所有可选干扰项在此表中
    protected List<ConfigAskLibraryModel> m_currentUnitAllAskList;
    protected List<ConfigSayLibraryModel> m_originalSayList;
    protected List<ConfigSayLibraryModel> m_randomSayList;
    //防止m_SayList表仅为一句无法在该表获取干扰项，所有可选干扰项在此表中
    protected List<ConfigSayLibraryModel> m_currentUnitAllSayList;
    protected List<ConfigTalkLibraryModel> m_originalTalkList;
    protected ConfigGrammarLibraryModel m_grammarModel;

    // protected ConfigPronunciationModel m_pronunciationModel;
    //protected List<string> m_originalPronunciationWord;
    //protected List<string> m_randomPronunciationWord;

    protected ConfigSongLyricModel m_originalSong;
    protected ConfigGameLevelModel m_GameLevel;

    protected bool m_isStarFlying;


    #region 陈述句中否定句选项引起歧义用到的参数
    protected bool mySayIsExistSpecialOption
    {
        get
        {
            return CurrentSay.IsExistSpecialOption;
        }
    }
    protected string mySaySpecialOption
    {
        get
        {
            return CurrentSay.SpecialOption;
        }
    }
    #endregion

    #region 问答句中否定句选项引起歧义用到的参数
    protected bool myAskIsExistSpecialOption
    {
        get
        {
            return CurrentAsk.IsExistSpecialOption;
        }
    }
    protected string myAskSpecialOption
    {
        get
        {
            return CurrentAsk.SpecialOption;
        }
    }
    #endregion

    #region 陈述句中出现多个关键词引起歧义用到的参数
    protected bool mySayIsExistSpecialSprite
    {
        get
        {
            //return CurrentSay.IsExistSpecialSprite;
            return false;
        }
    }
    protected List<Sprite> mySaySpecialSprite
    {
        get
        {
            return CurrentSay.SpecialSprite;
        }
    }
    #endregion

    #region 问答句中出现多个关键词引起歧义用到的参数
    protected bool myAskIsExistSpecialSprite
    {
        get
        {
            return CurrentAsk.IsExistSpecialSprite;
        }
    }
    protected List<Sprite> myAskSpecialSprite
    {
        get
        {
            return CurrentAsk.SpecialSprite;
        }
    }
    #endregion

    protected bool IsFirstPlay
    {
        get
        {
            return m_startData.guide && CurrentStar == 0;
        }
    }

    protected int m_currentIndex = -1;
    protected int m_currentGameLevel = 0;

    public ConfigWordLibraryModel CurrentWord
    {
        get
        {
            if (m_randomWordList != null)
                return m_randomWordList[m_currentIndex];
            return null;
        }
    }
    public ConfigAskLibraryModel CurrentAsk
    {
        get
        {
            if (m_randomAskList != null)
                return m_randomAskList[m_currentIndex];
            return null;
        }
    }
    public ConfigWordLibraryModel CurrentAskMapWord
    {
        get
        {
            return ConfigMapWordModel.GetWordModel(CurrentAsk.id);
        }
    }
    public ConfigSayLibraryModel CurrentSay
    {
        get
        {
            if (m_randomSayList != null)
                return m_randomSayList[m_currentIndex];
            return null;
        }
    }
    public ConfigWordLibraryModel CurrentSayMapWord
    {
        get
        {
            return ConfigMapWordModel.GetWordModel(CurrentSay.id);
        }
    }
    /// <summary>
    /// 音标对应的单词（没有去掉特殊符号的单词）
    /// </summary>
    public string CurrentPronunciationWord
    {
        get
        {
            if (m_randomPronunciationList_New != null)
                return m_randomPronunciationList_New[m_currentIndex].CurrentCorrespondWord;
            return null;
        }
    }
    /// <summary>
    /// 去掉特殊符号后的单词
    /// </summary>
    public string CurrentPronunciationRealWord
    {
        get
        {
            string realWord = CurrentPronunciationWord;
            if (realWord.Contains("-"))
                realWord = CurrentPronunciationWord.Split('-')[0];
            else if (realWord.Contains("@"))
                realWord = CurrentPronunciationWord.Split('@')[0];
            else
                realWord = CurrentPronunciationWord;
            return realWord;
        }
    }

    //发音定位
    public string CurrentPronunciationIndex
    {
        get
        {
            if (CurrentPronunciationWord.Contains("@"))
                return CurrentPronunciationWord.Split('@')[1];
            else
                return null;
        }
    }

    private int m_starNum = 0;
    public event Action<int> OnUpdateStarCountEvent;
    protected int StarCount
    {
        get { return m_starNum; }
        set
        {
            m_starNum = value;
            if (OnUpdateStarCountEvent != null)
                OnUpdateStarCountEvent(value);
        }
    }

    private float m_delayGameEnd = 0;
    public event Action<float> OnUpdateDelayGameEndEvent;
    protected float DelayGameEnd
    {
        get { return m_delayGameEnd; }
        set
        {
            m_delayGameEnd = value;
            if (OnUpdateDelayGameEndEvent != null)
                OnUpdateDelayGameEndEvent(value);
        }
    }

    public Action CustomGameEndEvent;

    public int CurrentStar { get; set; }

    protected virtual bool IsGameEnd
    {
        get
        {
            m_currentIndex++;
            Debug.Log("##########m_currentIndex : " + m_currentIndex);
            bool end = GameEndCondition;
            if (end)
                m_statisticsData.e = Utility.GetUnixTimeSecond(System.DateTime.Now);
            else
            {
                everyKnowledgePointStartTime = Utility.GetUnixTimeSecond(System.DateTime.Now);//统计每一个知识点的开始时间
            }
            return end;
        }
    }
    protected virtual bool GameEndCondition
    {
        get
        {
            return CurrentStar == m_starNum;
        }
    }

    public event Func<UITopBarStarFly> OnGameRightEvent;
    public event Func<UITopBarStarFly> OnGameWrongEvent;

    public void Refresh(params object[] data)
    {
        m_startData = data[0] as StartGameDataModel;
        SetData(m_startData.dataType, m_startData.dataList);
        InitStatisticsGameData();
        StarCount = m_startData.star;
        Refresh();
        PlayGame();
    }

    private void InitStatisticsGameData()
    {
        m_statisticsData = new StatisticsGameDataModel()
        {
            gt = m_startData.gameId,
            s = Utility.GetUnixTimeSecond(System.DateTime.Now),
            statisType = ConfigGameStatisticsModel.GetGameStatisticsTypeOfGameId(m_startData.gameId),
        };
    }

    private void SetData(DataType type, List<string> data)
    {
        switch (type)
        {
            case DataType.Word:
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Main"))
                {
                    //改成不读表模式
                    m_originalWordList = new List<ConfigWordLibraryModel>();
                    for (int i = 0; i < data.Count; ++i)
                    {
                        ConfigWordLibraryModel tempModel = new ConfigWordLibraryModel()
                        {
                            id = i.ToString(),
                            word = data[i],
                        };
                        m_originalWordList.Add(tempModel);
                    }
                }
                else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Work"))
                {
                    //照以前的模式
                    m_originalWordList = ConfigWordLibraryModel.GetWordList(data);
                }

                m_randomWordList = Utility.RandomSortList(m_originalWordList);
                break;
            case DataType.Ask:
                m_originalAskList = ConfigAskLibraryModel.GetAskList(data);
                m_currentUnitAllAskList = ConfigAskLibraryModel.GetAskWrongList(data);
                m_randomAskList = Utility.RandomSortList(m_originalAskList);
                break;
            case DataType.Say:
                m_originalSayList = ConfigSayLibraryModel.GetSayList(data);
                m_currentUnitAllSayList = ConfigSayLibraryModel.GetSayWrongList(data);
                m_randomSayList = Utility.RandomSortList(m_originalSayList);
                break;
            case DataType.Song:
                m_originalSong = ConfigSongLyricModel.GetModel(data[0]);
                break;
            case DataType.Talk:
                m_originalTalkList = ConfigTalkLibraryModel.GetTalkList(data);
                break;
            case DataType.Pronunciation:
                //TODO 以后改为不读表模式
                //m_originalPronunciationList_New = new List<ConfigPronunciationModel>();
                //for (int i = 0; i < data.Count; ++i)
                //{
                //    ConfigPronunciationModel tempModel = new ConfigPronunciationModel()
                //    {
                //        id = i.ToString(),
                //        pronunciation = data[i],
                //        //音标对应的单词没有~~
                //    };
                //    m_originalPronunciationList_New.Add(tempModel);
                //}
                m_originalPronunciationList_New = ConfigPronunciationModel.GetPronunciationList(data);
                m_randomPronunciationList_New = Utility.RandomSortList(m_originalPronunciationList_New);
                m_originalPronunciationList = new List<string>();
                foreach (var v in m_originalPronunciationList_New)
                {
                    m_originalPronunciationList.Add(v.pronunciation);
                }
                m_randomPronunciationList = Utility.RandomSortList(m_originalPronunciationList);

                break;
            case DataType.Grammar:
                m_grammarModel = ConfigGrammarLibraryModel.GetModel(data[0]);
                break;
        }
    }

    public virtual void Refresh() { }

    public abstract void PlayGame();

    public UITopBarStarFly FlyStar(bool right, bool isWord = true)
    {
        var ids = new List<string>();
        if (isWord)
            ids.Add(CurrentWord.id);
        return FlyStar(ids, right);
    }

    public UITopBarStarFly FlyStar(string id, bool right)
    {
        return FlyStar(new List<string> { id }, right);
    }

    public UITopBarStarFly FlyStar(List<string> ids, bool right)
    {
        StatisticsGameDataModel.KnowledgeDataModel wordModel = null;
        foreach (var id in ids)
        {
            bool find = false;
            foreach (var item in m_statisticsData.data)
            {
                if (item.id == id)
                {
                    find = true;
                    if (!right)
                    {
                        item.w++;
                        Debug.Log("Wrong!! Data ID Is : " + id + " , Wrong Num : " + item.w);
                    }
                    else
                    {
                        Debug.Log("Right!! Data ID Is : " + id);
                    }
                    wordModel = item;
                }
            }
            if (!find)
            {
                wordModel = new StatisticsGameDataModel.KnowledgeDataModel { id = id, w = right ? 0 : 1 };
                m_statisticsData.data.Add(wordModel);
                if (right)
                    Debug.Log("Right!! Data ID Is : " + id);
                else
                    Debug.Log("Wrong!! Data ID Is : " + id + " , Wrong Num : " + 1);
            }

            //根据统计类型添加统计数据
            //已通过单词
            if (m_startData.dataType == DataType.Word)
            {
                wordModel.word = CurrentWord.word;
            }

            switch (this.m_statisticsData.statisType)
            {
                case GameStatisticsType.OnlyTrueAnswer:

                    break;

                //有正确有错误的回答
                case GameStatisticsType.TrueAndFalseAnswer:

                    var modelResult = wordModel.result;
                    if (modelResult == null)
                    {
                        wordModel.result = new List<int>();
                        modelResult = wordModel.result;
                    }
                    modelResult.Add(right ? 1 : 0);
                    break;
            }

            //知识点通过的时间
            if (right)
            {
                //回答正确 - 记录当前知识点结束时间
                everyKnowledgePointEndTime = Utility.GetUnixTimeSecond(System.DateTime.Now);
                wordModel.duration = everyKnowledgePointEndTime - everyKnowledgePointStartTime;
                Debug.LogWarning(" 当前知识点" + ids[0] + "考察的时间为：" + everyKnowledgePointEndTime + "  -  " + everyKnowledgePointStartTime + "  = " + wordModel.duration);
            }

        }

        UITopBarStarFly flyStar = null;

        if (right && OnGameRightEvent != null)
        {
            m_isStarFlying = true;
            flyStar = OnGameRightEvent();
            flyStar.OnComplete += () =>
            {
                m_isStarFlying = false;
            };
        }
        else if (OnGameWrongEvent != null)
        {
            flyStar = OnGameWrongEvent();
        }
        return flyStar;
    }


    #region 2018年3月30日19:13:32 qiubin新增游戏数据统计部分
    private long everyKnowledgePointStartTime = 0;//每一个知识点开启的时间
    private long everyKnowledgePointEndTime = 0;//每一个知识点结束的时间
    #endregion

    #region 2018年4月2日12:00:16　新自然拼读数据部分
    protected List<ConfigPronunciationModel> m_originalPronunciationList_New = null;//考察的所有音标model
    protected List<ConfigPronunciationModel> m_randomPronunciationList_New = null;
    protected List<string> m_originalPronunciationList;//考察的所有音标model的音标string
    protected List<string> m_randomPronunciationList;


    /// <summary>
    /// 当前考察的自然拼读数据
    /// </summary>
    public ConfigPronunciationModel CurrentPronunciation
    {
        get
        {
            if (m_randomPronunciationList_New != null)
                return m_randomPronunciationList_New[m_currentIndex];
            return null;
        }
    }

    #endregion
}
