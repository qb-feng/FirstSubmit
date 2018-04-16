using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ConfigAskLibraryModel
{
    /// <summary>
    /// 数据唯一编号
    /// </summary>
    public string id;
    /// <summary>
    /// 问句
    /// </summary>
    public string ask;
    /// <summary>
    /// 肯定回答句子
    /// </summary>
    public string yes;
    /// <summary>
    /// 否定回答句子
    /// </summary>
    public string no;
    /// <summary>
    /// 句子干扰项
    /// </summary>
    public string wrong;
    /// <summary>
    /// 问句的答句
    /// </summary>
    public string answer { get { return yes + no; } }
    /// <summary>
    /// 返回答句的音频
    /// </summary>
    public string answerSound
    {
        get
        {
            if (!string.IsNullOrEmpty(yes))
            {
                return yesSound;
            }
            else
            {
                return noSound;
            }
        }
    }

    /// <summary>
    /// 肯定回答音频
    /// </summary>
    public string yesSound { get { return id + "_1"; } }
    /// <summary>
    /// 否定回答音频
    /// </summary>
    public string noSound { get { return id + "_0"; } }

    public Sprite sprite { get { return GetSprite(id); } }

    public Sprite MapWordSprite
    {
        get
        {
            //增加印射需要的类型判断
            switch (ConfigMapWordModel.GetModel(id).type)
            {
                case (int)ConfigMapType.UseIdMap:
                    //使用id的方式获取图片 - 直接加载图片
                    return LoadAssetBundleManager.Instance.Load<Sprite>(id + ".png");
            }

            if (ConfigMapWordModel.GetModel(id).word.ToString().Contains("|"))
            {
                return LoadAssetBundleManager.Instance.Load<Sprite>
                (ConfigMapWordModel.GetModel(id).word.ToString().Split('|')[0] + ".png");
            }
            else
            {
                return LoadAssetBundleManager.Instance.Load<Sprite>
                (ConfigMapWordModel.GetModel(id).word.ToString() + ".png");
            }

        }
    }

    public string MapWordString
    {
        get
        {
            if (ConfigMapWordModel.GetModel(id).word.Contains("|"))
            {
                return ConfigMapWordModel.GetModel(id).word.Split('|')[0];
            }
            else
            {
                return ConfigMapWordModel.GetModel(id).word;
            }
        }
    }

    /// <summary>
    /// 播放问句读音, 连续点击不会打断式播放
    /// </summary>
    /// <returns></returns>
    public AudioSource PlayAskSound()
    {
        return AudioManager.Instance.Play(id, false);
    }

    /// <summary>
    /// 播放答句读音, 连续点击不会打断式播放
    /// </summary>
    /// <returns></returns>
    public AudioSource PlayAnswerSound()
    {
        return AudioManager.Instance.Play(answerSound, false);
    }

    /// <summary>
    /// 播放整个句子读音, 包括问句和答句, 连续点击不会打断式播放
    /// </summary>
    /// <returns></returns>
    public List<AudioSource> PlaySentenceSound()
    {
        return AudioManager.Instance.PlayAppend(false, id, answerSound);
    } 

    /// <summary>
    /// 播放句子映射单词读音, 连续点击不会打断式播放
    /// </summary>
    /// <returns></returns>
    public AudioSource PlayMapWordSound()
    {
        return AudioManager.Instance.Play(MapWordString, false);
    }

    public static Sprite GetSprite(string id)
    {
        string spriteName = "";
        spriteName = ConfigMapWordModel.GetSpriteName(id);
        return LoadAssetBundleManager.Instance.Load<Sprite>(spriteName + ".png");
    }

    public static ConfigAskLibraryModel GetModel(string id)
    {
        return ConfigManager.Get<ConfigAskLibraryModel>().Find(m => m.id == id);
    }

    public static List<ConfigAskLibraryModel> GetAskList(List<string> ids)
    {
        var list = new List<ConfigAskLibraryModel>();
        foreach (var id in ids)
        {
            var model = ConfigManager.Get<ConfigAskLibraryModel>().Find(m => m.id == id);
            if (model != null)
                list.Add(model);
        }
        return list;
    }

    /// <summary>
    /// 获取当前单元所有问句
    /// </summary>
    /// <param name="unitName"></param>
    /// <returns></returns>
    public static List<ConfigAskLibraryModel> GetAskWrongList(List<string> ids)
    {
        var list = new List<ConfigAskLibraryModel>();
        foreach (var askId in ids)
        {
            var askModel = GetModel(askId);
            var wrongAskIds = askModel.wrong.Split('/');
            foreach (var wrongId in wrongAskIds)
            {
                list.Add(GetModel(wrongId));
            }
        }
        return list;
    }

    public static List<ConfigAskLibraryModel> GetAskListByUnitName(string unitName)
    {
        var list = new List<ConfigAskLibraryModel>();
        var model = ConfigManager.Get<ConfigAskLibraryModel>().Find(m => m.id.StartsWith(unitName));
        if (model != null)
            list.Add(model);
        return list;
    }

    /// <summary>
    ///是否存在2选1干扰项。陈述句否定选项时候利用"|"制作正确选项以及干扰选项，"|"前为错误（前边为句子中对应的映射单词，但是否定不选他）,"|"后为正确的干扰选项
    /// </summary>
    public bool IsExistSpecialOption
    {
        get
        {
            if (ConfigMapWordModel.GetModel(id) == null)
            {
                return false;
            }
            string word = ConfigMapWordModel.GetModel(id).word;
            return word.Contains("|");
        }
    }
    /// <summary>
    /// 干扰选项
    /// </summary>
    public string SpecialOption
    {
        get
        {
            if (ConfigMapWordModel.GetModel(id) == null)
            {
                return null;
            }
            string word = ConfigMapWordModel.GetModel(id).word;
            if (word.Contains("|"))
            {
                return word.Split('|')[1];
            }
            else
            {
                Debug.Log("SpecialOptionIsNull");
                return null;
            }
        }
    }

    /// <summary>
    /// 句子表述中是否出现多个关键词
    /// </summary>
    public bool IsExistSpecialSprite
    {
        get
        {
            if (ConfigMapWordModel.GetModel(id) == null)
            {
                return false;
            }
            string specialSprite = ConfigMapWordModel.GetModel(id).special;
            if (string.Empty == specialSprite)
                return false;
            else
                return true;
        }
    }

    /// <summary>
    /// 句子表述中出现多个关键词时为特殊项配置符合句子信息图片，0为需要的正确表述图片，1为干扰图片
    /// </summary>
    public List<Sprite> SpecialSprite
    {
        get
        {
            List<Sprite> temp = new List<Sprite>();
            if (IsExistSpecialSprite)
            {
                temp.Add(LoadAssetBundleManager.Instance.Load<Sprite>(ConfigMapWordModel.GetModel(id).id + ConfigMapWordModel.GetModel(id).special.Split('|')[0] + ".png"));
                temp.Add(LoadAssetBundleManager.Instance.Load<Sprite>(ConfigMapWordModel.GetModel(id).id + ConfigMapWordModel.GetModel(id).special.Split('|')[1] + ".png"));
            }
            else
                temp = null;
            return temp;
        }
    }
}
