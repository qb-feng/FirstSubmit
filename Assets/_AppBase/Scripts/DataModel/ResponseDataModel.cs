/// <summary>
/// 响应请求的的数据结构
/// </summary>
[System.Serializable]
public class ResponseDataModel
{
    /// <summary>
    /// 请求返回状态
    /// </summary>
    public int s;
    /// <summary>
    /// 请求返回的Token
    /// </summary>
    public string t;
    /// <summary>
    /// 请求返回携带的数据
    /// </summary>
    public string d;
    /// <summary>
    /// 附加信息
    /// </summary>
    public MsgModel msg;
}

/// <summary>
/// 附加信息
/// </summary>
[System.Serializable]
public partial class MsgModel
{

}

