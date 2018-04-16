using System;
using System.Collections.Generic;
using UnityEngine;


public enum UnitState
{
    Lock = 0,
    Open = 1,
}
public enum LessonState
{
    Lock = 0,
    Open = 1,
    GetGift = 2,
    GotGift = 3,
}

public enum LevelState
{
    Lock = 0,
    Open = 1,
    Finish = 2,
}

[System.Serializable]
public class ConfigLessonLevelModel
{
    /// <summary>
    /// 关卡唯一ID
    /// </summary>
    public string id;
    /// <summary>
    /// 游戏ID ,格式 : id-type#difficulty  
    /// 默认当游戏数据类型为一种-type可不填, 如果为多种, 需要指出哪种类型
    /// 1#3_3  表示当前游戏类型1 后面有下划线数据的是 上面有三个图片 下面有三个单词
    /// </summary>
    public string game;
    /// <summary>
    /// 关卡配置星数
    /// </summary>
    public int star;

    /// <summary>
    /// 该游戏配置的数据 - 注意:此是自然拼读需要的新版数据！！！！！
    /// </summary>
    public string data;

    private List<string> m_gameParameter;
    private List<string> GameParameter
    {
        get
        {
            if (m_gameParameter == null)
            {
                m_gameParameter = new List<string>();
                m_gameParameter.AddRange(game.Split('-', '#'));
                if (!game.Contains("-"))
                {
                    string dataType = ConfigGameLibraryModel.GetModel(int.Parse(m_gameParameter[0])).dataType;
                    m_gameParameter.Insert(1, dataType);
                }
                if (!game.Contains("#"))
                {
                    int difficulty = ConfigGameLibraryModel.GetModel(int.Parse(m_gameParameter[0])).difficulty;
                    m_gameParameter.Add(difficulty.ToString());
                }
            }
            return m_gameParameter;
        }
    }

    private List<string> m_gameData;
    /// <summary>
    /// 关卡数据
    /// </summary>
    public List<string> GameData
    {
        get
        {
            if (m_gameData == null)
            {
                m_gameData = new List<string>();
                switch (GameDataType)
                {
                    case DataType.Word:
                        foreach (var item in ConfigManager.Get<ConfigWordLibraryModel>())
                        {
                            if (id.Split('_')[3] == "T")//同步测验关卡
                            {
                                if (item.id.StartsWith(UnitId))
                                    if (!m_gameData.Contains(item.id))
                                        m_gameData.Add(item.id);
                            }
                            else if (id.Split('_')[3] == "P")//教学呈现关卡
                            {
                                if (item.id.StartsWith(LessonId))
                                    m_gameData.Add(item.id);
                            }
                            else
                            {
                                if (item.id.StartsWith(LessonId))
                                    m_gameData.Add(item.id);
                            }
                        }
                        if (m_gameData.Count == 0)
                            m_gameData.Add(LessonId);
                        break;
                    case DataType.Ask:
                        foreach (var item in ConfigManager.Get<ConfigAskLibraryModel>())
                        {
                            if (id.Split('_')[2] == "T")//同步测验关卡
                            {
                                if (item.id.StartsWith(UnitId))
                                    m_gameData.Add(item.id);
                            }
                            else
                            {
                                if (item.id.StartsWith(LessonId))
                                    m_gameData.Add(item.id);
                            }
                        }
                        break;
                    case DataType.Say:
                        foreach (var item in ConfigManager.Get<ConfigSayLibraryModel>())
                        {
                            if (id.Split('_')[2] == "T")//同步测验关卡
                            {
                                if (item.id.StartsWith(UnitId))
                                    m_gameData.Add(item.id);
                            }
                            else
                            {
                                if (item.id.StartsWith(LessonId))//教学呈现关卡
                                    m_gameData.Add(item.id);
                            }
                        }
                        break;
                    case DataType.Song:
                        foreach (var item in ConfigManager.Get<ConfigSongLyricModel>())
                        {
                            if (item.id.StartsWith(LessonId))
                                m_gameData.Add(item.id);
                        }
                        break;
                    case DataType.Talk:
                        foreach (var item in ConfigManager.Get<ConfigTalkLibraryModel>())
                        {
                            if (item.id.StartsWith(LessonId))
                                m_gameData.Add(item.id);
                        }
                        break;
                    case DataType.Pronunciation:
                        foreach (var item in ConfigManager.Get<ConfigPronunciationModel>())
                        {
                            if (item.id.StartsWith(LessonId))
                                m_gameData.Add(item.id);
                        }
                        break;
                    case DataType.Grammar:
                        foreach (var item in ConfigManager.Get<ConfigGrammarLibraryModel>())
                        {
                            if (item.id.StartsWith(LessonId))
                                m_gameData.Add(item.id);
                        }
                        break;
                }
            }
            return m_gameData;
        }
    }

