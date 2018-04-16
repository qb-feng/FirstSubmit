using UnityEngine;
using System.Collections;

[System.Serializable]
public class ConfigBookModel
{
    /// <summary>
    /// 书本唯一ID
    /// </summary>
    public string bookId;
    /// <summary>
    /// 书本全名, 可以根据全名唯一表示一本书
    /// </summary>
    public string fullName;
    /// <summary>
    /// 书本短名字
    /// </summary>
    public string shortName;
    /// <summary>
    /// 书卷, 比如上下册之分,或者欧美思的2A, 2B之分
    /// </summary>
    public string volume;


    public static ConfigBookModel GetModel(string bookId)
    {
        return ConfigManager.Get<ConfigBookModel>().Find(m => m.bookId == bookId);
    }
}
