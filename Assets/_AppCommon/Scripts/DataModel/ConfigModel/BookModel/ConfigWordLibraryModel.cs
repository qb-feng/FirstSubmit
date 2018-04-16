using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ConfigWordLibraryModel
{
    /// <summary>
    /// 单词唯一编号
    /// </summary>
    public string id;
    /// <summary>
    /// 单词
    /// </summary>
    public string word;
    /// <summary>
    /// 单词意思
    /// </summary>
    public string meaning;
    /// <summary>
    /// 单词考察字母
    /// </summary>
    public string letter;

    public Sprite sprite
    {
        get
        {
            var model = LoadFirstLeapResourceManager.Instance.LoadResource<Sprite>(word);
            if (model == null)
                model = GetSprite(word);
            return model;

        }
    }

    public string WordSound
    {
        get
        {
            string tempWord = word;
            var mapModel = ConfigMapWordModel.GetModel(id);
            if (mapModel != null)
                tempWord = mapModel.word;
            return tempWord;
        }
    }

    public AudioSource PlaySound()
    {
        return AudioManager.Instance.Play(WordSound, false);
    }
    public AudioSource PlayLetter()
    {
        return AudioManager.Instance.Play(letter, false);
    }

    public Sprite GetSprite(string word)
    {
        var mapModel = ConfigMapWordModel.GetModel(id);
        if (mapModel != null)
            word = mapModel.word;
        return GetSpriteByWord(word);
    }

    public static Sprite GetSpriteByWordId(string id)
    {
        var wordModel = ConfigWordLibraryModel.GetModel(id);
        string word = wordModel.word;
        var mapModel = ConfigMapWordModel.GetModel(id);
        if (mapModel != null)
            word = mapModel.word;
        return GetSpriteByWord(word);
    }

    public static Sprite GetSpriteByWord(string word)
    {
        return LoadAssetBundleManager.Instance.Load<Sprite>(word.ToString() + ".png");
    }

    public static ConfigWordLibraryModel GetModel(string id)
    {
        return ConfigManager.Get<ConfigWordLibraryModel>().Find(m => m.id == id);
    }

    public static List<ConfigWordLibraryModel> GetWordList(List<string> words)
    {
        var list = new List<ConfigWordLibraryModel>();
        foreach (var word in words)
        {
            var model = ConfigManager.Get<ConfigWordLibraryModel>().Find(m => m.word == word);
            if (model != null)
                list.Add(model);
        }

        if (list.Count == 0)
        {
            foreach (var word in words)
            {
                var model = ConfigManager.Get<ConfigWordLibraryModel>().Find(m => m.id == word);
                if (model != null)
                    list.Add(model);
            }
        }

        return list;
    }

    public static List<ConfigWordLibraryModel> GetWordListByUnitName(string unitName)
    {
        var list = new List<ConfigWordLibraryModel>();
        var model = ConfigManager.Get<ConfigWordLibraryModel>().Find(m => m.id.StartsWith(unitName));
        if (model != null)
            list.Add(model);
        return list;
    }
}
