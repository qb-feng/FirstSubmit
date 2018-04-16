using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
public class UIBolySend : UIBaseLevelGame
{
    private TextMeshProUGUI AskText { get { return Get<TextMeshProUGUI>("AskText"); } }
    private GameObject AskBg { get { return Get("AskBg"); } }
    private TextMeshProUGUI BolyAskText { get { return Get<TextMeshProUGUI>("BolyAskText"); } }
    private GameObject BolyAskBg { get { return Get("BolyAskBg"); } }
    private GameObject ImageDownBg { get { return Get("ImageDownBg"); } }
    private GameObject AudioButton { get { return Get("ImgAudioButton"); } }
    private Image SelectImage1 { get { return Get<Image>("SelectImage1"); } }
    private Image SelectImage2 { get { return Get<Image>("SelectImage2"); } }
    private Image SelectImage3 { get { return Get<Image>("SelectImage3"); } }
    private Transform ImageItem1 { get { return GetT("ImageItem1"); } }
    private Image SelectImage1Pos { get { return Get<Image>("SelectImage1Pos"); } }
    private Transform ImageItem2 { get { return GetT("ImageItem2"); } }
    private Image SelectImage2Pos { get { return Get<Image>("SelectImage2Pos"); } }
    private Transform ImageItem3 { get { return GetT("ImageItem3"); } }
    private Image SelectImage3Pos { get { return Get<Image>("SelectImage3Pos"); } }
    private Image Image1 { get { return Get<Image>("Image1"); } }
    private Image Image2 { get { return Get<Image>("Image2"); } }
    private Image Image3 { get { return Get<Image>("Image3"); } }
    private RectTransform ImageItem { get { return Get<RectTransform>("ImageItem"); } }
    private RectTransform ImageLetter { get { return Get<RectTransform>("ImageLetter"); } }
    private Transform Pos { get { return GetT("Pos"); } }

    private List<Image> SelectImageList = new List<Image>();
    private List<Image> ImageList = new List<Image>();
    private int CurrentIndex;
    private List<string> word_wrong = new List<string>();
    public static UIBolySend Instance { get; set; }
    private AudioSource myAudio;
    private string Audio;

