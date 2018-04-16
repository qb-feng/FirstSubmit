using System.Collections.Generic;

/// <summary>
/// 礼物类型
/// </summary>
public enum GiftType
{
    Monster = 1, // 怪物
    Furniture = 2, // 家具
    Seed = 3, // 树籽
    Flotage = 4, // 漂浮物
    Key = 5, // 钥匙
    ItemBox = 7, // 宝箱
}

[System.Serializable]
public class ConfigLessonModel : ConfigLessonBaseModel
{
    /// <summary>
    /// 课程关卡数据1_1_1_1/1_1_1_2
    /// </summary>
    public string levels;

    /// <summary>
    /// 课程类型名字 - Single Letter Sounds
    /// </summary>
    public string typeName;

    /// <summary>
    /// 课程标题 Aa,Bb,Cc
    /// </summary>
    public string titleName;

    /// <summary>
    /// 课程的中文名字 - 字母音
    /// </summary>
    public string chineseComments;


    /// <summary>
    /// 获取当前关卡model处于所有关卡model中的关卡顺序  比如传入modelid为1_1_1 ，会返回1-1 传入model id为2_1_1 会返回 2-13
    /// 参数2为  第1位表示level，第2为表示该关卡model的排序编号
    /// 参数1为  某个关卡ConfigLessonModel的id 比如 1_1_1
    /// 返回值该model的序号
    /// </summary>
    public static int GetConfigLessonModellIndex(string configLessonModelId, out string levelAndIndexString)
    {
        levelAndIndexString = null;
        if (string.IsNullOrEmpty(configLessonModelId))
        {
            UnityEngine.Debug.Log("输入关卡model的id不能为空 ！！！" + configLessonModelId);
            return -0;
        }
        string[] ids = configLessonModelId.Split('_');
        if (ids == null || ids.Length != 3)
        {
            UnityEngine.Debug.Log("输入关卡model的id格式错误 ！！！" + configLessonModelId);
            return 0;
        }

        int index = 0;
        int ids0 = int.Parse(ids[0]);
        int ids2 = int.Parse(ids[2]);
        foreach (var v in ConfigManager.Get<ConfigLessonModel>())
        {
            string[] tempIds = v.id.Split('_');
            int tempIds0 = int.Parse(tempIds[0]);
            if (tempIds0 < ids0)
            {
                ++index;
                continue;
            }
            else if (tempIds0 == ids0)
            {
                int tempIds2 = int.Parse(tempIds[2]);
                if (tempIds2 <= ids2)
                {
                    ++index;
                    continue;
                }
            }
        }
        levelAndIndexString = ids0 + "-" + index;
        //Debug.Log(configLessonModelId + "对应的关卡顺序为：" + levelAndIndexString);
        return index;
    }

    /// <summary>
    /// 根据lessonID 获取lessonModel;
    /// </summary>
    public static ConfigLessonModel GetModel(string lessonId)
    {
        for (int i = 0; i < ConfigManager.Get<ConfigLessonModel>().Count; i++)
        {
            if (ConfigManager.Get<ConfigLessonModel>()[i].id.Equals(lessonId))
            {
                return ConfigManager.Get<ConfigLessonModel>()[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 输入的是book的书id
    /// </summary>
    public static List<ConfigLessonModel> GetLessons(string book)
    {
        var lessons = new List<ConfigLessonModel>();
        foreach (var item in ConfigManager.Get<ConfigLessonModel>())
        {
            string b = item.id.Split('_')[0];
            if (b.Equals(book))
                lessons.Add(item);
        }
        return lessons;
    }
}
