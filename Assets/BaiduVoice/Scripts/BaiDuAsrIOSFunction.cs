using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
/// ios 的方法
/// </summary>
public class BaiDuAsrIOSFunction
{

    #region 需要调用ios的方法

#if UNITY_IOS
    /// <summary>
    /// 初始化方法
    /// </summary>
    [DllImport("__Internal")]
    public static extern void InitBaiDuVoiceManager();

    /// <summary>
    /// 开始录音的方法
    /// </summary>
    [DllImport("__Internal")]
    public static extern void BaiDu_StartRecordVoice(string apikey, string secretkey, string appid, string filepath, int pid);

    /// <summary>
    /// 取消录音的方法
    /// </summary>
    [DllImport("__Internal")]
    public static extern void BaiDu_CancleRecordVoice();

    /// <summary>
    /// 结束录音的方法 - 等待识别完成
    /// </summary>
    [DllImport("__Internal")]
    public static extern void BaiDu_StopRecordVoice();

    /// <summary>
    /// 注销识别引擎
    /// </summary>
    [DllImport("__Internal")]
    public static extern void BaiDu_ReleaseRecordVoice();
#endif

    #endregion
}
