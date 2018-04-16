using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class UIDogJumping : UIBaseLevelGame
{

    public static UIDogJumping Instance { get; set; }
    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        Instance = null;
    }

    //三条绳子
    private Transform Up { get { return GetT("Up"); } }
    private Transform Center { get { return GetT("Center"); } }
    private Transform Down { get { return GetT("Down"); } }

    //三个浮动的板子
    private GameObject UpBoard;
    private GameObject CenterBoard;
    private GameObject DownBoard;

    //播放声音
    private Transform Audio { get { return GetT("Audio"); } }
    //小鱼出生位置
    private Transform FishBirth { get { return GetT("FishBirth"); } }

    //小猫出生位置
    private Transform CatBirth { get { return GetT("CatBirth"); } }

    //问句
    private TextMeshProUGUI AskText { get { return Get<TextMeshProUGUI>("AskText"); } }
    //单词图片
    private Image AskImage { get { return Get<Image>("AskImage"); } }
    //单词
    private List<string> Words = new List<string>();
    public string Current_Answer;
    private string Current_Sentence;
    private bool audio_play = true;
    public bool audio_in ;

    AudioSource audioSource;

    public Action OnRefreshCat;

    void OnEnable()
    {
        OnRefreshCat = OnRefreshCatHandler;
    }

    void OnDisable()
    {
        OnRefreshCat = null;
    }
    void OnApplicationQuit()
    {
        OnRefreshCat = null;
    }

    public override void Refresh()
    {
        Up.ClearAllChild();
        Center.ClearAllChild();
        Down.ClearAllChild();
        CatBirth.ClearAllChild();
        FishBirth.ClearAllChild();
        InitCat();
        UpBoard=CreateUIItem("UIDogJumpingSpringboard", Up);
        UpBoard.AddComponent<UIDogJumpingMove>().Init(1, 2.5f);
        CenterBoard = CreateUIItem("UIDogJumpingSpringboard", Center);
        CenterBoard.AddComponent<UIDogJumpingMove>().Init(-1, 3f);
        DownBoard = CreateUIItem("UIDogJumpingSpringboard", Down);
        DownBoard.AddComponent<UIDogJumpingMove>().Init(1, 2f);

        UGUIEventListener.Get(Audio).onPointerClick = OnAudioClick;
        UGUIEventListener.Get(Audio).onPointerEnter = OnAudioEnter;
        UGUIEventListener.Get(Audio).onPointerExit = OnAudioExit;
        audio_in = false;
    }
    private void OnAudioClick(PointerEventData data)
    {
        //Audio.DOScale(1.2f, 1f).From();
        switch (StartData.dataType)
        {
            case DataType.Word:
                if (CurrentWord.PlaySound().isPlaying)
                    return;
                CurrentWord.PlaySound();
                break;         
            case DataType.Say:
                if (audio_play)
                {
                    audio_play = false;
                    StartCoroutine(PlaySayAudio());
                }
                break;
        }
    }
    private void OnAudioEnter(PointerEventData data)
    {
        audio_in = true;
    }
    private void OnAudioExit(PointerEventData data)
    {
        audio_in = false;
    }

    public override void PlayGame()
    {
 
       if(IsGameEnd)
           return;
       FishBirth.ClearAllChild();
       InitCat();
       CreateCat();
       switch (StartData.dataType)
       {
           case DataType.Word:
               SetWord();
               break;
           case DataType.Say:
               SetSay();
               break;
       }
    }

    void SetWord()
    {
        List<ConfigWordLibraryModel> list = new List<ConfigWordLibraryModel>();
        foreach (var item in m_originalWordList)
        {
            list.Add(item);
        }
        for (int i = 0; i < list.Count; i++)
        {
            Words.Add(list[i].word);
        }
        Words.Remove(CurrentWord.word);
        Utility.RandomSortList(Words);
        int RandomAnswer = UnityEngine.Random.Range(0, 3);
        Words.Insert(RandomAnswer, CurrentWord.word);

        CreateUIItem("UIDogJumpingFish", FishBirth).AddComponent<UIDogJumpingFish>().Init(Words[0]);
        CreateUIItem("UIDogJumpingFish", FishBirth).AddComponent<UIDogJumpingFish>().Init(Words[1]);
        CreateUIItem("UIDogJumpingFish", FishBirth).AddComponent<UIDogJumpingFish>().Init(Words[2]);

        AskImage.enabled = true;
        AskText.enabled = false;
        AskImage.sprite = CurrentWord.sprite;        
        //AskImage.SetNativeSize();
        Current_Answer = CurrentWord.word;
        audioSource = CurrentWord.PlaySound();
    }
    void SetSay()
    {
        List<ConfigSayLibraryModel> list = new List<ConfigSayLibraryModel>();
        foreach (var item in m_originalSayList)
        {
            list.Add(item);
        }
        for (int i = 0; i < list.Count; i++)
        {
            Words.Add(list[i].MapWordString);
        }
        string s = CurrentSay.id.Split('_')[0] + CurrentSay.id.Split('_')[1];
        for (int i = 0; i < ConfigManager.Get<ConfigWordLibraryModel>().Count; i++)
        {
            string w = ConfigManager.Get<ConfigWordLibraryModel>()[i].id;
            if (s == w.Split('_')[0] + w.Split('_')[1])
            {
                string word = ConfigManager.Get<ConfigWordLibraryModel>()[i].word;
                if (Words.Contains(word))
                {
                    Words.Add(word);
                    break;
                }
            }

        }
        Current_Answer = CurrentSay.MapWordString;
        Current_Sentence = CurrentSay.yes.Replace('_', ' ') + CurrentSay.no.Replace('_', ' ');
        for (int i = 0; i < Current_Answer.Split(' ').Length; i++)
        {
            if (Current_Sentence.Contains(Current_Answer.Split(' ')[i]))
            {
                for (int j = 0; j < Words.Count; j++)
                {
                    if (Words[j] == Current_Answer)
                    {
                        Words[j] = Current_Answer.Split(' ')[i];
                        Current_Answer = Words[j];
                        Current_Sentence = Current_Sentence.Replace(Words[j], "____");
                        break;
                    }
                }
            }
        }
        audio_play = false;
        StartCoroutine(PlaySayAudio());

        //配置meshText
        AskImage.enabled = false;
        AskText.enabled = true;
        Words.Remove(Current_Answer);
        Utility.RandomSortList(Words);
        int RandomAnswer = UnityEngine.Random.Range(0, 3);
        Words.Insert(RandomAnswer, Current_Answer);
        CreateUIItem("UIDogJumpingFish", FishBirth).AddComponent<UIDogJumpingFish>().Init(Words[0]);
        CreateUIItem("UIDogJumpingFish", FishBirth).AddComponent<UIDogJumpingFish>().Init(Words[1]);
        CreateUIItem("UIDogJumpingFish", FishBirth).AddComponent<UIDogJumpingFish>().Init(Words[2]);
        AskText.text = Current_Sentence;
      
    }
    IEnumerator PlaySayAudio()
    {
        audioSource = AudioManager.Instance.Play(CurrentSay.answerSound);
        var length = audioSource.clip.length;
        yield return new WaitForSeconds(length);
        audio_play = true;
    }

    public void DecideStar(bool TrueOrFalse)
    {
        if (TrueOrFalse == true)
        {
            switch (StartData.dataType)
            {
                case DataType.Word:
                    CurrentWord.PlaySound();
                    //TextLong.enabled = true;
                    //TextLong.text = Current_Answer;
                    //TextLong.transform.DOScale(1.5f, 2f).From();
                    //对了飞星
                    UITopBarStarFly  fly1 = FlyStar(true, true);
                    fly1.OnComplete += ()=>
                    {
                        PlayGame();   
                    };            
                    break;
                case DataType.Say:
                    if (audio_play)
                    {
                        if (audioSource != null)
                            audio_play = false;
                        StartCoroutine(PlaySayAudio());
                    }
                    AskText.text = Current_Sentence.Replace("____", Current_Answer);
                    AskText.transform.DOScale(1.5f, 2f).From();
                    //对了飞星
                    UITopBarStarFly fly2 = FlyStar(true, false);
                    fly2.OnComplete += () =>
                    {
                        PlayGame();   
                    };
                    break;
                   
            }
        }
        else
        {
            FlyStar(false, false);
        }
    }
    public void CreateCat()
    {
        CreateUIItem("UIDogJumpingDog", CatBirth).AddComponent<UIDogJumpingDog>();
    }

    private void OnRefreshCatHandler()
    {
        CatBirth.transform.ClearAllChild();
        CreateCat();
    }

    private void InitCat()
    {
        for (int i = 0; i < CatBirth.childCount; i++)
        {
            CatBirth.GetChild(i).gameObject.GetComponent<MeshRenderer>().enabled = false;
            GameObject.Destroy(CatBirth.GetChild(i).gameObject);
        }               
    }

    public void InitSpringboard()
    {
        UpBoard.GetComponent<BoxCollider2D>().isTrigger = true;
        CenterBoard.GetComponent<BoxCollider2D>().isTrigger = true;
        DownBoard.GetComponent<BoxCollider2D>().isTrigger = true;
    }
}

