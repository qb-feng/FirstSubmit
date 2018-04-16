using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
public class UIBolyShoot : UIBaseLevelGame
{
    private TextMeshProUGUI AskText { get { return Get<TextMeshProUGUI>("AskText"); } }
    private GameObject AskBg { get { return Get("AskBg"); } }
    private TextMeshProUGUI BolyAskText { get { return Get<TextMeshProUGUI>("BolyAskText"); } }
    private GameObject BolyAskBg { get { return Get("BolyAskBg"); } }
    private GameObject AudioButton { get { return Get("ImgAudioButton"); } }
    private Image SelectImage1 { get { return Get<Image>("SelectImage1"); } }
    private Image SelectImage2 { get { return Get<Image>("SelectImage2"); } }
    private Image SelectImage3 { get { return Get<Image>("SelectImage3"); } }
    private SkeletonGraphic ShootTarget { get { return Get<SkeletonGraphic>("ShootTarget"); } }
    private RectTransform Boly { get { return Get<RectTransform>("Boly"); } }

    private List<Image> SelectImageList = new List<Image>();
    private int CurrentIndex;
    private List<string> word_wrong = new List<string>();
    public static UIBolyShoot Instance { get; set; }
    private AudioSource myAudio;
    private string Audio;
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
        //UGUIEventListener.Get(AudioButton).onPointerClick = AudioButtonClick;
        SelectImageList.Add(SelectImage1);
        SelectImageList.Add(SelectImage2);
        SelectImageList.Add(SelectImage3);
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

