using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ConfigPronunciationModel
{
    public const string PrefixAudio = "pronunciation_";

    /// <summary>
    /// 发音数据ID
    /// </summary>
    public string id;
    /// <summary>
    /// 发音
    /// </summary>
    public string pronunciation;
    /// <summary>
    /// 包含发音的单词 以/划分多个单词
    /// </summary>
    public string word;

    /// <summary>
    /// 新版图集加载方式 - 以id命名加载图集
    /// </summary>
    public Sprite sprite
    {
        get
        {
            var result = LoadFirstLeapResourceManager.Instance.LoadResource<Sprite>(pronunciation);
            if (result == null)
                result = LoadAssetBundleManager.Instance.Load<Sprite>(pronunciation);
            return result;
        }
    }

    public static ConfigPronunciationModel GetModel(string id)
    {
        return ConfigManager.Get<ConfigPronunciationModel>().Find(m => m.id == id);
    }

    private static AudioSource m_audio;
    public static AudioSource PlayPronunciation(string pronunciation)
    {
        if (!(m_audio && m_audio.isPlaying))
        {
            m_audio = AudioManager.Instance.Play(pronunciation);
            return m_audio;
        }
        return null;
    }



    #region 2018年4月2日13:43:50 qiubin新修改的数据结构
    /// <summary>
    /// 获取音标的list 参数：要获取的音标的id
    /// </summary>
    public static List<ConfigPronunciationModel> GetPronunciationList(List<string> ids)
    {
        var list = new List<ConfigPronunciationModel>();
        var allPro = ConfigManager.Get<ConfigPronunciationModel>();
        foreach (var v in ids)
        {
            var model = allPro.Find((a) => { return a.id.Equals(v); });
            if (model != null)
                list.Add(model);
        }

        return list;
    }


    private string currentWord = null;
    /// <summary>
    /// 当前音标对应的单词
    /// </summary>
    public string CurrentCorrespondWord
    {
        get
        {
            if (currentWord == null)
            {
                var words = GetCurrentCorrespondWords();
                currentWord = words[Random.Range(0, words.Count)];
            }
            return currentWord;
        }
    }

    /// <summary>
    /// 获取当前音标对应的单词数据
    /// </summary>
    public List<string> GetCurrentCorrespondWords()
    {
        return new List<string>(word.Split('/'));
    }

    private AudioSource audioSource = null;
    /// <summary>
    /// 播放当前音标的声音 - 是否打断式播放
    /// </summary>
    public AudioSource PlayCurrentPronunciationAudio(bool isForce = true)
    {
        if (isForce && audioSource != null)
        {
            audioSource.Stop();
        }
        audioSource = AudioManager.Instance.Play(pronunciation);

        return audioSource;
    }



    #endregion
}