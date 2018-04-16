public partial class ServerInfo
{
    /// <summary>
    /// 逻辑服务器地址
    /// </summary>
    private static string s_logicServer = ""; //内网测试服务器 : http://192.168.1.212:8090
    public static string LogicServer
    {
        get
        {
            if (string.IsNullOrEmpty(s_logicServer))
                s_logicServer = "";//ConfigChannelModel.GetModel().ServerAddress;
            return s_logicServer;
        }
        set { s_logicServer = value; }
    }

    public static string GetUrl(string method)
    {
        return Utility.CombineUrl(LogicServer, method);
    }
}
