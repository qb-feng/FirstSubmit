using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
public class UITangram : UIBaseLevelGame
{
    private GameObject ImageLaba { get { return Get("ImageLaba"); } }
    private RectTransform ImageTrack1 { get { return Get<RectTransform>("ImageTrack1"); } }
    private RectTransform ImageTrack2 { get { return Get<RectTransform>("ImageTrack2"); } }
    private GameObject Confirm { get { return Get("Confirm"); } }
    private Transform CarUpPos { get { return GetT("CarUpPos"); } }
    private Transform CarDownPos { get { return GetT("CarDownPos"); } }
    private Transform CarSayPos { get { return GetT("CarSayPos"); } }
    private float height;
    private RectTransform rect1;
    private RectTransform rect2;
    private List<string> Words = new List<string>();
    private string Current_Word;//不管是 单词 还是句子都用这个。
    private string Current_Sentence;
    private bool audio_play = true;
    public override void Refresh()
    {
        rect1 = ImageTrack1;
        rect2 = ImageTrack2;
        UGUIEventListener.Get(Confirm).onPointerClick = ButtonConfirm;
        UGUIEventListener.Get(ImageLaba).onPointerClick = AudioButtonClick;
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
    private UITangramImageItem currentItem;
    private void ButtonConfirm(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (!string.IsNullOrEmpty(currentItem.TextWord.text))
        {
            if (currentItem.TextWord.text == Current_Word)
                CheckMatch(true);
            else
                CheckMatch();
        }
    }
    void Start()
    {
        StartCoroutine(Track());
    }
    IEnumerator Track()
    {
        while (true)
        {
            rect1.DOAnchorPosY(-1079, 5).OnComplete(delegate
            {
                rect1.anchoredPosition = new Vector2(292, 1079);
            });
            rect2.DOAnchorPosY(0, 5);

            rect2.ClearAllChild();

            int pos_x = Random.Range(170, -175);
            var v1 = CreateUIItem<UITangramItem>(rect2);
            int index = Random.Range(0, Words.Count);
            v1.Init(Words[index]);
            v1.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos_x, -350);
            v1.gameObject.AddComponent<UITangramSelectCtrl>();

            pos_x = Random.Range(170, -175);
            var v2 = CreateUIItem<UITangramItem>(rect2);
            index = Random.Range(0, Words.Count);
            v2.Init(Words[index]);
            v2.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos_x, 0);
            v2.gameObject.AddComponent<UITangramSelectCtrl>();

            pos_x = Random.Range(170, -175);
            var v3 = CreateUIItem<UITangramItem>(rect2);
            index = Random.Range(0, Words.Count);
            v3.Init(Words[index]);
            v3.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos_x, 350);
            v3.gameObject.AddComponent<UITangramSelectCtrl>();

            yield return new WaitForSeconds(5);
            RectTransform rect;
            rect = rect1;
            rect1 = rect2;
            rect2 = rect;
        }
    }

    IEnumerator TrackWord()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
        }
    }

    public override void PlayGame()
    {
        if (IsGameEnd)
            return;

        CarUpPos.ClearAllChild();
        CarDownPos.ClearAllChild();
        CarSayPos.ClearAllChild();
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

        if (Current_Sentence.Contains("#"))
        {
            var up = Current_Sentence.Split('#')[0];
            var down = Current_Sentence.Split('#')[1];

            var itemups = up.Split('/');
            var itemdowns = down.Split('/');

            for (int i = 0; i < itemups.Length; i++)
            {
                if (!string.IsNullOrEmpty(itemups[i]))
                {
                    CreateUIItem("UITangramTextItem", CarUpPos).GetComponent<TextMeshProUGUI>().text = itemups[i];
                }
                else
                {
                    currentItem = CreateUIItem<UITangramImageItem>(CarUpPos);
                }
            }
            for (int i = 0; i < itemdowns.Length; i++)
            {
                if (!string.IsNullOrEmpty(itemdowns[i]))
                {
                    CreateUIItem("UITangramTextItem", CarDownPos).GetComponent<TextMeshProUGUI>().text = itemdowns[i];
                }
                else
                {
                    currentItem = CreateUIItem<UITangramImageItem>(CarDownPos);
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(CarUpPos.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(CarDownPos.GetComponent<RectTransform>());
        }
        else
        {

            var itemups = Current_Sentence.Split('/');

            for (int i = 0; i < itemups.Length; i++)
            {
                if (!string.IsNullOrEmpty(itemups[i]))
                {
                    CreateUIItem("UITangramTextItem", CarSayPos).GetComponent<TextMeshProUGUI>().text = itemups[i];
                }
                else
                {
                    currentItem = CreateUIItem<UITangramImageItem>(CarSayPos);
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(CarSayPos.GetComponent<RectTransform>());
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
        Current_Sentence = CurrentAsk.ask.Replace('_', ' ') + "#" + CurrentAsk.yes.Replace('_', ' ') + CurrentAsk.no.Replace('_', ' ');
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
                        Current_Sentence = Current_Sentence.Replace(Words[j], "//");
                        break;
                    }
                }
            }
        }

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

        if(!string.IsNullOrEmpty(CurrentSay.yes) && !string.IsNullOrEmpty(CurrentSay.no))
            Current_Sentence = CurrentSay.yes.Replace('_', ' ') + "#" + CurrentSay.no.Replace('_', ' ');
        else
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
                        Current_Sentence = Current_Sentence.Replace(Words[j], "//");
                        break;
                    }
                }
            }
        }
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
                case DataType.Ask:
                    FlyStar(CurrentAsk.id, false);
                    break;
                case DataType.Say:
                    FlyStar(CurrentSay.id, false);
                    break;
            }
        }
    }
}