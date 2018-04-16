[System.Serializable]
public enum ConfigMapType
{
    /// <summary>
    /// 使用单词的印射 - 老方法
    /// </summary>
    UseWordMap = 0,
    /// <summary>
    /// 使用Id的印射 - 新模式（暂时作用是用来给问句和单句的图片根据id方式去找）
    /// </summary>
    UseIdMap = 1,
}

[System.Serializable]
public class ConfigMapWordModel
{
    /// <summary>
    /// 句子的唯一编号
    /// </summary>
    public string id;
    /// <summary>
    /// 映射的单词的编号
    /// </summary>
    public string word;
    /// <summary>
    /// 句子中出现多个单词的特殊情况,0|1含义:句子id+0/1表示图片名称,0为真1为假
    /// </summary>
    public string special;
    /// <summary>
    /// 印射单词的类型
    /// </summary>
    public int type;

    public static ConfigMapWordModel GetModel(string id)
    {
        return ConfigManager.Get<ConfigMapWordModel>().Find(m => m.id == id);
    }

    public static string GetSpriteName(string id)
    {
        var model = GetModel(id);
        if (model == null)
            return id;
        else
        {
            if (model.type == (int)ConfigMapType.UseIdMap)
            {
                //使用id方式获取图片
                return id;
            }

            if (model.word.Contains("|"))
            {
                return model.word.Split('|')[0];
            }
            else
            {
                return model.word;
            }
        }
    }

    public static ConfigWordLibraryModel GetWordModel(string id)
    {
        foreach (var item in ConfigManager.Get<ConfigMapWordModel>())
        {
            if (item.id == id)
                return ConfigWordLibraryModel.GetModel(item.word);
        }
        return null;
    }
}
