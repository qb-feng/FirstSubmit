using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏统计类型
/// </summary>
public enum GameStatisticsType
{
    None = 0,
    /// <summary>
    /// 只有正确的回答类型
    /// </summary>
    OnlyTrueAnswer = 1,

    /// <summary>
    /// 具有正确和错误的回答类型
    /// </summary>
    TrueAndFalseAnswer = 2,
}

/// <summary>
/// 所有游戏的统计model
/// </summary>
[System.Serializable]
public class ConfigGameStatisticsModel
{
    /// <summary>
    /// 游戏id
    /// </summary>
    public int gameId;

    /// <summary>
    /// 游戏统计类型
    /// </summary>
    public int gameStatisticsType;

    /// <summary>
    /// 获取指定游戏id的统计类型
    /// </summary>
    public static GameStatisticsType GetGameStatisticsTypeOfGameId(int gameId)
    {
        var findModel = ConfigManager.Get<ConfigGameStatisticsModel>().Find((v) => { return v.gameId == gameId; });
        return findModel == null ? GameStatisticsType.None : (GameStatisticsType)findModel.gameStatisticsType;

    }
}
