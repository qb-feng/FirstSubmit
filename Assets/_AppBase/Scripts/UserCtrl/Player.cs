using UnityEngine;
using System.Collections;

public partial class Player : BaseInstance<Player>
{
    /// <summary>
    /// 用户数据模型
    /// </summary>
    private UserModel m_userModel;

    public string Id
    {
        get { return m_userModel.id; }
        set { m_userModel.id = value; }
    }

    public string Token
    {
        get { return m_userModel.t; }
        set { m_userModel.t = value; }
    }
}
