using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AudioManager : BaseInstance<AudioManager>, IOnUIChange
{
    [System.Serializable]
    public class UserSettings
    {
        public bool BGMSwitch;
    }

    public class SequenceAudioModel
    {
        public string name;
        public AudioSource audio;
        public float duration;
    }

    #region SoundPath
    private string m_rootPath = "Sounds/";
    private string[] m_childPaths = new string[] { "",
                                                 "Alphabet/",
                                                 "SFX/",
                                                 "Word/",
                                                 "Song/",
                                                 "Pronunciation/"
                                                 };
    #endregion

    #region Field
    private Dictionary<string, AudioSource> m_cacheAudio = new Dictionary<string, AudioSource>();
    private List<SequenceAudioModel> m_sequenceAudio = new List<SequenceAudioModel>();
    private AudioSource m_bgAudio;
    public bool BgAudioActive
    {
        get
        {
            var settings = PersistentManager.LoadObj<UserSettings>(PersistentName.UserSettings);
            if (settings != null)
                return settings.BGMSwitch;
            return true;
        }
        set
        {
            var settings = new UserSettings();
            settings.BGMSwitch = value;
            PersistentManager.SaveByName(PersistentName.UserSettings, JsonUtility.ToJson(settings));
            if (value)
                m_bgAudio.UnPause();
            else
                m_bgAudio.Pause();
        }
    }
    #endregion

    void Awake()
    {
        UIManager.RegisterObserver(this);
    }

    //public void PlayBg()
    //{
    //    m_bgAudio = Play("BgSound", destroy: false, volume: 0.3f);
    //    m_bgAudio.Pause();
    //    m_bgAudio.loop = true;
    //    BgAudioActive = BgAudioActive;
    //}

    public void FreezeBgSound(bool freeze)
    {
        if (BgAudioActive)
        {
            if (m_bgAudio)
                if (freeze)
                    m_bgAudio.Pause();
                else
                    m_bgAudio.UnPause();
        }
    }

    public void OnBeforeChange(Type previous, Type next)
    {
    }

    public void OnAfterChange(Type previous, Type next, UIBaseInit nextUI)
    {
        if (!(nextUI is UIBaseLevelGame))
        {
            foreach (var model in m_sequenceAudio)
            {
                if (model.audio)
                    Destroy(model.audio.gameObject);
            }
            m_sequenceAudio.Clear();
        }
    }

    private AudioClip LoadAudio(string name)
    {
        if (name == "ee")
        {
            name = "Lower_ee";
        }
        else if (name == "Ee")
        {
            name = "Upper_Ee";
        }
        AudioClip clip = null;

        clip = LoadFirstLeapResourceManager.Instance.LoadResource<AudioClip>(name);
        if (clip)
            return clip;

        foreach (string path in m_childPaths)
        {
            clip = Resources.Load<AudioClip>(m_rootPath + path + name);
            if (clip)
                return clip;
        }
        clip = LoadAssetBundleManager.Instance.Load<AudioClip>(name + ".mp3");
        if (clip)
            return clip;

        Debug.LogWarning("The Sound Is Not Found!! Please Check The Sound Name : " + name);
        return clip;
    }

    /// <summary>
    /// 用于想要连续播放几个音频的方法, 默认播放没有间隔时间
    /// 该方法只是一个队列, 所有的累加音频都会按照顺序播放
    /// </summary>
    /// <param name="names">连续播放音频的数组</param>
    /// <returns></returns>
    public List<AudioSource> PlayAppend(params string[] names)
    {
        return PlayAppend(0, true, names);
    }

    /// <summary>
    /// 用于想要连续播放几个音频的方法, 默认播放没有间隔时间
    /// 该方法只是一个队列, 所有的累加音频都会按照顺序播放
    /// </summary>
    /// <param name="force">已经在队列中的音频是否继续添加播放,true表示继续添加播放, false表示返回队列中音频</param>
    /// <param name="names"></param>
    /// <returns></returns>
    public List<AudioSource> PlayAppend(bool force, params string[] names)
    {
        return PlayAppend(0, force, names);
    }

    /// <summary>
    /// 用于想要连续播放几个音频的方法, 增加播放间隔时间配置
    /// 该方法只是一个队列, 所有的累加音频都会按照顺序播放
    /// </summary>
    /// <param name="interval">每个音频播放间隔时间, 默认从第一个音频依次往后增加</param>
    /// <param name="names">连续播放音频的数组</param>
    /// <returns></returns>
    public List<AudioSource> PlayAppend(float interval, bool force, params string[] names)
    {
        var audioList = new List<AudioSource>();
        Action<string, float, float> addAudio = (name, delay, interval2) =>
        {
            var audio = Play(name, destroy: false);
            audio.Stop();
            audio.PlayDelayed(delay + interval2);
            float duration = audio.clip.length + interval2;
            Destroy(audio.gameObject, delay + duration);
            var model = new SequenceAudioModel() { name = name, audio = audio, duration = duration };
            m_sequenceAudio.Add(model);
            audioList.Add(audio);
        };
        for (int i = 0; i < names.Length; i++)
        {
            if (m_sequenceAudio.Count > 0)
            {
                float delayTime = 0;
                var tempRemove = new List<SequenceAudioModel>();
                foreach (var model in m_sequenceAudio)
                {
                    if (model.audio)
                    {
                        delayTime += model.duration - model.audio.time;
                        audioList.Add(model.audio);
                    }
                    else
                    {
                        tempRemove.Add(model);
                    }
                }
                foreach (var model in tempRemove)
                {
                    m_sequenceAudio.Remove(model);
                }

                bool find = false;
                foreach (var model in m_sequenceAudio)
                {
                    if (model.name.Equals(names[i]))
                        find = true;
                    audioList.Add(model.audio);
                }
                if (find)
                    return audioList;

                float tempInterval = 0;
                if (i > 0)
                    tempInterval += interval;
                addAudio(names[i], delayTime, tempInterval);
            }
            else
            {
                addAudio(names[i], 0, 0);
            }
        }
        return audioList;
    }

    public AudioSource Play(string name, bool force = true, bool destroy = true, float volume = 1, float pitch = 1)
    {
        if (!force)
        {
            bool isPlaying = m_cacheAudio.ContainsKey(name) && m_cacheAudio[name];
            if (isPlaying)
                return m_cacheAudio[name];
        }
        AudioClip clip = LoadAudio(name);
        AudioSource source = Play(clip, transform, volume, pitch, destroy);
        if (m_cacheAudio.ContainsKey(name))
            m_cacheAudio[name] = source;
        else
            m_cacheAudio.Add(name, source);
        return source;
    }

    public AudioSource Play(AudioClip clip)
    {
        return Play(clip, transform, 1f, 1f);
    }

    /// <summary>
    /// 在指定物体上播放指定声音, 可控制声音大小, 声音大小范围为0 到 1,  可控制声音播放速度, 范围为0 到 1
    /// </summary>
    /// <param name="clip">声音</param>
    /// <param name="emitter">指定物体</param>
    /// <param name="volume">播放大小</param>
    /// <param name="pitch">播放速度</param>
    /// <returns></returns>
    public AudioSource Play(AudioClip clip, Transform emitter, float volume, float pitch, bool destroy = true)
    {
        AudioSource source = Play(clip, emitter.position, volume, pitch, destroy);
        if (source == null)
        {
            Debug.LogWarning("Audio Source Is Null");
            return null;
        }
        source.transform.SetParent(emitter);
        return source;
    }

    public AudioSource Play(AudioClip clip, Vector3 point, float volume, float pitch, bool destroy = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("Clip Is Null!! Please Check!!");
            return null;
        }
        GameObject go = new GameObject("Audio: " + clip.name);
        go.transform.position = point;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        if (destroy)
            Destroy(source.gameObject, source.clip.length + 0.5f);
        return source;
    }

    private float audioTimeAmount;

    public float PlayThreeAudio(string one, string two, string three, Action onResultBackFunc = null)
    {
        StartCoroutine(PlayThreeAudioIE(one, two, three, onResultBackFunc));
        return audioTimeAmount * 3;
    }

    public IEnumerator PlayThreeAudioIE(string one, string two, string three, Action onResultBackFunc = null)
    {
        audioTimeAmount = 0;
        AudioSource IEAudio = Play(one);
        audioTimeAmount += IEAudio.clip.length;
        yield return new WaitForSeconds(IEAudio.clip.length);
        IEAudio = Play(two);
        audioTimeAmount += IEAudio.clip.length;
        yield return new WaitForSeconds(IEAudio.clip.length);
        IEAudio = Play(three);
        audioTimeAmount += IEAudio.clip.length;
        yield return new WaitForSeconds(IEAudio.clip.length);

        if (onResultBackFunc != null)
            onResultBackFunc();

    }
}