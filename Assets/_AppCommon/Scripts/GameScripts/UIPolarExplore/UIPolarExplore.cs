using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
public class UIPolarExplore : UIBaseLevelGame
{
    private GameObject WordBg { get { return Get("WordBg"); } }
    private TextMeshProUGUI WordText { get { return Get<TextMeshProUGUI>("WordText"); } }
    private TextMeshProUGUI AskText { get { return Get<TextMeshProUGUI>("AskText"); } }
    private GameObject AskBg { get { return Get("AskBg"); } }
    private TextMeshProUGUI BolyAskText { get { return Get<TextMeshProUGUI>("BolyAskText"); } }
    private GameObject BolyAskBg { get { return Get("BolyAskBg"); } }
    private GameObject AudioButton { get { return Get("ImgAudioButton"); } }
    private Image SelectImage1 { get { return Get<Image>("SelectImage1"); } }
    private Image SelectImage2 { get { return Get<Image>("SelectImage2"); } }
    private Image SelectImage3 { get { return Get<Image>("SelectImage3"); } }
    private RectTransform Octopus { get { return Get<RectTransform>("Octopus"); } }
    private RectTransform Boly { get { return Get<RectTransform>("Boly"); } }
    private RectTransform ImageBg1 { get { return Get<RectTransform>("ImageBg1"); } }
    private RectTransform ImageBg2 { get { return Get<RectTransform>("ImageBg2"); } }
    private Image Image1 { get { return Get<Image>("Image1"); } }
    private Image Image2 { get { return Get<Image>("Image2"); } }
    private Image Image3 { get { return Get<Image>("Image3"); } }

    private int bolyState = 1; //波力所在的位置
    private int bgState = 1; //哪个背景在中间

    private List<Image> SelectImageList = new List<Image>();
    private List<Image> ImageList = new List<Image>();
    private int CurrentIndex;

    private List<string> word_wrong = new List<string>();
    public static UIPolarExplore Instance { get; set; }
    private AudioSource myAudio;
    private string Audio;
    private string saystringid;
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

        width = ImageBg1.rect.width;

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

    private float width = 0;

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

    /// <summary>
    /// 将正确框或者错误框逐渐隐藏消失
    /// </summary>
    private void SetFalseTrueAndFalse(Image image)
    {
        image.enabled = false;
        Color imageColor = image.color;
        imageColor.a = 0;
        image.color = imageColor;
        image.enabled = true;

        DOTween.To(() => 0, x => { imageColor.a = x; image.color = imageColor; }, 1f, colorFalseTime * 0.5f).onComplete = () =>  //0 -1
        {
            DOTween.To(() => 1f, x => { imageColor.a = x; image.color = imageColor; }, 0.5f, colorFalseTime * 0.5f).onComplete = () => //1 - 0.5
            {
                DOTween.To(() => 0.5f, x => { imageColor.a = x; image.color = imageColor; }, 1f, colorFalseTime * 0.5f).onComplete = () =>  //0.5 -1
                {
                    //1 - 0
                    DOTween.To(() => 1f, x => { imageColor.a = x; image.color = imageColor; }, 0, colorFalseTime * 0.5f).onComplete = () =>
                    {
                        image.enabled = false;
                        imageColor.a = 1;
                        image.color = imageColor;
                    };
                };
            };
        };
    }
    private float colorFalseTime = 0.4f;//正确框或错误框逐渐消失的时间
    private bool isSeleteAnswer = false;//是否选择了答案（用来控制玩家只能点一次 true表示选择了答案 - 不能再次选择）
    private Image currentAnswerImage = null;//当前选择的图片的正确图或者错误图
    private void CheckSelectAnswer(Image seleteImage, int seleteIndex)
    {
        //选择正确或失败的图片出现
        Image image = null;
        if (CurrentIndex == seleteIndex)
        {
            image = seleteImage.transform.Find("True").GetComponent<Image>();
            CheckMatch(true);
        }
        else
        {
            image = seleteImage.transform.Find("False").GetComponent<Image>();
            CheckMatch();
        }
        currentAnswerImage = image;
        //正确项或错误项开始闪
        SetFalseTrueAndFalse(currentAnswerImage);
    }

    private void BtnSelectImage1(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (isSeleteAnswer)
        {
            return;
        }
        isSeleteAnswer = true;

        CheckSelectAnswer(SelectImage1, 1);
    }
    private void BtnSelectImage2(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (isSeleteAnswer)
        {
            return;
        }
        isSeleteAnswer = true;

        CheckSelectAnswer(SelectImage2, 2);
    }
    private void BtnSelectImage3(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (isSeleteAnswer)
        {
            return;
        }
        isSeleteAnswer = true;

        CheckSelectAnswer(SelectImage3, 3);
    }

