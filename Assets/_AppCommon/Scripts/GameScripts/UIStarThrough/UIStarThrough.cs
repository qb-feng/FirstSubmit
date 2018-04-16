using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class UIStarThrough : UIBaseLevelGame
{
    private Transform TextWARNING { get { return GetT("TextWARNING"); } }
    private RectTransform ImageFly { get { return Get<RectTransform>("ImageFly"); } }
    private RectTransform ImageBlue1 { get { return Get<RectTransform>("ImageBlue1"); } }
    private RectTransform ImageBlue2 { get { return Get<RectTransform>("ImageBlue2"); } }
    private Transform FallingStonePos { get { return GetT("FallingStonePos"); } }
    public RectTransform ImageBg { get { return Get<RectTransform>("ImageBg"); } }
    private List<string> Words = new List<string>();
    private string Current_Word;//不管是 单词 还是句子都用这个。
    private string Current_Sentence;
    private bool isnext = true;//为了不让一个单词重复的飞星，必须等到playgame刷新了到下一个单词才能再检测飞星。
    private bool audio_play = true;
    private TextMeshProUGUI TextSentence { get { return Get<TextMeshProUGUI>("TextSentence"); } }
    public override void Refresh()
    {

    }

    void Start()
    {
        StartCoroutine(Create());
        ImageBlue1.DOScale(1.2f, 0.05f).SetLoops(-1, LoopType.Yoyo);
        ImageBlue2.DOScale(1.2f, 0.05f).SetLoops(-1, LoopType.Yoyo);
    }
    IEnumerator Create()
    {
        while (true)
        {
            CreateUIItem<UIStarThroughFallIngStoneItem>(FallingStonePos).Init(this, RefreshWord());
            yield return new WaitForSeconds(0.5f);
        }
    }
    public override void PlayGame()
    {
        if (IsGameEnd)
        {
            return;
        }

        Words.Clear();


        switch (StartData.dataType)
        {
            case DataType.Ask:
                SetAsk();
                break;
            case DataType.Say:
                SetSay();
                break;
        }
        TextSentence.text = Current_Sentence;

        isnext = true; //此时已经刷新到下一个单词了，可以检测了
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
    private void AudioButtonClick(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (audio_play)
        {
            audio_play = false;
            switch (StartData.dataType)
            {
                case DataType.Ask:
                    StartCoroutine(PlayAskAudio());
                    break;
                case DataType.Say:
                    StartCoroutine(PlaySayAudio());
                    break;
            }
        }
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

        string s = CurrentAsk.id.Split('_')[0] + CurrentAsk.id.Split('_')[1];
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

        Current_Word = CurrentAsk.MapWordString;
        Current_Sentence = CurrentAsk.ask.Replace('_', ' ') + CurrentAsk.yes.Replace('_', ' ') + CurrentAsk.no.Replace('_', ' ');
        for (int i = 0; i < Current_Word.Split(' ').Length; i++)
        {
            if (Current_Sentence.Contains(Current_Word.Split(' ')[i]))
            {
                for (int j = 0; j < Words.Count; j++)
                {
                    if (Words[j] == Current_Word)
                    {
                        Words[j] = Current_Word.Split(' ')[i];
                        Current_Word = Words[j];
                        Current_Sentence = Current_Sentence.Replace(Words[j], "(    )");
                        break;
                    }
                }
            }
        }

        //        Words = Utility.RandomSortList(Words);
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

        Current_Word = CurrentSay.MapWordString;
        Current_Sentence = CurrentSay.yes.Replace('_', ' ') + CurrentSay.no.Replace('_', ' ');
        for (int i = 0; i < Current_Word.Split(' ').Length; i++)
        {
            if (Current_Sentence.Contains(Current_Word.Split(' ')[i]))
            {
                for (int j = 0; j < Words.Count; j++)
                {
                    if (Words[j] == Current_Word)
                    {
                        Words[j] = Current_Word.Split(' ')[i];
                        Current_Word = Words[j];
                        Current_Sentence = Current_Sentence.Replace(Words[j], "(    )");
                        break;
                    }
                }
            }
        }
        //        Words = Utility.RandomSortList(Words);
        audio_play = false;
        StartCoroutine(PlaySayAudio());
    }
    IEnumerator PlaySayAudio()
    {
        var length = AudioManager.Instance.Play(CurrentSay.answerSound).clip.length;
        yield return new WaitForSeconds(length);
        audio_play = true;
    }
    public void Check(string word)
    {
        if (isnext)
        {
            if (word == Current_Word)
            {
                isnext = false;//进来了就不让再检测了。到playgame里再开放
                CheckMatch(true);
            }
            else
            {
                CheckMatch();
            }
        }
    }
    private void CheckMatch(bool b = false)
    {
        if (b)
        {
            StartCoroutine(FlyTrue());
            StartCoroutine(WaitRightTwo());
        }
        else
        {
            StartCoroutine(FlaseStone());
            switch (StartData.dataType)
            {
                case DataType.Ask:
                    FlyStar(CurrentAsk.id, false);
                    break;
                case DataType.Say:
                    FlyStar(CurrentSay.id, false);
                    break;
            }
        }
    }

    IEnumerator FlyTrue()
    {
        audio_play = false;
        UITopBarStarFly fly;
        switch (StartData.dataType)
        {
            case DataType.Word:
                fly = FlyStar(true);
                fly.OnComplete += () =>
                {
                    audio_play = true;
                    PlayGame();
                };
                break;
            case DataType.Ask:
                TextSentence.text = Current_Sentence.Replace("(    )", "(" + Current_Word + ")");

                var length = AudioManager.Instance.Play(CurrentAsk.id).clip.length;
                yield return new WaitForSeconds(length);
                length = AudioManager.Instance.Play(CurrentAsk.answerSound).clip.length;
                yield return new WaitForSeconds(length);

                fly = FlyStar(CurrentAsk.id, true);
                fly.OnComplete += () =>
                {
                    audio_play = true;
                    PlayGame();
                };
                break;
            case DataType.Say:
                TextSentence.text = Current_Sentence.Replace("(    )", "(" + Current_Word + ")");

                var length1 = AudioManager.Instance.Play(CurrentSay.answerSound).clip.length;
                yield return new WaitForSeconds(length1);

                fly = FlyStar(CurrentSay.id, true);
                fly.OnComplete += () =>
                {
                    audio_play = true;
                    PlayGame();
                };
                break;
        }
    }
    IEnumerator WaitRightTwo()
    {
//        ImageBlue1.gameObject.SetActive(true);
//        ImageBlue2.gameObject.SetActive(true);
//        ImageBlue1.DOScale(1.2f, 0.05f).SetLoops(-1, LoopType.Yoyo);
//        ImageBlue2.DOScale(1.2f, 0.05f).SetLoops(-1, LoopType.Yoyo);
        ImageFly.DOAnchorPosY(250, 0.5f);
        ImageFly.DOScale(new Vector2(0.6f,0.6f), 0.5f);
        yield return new WaitForSeconds(2);
//        ImageBlue1.gameObject.SetActive(false);
//        ImageBlue2.gameObject.SetActive(false);
        ImageFly.DOAnchorPosY(85, 0.5f);
        ImageFly.DOScale(new Vector2(1f, 1f), 0.5f);
    }

    IEnumerator FlaseStone()
    {
        TextWARNING.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        
        TextWARNING.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        TextWARNING.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        TextWARNING.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        TextWARNING.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        TextWARNING.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);
        TextWARNING.gameObject.SetActive(true);


        TextWARNING.parent.gameObject.SetActive(false);
    }
}