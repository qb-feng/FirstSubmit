using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
public class UIBottleMessage : UIBaseLevelGame
{
    private string Current_Word;//不管是 单词 还是句子都用这个。
    private string Current_Sentence;
    private bool audio_play = true;
    private GameObject ImageDontClick { get { return Get("ImageDontClick"); } }
    private TextMeshProUGUI TextQuestion { get { return Get<TextMeshProUGUI>("TextQuestion"); } }
    private Image ImageWord { get { return Get<Image>("ImageWord"); } }//单词时用图片
    private Image ImageDot { get { return Get<Image>("ImageDot"); } }//跑马灯 4个点
    private GameObject ImgAudioButton { get { return Get("ImgAudioButton"); } }
    private GameObject ImageClick1 { get { return Get("ImageClick1"); } }
    private GameObject ImageClick2 { get { return Get("ImageClick2"); } }
    public GameObject ImageTextPos1 { get { return Get("ImageTextPos1"); } }
    private TextMeshProUGUI TextPos1 { get { return Get<TextMeshProUGUI>("TextPos1"); } }
    public GameObject ImageTextPos2 { get { return Get("ImageTextPos2"); } }
    private TextMeshProUGUI TextPos2 { get { return Get<TextMeshProUGUI>("TextPos2"); } }
    private List<string> Words = new List<string>();
    public override void Refresh()
    {
        UGUIEventListener.Get(ImgAudioButton).onPointerClick = AudioButtonClick;

        ImageClick1.AddComponent<UIBottleMessageSelectCtrl>().Init(this, 1);
        ImageClick2.AddComponent<UIBottleMessageSelectCtrl>().Init(this, 2);
//        UGUIEventListener.Get(ImageClick1).onPointerClick = Click1;
        
    }
    
    private void Click1(UnityEngine.EventSystems.PointerEventData arg0)
    {
        StartCoroutine(WaitOne());
    }

