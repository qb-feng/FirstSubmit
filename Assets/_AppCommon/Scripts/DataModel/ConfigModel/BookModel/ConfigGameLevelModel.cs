using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class ConfigGameLevelModel
{
    /// <summary>
    /// GameID：游戏ID
    /// </summary>
    public string GameID;
    /// <summary>
    /// LevelId：关卡ID
    /// </summary>
    public string LevelID;
    /// <summary>
    /// 界面布局设置
    /// </summary>
    public string Layout;
    /// <summary>
    /// 障碍物位置
    /// </summary>
    public string Obstacle;
    /// <summary>
    /// 小球初始位置
    /// </summary>
    public int BallPosition;
    /// <summary>
    /// 正确答案位置
    /// </summary>
    public int RightPosition;
    /// <summary>
    /// 错误答案位置
    /// </summary>
    public int WrongPosition;


    public static int m_currentStatusNameLevelCount
    {
        get
        {
            return m_currentStatusNameAll.Count;
        }
    }

    //获取当前游戏ID下所有关卡配置
    private static List<ConfigGameLevelModel> m_currentStatusNameAll=new List<ConfigGameLevelModel> ();    
    public static List<ConfigGameLevelModel> GetCurrentStatusNameAll(UIGameName sName)
    {
        m_currentStatusNameAll.Clear();
        foreach (var item in ConfigManager.Get<ConfigGameLevelModel>())
        {
            int cID = -1;
            if (int.TryParse(item.GameID,out cID)){
                if (cID == (int)sName)
                {
                    m_currentStatusNameAll.Add(item);
                }
                else
                {
                    Debug.LogError("当前游戏不存在关卡！请检查");
                }
            }
            else
            {
                Debug.LogError("UIGameName转换出错请检查");
            }           
        }
        return m_currentStatusNameAll;
    }

    //查找并加载相应关卡
    public static char[] FindGameLevel(int level){
        foreach (var item in m_currentStatusNameAll)
        {
            int cID = -1;
            if (int.TryParse(item.LevelID, out cID))
            {                
                if (cID == level)
                {
                   string m_layoutString = item.Layout.Replace("_","");
                   char[] m_layoutCharArray = m_layoutString.ToCharArray();
                   return m_layoutCharArray;
                }
                else
                {
                    //Debug.LogError("当前游戏不存在关卡！请检查");
                    continue;
                }
            }
            else
            {
                Debug.LogError("UIGameName转换出错请检查");
                return null;
            }
        }
        return null;
    }

    //传递小球位置，正确错误选项位置参数
    public static int[] FindPosition(int level)
    {
        foreach (var item in m_currentStatusNameAll)
        {
            int cID = -1;
            if (int.TryParse(item.LevelID, out cID))
            {
                if (cID == level)
                {                  
                    int[] m_positin = new int[] { item.BallPosition,item.RightPosition,item.WrongPosition };
                    return m_positin;
                }
                else
                {
                    //Debug.LogError("当前游戏不存在关卡！请检查");
                    continue;
                }
            }
            else
            {
                Debug.LogError("UIGameName转换出错请检查");
                return null;
            }
        }
        return null;
    }

    //传递障碍位置参数
    public static int[] Findobstacle(int level)
    {
        foreach (var item in m_currentStatusNameAll)
        {
            int cID = -1;
            if (int.TryParse(item.LevelID, out cID))
            {
                if (cID == level)
                {
                    if (item.Obstacle == "")
                    {
                        return null;
                    }
                    else
                    {
                        string[] m_obstacle = item.Obstacle.Split('_');

                        int[] m_positin=new int[m_obstacle.Length];
                     
                        for (int i = 0; i < m_obstacle.Length; i++)
                        {
                            int cint = -1;
                            if (int.TryParse(m_obstacle[i], out cint))
                            {
                                m_positin[i] = cint;
                            }
                            else
                            {
                                break;
                            }
                        }
                        return m_positin;
                    }
                    
                }
                else
                {
                   // Debug.LogError("当前游戏不存在关卡！请检查");
                    continue;
                }
            }
            else
            {
                Debug.LogError("UIGameName转换出错请检查");
                return null;
            }
        }
        return null;
    }
}