        //取消掉玩家控制的播放按钮
        AudioButton.gameObject.SetActive(false);

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
            /*
            unitid = unitid.Remove(unitid.Length - 1);
            unitid = unitid + "1";

            foreach (var item in ConfigManager.Get<ConfigWordLibraryModel>())
            {
                if (item.id.Remove(item.id.Length - 4) == unitid)
                    word_wrong.Add(item.word);
            }*/
            word_wrong.Add("apple");
            word_wrong.Add("cake");
            word_wrong.Add("cat");
        }
    }

    private bool isclick = true;
    private void AudioButtonClick()
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

        //开启自动播放句子的协程
        StopCoroutine("CtrlAutoPlayAudio");
        StartCoroutine("CtrlAutoPlayAudio");

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

        //        this.WaitOneFrame(() =>
        //        {
        //            float f = AskText.GetComponent<RectTransform>().rect.width / 2;
        //            AudioButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-f - 160, -83);
        //        });
        StartCoroutine(PlayAudio());
    }
    IEnumerator PlayAudio()
    {
        AskBg.SetActive(true);
        var length = AudioManager.Instance.Play(Audio, false).clip.length;
        yield return new WaitForSeconds(length + 1);
        AskBg.SetActive(false);
        isclick = true;
    }
    /// <summary>
    ///  陈诉句
    /// </summary>
    private void SetSay()
    {
        List<string> list = new List<string>();
        //为了替换成使用新的图片 - 这里填加一个保存句子的印射单词对应的句子对应的Model
        Dictionary<string, ConfigSayLibraryModel> listModes = new Dictionary<string, ConfigSayLibraryModel>();//key为单词，value为id

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
                    SelectImageList[i].transform.parent.gameObject.SetActive(true);
                    if (list[i] == CurrentSay.MapWordString)
                        CurrentIndex = i + 1;
                }
            }
            else
            {
                listModes = new Dictionary<string, ConfigSayLibraryModel>();
                listModes.Add(CurrentSay.MapWordString, CurrentSay);

                //if (CurrentSay.no != null)
                //预留肯定句多关键词处理            
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
        //        this.WaitOneFrame(() =>
        //        {
        //            float f = AskText.GetComponent<RectTransform>().rect.width / 2;
        //            AudioButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-f - 160, -83);
        //        });

        StartCoroutine(PlaySayAudio());
    }

    private string saystringid;
    IEnumerator PlaySayAudio()
    {
        AskBg.SetActive(true);
        myAudio = AudioManager.Instance.Play(saystringid, false);
        var length = myAudio.clip.length;
        yield return new WaitForSeconds(length);
        AskBg.SetActive(false);
        isclick = true;
    }
    IEnumerator PlaySayAudioEnd()
    {
        StopCoroutine(PlaySayAudio());
        if (myAudio != null)
            myAudio.Stop();
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
        //        this.WaitOneFrame(() =>
        //        {
        //            float f = AskText.GetComponent<RectTransform>().rect.width / 2;
        //            AudioButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-f - 160, -83);
        //        });
        myAudio = AudioManager.Instance.Play(CurrentAsk.answerSound);
        length = myAudio.clip.length;
        yield return new WaitForSeconds(length);
        BolyAskBg.SetActive(false);
        isclick = true;
    }
    IEnumerator PlayAskAudioEnd()
    {
        StopCoroutine(PlayAskAudio());
        if (myAudio != null)
            myAudio.Stop();
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
        if (b)
        {
            StopCoroutine("CtrlAutoPlayAudio");
            StartCoroutine(PlayAudioYes());
            SetSkeYes();
        }
        else
        {
            StartCoroutine(PlayAudioNo());
            SetSkeNo();
        }
    }

    /// <summary>
    /// 进行下一关游戏的等待时间
    /// </summary>
    private float goNextPlayGameWaitTime = -1f;
    private float GoNextPlayGameWaitTime
    {
        get
        {
            if (goNextPlayGameWaitTime == -1)
                goNextPlayGameWaitTime = float.Parse(ConfigGlobalValueModel.GetValue("GoNextPlayGameWaitTime"));
            return goNextPlayGameWaitTime;
        }

    }
    private void BolyMove()
    {
        Invoke("PlayGame", GoNextPlayGameWaitTime);
        click = true;
    }

    private void SetSkeYes()
    {
        ShootTarget.AnimationState.ClearTracks();
        ShootTarget.Skeleton.SetToSetupPose();
        ShootTarget.AnimationState.SetAnimation(0, "animation", false);
        ShootTarget.AnimationState.Complete += AnimalCompleteYes;
    }
    private void AnimalCompleteYes(Spine.TrackEntry trackEntry)
    {
        switch (StartData.dataType)
        {
            case DataType.Word:
                AudioManager.Instance.Play(Audio);
                UITopBarStarFly fly = FlyStar(true);
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
        ShootTarget.AnimationState.Complete -= AnimalCompleteYes;
    }
    private void SetSkeNo()
    {
        ShootTarget.AnimationState.ClearTracks();
        ShootTarget.Skeleton.SetToSetupPose();
        ShootTarget.AnimationState.SetAnimation(0, "shefei", false);
        ShootTarget.AnimationState.Complete += AnimalCompleteNo;
    }

    private void AnimalCompleteNo(Spine.TrackEntry trackEntry)
    {
        click = true;
        UITopBarStarFly fly = null;
        switch (StartData.dataType)
        {
            case DataType.Word:
                fly = FlyStar(false);
                break;
            case DataType.Ask:
                fly = FlyStar(CurrentAsk.id, false);
                break;
            case DataType.Say:
                fly = FlyStar(CurrentSay.id, false);
                break;
        }
        //可以重新选择的回调
        fly.OnComplete += () => { isSeleteAnswer = false; };

        ShootTarget.AnimationState.Complete -= AnimalCompleteNo;

    }

    IEnumerator PlayAudioYes()
    {
        var lenght = AudioManager.Instance.Play("lagong").clip.length;
        yield return new WaitForSeconds(lenght);
        AudioManager.Instance.Play("zhongba");
    }

    IEnumerator PlayAudioNo()
    {
        var lenght = AudioManager.Instance.Play("lagong").clip.length;
        yield return new WaitForSeconds(lenght);
        AudioManager.Instance.Play("tuoba");
    }


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
            if (isclick)
            {
                yield return wf;
                AudioButtonClick();
            }
            yield return 0;
        }
    }
}