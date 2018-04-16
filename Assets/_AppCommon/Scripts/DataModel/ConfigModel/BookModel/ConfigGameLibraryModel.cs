/// <summary>
/// 游戏所需数据类型
/// </summary>
public enum DataType
{
    None = 0,
    Word = 1,
    Ask = 2,
    Say = 3,
    Song = 4,
    Talk = 5,
    Pronunciation = 6,
    Grammar = 7
}

[System.Serializable]
public class ConfigGameLibraryModel
{
    /// <summary>
    /// 游戏唯一ID
    /// </summary>
    public int id;
    /// <summary>
    /// 关卡游戏类型本地化Key
    /// </summary>
    public string gameName;
    /// <summary>
    /// 游戏模式
    /// </summary>
    public string gameMode;
    /// <summary>
    /// 游戏操作方式
    /// </summary>
    public string gameOperator;
    /// <summary>
    /// 知识点分类
    /// </summary>
    public string sortType;
    /// <summary>
    /// 教研类型
    /// </summary>
    public string teachingType;
    /// <summary>
    /// 教研重点
    /// </summary>
    public string teachingFocus;
    /// <summary>
    /// 教学类型(旧)
    /// </summary>
    [System.Obsolete]
    public string teachType;
    /// <summary>
    /// 所需数据类型
    /// 其中如果含有“&”表示该游戏所必须的几种数据类型
    /// 其中如果含有“|”表示该游戏可选的几种数据类型
    /// </summary>
    public string dataType;
    /// <summary>
    /// 游戏主类型
    /// </summary>
    public int gameType;
    /// <summary>
    /// 是否通用
    /// </summary>
    public int common;
    /// <summary>
    /// 游戏考察的能力
    /// </summary>
    public string ability;
    /// <summary>
    /// 游戏难度数量, 默认1种难度
    /// </summary>
    public int difficulty;
    /// <summary>
    ///  游戏描述
    /// </summary>
    public string describe;

    public string GetTeachType(string datatype)
    {
        if (teachingType.Contains("|"))
        {
            var item = dataType.Split('|');
            for (int i = 0; i < item.Length; i++)
            {
                if (item[i] == datatype)
                {
                    return teachingType.Split('|')[i];
                }
            }
        }
        else
        {
            return teachingType;
        }
        return null;
    }

    public static ConfigGameLibraryModel GetModel(int id)
    {
        return ConfigManager.Get<ConfigGameLibraryModel>().Find(m => m.id == id);
    }

    public static bool IsContainGame(int gameId)
    {
        var model = GetModel(gameId);
        return model != null;
    }
}
