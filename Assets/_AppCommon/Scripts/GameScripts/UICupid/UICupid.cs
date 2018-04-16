using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
public class UICupid : UIBaseLevelGame
{
    private RectTransform ImageCupid { get { return Get<RectTransform>("ImageCupid"); } }
    private GameObject ImageLaba { get { return Get("ImageLaba"); } }
    private GameObject BtnLeft { get { return Get("BtnLeft"); } }
    private GameObject BtnRight { get { return Get("BtnRight"); } }
    private Transform ArrowPos { get { return GetT("ArrowPos"); } }
    private Transform BallonPos { get { return GetT("BallonPos"); } }
    private List<string> Words = new List<string>();
    private string Current_Word;//不管是 单词 还是句子都用这个。
    private float speed = 50;
    private bool audio_play = true;
    public override void Refresh()
    {
//        UGUIEventListener.Get(BtnLeft).onPointerClick = ButtonLeft;
        UGUIEventListener.Get(BtnRight).onPointerClick = ButtonRight;
        UGUIEventListener.Get(ImageLaba).onPointerClick = AudioButtonClick;
        ImageCupid.DOAnchorPosY(0, 3).SetLoops(-1, LoopType.Yoyo);
    }
    
    void Start()
    {
        StartCoroutine(CreateBallon());
    }
    private void AudioButtonClick(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (audio_play)
        {
            audio_play = false;
            switch (StartData.dataType)
            {
                case DataType.Word:
                    StartCoroutine(PlayWordAudio());
                    break;
                case DataType.Ask:
                    StartCoroutine(PlayAskAudio());
                    break;
                case DataType.Say:
                    StartCoroutine(PlaySayAudio());
                    break;
            }
        }
    }
    private void ButtonLeft(UnityEngine.EventSystems.PointerEventData t)
    {
        var world = UIManager.Instance.WorldCamera.ScreenToWorldPoint(t.position);
        var localTrackPoint = transform.InverseTransformPoint(world);
        var localPlayer = transform.InverseTransformPoint(ImageCupid.position);
        float distance = Mathf.Abs(localTrackPoint.x - localPlayer.x);
        float time = distance / speed;
//        Debug.Log(distance + "   " + time);
        ImageCupid.DOAnchorPosY(localTrackPoint.y, time);
    }
    private void ButtonRight(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (Waitone)
        {
            StartCoroutine(WaitSecond());
            AudioManager.Instance.Play("CupidShoot");
            CreateUIItem<UICupidArrow>(ArrowPos).Init(this);
        }
    }

    private bool Waitone = true;
    IEnumerator WaitSecond()
    {
        Waitone = false;
        yield return new WaitForSeconds(0.5f);
        Waitone = true;
    }

    private string RefreshWord()
    {
        Words = Utility.RandomSortList(Words);
        string word = "";
        if (Random.Range(1, 11) < 5)
            word = Current_Word;
        else
            word = Words[0];

        return word;
    }
    IEnumerator CreateBallon()
    {
        while (true)
        {
            CreateUIItem<UICupidBallonItem>(BallonPos).Init(RefreshWord(), this);
            yield return new WaitForSeconds(1.5f);
        }
    }
    
