using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ConfigGameTestDataModel
{
    /// <summary>
    /// 游戏ID及制定的数据类型
    /// </summary>
    public int gameId;
    /// <summary>
    /// 游戏数据类型
    /// </summary>
    public int dataType;
    /// <summary>
    /// 游戏所需数据
    /// </summary>
    public string data;
    /// <summary>
    /// 游戏星数
    /// </summary>
    public int star;
    /// <summary>
    /// 是否需要引导
    /// </summary>
    public bool guide;

    /// <summary>
    /// 游戏难度
    /// </summary>
    public int difficulty;

    public List<string> DataList
    {
        get { return new List<string>(data.Split(',')); }
    }

    public static ConfigGameTestDataModel GetTestDataModel(int gameId, int dataType)
    {
        ConfigGameTestDataModel model = null;
        ConfigGameTestDataModel modelDefault = null;
        foreach (var item in ConfigManager.Get<ConfigGameTestDataModel>())
        {
            if (item.gameId == gameId && item.dataType == dataType)
            {
                model = item;
            }
            if (item.gameId == -1 && item.dataType == dataType)
            {
                modelDefault = item;
            }
        }
        return model != null ? model : modelDefault;
    }
}
