[System.Serializable]
public class ConfigSongLyricModel
{
    /// <summary>
    ///  关卡id
    /// </summary>
    public string id;
    /// <summary>
    ///  歌词内容
    /// </summary>
    public string content;
    /// <summary>
    /// 最后要显示的SightWord
    /// </summary>
    public string sightWords;

    public static ConfigSongLyricModel GetModel(string songId)
    {
        return ConfigManager.Get<ConfigSongLyricModel>().Find(m => m.id == songId);
    }
}