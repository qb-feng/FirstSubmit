[System.Serializable]
public class ConfigVersionModel
{
    /// <summary>
    /// 发布版本编号
    /// </summary>
    public string release;
    /// <summary>
    /// 是否是发布版本
    /// </summary>
    public bool isRelease;
    /// <summary>
    /// 是否打开Log
    /// </summary>
    public bool debug;
    /// <summary>
    /// 请求超时时间
    /// </summary>
    public int timeOut;
    /// <summary>
    /// 更新提示描述
    /// </summary>
    public string updateDescribe;
    /// <summary>
    /// AssetBundle的版本号, 当有新AB资源更新时候,需要提高版本号
    /// </summary>
    public string abVersion;

    /// 内网地址 : http://192.168.1.212:8088/
    /// 外网地址 : http://cdn.boly.abottletree.com/

    /// <summary>
    /// IOS AB 生产环境下载地址
    /// </summary>
    public string iosABDownloadUrl;
    /// <summary>
    /// Android AB 生产下载地址
    /// </summary>
    public string androidABDownloadUrl;
    /// <summary>
    /// Windows AB 生产环境下载地址
    /// </summary>
    public string winABDownloadUrl;
    /// <summary>
    /// IOS AB 开发环境下载地址
    /// </summary>
    public string iosABDebugUrl;
    /// <summary>
    /// Android AB 开发环境下载地址
    /// </summary>
    public string androidABDebugUrl;
    /// <summary>
    /// Windows AB 开发环境下载地址
    /// </summary>
    public string winABDebugUrl;
    /// <summary>
    /// 是否显示公告
    /// </summary>
    public bool isShowNotice;

    public string ABDownloadUrl
    {
        get
        {
            if (isRelease)
            {
#if UNITY_ANDROID
                return androidABDownloadUrl;
#elif UNITY_IOS
            return iosABDownloadUrl;
#elif UNITY_STANDALONE_WIN
            return winABDownloadUrl;
#endif
            }
            else
            {
#if UNITY_ANDROID
                return androidABDebugUrl;
#elif UNITY_IOS
            return iosABDebugUrl;
#elif UNITY_STANDALONE_WIN
            return winABDebugUrl;
#endif
            }
        }
    }

    public static ConfigVersionModel GetVersion()
    {
        return ConfigManager.Get<ConfigVersionModel>().Find(m => m.release == AppChannel.Release);
    }
}