    /// <summary>
    /// 游戏ID
    /// </summary>
    public int GameId { get { return int.Parse(GameParameter[0]); } }

    /// <summary>
    /// 游戏数据类型
    /// </summary>
    public DataType GameDataType
    {
        get
        {
            int dt = -1;
            if (int.TryParse(GameParameter[1], out dt))
                return (DataType)dt;
            else
            {
                Debug.LogWarning(string.Format("GameDataType Error, GameParameter : {0}, {1}, {2}",
                    GameParameter[0], GameParameter[1], GameParameter[2]));
                return DataType.None;
            }
        }
    }

    /// <summary>
    /// 游戏难度
    /// </summary>
    public int Difficulty { get { return int.Parse(GameParameter[2]); } }

    public static List<string> GetLevels(string lessonId)
    {
        var levels = new List<string>();
        foreach (var item in ConfigManager.Get<ConfigLessonLevelModel>())
        {
            if (item.id.StartsWith(lessonId))
                levels.Add(item.id);
        }
        return levels;
    }

    public static List<string> GetGameLevels(string lessonId)
    {
        var levels = new List<string>();
        foreach (var item in ConfigManager.Get<ConfigLessonLevelModel>())
        {
            if (item.id.StartsWith(lessonId) && (!item.id.Split('_')[3].Equals("T") && !item.id.Split('_')[3].Equals("P")))
                levels.Add(item.id);
        }
        return levels;
    }

    public static List<string> GetTestLevels(string unitId)
    {
        var levels = new List<string>();
        foreach (var item in ConfigManager.Get<ConfigLessonLevelModel>())
        {
            if (item.id.StartsWith(unitId) && item.id.Split('_')[3].Equals("T"))
                levels.Add(item.id);
        }
        return levels;
    }

    public static List<string> GetPresentLevels(string lessonId)
    {
        var levels = new List<string>();
        foreach (var item in ConfigManager.Get<ConfigLessonLevelModel>())
        {
            if (item.id.StartsWith(lessonId) && item.id.Split('_')[3].Equals("P"))
                levels.Add(item.id);
        }
        return levels;
    }

    public static ConfigLessonLevelModel GetLevelModel(string levelId)
    {
        foreach (var item in ConfigManager.Get<ConfigLessonLevelModel>())
        {
            if (item.id.Equals(levelId))
                return item;
        }
        return null;
    }

    public static List<ConfigLessonLevelModel> GetLevelsModel(List<string> levels)
    {
        var list = new List<ConfigLessonLevelModel>();
        foreach (var levelId in levels)
        {
            foreach (ConfigLessonLevelModel item in ConfigManager.Get<ConfigLessonLevelModel>())
            {
                if (item.id.Equals(levelId))
                {
                    list.Add(item);
                }
            }
        }
        return list;
    }