    public override void PlayGame()
    {
        if (IsGameEnd)
            return;

        SelectImageList[0].transform.parent.gameObject.SetActive(false);
        SelectImageList[1].transform.parent.gameObject.SetActive(false);
        SelectImageList[2].transform.parent.gameObject.SetActive(false);

        SelectImage1.transform.Find("True").GetComponent<Image>().enabled = false;
        SelectImage1.transform.Find("False").GetComponent<Image>().enabled = false;
        SelectImage2.transform.Find("True").GetComponent<Image>().enabled = false;
        SelectImage2.transform.Find("False").GetComponent<Image>().enabled = false;
        SelectImage3.transform.Find("True").GetComponent<Image>().enabled = false;
        SelectImage3.transform.Find("False").GetComponent<Image>().enabled = false;

        ImageList[0].gameObject.SetActive(false);
        ImageList[1].gameObject.SetActive(false);
        ImageList[2].gameObject.SetActive(false);

        isSeleteAnswer = false;//答案可以重新选择
        isclick = false;//不可以点击

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

        WordText.text = CurrentWord.word;
        Audio = CurrentWord.WordSound;
        StartCoroutine(PlayAudio());
    }
    IEnumerator PlayAudio()
    {
        WordBg.SetActive(true);
        var length = AudioManager.Instance.Play(Audio, false).clip.length + 1;
        yield return new WaitForSeconds(length);
        WordBg.SetActive(false);
        Octopus.gameObject.SetActive(true);
        switch (bolyState)
        {
            case 1:
                Octopus.anchoredPosition = new Vector2(0, 0);
                break;
            case 2:
                Octopus.anchoredPosition = new Vector2(700, 0);
                break;
        }
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
                list.Add(item.MapWordString);

            if (!listModes.ContainsKey(item.MapWordString))
                listModes.Add(item.MapWordString, item);
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
                    //            if (list[i].HaveDiff)//如果有特别标记的句子要加个标签
                    //            {
                    //                if (!string.IsNullOrEmpty(list[i].yes))
                    //                {
                    //                    ImageList[i].sprite = GetS("UIMakeClothesRight");
                    //                }
                    //                else
                    //                {
                    //                    ImageList[i].sprite = GetS("UIMakeClothesWrong");
                    //                }
                    //                ImageList[i].gameObject.SetActive(true);
                    //            }
                    SelectImageList[i].transform.parent.gameObject.SetActive(true);
                    if (list[i] == CurrentSay.MapWordString)
                        CurrentIndex = i + 1;
                }
            }
            else
            {
                //if (CurrentSay.no != null)
                //预留肯定句多关键词处理           
                listModes = new Dictionary<string, ConfigSayLibraryModel>();
                listModes.Add(CurrentSay.MapWordString, CurrentSay);
                list = new List<string>();
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
            }
        }
        else
        {
            List<Image> tempImage = new List<Image>();
            tempImage.Add(SelectImage1);
            tempImage.Add(SelectImage2);
            tempImage = Utility.RandomSortList(tempImage);
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
        StartCoroutine(PlaySayAudio());
    }
    IEnumerator PlaySayAudio()
    {
        AskBg.SetActive(true);
        myAudio = AudioManager.Instance.Play(saystringid, false);
        var length = myAudio.clip.length;
        yield return new WaitForSeconds(length);
        AskBg.SetActive(false);
        Octopus.gameObject.SetActive(true);
        switch (bolyState)
        {
            case 1:
                Octopus.anchoredPosition = new Vector2(0, 0);
                break;
            case 2:
                Octopus.anchoredPosition = new Vector2(700, 0);
                break;
        }
        isclick = true;
    }
    IEnumerator PlaySayAudioEnd()
    {
        if (myAudio != null)
        {
            myAudio.Stop();
        }
        myAudio = AudioManager.Instance.Play(saystringid);
        var length = myAudio.clip.length;
        yield return new WaitForSeconds(length);
        FlyStar(CurrentSay.id, true).OnComplete += () =>
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
            }
        }
        else
        {
            List<Image> tempImage = new List<Image>();
            tempImage.Add(SelectImage1);
            tempImage.Add(SelectImage2);
            tempImage = Utility.RandomSortList(tempImage);
            for (int i = 0; i < 2; i++)
            {
                tempImage[i].sprite = myAskSpecialSprite[i];
                tempImage[i].transform.parent.gameObject.SetActive(true);
            }
            CurrentIndex = int.Parse(tempImage[0].name.Remove(0, tempImage[0].name.Length - 1));
        }

        AskText.text = CurrentAsk.ask.Replace('_', ' ');
        BolyAskText.text = CurrentAsk.answer.Replace('_', ' ');
        saystringid = CurrentAsk.answerSound;

        Audio = CurrentAsk.MapWordString;
        StartCoroutine(PlayAskAudio());
    }

    IEnumerator PlayAskAudio()
    {
        AskBg.SetActive(true);
        myAudio = AudioManager.Instance.Play(CurrentAsk.id, false);
        var length = myAudio.clip.length;
        yield return new WaitForSeconds(length);
        AskBg.SetActive(false);
        BolyAskBg.SetActive(true);
        myAudio = AudioManager.Instance.Play(CurrentAsk.answerSound);
        length = myAudio.clip.length;
        yield return new WaitForSeconds(length);
        BolyAskBg.SetActive(false);
        Octopus.gameObject.SetActive(true);
        switch (bolyState)
        {
            case 1:
                Octopus.anchoredPosition = new Vector2(0, 0);
                break;
            case 2:
                Octopus.anchoredPosition = new Vector2(700, 0);
                break;
        }
        isclick = true;
    }
    IEnumerator PlayAskAudioEnd()
    {
        if (myAudio != null)
        {
            myAudio.Stop();
        }
        myAudio = AudioManager.Instance.Play(CurrentAsk.id);
        var length = myAudio.clip.length;
        yield return new WaitForSeconds(length);
        myAudio = AudioManager.Instance.Play(CurrentAsk.answerSound);
        length = myAudio.clip.length;
        yield return new WaitForSeconds(length);
        FlyStar(CurrentAsk.id, true).OnComplete += () =>
        {
            BolyMove();
        };
    }
    private bool click = true;
    private void CheckMatch(bool b = false)
    {
        if (b)
        {
            switch (StartData.dataType)
            {
                case DataType.Word:
                    AudioManager.Instance.Play(Audio);
                    FlyStar(true);
                    BolyMove();
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
            UITopBarStarFly flaseFly = null;
            switch (StartData.dataType)
            {
                case DataType.Word:
                    flaseFly = FlyStar(false);
                    break;
                case DataType.Ask:
                    flaseFly = FlyStar(CurrentAsk.id, false);
                    break;
                case DataType.Say:
                    flaseFly = FlyStar(CurrentSay.id, false);
                    break;
            }
            //重置答案选择器
            flaseFly.OnComplete += () => { isSeleteAnswer = false; };
        }
    }

    private void BolyMove()
    {
        switch (bolyState)
        {
            case 1:
                SetSke("huachuan");
                Boly.DOAnchorPosX(0, 2).OnComplete(delegate
                {
                    SetSke("animation");
                    Octopus.gameObject.SetActive(false);
                    bolyState = 2;
                    PlayGame();
                    click = true;
                });
                break;
            case 2:
                SetSke("huachuan");
                Boly.DOAnchorPosX(700, 2).OnComplete(delegate
                {
                    Octopus.gameObject.SetActive(false);
                    Boly.DOAnchorPosX(1200, 1.5f).OnComplete(delegate
                    {
                        Boly.anchoredPosition = new Vector2(-1200, 25);
                        ImageBg2.gameObject.SetActive(true);
                        if (bgState == 1)
                        {
                            ImageBg2.gameObject.SetActive(true);
                            ImageBg1.DOAnchorPosX(-width, 2);
                            ImageBg2.DOAnchorPosX(width, 0);
                            ImageBg2.DOAnchorPosX(0, 2).OnComplete(delegate
                            {
                                ImageBg1.DOAnchorPosX(width, 0);
                                ImageBg1.gameObject.SetActive(false);
                                bgState = 2;
                                Boly.DOAnchorPosX(-700, 1.5f).OnComplete(delegate
                                {
                                    SetSke("animation");
                                    bolyState = 1;
                                    PlayGame();
                                    click = true;
                                });
                            });
                        }
                        else
                        {
                            ImageBg1.gameObject.SetActive(true);
                            ImageBg2.DOAnchorPosX(-width, 2);
                            ImageBg1.DOAnchorPosX(0, 2).OnComplete(delegate
                            {
                                ImageBg2.DOAnchorPosX(width, 0);
                                ImageBg2.gameObject.SetActive(false);
                                bgState = 1;
                                Boly.DOAnchorPosX(-700, 1.5f).OnComplete(delegate
                                {
                                    SetSke("animation");
                                    bolyState = 1;
                                    PlayGame();
                                    click = true;
                                });
                            });
                        }
                    });
                });
                break;
        }
    }

    private void SetSke(string skename)
    {
        switch (skename)
        {
            case "huachuan":
                StartCoroutine("PlayAudioRow");
                break;
            case "animation":
                StopCoroutine("PlayAudioRow");
                break;
        }
        Boly.GetComponent<SkeletonGraphic>().AnimationState.ClearTracks();
        Boly.GetComponent<SkeletonGraphic>().Skeleton.SetToSetupPose();
        Boly.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, skename, true);
    }

    private IEnumerator PlayAudioRow()
    {
        while (true)
        {
            AudioManager.Instance.Play("rowAudio");
            yield return new WaitForSeconds(0.6f);
        }
    }

}