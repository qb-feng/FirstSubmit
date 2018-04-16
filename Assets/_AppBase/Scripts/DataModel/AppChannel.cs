using UnityEngine;

[System.Serializable]
public class AppChannel
{
    /// <summary>
    /// 发布版本编号
    /// </summary>
    public static string Release { get { return Application.version; } }
    /// <summary>
    /// 渠道名
    /// </summary>
    public static string Channel
    {
        get
        {
#if UNITY_EDITOR
            return "editor";
#elif UNITY_ANDROID
            return GetAndroidChannel();
#elif UNITY_IOS
            return "AppStore";
#elif UNITY_STANDALONE_WIN
            return "win";
#endif
        }
    }

    private static string GetAndroidChannel()
    {
        using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var context = activity.GetStatic<AndroidJavaObject>("currentActivity");
            using (var jc = new AndroidJavaClass("com.puhanda.english.BuildChannelApkTool"))
            {
                string channel = jc.CallStatic<string>("getChannel", context);
                Debug.Log("Android Apk Channel Is : " + channel);
                return channel;
            }
        }
    }

    public static string GetChannelChinese()
    {
        switch (Channel)
        {
            case "editor":
                return "编辑器";
            case "AppStore":
                return "AppStore";
            case "yingyongbao":
                return "应用宝";
            case "m360":
                return "_360手机助手";
            case "wandoujia":
                return "豌豆荚";
            case "xiaomi":
                return "小米应用商店";
            case "huawei":
                return "华为应用商店";
            case "oppo":
                return "Oppo应用商店";
            case "vivo":
                return "Vivo应用商店";
            case "lejiaolexue":
                return "乐教乐学";
            default:
                return "未知渠道";
        }
    }
}