    IEnumerator WaitOne()
    {
        ImageTextPos1.SetActive(true);
        yield return new WaitForSeconds(1);
        ImageTextPos1.SetActive(false);
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
            }
        }
    }
    public override void PlayGame()
    {
        if (IsGameEnd)
            return;

        Words.Clear();
        TextQuestion.text = "";

        ImageClick1.transform.localScale = new Vector2(1, 1);
        ImageClick2.transform.localScale = new Vector2(1, 1);

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
        TextQuestion.text = Current_Sentence;
        TextPos1.text = Words[0];
        TextPos2.text = Words[1];
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
        
        Current_Word = CurrentWord.word;
        ImageWord.sprite = CurrentWord.sprite;
        ImageWord.gameObject.SetActive(true);

        audio_play = false;
        StartCoroutine(PlayWordAudio());
    }
    IEnumerator PlayWordAudio()
    {
        ImageDot.gameObject.SetActive(true);
        ImageDot.fillAmount = 0;
        ImageDot.DOFillAmount(1, 1).SetLoops(-1);
        var length = AudioManager.Instance.Play(CurrentWord.word).clip.length;
        yield return new WaitForSeconds(1);
        ImageDot.DOKill();
        ImageDot.gameObject.SetActive(false);
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
        if (list.Count > 2)
        {
            while (list.Count > 2)
            {
                list.Remove(list[Random.Range(0, list.Count)]);
            }
        }

        if (!list.Contains(CurrentAsk))
        {
            list[0] = CurrentAsk;
        }
        list = Utility.RandomSortList(list);

        Words.Add(list[0].yes.Replace('_', ' ') + list[0].no.Replace('_', ' '));
        Words.Add(list[1].yes.Replace('_', ' ') + list[1].no.Replace('_', ' '));

//        if (list.Count < 3)
//        {
//            string s = CurrentAsk.id.Split('_')[0] + CurrentAsk.id.Split('_')[1];
//            for (int i = 0; i < ConfigManager.Get<ConfigWordLibraryModel>().Count; i++)
//            {
//                string w = ConfigManager.Get<ConfigWordLibraryModel>()[i].id;
//                if (s == w.Split('_')[0] + w.Split('_')[1])
//                {
//                    string word = ConfigManager.Get<ConfigWordLibraryModel>()[i].word;
//                    if (word != list[0].MapWordString && word != list[1].MapWordString)
//                    {
//                        Words.Add(word);
//                        break;
//                    }
//                }
//            }
//        }
//        else
//        {
//            Words.Add(list[2].MapWordString);
//        }

        Current_Sentence = CurrentAsk.ask.Replace('_', ' ');
        Current_Word = CurrentAsk.yes.Replace('_', ' ') + CurrentAsk.no.Replace('_', ' ');
        //        TextContent.text = CurrentAsk.ask.Replace('_', ' ') + CurrentAsk.yes.Replace('_', ' ') + CurrentAsk.no.Replace('_', ' ');

        audio_play = false;
        StartCoroutine(PlayAskAudio());

    }

    IEnumerator PlayAskAudio()
    {
        ImageDot.gameObject.SetActive(true);
        ImageDot.fillAmount = 0;
        ImageDot.DOFillAmount(1, 1).SetLoops(-1);

        var length = AudioManager.Instance.Play(CurrentAsk.id).clip.length;
        yield return new WaitForSeconds(length);
        length = AudioManager.Instance.Play(CurrentAsk.answerSound).clip.length;
        yield return new WaitForSeconds(length);
        
        ImageDot.DOKill();
        ImageDot.gameObject.SetActive(false);
        audio_play = true;
    }

    /// <summary>
    ///  陈诉句
    /// </summary>
    private void SetSay()
    {
        List<ConfigSayLibraryModel> list = new List<ConfigSayLibraryModel>();
        foreach (var item in m_randomSayList)
        {
            list.Add(item);
        }
        if (list.Count > 2)
        {
            while (list.Count > 2)
            {
                list.Remove(list[Random.Range(0, list.Count)]);
            }
        }

        if (!list.Contains(CurrentSay))
        {
            list[0] = CurrentSay;
        }
        list = Utility.RandomSortList(list);

        Words.Add(list[0].MapWordString);
        Words.Add(list[1].MapWordString);
        //        if (list.Count < 3)
        //        {
        //            string s = CurrentSay.id.Split('_')[0] + CurrentSay.id.Split('_')[1];
        //            for (int i = 0; i < ConfigManager.Get<ConfigWordLibraryModel>().Count; i++)
        //            {
        //                string w = ConfigManager.Get<ConfigWordLibraryModel>()[i].id;
        //                if (s == w.Split('_')[0] + w.Split('_')[1])
        //                {
        //                    string word = ConfigManager.Get<ConfigWordLibraryModel>()[i].word;
        //                    if (word != list[0].MapWordString && word != list[1].MapWordString)
        //                    {
        //                        Words.Add(word);
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Words.Add(list[2].MapWordString);
        //        }

        Current_Word = CurrentSay.MapWordString;
        Current_Sentence = CurrentSay.yes.Replace('_', ' ');
        //        TextContent.text = CurrentSay.yes.Replace('_', ' ') + CurrentSay.no.Replace('_', ' ');

        audio_play = false;
        StartCoroutine(PlaySayAudio());
    }
    IEnumerator PlaySayAudio()
    {
        ImageDot.gameObject.SetActive(true);
        ImageDot.fillAmount = 0;
        ImageDot.DOFillAmount(1, 1).SetLoops(-1);

        var length = AudioManager.Instance.Play(CurrentSay.answerSound).clip.length;
        yield return new WaitForSeconds(length);

        ImageDot.DOKill();
        ImageDot.gameObject.SetActive(false);
        audio_play = true;
    }

    public void CheckMatch(bool b = false)
    {
        UITopBarStarFly fly;
        ImageDontClick.SetActive(true);
        if (b)
        {
            switch (StartData.dataType)
            {
                case DataType.Word:
                    fly = FlyStar(true);
                    fly.OnComplete += () =>
                    {
                        ImageClick1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-645, 440);
                        ImageClick2.GetComponent<RectTransform>().anchoredPosition = new Vector2(-145, 440);
                        ImageDontClick.SetActive(false);
                        PlayGame();
                    };
                    break;
                case DataType.Ask:
                    fly = FlyStar(CurrentAsk.id, true);
                    fly.OnComplete += () =>
                    {
                        ImageClick1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-645, 440);
                        ImageClick2.GetComponent<RectTransform>().anchoredPosition = new Vector2(-145, 440);
                        ImageDontClick.SetActive(false);
                        PlayGame();
                    };
                    break;
                case DataType.Say:
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
                    StartCoroutine(FlyFlase());
                    break;
                case DataType.Ask:
                    FlyStar(CurrentAsk.id, false);
                    StartCoroutine(FlyFlase());
                    break;
//                case DataType.Say:
//                    FlyStar(CurrentSay.id, false);
//                    break;
            }
        }
    }

    IEnumerator FlyFlase()
    {
        yield return new WaitForSeconds(0.5f);
        ImageClick1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-645, 440);
        ImageClick2.GetComponent<RectTransform>().anchoredPosition = new Vector2(-145, 440);
        ImageClick1.transform.localScale = new Vector2(1, 1);
        ImageClick2.transform.localScale = new Vector2(1, 1);
    }
    public void CheckAnswer(int index)
    {
        if (index == 1)
        {
            if (TextPos1.text == Current_Word)
                CheckMatch(true);
            else
                CheckMatch();
        }
        else
        {
            if (TextPos2.text == Current_Word)
                CheckMatch(true);
            else
                CheckMatch();
        }
    }
}