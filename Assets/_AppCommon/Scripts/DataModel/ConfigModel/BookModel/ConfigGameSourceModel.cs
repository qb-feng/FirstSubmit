using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ConfigGameSourceModel
{
    /// <summary>
    /// 游戏唯一ID
    /// </summary>
    public int gameId;
    /// <summary>
    /// 对应的游戏所依赖的资源名字
    /// </summary>
    public string source;
    /// <summary>
    /// 依赖资源的扩展名
    /// </summary>
    public string extension;

    public string SourceName { get { return source + extension; } }

    public static List<string> GetGameSourceList(List<string> levels)
    {
        var sourceList = new List<string>();
        foreach (var levelId in levels)
        {
            int gameId = ConfigLessonLevelModel.GetLevelModel(levelId).GameId;
            foreach (var item in ConfigManager.Get<ConfigGameSourceModel>())
            {
                if (item.gameId.Equals(gameId))
                {
                    if (!sourceList.Contains(item.SourceName))
                    {
                        sourceList.Add(item.SourceName);
                    }
                }
            }
        }
        return sourceList;
    }

    public static List<string> GetGameSourceList(int gameId)
    {
        var sourceList = new List<string>();
        foreach (var item in ConfigManager.Get<ConfigGameSourceModel>())
        {
            if (item.gameId.Equals(gameId))
            {
                if (!sourceList.Contains(item.SourceName))
                {
                    sourceList.Add(item.SourceName);
                }
            }
        }
        return sourceList;
    }
}