    //2017年11月3日17:21:18 qiubin修改
    private float doTweenTime = 0.618f;//当前tween动画的时间

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
    }
    public override void Refresh()
    {
        UGUIEventListener.Get(AudioButton).onPointerClick = AudioButtonClick;
        SelectImageList.Add(SelectImage1);
        SelectImageList.Add(SelectImage2);
        SelectImageList.Add(SelectImage3);
        ImageList.Add(Image1);
        ImageList.Add(Image2);
        ImageList.Add(Image3);
        UGUIEventListener.Get(SelectImage1).onPointerClick = BtnSelectImage1;
        UGUIEventListener.Get(SelectImage2).onPointerClick = BtnSelectImage2;
        UGUIEventListener.Get(SelectImage3).onPointerClick = BtnSelectImage3;

        switch (StartData.dataType)
        {
            case DataType.Ask:
                unitid = m_randomAskList[0].id.Remove(m_randomAskList[0].id.Length - 4);
                WordWrong();
                break;
            case DataType.Say:
                unitid = m_randomSayList[0].id.Remove(m_randomSayList[0].id.Length - 4);
                WordWrong();
                break;
        }
    }
    private string unitid;
    private void WordWrong()
    {
        if (unitid[unitid.Length - 1] == '2')
        {
            unitid = unitid.Remove(unitid.Length - 1);
            unitid = unitid + "1";
        }
        else if (unitid[unitid.Length - 1] == '6')
        {
            unitid = unitid.Remove(unitid.Length - 1);
            unitid = unitid + "5";
        }

        foreach (var item in ConfigManager.Get<ConfigWordLibraryModel>())
        {
            if (item.id.Remove(item.id.Length - 4) == unitid)
                word_wrong.Add(item.word);
        }

        if (word_wrong.Count == 0)
        {
//             unitid = unitid.Remove(unitid.Length - 1);
//             unitid = unitid + "1";
// 
//             foreach (var item in ConfigManager.Get<ConfigWordLibraryModel>())
//             {
//                 if (item.id.Remove(item.id.Length - 4) == unitid)
//                     word_wrong.Add(item.word);
//             }
            word_wrong.Add("apple");
            word_wrong.Add("cake");
            word_wrong.Add("cat");
        }
    }
    private bool isclick = true;
    private void AudioButtonClick(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (!isclick) return;
        isclick = false;

        switch (StartData.dataType)
        {
            case DataType.Word:
                StartCoroutine(PlayAudio());
                break;
            case DataType.Ask:
                StartCoroutine(PlayAskAudio());
                break;
            case DataType.Say:
                StartCoroutine(PlaySayAudio());
                break;
        }
    }
    private void BtnSelectImage1(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (CurrentIndex == 1)
        {
            SelectImage1Pos.transform.SetParent(transform);
            SelectImage1Pos.transform.DOScale(0.7f, doTweenTime);
            SelectImage1Pos.transform.DOMove(ImageItem.transform.position, doTweenTime).OnComplete(delegate
            {
                SelectImage1Pos.transform.SetParent(ImageItem);
                ImageDownBg.SetActive(false);
                ImageLetter.DOScale(Vector3.zero, 1);
                ImageLetter.DOMove(Pos.position, 1).OnComplete(delegate
                {
                    CheckMatch(true);
                });
            });
        }
        else
            CheckMatch();
    }
    private void BtnSelectImage2(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (CurrentIndex == 2)
        {
            SelectImage2Pos.transform.SetParent(transform);
            SelectImage2Pos.transform.DOScale(0.7f, doTweenTime);
            SelectImage2Pos.transform.DOMove(ImageItem.transform.position, doTweenTime).OnComplete(delegate
            {
                SelectImage2Pos.transform.SetParent(ImageItem);
                ImageDownBg.SetActive(false);
                ImageLetter.DOScale(Vector3.zero, 1);
                ImageLetter.DOMove(Pos.position, 1).OnComplete(delegate
                {
                    CheckMatch(true);
                });
            });
        }
        else
            CheckMatch();
    }
    private void BtnSelectImage3(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (CurrentIndex == 3)
        {
            SelectImage3Pos.transform.SetParent(transform);
            SelectImage3Pos.transform.DOScale(0.7f, doTweenTime);
            SelectImage3Pos.transform.DOMove(ImageItem.transform.position, doTweenTime).OnComplete(delegate
            {
                SelectImage3Pos.transform.SetParent(ImageItem);
                ImageDownBg.SetActive(false);
                ImageLetter.DOScale(Vector3.zero, 1);
                ImageLetter.DOMove(Pos.position, 1).OnComplete(delegate
                {
                    CheckMatch(true);
                });
            });
        }
        else
            CheckMatch();
    }
    public override void PlayGame()
    {
        if (IsGameEnd)
            return;
        ImageItem3.gameObject.SetActive(true);
        SelectImageList[0].transform.parent.gameObject.SetActive(false);
        SelectImageList[1].transform.parent.gameObject.SetActive(false);
        SelectImageList[2].transform.parent.gameObject.SetActive(false);
        ImageList[0].gameObject.SetActive(false);
        ImageList[1].gameObject.SetActive(false);
        ImageList[2].gameObject.SetActive(false);
        //        SelectImage2Pos.gameObject()

        switch (CurrentIndex)
        {
            case 1:
                SelectImage1Pos.transform.SetParent(ImageItem1);
                SelectImage1Pos.rectTransform.anchoredPosition = new Vector2(0, 9);
                SelectImage1Pos.rectTransform.localScale = Vector3.one;
                break;
            case 2:
                SelectImage2Pos.transform.SetParent(ImageItem2);
                SelectImage2Pos.rectTransform.anchoredPosition = new Vector2(0, 9);
                SelectImage2Pos.rectTransform.localScale = Vector3.one;
                break;
            case 3:
                SelectImage3Pos.transform.SetParent(ImageItem3);
                SelectImage3Pos.rectTransform.anchoredPosition = new Vector2(0, 9);
                SelectImage3Pos.rectTransform.localScale = Vector3.one;
                break;
        }

        ImageDownBg.SetActive(true);
        ImageLetter.DOScale(Vector3.one, 0);
        ImageLetter.anchoredPosition = new Vector2(0, -45);
        //            ImageLetter.DOAnchorPosY(-45, 0);

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
        if (list.Count > 3) // 如果超过3个单词，则要缩减一下。
        {
            while (list.Count > 3)
            {
                list.Remove(list[Random.Range(0, list.Count)]);
            }
        }

        if (!list.Contains(CurrentWord))
        {
            list[0] = CurrentWord;
        }
        list = Utility.RandomSortList(list);

        for (int i = 0; i < list.Count; i++)
        {
            SelectImageList[i].sprite = list[i].sprite;
            SelectImageList[i].transform.parent.gameObject.SetActive(true);
            if (list[i] == CurrentWord)
                CurrentIndex = i + 1;
        }

        AskText.text = CurrentWord.word;
        Audio = CurrentWord.word;

        isclick = false;
        StartCoroutine(PlayAudio());
    }
    IEnumerator PlayAudio()
    {
        AskBg.SetActive(true);
        var length = AudioManager.Instance.Play(Audio, false).clip.length;
        yield return new WaitForSeconds(length);
        isclick = true;
    }
    /// <summary>
    ///  陈诉句
    /// </summary>
    private void SetSay()
    {
        //为了替换成使用新的图片 - 这里填加一个保存句子的印射单词对应的句子对应的Model
        Dictionary<string, ConfigSayLibraryModel> listModes = new Dictionary<string, ConfigSayLibraryModel>();//key为单词，value为id
        List<string> list = new List<string>();
        foreach (var item in m_randomSayList)
        {
            if (!list.Contains(item.MapWordString))
            {
                list.Add(item.MapWordString);
                if (!listModes.ContainsKey(item.MapWordString))
                    listModes.Add(item.MapWordString, item);
            }
        }
        if (list.Count > 3)
        {
            while (list.Count > 3)
            {
                list.Remove(list[Random.Range(0, list.Count)]);
            }
        }

        if (!list.Contains(CurrentSay.MapWordString))
        {
            list[0] = CurrentSay.MapWordString;
        }

        if (list.Count < 3)
        {
            while (list.Count < 3)
            {
                word_wrong = Utility.RandomSortList(word_wrong);
                for (int i = 0; i < word_wrong.Count; i++)
                {
                    if (list.Count < 3)
                    {
                        if (!list.Contains(word_wrong[i]))
                        {
                            list.Add(word_wrong[i]);
                        }
                    }
                }
            }
        }
        list = Utility.RandomSortList(list);

        if (!mySayIsExistSpecialSprite)
        {
            if (!mySayIsExistSpecialOption)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (listModes.ContainsKey(list[i]))
                    {
                        //新方式获取图片
                        SelectImageList[i].sprite = listModes[list[i]].MapWordSprite;
                    }
                    else
                    {
                        SelectImageList[i].sprite = LoadAssetBundleManager.Instance.Load<Sprite>(list[i] + ".png");
                    }
                    SelectImageList[i].transform.parent.gameObject.SetActive(true);
                    if (list[i] == CurrentSay.MapWordString)
                        CurrentIndex = i + 1;
                }
            }
            else
            {
                list = new List<string>();
                listModes = new Dictionary<string, ConfigSayLibraryModel>();
                listModes.Add(CurrentSay.MapWordString, CurrentSay);

                list.Add(CurrentSay.MapWordString);
                list.Add(mySaySpecialOption);
                list = Utility.RandomSortList(list);
                for (int i = 0; i < 2; i++)
                {
                    if (listModes.ContainsKey(list[i]))
                    {
                        //新方式获取图片
                        SelectImageList[i].sprite = listModes[list[i]].MapWordSprite;
                    }
                    else
                    {
                        SelectImageList[i].sprite = LoadAssetBundleManager.Instance.Load<Sprite>(list[i] + ".png");
                    }

                    SelectImageList[i].transform.parent.gameObject.SetActive(true);
                    if (list[i] != CurrentSay.MapWordString)
                        CurrentIndex = i + 1;
                }
                ImageItem3.gameObject.SetActive(false);
            }
        }
        else
        {
            List<Image> tempImage = new List<Image>();
            tempImage.Add(SelectImage1);
            tempImage.Add(SelectImage2);
            tempImage = Utility.RandomSortList(tempImage);
            ImageItem3.gameObject.SetActive(false);
            for (int i = 0; i < 2; i++)
            {
                tempImage[i].sprite = mySaySpecialSprite[i];
                tempImage[i].transform.parent.gameObject.SetActive(true);
            }
            CurrentIndex = int.Parse(tempImage[0].name.Remove(0, tempImage[0].name.Length - 1));
        }


        AskText.text = CurrentSay.answer.Replace('_', ' ');
        saystringid = CurrentSay.answerSound;

        Audio = CurrentSay.MapWordString;
        isclick = false;
        StartCoroutine(PlaySayAudio());
    }

    private string saystringid;
    IEnumerator PlaySayAudio()
    {
        AskBg.SetActive(true);
        myAudio = AudioManager.Instance.Play(saystringid, false);
        var length = myAudio.clip.length;
        yield return new WaitForSeconds(length);
        isclick = true;
    }
    IEnumerator PlaySayAudioEnd()
    {
        StopCoroutine(PlaySayAudio());
        if (myAudio != null)
        {
            myAudio.Stop();
        }
        var length = AudioManager.Instance.Play(saystringid).clip.length;
        yield return new WaitForSeconds(length);
        UITopBarStarFly fly = FlyStar(CurrentSay.id, true);
        fly.OnComplete += () =>
        {
            BolyMove();
        };
    }
    /// <summary>
    ///  疑问句
    /// </summary>
    private void SetAsk()
    {
        List<string> list = new List<string>();
        Dictionary<string, ConfigAskLibraryModel> listModes = new Dictionary<string, ConfigAskLibraryModel>();//qiubin增加句子映射单词对应的model容器

        foreach (var item in m_randomAskList)
        {
            if (!list.Contains(item.MapWordString))
                list.Add(item.MapWordString);
            if (!listModes.ContainsKey(item.MapWordString))
            {
                listModes.Add(item.MapWordString, item);
            }
        }
        if (list.Count > 3)
        {
            while (list.Count > 3)
            {
                list.Remove(list[Random.Range(0, list.Count)]);
            }
        }

        if (!list.Contains(CurrentAsk.MapWordString))
        {
            list[0] = CurrentAsk.MapWordString;
        }

        if (list.Count < 3)
        {
            while (list.Count < 3)
            {
                word_wrong = Utility.RandomSortList(word_wrong);
                for (int i = 0; i < word_wrong.Count; i++)
                {
                    if (list.Count < 3)
                    {
                        if (!list.Contains(word_wrong[i]))
                        {
                            list.Add(word_wrong[i]);
                        }
                    }
                }
            }
        }
        list = Utility.RandomSortList(list);
        if (!myAskIsExistSpecialSprite)
        {
            if (!myAskIsExistSpecialOption)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (listModes.ContainsKey(list[i]))
                    {
                        //新方式获取图片
                        SelectImageList[i].sprite = listModes[list[i]].MapWordSprite;
                    }
                    else
                    {
                        SelectImageList[i].sprite = LoadAssetBundleManager.Instance.Load<Sprite>(list[i] + ".png");
                    }

                    SelectImageList[i].transform.parent.gameObject.SetActive(true);
                    if (list[i] == CurrentAsk.MapWordString)
                        CurrentIndex = i + 1;
                }
            }
            else
            {
                listModes = new Dictionary<string, ConfigAskLibraryModel>();
                listModes.Add(CurrentAsk.MapWordString, CurrentAsk);

                list = new List<string>();
                list.Add(CurrentAsk.MapWordString);
                list.Add(myAskSpecialOption);
                list = Utility.RandomSortList(list);
                for (int i = 0; i < 2; i++)
                {
                    if (listModes.ContainsKey(list[i]))
                    {
                        //新方式获取图片
                        SelectImageList[i].sprite = listModes[list[i]].MapWordSprite;
                    }
                    else
                    {
                        SelectImageList[i].sprite = LoadAssetBundleManager.Instance.Load<Sprite>(list[i] + ".png");
                    }

                    SelectImageList[i].transform.parent.gameObject.SetActive(true);
                    if (list[i] != CurrentAsk.MapWordString)
                        CurrentIndex = i + 1;
                }
                ImageItem3.gameObject.SetActive(false);
            }
        }
        else
        {
            List<Image> tempImage = new List<Image>();
            tempImage.Add(SelectImage1);
            tempImage.Add(SelectImage2);
            tempImage = Utility.RandomSortList(tempImage);
            ImageItem3.gameObject.SetActive(false);
            for (int i = 0; i < 2; i++)
            {
                tempImage[i].sprite = myAskSpecialSprite[i];
                tempImage[i].transform.parent.gameObject.SetActive(true);
            }
            CurrentIndex = int.Parse(tempImage[0].name.Remove(0, tempImage[0].name.Length - 1));
        }

        BolyAskText.text = CurrentAsk.ask.Replace('_', ' ');
        AskText.text = CurrentAsk.answer.Replace('_', ' ');
        saystringid = CurrentAsk.answerSound;

        isclick = false;
        Audio = CurrentAsk.MapWordString;
        StartCoroutine(PlayAskAudio());
    }

    IEnumerator PlayAskAudio()
    {
        BolyAskBg.SetActive(true);
        myAudio = AudioManager.Instance.Play(CurrentAsk.id, false);
        var length = myAudio.clip.length;
        yield return new WaitForSeconds(length);
        //        BolyAskBg.SetActive(false);
        AskBg.SetActive(true);
        myAudio = AudioManager.Instance.Play(CurrentAsk.answerSound);
        length = myAudio.clip.length;
        yield return new WaitForSeconds(length);
        //        AskBg.SetActive(false);
        isclick = true;
    }
    IEnumerator PlayAskAudioEnd()
    {
        StopCoroutine(PlayAskAudio());
        if (myAudio != null)
        {
            myAudio.Stop();
        }
        var length = AudioManager.Instance.Play(CurrentAsk.id).clip.length;
        yield return new WaitForSeconds(length);
        length = AudioManager.Instance.Play(CurrentAsk.answerSound).clip.length;
        yield return new WaitForSeconds(length);
        UITopBarStarFly fly = FlyStar(CurrentAsk.id, true);
        fly.OnComplete += () =>
        {
            BolyMove();
        };
    }
    private bool click = true;
    private void CheckMatch(bool b = false)
    {
        if (!click) return;
        click = false;
        if (b)
        {
            UITopBarStarFly fly;
            switch (StartData.dataType)
            {
                case DataType.Word:
                    AudioManager.Instance.Play(Audio);
                    fly = FlyStar(true);
                    fly.OnComplete += () =>
                    {
                        BolyMove();
                    };
                    break;
                case DataType.Ask:
                    StartCoroutine(PlayAskAudioEnd());
                    break;
                case DataType.Say:
                    StartCoroutine(PlaySayAudioEnd());
                    break;
            }
        }
        else
        {
            click = true;
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

    private void BolyMove()
    {
        BolyAskBg.SetActive(false);
        AskBg.SetActive(false);
        PlayGame();
        click = true;
    }

}