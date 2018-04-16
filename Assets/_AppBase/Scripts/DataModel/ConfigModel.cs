[System.Serializable]
public class ConfigModel
{
    /// <summary>
    /// 配置表名字
    /// </summary>
    public string name;
    /// <summary>
    /// 配置表版本号
    /// </summary>
    public int version;
}

[System.Serializable]
public class ConfigCacheModel : ConfigModel
{
    public string content;
}
