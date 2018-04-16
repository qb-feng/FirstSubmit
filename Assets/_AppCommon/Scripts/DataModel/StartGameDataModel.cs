using System.Collections.Generic;

public class StartGameDataModel
{
    /// <summary>
    /// 游戏ID
    /// </summary>
    public int gameId;
    /// <summary>
    /// 开始游戏所需数据类型
    /// </summary>
    public DataType dataType;
    /// <summary>
    /// 游戏所需数据
    /// </summary>
    public List<string> dataList;
    /// <summary>
    /// 游戏星数
    /// </summary>
    public int star;
    /// <summary>
    /// 是否需要引导
    /// </summary>
    public bool guide;
    /// <summary>
    /// 难度等级
    /// </summary>
    public int difficulty;
    /// <summary>
    /// 游戏描述
    /// </summary>
    public string describe;
}
