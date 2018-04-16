using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
public class UIHitMole : UIBaseLevelGame
{
    public static UIHitMole Instance { get; set; }
    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        Instance = null;
    }
    //private RectTransform Hammer { get { return GetR("Hammer"); } }
    private Transform MolePos0 { get { return GetT("MolePos0"); } }
    private Transform MolePos1 { get { return GetT("MolePos1"); } }
    private Transform MolePos2 { get { return GetT("MolePos2"); } }
    private Transform MolePos3 { get { return GetT("MolePos3"); } }
    private Transform MolePos4 { get { return GetT("MolePos4"); } }
    private TextMeshProUGUI TextUp { get { return Get<TextMeshProUGUI>("TextUp"); } }
    private Image Audio { get { return Get<Image>("Audio"); } }
    public Tween hammerTween;
    private List<Transform> MolePos;
    private List<string> Words;
    public static float ItemRunTime = 1f;//item上下移动的时间
    private Image WordImage { get { return Get<Image>("WordImage"); } }//单词图片
    private AudioSource asource = null;

    public override void Refresh()
    {
        UGUIEventListener.Get(Audio).onPointerClick = (c) => { OnClickPlaying(); };
    }
    private void OnClickPlaying()
    {
        if (asource != null && asource.isPlaying)
            return;
        asource = CurrentWord.PlaySound();

    }
    public override void PlayGame()
    {
        StopCoroutine("CtrlAutoPlayAudio");
        if (IsGameEnd)
            return;
        Init();
        OnClickPlaying();
        CreateMole();

        StartCoroutine("CtrlAutoPlayAudio");
    }

    private void Init()
    {
        MolePos = new List<Transform>();
        Words = new List<string>();
        MolePos.Add(MolePos0);
        MolePos.Add(MolePos1);
        MolePos.Add(MolePos2);
        MolePos.Add(MolePos3);
        MolePos.Add(MolePos4);

        for (int i = 0; i < MolePos.Count; i++)
        {
            MolePos[i].ClearAllChild();
        }
        TextUp.text = CurrentWord.word;
        Audio.sprite = CurrentWord.sprite;
        Audio.SetNativeSize();

        List<ConfigWordLibraryModel> list = new List<ConfigWordLibraryModel>();
        foreach (var item in m_randomWordList)
        {
            list.Add(item);
        }
        list = Utility.RandomSortList(list);
        if (list.Count > 2) // 如果超过3个单词，则要缩减一下。
        {
            while (list.Count > 2)
            {
                list.Remove(list[Random.Range(0, list.Count)]);
            }
        }

        if (!list.Contains(CurrentWord))
        {
            list[0] = CurrentWord;
        }
        list = Utility.RandomSortList(list);

        Words.Add(list[0].word);
        Words.Add(list[1].word);

        //单词图片
        WordImage.sprite = CurrentWord.sprite;

    }

    public void CreateMole()
    {
        for (int i = 0; i < MolePos.Count; i++)
        {
            MolePos[i].ClearAllChild();
        }
        int randomIndexRight = Random.Range(0, MolePos.Count);
        CreateUIItem("UIHitMoleMole", MolePos[randomIndexRight]).AddComponent<UIHitMoleMole>().InitMole(Words[0]);
        int randomIndexWrong = Random.Range(0, MolePos.Count - 1);
        while (randomIndexRight == randomIndexWrong)
        {
            randomIndexWrong = Random.Range(0, MolePos.Count - 1);
        }
        CreateUIItem("UIHitMoleMole", MolePos[randomIndexWrong]).AddComponent<UIHitMoleMole>().InitMole(Words[1]);
        int randomIndexMine = Random.Range(0, MolePos.Count - 2);
        while (randomIndexMine == randomIndexRight || randomIndexMine == randomIndexWrong)
        {
            randomIndexMine = Random.Range(0, MolePos.Count - 2);
        }
        CreateUIItem("UIHitMoleMine", MolePos[randomIndexMine]).AddComponent<UIHitMoleMine>();
    }

    public void KillAllAnimation()
    {
        for (int i = 0; i < MolePos.Count; i++)
        {
            if (MolePos[i].childCount > 0)
            {
                if (MolePos[i].GetChild(0).GetComponent<UIHitMoleMole>() != null)
                {
                    MolePos[i].GetChild(0).GetComponent<UIHitMoleMole>().KillAnimation();
                }
                else
                {
                    MolePos[i].GetChild(0).GetComponent<UIHitMoleMine>().KillAnimation();
                }
            }
            else
            {
                continue;
            }
        }
    }


    #region 增加自动播放声音
    private float autoPlayAudioWaitTime = -1f;//自动播放声音的停顿时间
    private float AutoPlayAudioWaitTime
    {
        get
        {
            if (autoPlayAudioWaitTime == -1f)
            {
                autoPlayAudioWaitTime = float.Parse(ConfigGlobalValueModel.GetValue("GamePlayAudioWaitTime"));
            }
            return autoPlayAudioWaitTime;
        }
    }
    /// <summary>
    /// 控制自动播放声音的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator CtrlAutoPlayAudio()
    {
        yield return 0;

        var wf = new WaitForSeconds(AutoPlayAudioWaitTime);

        while (true)
        {
            if (asource == null || !asource.isPlaying)
            {
                yield return wf;
                OnClickPlaying();
            }
            yield return 0;
        }
    }

    #endregion

}