    public static List<string> GetLevelDataSource(List<string> levels)
    {
        var allDataList = new List<string>();
        Action<string> dataAdd = (d) =>
        {
            if (!allDataList.Contains(d))
                allDataList.Add(d);
        };
        var levelsModel = GetLevelsModel(levels);
        foreach (var levelModel in levelsModel)
        {
            //var wordArray = item.Data.Replace("&", "/").Replace("|", "/").Replace("-", "/").Split('/');
            switch (levelModel.GameDataType)
            {
                case DataType.Word:
                    //单词资源标记是直接用单词标记, 不是用id标记, 所以需要转化一下
                    foreach (var data in levelModel.GameData)
                    {
                        //先判断单词是否存在映射单词情况
                        var mapModel = ConfigMapWordModel.GetModel(data);
                        if (mapModel != null)
                        {
                            dataAdd(mapModel.word + ".mp3");
                            dataAdd(mapModel.word + ".png");
                        }
                        else
                        {
                            var wordModel = ConfigWordLibraryModel.GetModel(data);
                            if (wordModel != null)
                            {
                                dataAdd(wordModel.word + ".mp3");
                                dataAdd(wordModel.word + ".png");
                            }
                        }
                    }
                    break;
                case DataType.Ask:
                case DataType.Say:
                    //问句单句存在肯定或者否定的句子, 也要把这些资源加上
                    Action<List<string>> sentenceDataAdd = dataList =>
                    {
                        foreach (var data in dataList)
                        {
                            if (levelModel.GameDataType == DataType.Ask)
                            {
                                dataAdd(data + ".mp3");
                            }
                            dataAdd(data + "_0.mp3");
                            dataAdd(data + "_1.mp3");
                        }
                        //问句单句存在单词映射表, 所以需要加上过滤出映射的资源
                        foreach (var mapWord in AddMapSource(dataList))
                        {
                            dataAdd(mapWord);
                        }
                    };
                    sentenceDataAdd(levelModel.GameData);
                    foreach (var dataId in levelModel.GameData)
                    {
                        List<string> dataList = new List<string>();
                        if (levelModel.GameDataType == DataType.Ask)
                        {
                            var askModel = ConfigAskLibraryModel.GetModel(dataId);
                            dataList.AddRange(askModel.wrong.Split('/'));
                        }
                        else if (levelModel.GameDataType == DataType.Say)
                        {
                            var sayModel = ConfigSayLibraryModel.GetModel(dataId);
                            dataList.AddRange(sayModel.wrong.Split('/'));
                        }
                        sentenceDataAdd(dataList);
                    }
                    //存在关卡使用上一个课单词的情况, 于是需要把上一课单词全部包含进入, 当然只包含图片即可
                    string unitId = levelModel.UnitId;
                    int wordLesson = int.Parse(levelModel.id.Split('_')[2]) - 1;
                    string wordLessonId = unitId + "_" + wordLesson;
                    foreach (var item in ConfigManager.Get<ConfigWordLibraryModel>())
                    {
                        if (item.id.StartsWith(wordLessonId))
                        {
                            dataAdd(item.word + ".png");
                        }
                    }
                    break;
                case DataType.Song:
                    foreach (var data in levelModel.GameData)
                    {
                        dataAdd(data + ".mp3");
                    }
                    break;
                case DataType.Talk:
                    var talkModelList = ConfigTalkLibraryModel.GetTalkList(levelModel.GameData);
                    dataAdd(levelModel.LessonId + ".prefab");
                    for (int i = 0; i < talkModelList.Count; i++)
                    {
                        dataAdd(levelModel.LessonId + "_" + (i + 1) + ".prefab");
                        for (int j = 0; j < talkModelList[i].AudioAmount; j++)
                        {
                            dataAdd(talkModelList[i].id + "_" + (j + 1) + ".mp3");
                        }
                    }

                    dataAdd(levelModel.LessonId + "_7_1.jpg");
                    break;
                case DataType.Pronunciation:
                    var proModel = ConfigPronunciationModel.GetModel(levelModel.GameData[0]);
                    foreach (var pro in proModel.pronunciation.Split('/'))
                    {
                        if (string.IsNullOrEmpty(pro))
                            continue;
                        //特殊判断h,o,t, h,a,t这对自然拼读
                        if (pro.Contains(","))
                        {
                            //foreach (var item in pro.Split(','))
                            //{
                            //    dataAdd("pronunciation_" + item + ".mp3");
                            //}
                            dataAdd("pronunciation_" + pro + ".mp3");
                        }
                        else
                            dataAdd("pronunciation_" + pro + ".mp3");
                    }
                    foreach (var word in proModel.word.Split('/'))
                    {
                        if (string.IsNullOrEmpty(word))
                            continue;
                        dataAdd(word.Split('-', '@')[0] + ".mp3");
                        dataAdd(word.Split('-', '@')[0] + ".png");
                    }
                    break;
                case DataType.Grammar:
                    dataAdd(levelModel.LessonId + "_7_1.jpg");
                    break;
            }
        }
        return allDataList;
    }

    private static List<string> AddMapSource(List<string> dataList)
    {
        var allMapDataList = new List<string>();
        Action<string> addData = d =>
        {
            if (!allMapDataList.Contains(d))
                allMapDataList.Add(d);
        };
        foreach (var dataId in dataList)
        {
            if (string.IsNullOrEmpty(dataId))
                continue;
            //处理映射单词表中映射关联的数据
            var mapModel = ConfigMapWordModel.GetModel(dataId);
			if (mapModel != null)
            {
				if(mapModel.type == (int)ConfigMapType.UseWordMap)
				{
					//先判断有没有特殊图片
					if (string.IsNullOrEmpty(mapModel.special))
					{
						foreach (var word in mapModel.word.Split('|'))
						{
							if (string.IsNullOrEmpty(word))
								continue;
							addData(word + ".mp3");
							addData(word + ".png");
						}
					}
					else
					{
						foreach (var split in mapModel.special.Split('|'))
						{
							addData(mapModel.id + split + ".png");
						}
					}
				}	
				else if(mapModel.type == (int)ConfigMapType.UseIdMap)
				{
					//添加句子映射的图片. 声音已经在上面添加过
					addData (dataId + ".png");
				}
            }
        }
        return allMapDataList;
    }

    public string BookId
    {
        get
        {
            return id.Split('_')[0];
        }
    }

    public string UnitId
    {
        get
        {
            return BookId + "_" + id.Split('_')[1];
        }
    }

    public string LessonId
    {
        get
        {
            return UnitId + "_" + id.Split('_')[2];
        }
    }
}
