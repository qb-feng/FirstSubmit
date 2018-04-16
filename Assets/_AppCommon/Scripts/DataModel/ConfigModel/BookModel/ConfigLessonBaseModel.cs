using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ConfigLessonBaseModel
{
    /// <summary>
    /// 课程唯一ID
    /// </summary>
    public string id;
    /// <summary>
    /// 课程名
    /// </summary>
    public string name;

    private List<string> m_levels;
    /// <summary>
    /// 配置的关卡数量
    /// </summary>
    public List<string> Levels
    {
        get
        {
            if (m_levels == null)
            {
                m_levels = ConfigLessonLevelModel.GetLevels(id);
            }
            return m_levels;
        }
    }

    private List<string> m_gameLevels;
    public List<string> GameLevels
    {
        get
        {
            if (m_gameLevels == null)
                m_gameLevels = ConfigLessonLevelModel.GetGameLevels(id);
            return m_gameLevels;
        }
    }

    private List<string> m_testLevels;
    public List<string> TestLevels
    {
        get
        {
            if (m_testLevels == null)
                m_testLevels = ConfigLessonLevelModel.GetTestLevels(id);
            return m_testLevels;
        }
    }

    private List<string> m_presentLevels;
    public List<string> PresentLevels
    {
        get
        {
            if (m_presentLevels == null)
                m_presentLevels = ConfigLessonLevelModel.GetPresentLevels(id);
            return m_presentLevels;
        }
    }

    public string BookId
    {
        get
        {
            return id.Split('_')[0];
        }
    }

    public string UnitId
    {
        get
        {
            return BookId + "_" + id.Split('_')[1];
        }
    }
}
