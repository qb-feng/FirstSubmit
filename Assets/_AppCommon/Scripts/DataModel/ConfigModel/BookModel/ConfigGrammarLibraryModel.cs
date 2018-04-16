using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 当前表格式为molde中包含当前课程的所有语法，用#键分割
/// </summary>
[System.Serializable]
public class ConfigGrammarLibraryModel
{
    /// <summary>
    /// 数据唯一编号
    /// </summary>
    public string id;

    /// <summary>
    /// 语法重点
    /// </summary>
    public string grammar;

    public static ConfigGrammarLibraryModel GetModel(string grammarId)
    {
        return ConfigManager.Get<ConfigGrammarLibraryModel>().Find(m => m.id == grammarId);
    }
}
