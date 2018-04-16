using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
public class UIAntSoldier : UIBaseLevelGame {

    public static UIAntSoldier Instance { get; set; }

    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        Instance = null;
    }

    private RectTransform UIAntSoldierAnt { get { return GetR("UIAntSoldierAnt"); } }
    private Transform Audio { get { return GetT("Audio"); } }
    private TextMeshProUGUI TextShort { get { return Get<TextMeshProUGUI>("TextShort"); } }
    private TextMeshProUGUI TextLong { get { return Get<TextMeshProUGUI>("TextLong"); } }
    private Transform SelectAnsOne { get { return GetT("SelectAnsOne"); } }
    private Transform SelectAnsTwo { get { return GetT("SelectAnsTwo"); } }
    public bool isRight;
    private Vector3[] AntPath1;
    private Vector3[] AntPath2;
    private Tween antTween;
    private bool tweenIsPlaying;

    //单词
    private List<string> Words = new List<string>();
    private string Current_Answer;
    private string Current_Sentence;
    private bool audio_play = true;

    AudioSource audioSource;

    public override void Refresh()
    {
        AntPath1 = new Vector3[] { new Vector3(-5.9f, 1.4f, 0), new Vector3(-2.9f,1.2f,0),new Vector3(-1.4f, 1f, 0), new Vector3(0.1f, 1.05f, 0) };
        AntPath2 = new Vector3[] { new Vector3(3.4f, 1.5f, 0), new Vector3(5.4f, 1.7f,0),new Vector3(8f,1.4f,0),new Vector3(10.2f,1.3f,0) };
        UGUIEventListener.Get(Audio).onPointerClick = OnClickPlay;       
    }
    private void OnClickPlay(PointerEventData data)
    {
        //Audio.DOScale(1.2f, 1f).From();
        switch (StartData.dataType)
        {
            case DataType.Word:
                if (CurrentWord.PlaySound().isPlaying)
                    return;
                CurrentWord.PlaySound();
                break;
            case DataType.Ask:
                if (audio_play)
                {
                    audio_play = false;
                    StartCoroutine(PlayAskAudio());
                }
                break;
            case DataType.Pronunciation:
                if (audio_play)
                {
                    audio_play = false;
                    StartCoroutine(PlayPronunciationAudio());
                }
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

    public override void PlayGame()
    {       
        if (IsGameEnd)
            return;
        Init();
        switch (StartData.dataType)
        {
            case DataType.Word:
                SetWord();
                break;
            case DataType.Ask:
                SetAsk();
                break;
            case DataType.Pronunciation:
                SetPronunciation();
                break;
            case DataType.Say:
                SetSay();
                break;
        }
        TextShort.transform.DOScale(1.5f, 1.5f).From();
        TextShort.DOFade(0, 1.5f).From();
    }

    public bool JudgeAnswer(string compare)
    {
        if (compare == Current_Answer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Decidestar(bool TrueOrFalse)
    {
        if (TrueOrFalse == true)
        {           
            switch (StartData.dataType)
            {
                case DataType.Word:
                    CurrentWord.PlaySound();
                    TextLong.enabled = true;
                    TextLong.text = Current_Answer;
                    TextLong.transform.DOScale(1.5f, 2f).From();
                    //对了飞星
                    UITopBarStarFly  fly = FlyStar(true, true);
                    fly.OnComplete += ()=>
                    {
                        StartCoroutine(flyWaitAnt());                 
                    };
                    break;
                case DataType.Ask:
                    if (audio_play)
                    {
                        audio_play = false;
                        StartCoroutine(PlayAskAudio());
                    }                   
                    TextLong.text = Current_Sentence.Replace("____", Current_Answer);
                    TextLong.transform.DOScale(1.5f, 2f).From();
                    //对了飞星
                    UITopBarStarFly fly1 = FlyStar(true, false);
                    fly1.OnComplete += () =>
                    {
                        StartCoroutine(flyWaitAnt());                
                
                    };
                    break;
                case DataType.Pronunciation:
                    AudioManager.Instance.Play(CurrentPronunciationRealWord);
                    TextLong.enabled = true;
                    TextLong.text = CurrentPronunciationRealWord;
                    TextLong.transform.DOScale(1.5f, 2f).From();
                    //对了飞星
                    UITopBarStarFly fly2 = FlyStar(true, false);
                    fly2.OnComplete += () =>
                    {
                        StartCoroutine(flyWaitAnt());                     
                    };
                    break;
                case DataType.Say:
                   if (audio_play)
                   {
                       if (audioSource != null) 
                        audio_play = false;
                        StartCoroutine(PlaySayAudio());
                    }                   
                    TextLong.text = Current_Sentence.Replace("____", Current_Answer);
                    TextLong.transform.DOScale(1.5f, 2f).From();
                    //对了飞星
                    UITopBarStarFly fly3 = FlyStar(true, false);
                    fly3.OnComplete += () =>
                    {
                        StartCoroutine(flyWaitAnt());                     
                    };
                    break;
            }            
            if (tweenIsPlaying)
            {
                antTween.onComplete = () =>
                {
                    UIAntSoldierAnt.DOPath(AntPath2, 4f);
                };
            }
            else
            {
                UIAntSoldierAnt.DOPath(AntPath2, 4f);
            }           
           
        }
        else
        {
            FlyStar(false, false);
        }
    }

    IEnumerator flyWaitAnt()
    {
        yield return new WaitForSeconds(2.5f);
        if (audioSource != null)
            audioSource.Stop();
        PlayGame();
    }

    private void Init()
    {
        DOTween.KillAll();
        antTween = null;
        SelectAnsOne.ClearAllChild();
        SelectAnsTwo.ClearAllChild();
        TextShort.enabled = false;
        TextLong.enabled = false;
        UIAntSoldierAnt.anchoredPosition = new Vector2(-911f, 155f);
        antTween = UIAntSoldierAnt.DOPath(AntPath1, 3f);
        tweenIsPlaying = true;
        antTween.onComplete = () =>
        {
            tweenIsPlaying = false;
        };
    }

    private void SetWord()
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
        int RandomAnswer = Random.Range(0,2);
        Words.Insert(RandomAnswer, CurrentWord.word);

        CreateUIItem("UIAntSoldierSelect", SelectAnsOne).AddComponent<UIAntSoldierSelect>().InitWord(Words[0]);
        CreateUIItem("UIAntSoldierSelect", SelectAnsTwo).AddComponent<UIAntSoldierSelect>().InitWord(Words[1]);

        //TextShort.enabled = true;
        TextShort.text = CurrentWord.word;
        Current_Answer = CurrentWord.word;
        audioSource= CurrentWord.PlaySound();
    }
    private void SetAsk()
    {
        List<ConfigAskLibraryModel> list = new List<ConfigAskLibraryModel>();
        foreach (var item in m_originalAskList)
        {
            list.Add(item);
        }
        for (int i = 0; i < list.Count; i++)
        {
            Words.Add(list[i].MapWordString);
        }

        //*****不知道前人写来干嘛用的*****
        //*****不用也没什么Bug*****
        string s = CurrentAsk.id.Split('_')[0] + CurrentAsk.id.Split('_')[1];

        for (int i = 0; i < ConfigManager.Get<ConfigWordLibraryModel>().Count; i++)
        {
            string w = ConfigManager.Get<ConfigWordLibraryModel>()[i].id;
            if(s==w.Split('_')[0]+w.Split('_')[1])
            {
                string word = ConfigManager.Get<ConfigWordLibraryModel>()[i].word;
                if (Words.Contains(word))
                {
                    Words.Add(word);
                        break;
                }
            }
        }       
        
        Current_Answer = CurrentAsk.MapWordString;
        Current_Sentence = CurrentAsk.ask.Replace('_', ' ') + "  " + CurrentAsk.yes.Replace('_', ' ') + "  " + CurrentAsk.no.Replace('_', ' ');
        
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
        StartCoroutine(PlayAskAudio());
        
        //配置meshText
        Words.Remove(Current_Answer);
        Utility.RandomSortList(Words);
        int RandomAnswer = Random.Range(0, 2);
        Words.Insert(RandomAnswer, Current_Answer);
        
        CreateUIItem("UIAntSoldierSelect", SelectAnsOne).AddComponent<UIAntSoldierSelect>().InitWord(Words[0]);
        CreateUIItem("UIAntSoldierSelect", SelectAnsTwo).AddComponent<UIAntSoldierSelect>().InitWord(Words[1]);
        TextLong.enabled = true;
        TextLong.text = Current_Sentence;
        
    }
    IEnumerator PlayAskAudio()
    {
        var length = AudioManager.Instance.Play(CurrentAsk.id).clip.length;
        yield return new WaitForSeconds(length);
        audioSource= AudioManager.Instance.Play(CurrentAsk.answerSound);
        length = audioSource.clip.length;
        yield return new WaitForSeconds(length);
        audio_play = true;
    }
    private void SetPronunciation()
    {      
        List<string> list = new List<string>();
        foreach (var item in m_originalPronunciationList)
        {
            if (item.Contains("-1"))
            {
                item.Replace("-1", "");
            }
            if (item.Contains("-2"))
            {
                item.Replace("-2", "");
            }
            list.Add(item);
        }

        if (CurrentPronunciationRealWord.Contains("-1"))
        {
            CurrentPronunciationRealWord.Replace("-1", "");
        }
        if (CurrentPronunciationRealWord.Contains("-2"))
        {
            CurrentPronunciationRealWord.Replace("-2", "");
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (CurrentPronunciationRealWord.Contains(list[i]))
            {
                Current_Answer = list[i];
                break;
            }
        }
        audio_play = false;
        StartCoroutine(PlayPronunciationAudio());

        //配置MeshText
        list.Remove(Current_Answer);
        int RandomAnswer = Random.Range(0, 2);
        list.Insert(RandomAnswer, Current_Answer);
        CreateUIItem("UIAntSoldierSelect", SelectAnsOne).AddComponent<UIAntSoldierSelect>().InitWord(list[0]);
        CreateUIItem("UIAntSoldierSelect", SelectAnsTwo).AddComponent<UIAntSoldierSelect>().InitWord(list[1]);
        TextLong.enabled = true;
        //TextShort.enabled = true;
        
        switch (Current_Answer.Length)
        {
            case 1:
                TextLong.text = CurrentPronunciationRealWord.Replace(Current_Answer, "_");
                break;
            case 2:
                TextLong.text = CurrentPronunciationRealWord.Replace(Current_Answer, "__");
                break;
            case 3:
                TextLong.text = CurrentPronunciationRealWord.Replace(Current_Answer, "___");
                break;
            case 4:
                TextLong.text = CurrentPronunciationRealWord.Replace(Current_Answer, "____");
                break;
        }        
        
    }
    IEnumerator PlayPronunciationAudio()
    {
        audioSource= AudioManager.Instance.Play(CurrentPronunciationRealWord);
        var length = audioSource.clip.length;
        yield return new WaitForSeconds(length);
        audio_play = true;
    }
    private void SetSay()
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
            if(Current_Sentence.Contains(Current_Answer.Split(' ')[i]))
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
        Words.Remove(Current_Answer);
        Utility.RandomSortList(Words);
        int RandomAnswer = Random.Range(0, 2);
        Words.Insert(RandomAnswer, Current_Answer);
        CreateUIItem("UIAntSoldierSelect", SelectAnsOne).AddComponent<UIAntSoldierSelect>().InitWord(Words[0]);
        CreateUIItem("UIAntSoldierSelect", SelectAnsTwo).AddComponent<UIAntSoldierSelect>().InitWord(Words[1]);
        TextLong.enabled = true;
        TextLong.text = Current_Sentence;

    }
    IEnumerator PlaySayAudio()
    {
        audioSource=AudioManager.Instance.Play(CurrentSay.answerSound);
        var length = audioSource.clip.length;
        yield return new WaitForSeconds(length);
        audio_play = true;
    }
}
