using System.Collections.Generic;
[System.Serializable]
public class StatisticsGameDataModel
{
    /// <summary>
    /// 游戏唯一ID
    /// </summary>
    public int gt;
    /// <summary>
    /// 所需数据类型
    /// </summary>
    public string dt;
    /// <summary>
    /// 开始时间
    /// </summary>
    public long s;
    /// <summary>
    /// 结束时间
    /// </summary>
    public long e;
    /// <summary>
    /// 学习的单词
    /// </summary>
    public List<KnowledgeDataModel> data;

    /// <summary>
    /// 统计的类型
    /// </summary>
    public GameStatisticsType statisType;


    public StatisticsGameDataModel()
    {
        data = new List<KnowledgeDataModel>();
    }

    [System.Serializable]
    public class KnowledgeDataModel
    {
        /// <summary>
        /// 考察数据唯一ID
        /// </summary>
        public string id;
        /// <summary>
        /// 单词的错误数
        /// </summary>
        public int w;

        #region 2018年3月30日19:03:57 qiubin新增数据结构
        /// <summary>
        /// 考察的单词
        /// </summary>
        public string word;

        /// <summary>
        /// 考察经历的时间
        /// </summary>
        public long duration;

        /// <summary>
        /// 考察时玩家回答的结果统计
        /// </summary>
        public List<int> result;

        #endregion
    }
}