    public override void PlayGame()
    {
        if (IsGameEnd)
            return;


        isNext = true;

        Words.Clear();
        switch (StartData.dataType)
        {
            case DataType.Word:
                SetWord();
                break;
            case DataType.Ask:
                SetAsk();
                break;
            case DataType.Say:
                SetSay();
                break;
        }
    }
    /// <summary>
    ///  单词
    /// </summary>
    private void SetWord()
    {
        List<ConfigWordLibraryModel> list = new List<ConfigWordLibraryModel>();
        foreach (var item in m_randomWordList)
        {
            list.Add(item);
        }

        for (int i = 0; i < list.Count; i++)
        {
            Words.Add(list[i].word);
        }

        Current_Word = CurrentWord.word;

        Words = Utility.RandomSortList(Words);
        audio_play = false;
        StartCoroutine(PlayWordAudio());
    }
    IEnumerator PlayWordAudio()
    {
        var length = AudioManager.Instance.Play(CurrentWord.word).clip.length;
        yield return new WaitForSeconds(length);
        audio_play = true;
    }
    /// <summary>
    ///  疑问句
    /// </summary>
    private void SetAsk()
    {
        List<ConfigAskLibraryModel> list = new List<ConfigAskLibraryModel>();
        foreach (var item in m_randomAskList)
        {
            list.Add(item);
        }

        for (int i = 0; i < list.Count; i++)
        {
            Words.Add(list[i].MapWordString);
        }

//        string s = CurrentAsk.id.Split('_')[0] + CurrentAsk.id.Split('_')[1];
//        for (int i = 0; i < ConfigManager.Get<ConfigWordLibraryModel>().Count; i++)
//        {
//            string w = ConfigManager.Get<ConfigWordLibraryModel>()[i].id;
//            if (s == w.Split('_')[0] + w.Split('_')[1])
//            {
//                string word = ConfigManager.Get<ConfigWordLibraryModel>()[i].word;
//                if (!Words.Contains(word))
//                {
//                    Words.Add(word);
//                }
//            }
//        }

        Current_Word = CurrentAsk.MapWordString;

        Words = Utility.RandomSortList(Words);
        audio_play = false;
        StartCoroutine(PlayAskAudio());
    }
    IEnumerator PlayAskAudio()
    {
        var length = AudioManager.Instance.Play(CurrentAsk.id).clip.length;
        yield return new WaitForSeconds(length);
        length = AudioManager.Instance.Play(CurrentAsk.answerSound).clip.length;
        yield return new WaitForSeconds(length);
        audio_play = true;
    }
    /// <summary>
    ///  疑问句
    /// </summary>
    private void SetSay()
    {
        List<ConfigSayLibraryModel> list = new List<ConfigSayLibraryModel>();
        foreach (var item in m_randomSayList)
        {
            list.Add(item);
        }

        for (int i = 0; i < list.Count; i++)
        {
            Words.Add(list[i].MapWordString);
        }

        //        string s = CurrentSay.id.Split('_')[0] + CurrentSay.id.Split('_')[1];
        //        for (int i = 0; i < ConfigManager.Get<ConfigWordLibraryModel>().Count; i++)
        //        {
        //            string w = ConfigManager.Get<ConfigWordLibraryModel>()[i].id;
        //            if (s == w.Split('_')[0] + w.Split('_')[1])
        //            {
        //                string word = ConfigManager.Get<ConfigWordLibraryModel>()[i].word;
        //                if (!Words.Contains(word))
        //                {
        //                    Words.Add(word);
        //                }
        //            }
        //        }

        Current_Word = CurrentSay.MapWordString;

        Words = Utility.RandomSortList(Words);
        audio_play = false;
        StartCoroutine(PlaySayAudio());
    }
    IEnumerator PlaySayAudio()
    {
        var length = AudioManager.Instance.Play(CurrentSay.answerSound).clip.length;
        yield return new WaitForSeconds(length);
        audio_play = true;
    }

    private void CheckMatch(bool b = false)
    {
        if (b)
        {
            UITopBarStarFly fly;
            switch (StartData.dataType)
            {
                case DataType.Word:
                    fly = FlyStar(true);
                    fly.OnComplete += () =>
                    {
                        PlayGame();
                    };
                    break;
                case DataType.Ask:
                    AudioManager.Instance.Play(CurrentAsk.MapWordString);
                    fly = FlyStar(CurrentAsk.id, true);
                    fly.OnComplete += () =>
                    {
                        PlayGame();
                    };
                    break;
                case DataType.Say:
                    AudioManager.Instance.Play(CurrentSay.MapWordString);
                    fly = FlyStar(CurrentSay.id, true);
                    fly.OnComplete += () =>
                    {
                        PlayGame();
                    };
                    break;
            }
        }
        else
        {
            switch (StartData.dataType)
            {
                case DataType.Word:
                    FlyStar(false);
                    break;
                case DataType.Ask:
                    FlyStar(CurrentAsk.id, false);
                    break;
                case DataType.Say:
                    FlyStar(CurrentSay.id, false);
                    break;
            }
        }
    }

    private bool isNext = true;//为了不让一个单词重复的飞星，必须等到playgame刷新了到下一个单词才能再检测飞星。
    public void Check(string word)
    {
        if (isNext)
        {
            if (word == Current_Word)
            {
                isNext = false;
                CheckMatch(true);
            }
            else
            {
                CheckMatch();
            }
        }
    }
}