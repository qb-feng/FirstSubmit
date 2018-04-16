using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class UISnowMountainTop : UIBaseLevelGame
{
    private GameObject ImageLaba { get { return Get("ImageLaba"); } }
    private List<string> Words = new List<string>();
    private bool audio_play = true;
    private TextMeshProUGUI TextSentence { get { return Get<TextMeshProUGUI>("TextSentence"); } }
    private Image ImageWord { get { return Get<Image>("ImageWord"); } }
    private string Current_Word;//不管是 单词 还是句子都用这个。
    private string Current_Sentence;


    private RectTransform MounRoad { get { return Get<RectTransform>("MounRoad"); } }
    private Rigidbody2D MounOut1 { get { return Get<Rigidbody2D>("MounOut1"); } }
    private Rigidbody2D MounOut2 { get { return Get<Rigidbody2D>("MounOut2"); } }
    private Rigidbody2D MounInside1 { get { return Get<Rigidbody2D>("MounInside1"); } }
    private Rigidbody2D MounInside2 { get { return Get<Rigidbody2D>("MounInside2"); } }
    private Rigidbody2D TreeItem1 { get { return Get<Rigidbody2D>("TreeItem1"); } }
    private Rigidbody2D TreeItem2 { get { return Get<Rigidbody2D>("TreeItem2"); } }
    private Rigidbody2D UnderItem1 { get { return Get<Rigidbody2D>("UnderItem1"); } }
    private Rigidbody2D UnderItem2 { get { return Get<Rigidbody2D>("UnderItem2"); } }
    private RectTransform ImageTree1 { get { return Get<RectTransform>("ImageTree1"); } }
    private RectTransform ImageTree2 { get { return Get<RectTransform>("ImageTree2"); } }
    private RectTransform ImageTree3 { get { return Get<RectTransform>("ImageTree3"); } }
    private RectTransform ImageTree4 { get { return Get<RectTransform>("ImageTree4"); } }
    private GameObject ImageStone1 { get { return Get("ImageStone1"); } }
    private GameObject ImageStone2 { get { return Get("ImageStone2"); } }



    private GameObject ImageClick { get { return Get("ImageClick"); } }
    protected RectTransform ImagePlayer { get { return Get<RectTransform>("ImagePlayer"); } }
    protected Image player_image;
    //private Rigidbody2D Rigi_player;

    private RectTransform ImageCloud1 { get { return Get<RectTransform>("ImageCloud1"); } }
    private RectTransform ImageCloud2 { get { return Get<RectTransform>("ImageCloud2"); } }
    private RectTransform ImageQQ1 { get { return Get<RectTransform>("ImageQQ1"); } }
    private RectTransform ImageQQ2 { get { return Get<RectTransform>("ImageQQ2"); } }
    private TextMeshProUGUI TextCloud1 { get { return Get<TextMeshProUGUI>("TextCloud1"); } }
    private TextMeshProUGUI TextCloud2 { get { return Get<TextMeshProUGUI>("TextCloud2"); } }
    private TextMeshProUGUI TextQQ1 { get { return Get<TextMeshProUGUI>("TextQQ1"); } }
    private TextMeshProUGUI TextQQ2 { get { return Get<TextMeshProUGUI>("TextQQ2"); } }


    private RectTransform Moun_out1;
    private RectTransform Moun_out2;

    private RectTransform Moun_In1;
    private RectTransform Moun_In2;

    private RectTransform Tree_1;
    private RectTransform Tree_2;

    private RectTransform Under_Iten1;
    private RectTransform Under_Iten2;

    private Rigidbody2D Rigi_Cloud1;
    private Rigidbody2D Rigi_Cloud2;
    private Rigidbody2D Rigi_QQ1;
    private Rigidbody2D Rigi_QQ2;

    private float tan10 = 0.17632698070846f;   //   tan10° = 0.17632698070846
    private float Length;

    protected bool isclick = true;
    protected virtual void ClickPlayer(PointerEventData arg0)
    {
        if (isclick)
        {
            isclick = false;
            player_image.sprite = GetS("UISnowMountainTopPlayer2");
            ImagePlayer.DOAnchorPosY(800, 0.5f).SetEase(Ease.OutCubic).OnComplete(delegate
            {
                ImagePlayer.DOAnchorPosY(480, 0.5f).SetEase(Ease.InQuart).OnComplete(delegate
                {
                    isclick = true;
                });
                player_image.sprite = GetS("UISnowMountainTopPlayer1");
            });
        }
    }
    private void AudioButtonClick()
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

    private void SpeedPlus()
    {
        MounOut1.velocity = new Vector2(-5, 5 * tan10);
        MounOut2.velocity = new Vector2(-5, 5 * tan10);

        MounInside1.velocity = new Vector2(-4, 4 * tan10);
        MounInside2.velocity = new Vector2(-4, 4 * tan10);

        TreeItem1.velocity = new Vector2(-5, 5 * tan10);
        TreeItem2.velocity = new Vector2(-5, 5 * tan10);

        UnderItem1.velocity = new Vector2(-9, 9 * tan10);
        UnderItem2.velocity = new Vector2(-9, 9 * tan10);

        Rigi_Cloud1.velocity = new Vector2(-7f, 7f * tan10);
        Rigi_Cloud2.velocity = new Vector2(-7f, 7f * tan10);
        Rigi_QQ1.velocity = new Vector2(-8f, 8f * tan10);
        Rigi_QQ2.velocity = new Vector2(-8f, 8f * tan10);
    }
    protected void Speed1(bool b)
    {
        if (b)
        {
            Rigi_Cloud1.velocity = new Vector2(-4f, 4f * tan10);
            Rigi_Cloud2.velocity = new Vector2(-4f, 4f * tan10);
            Rigi_QQ1.velocity = new Vector2(-5f, 5f * tan10);
            Rigi_QQ2.velocity = new Vector2(-5f, 5f * tan10);
        }
        else
        {
            Rigi_Cloud1.velocity = new Vector2(-1f, 1f * tan10);
            Rigi_Cloud2.velocity = new Vector2(-1f, 1f * tan10);
            Rigi_QQ1.velocity = new Vector2(-1.25f, 1.25f * tan10);
            Rigi_QQ2.velocity = new Vector2(-1.25f, 1.25f * tan10);
        }
    }
    protected void Speed(bool b)
    {
        if (b)
        {
            MounOut1.velocity = new Vector2(-2, 2 * tan10);
            MounOut2.velocity = new Vector2(-2, 2 * tan10);

            MounInside1.velocity = new Vector2(-1, 1 * tan10);
            MounInside2.velocity = new Vector2(-1, 1 * tan10);

            TreeItem1.velocity = new Vector2(-2, 2 * tan10);
            TreeItem2.velocity = new Vector2(-2, 2 * tan10);

            UnderItem1.velocity = new Vector2(-6, 6 * tan10);
            UnderItem2.velocity = new Vector2(-6, 6 * tan10);
        }
        else
        {
            MounOut1.velocity = new Vector2(-0.5f, 0.5f * tan10);
            MounOut2.velocity = new Vector2(-0.5f, 0.5f * tan10);

            MounInside1.velocity = new Vector2(-0.25f, 0.25f * tan10);
            MounInside2.velocity = new Vector2(-0.25f, 0.25f * tan10);

            TreeItem1.velocity = new Vector2(-0.5f, 0.5f * tan10);
            TreeItem2.velocity = new Vector2(-0.5f, 0.5f * tan10);

            UnderItem1.velocity = new Vector2(-1.5f, 1.5f * tan10);
            UnderItem2.velocity = new Vector2(-1.5f, 1.5f * tan10);
        }
    }
    public override void Refresh()
    {
        ImageLaba.SetActive(false);
        //UGUIEventListener.Get(ImageLaba).onPointerClick = AudioButtonClick;
        ImagePlayer.gameObject.AddComponent<UISnowMountainTopPlayer>().Init(this);

        UGUIEventListener.Get(ImageClick).onPointerClick = ClickPlayer;
        player_image = ImagePlayer.GetComponent<Image>();


        Moun_out1 = MounOut1.GetComponent<RectTransform>();
        Moun_out2 = MounOut2.GetComponent<RectTransform>();

        Moun_In1 = MounInside1.GetComponent<RectTransform>();
        Moun_In2 = MounInside2.GetComponent<RectTransform>();

        Tree_1 = TreeItem1.GetComponent<RectTransform>();
        Tree_2 = TreeItem2.GetComponent<RectTransform>();

        Under_Iten1 = UnderItem1.GetComponent<RectTransform>();
        Under_Iten2 = UnderItem2.GetComponent<RectTransform>();

        Rigi_Cloud1 = ImageCloud1.GetComponent<Rigidbody2D>();
        Rigi_Cloud2 = ImageCloud2.GetComponent<Rigidbody2D>();
        Rigi_QQ1 = ImageQQ1.GetComponent<Rigidbody2D>();
        Rigi_QQ2 = ImageQQ2.GetComponent<Rigidbody2D>();

        //Rigi_player = ImagePlayer.GetComponent<Rigidbody2D>();

        Length = MounRoad.rect.width;

        //        Rigi_player.velocity = new Vector2(0.005f, -0.005f * tan10);


        //ImageTree1.anchoredPosition = new Vector2(-400, Random.Range(50, 200));
        //ImageTree2.anchoredPosition = new Vector2(680, Random.Range(50, 200));
        //ImageTree3.anchoredPosition = new Vector2(-400, Random.Range(50, 200));
        //ImageTree4.anchoredPosition = new Vector2(680, Random.Range(50, 200));
        ImageTree1.anchoredPosition = new Vector2(-400, 188);
        ImageTree2.anchoredPosition = new Vector2(680, 159);
        ImageTree3.anchoredPosition = new Vector2(-400, 188);
        ImageTree4.anchoredPosition = new Vector2(680, 159);

        Speed(true);


    }

    void Start()
    {
        if (Random.Range(0, 2) == 0)
        {
            Rigi_Cloud2.gameObject.SetActive(true);
            Rigi_Cloud2.velocity = new Vector2(-4f, 4f * tan10);
            TextCloud2.text = RefreshWord();
        }
        else
        {
            Rigi_QQ2.gameObject.SetActive(true);
            Rigi_QQ2.velocity = new Vector2(-5f, 5f * tan10);
            TextQQ2.text = RefreshWord();
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
            case DataType.Word:
                SetWord();
                ImageWord.sprite = CurrentWord.sprite;
                ImageWord.transform.parent.gameObject.SetActive(true);
                break;
            case DataType.Ask:
                SetAsk();
                TextSentence.text = Current_Sentence;
                TextSentence.transform.parent.gameObject.SetActive(true);
                break;
            case DataType.Say:
                SetSay();
                TextSentence.text = Current_Sentence;
                TextSentence.transform.parent.gameObject.SetActive(true);
                break;
        }

        StopCoroutine("CtrlAutoPlayAudio");
        StartCoroutine("CtrlAutoPlayAudio");
        //        isnext = true; //此时已经刷新到下一个单词了，可以检测了
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

        //        Words = Utility.RandomSortList(Words);

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
    void FixedUpdate()
    {
        if (Moun_out1.anchoredPosition.x <= -1920)
        {
            Moun_out1.anchoredPosition = new Vector2(Moun_out2.anchoredPosition.x + 1920, Moun_out2.anchoredPosition.y);
        }

        if (Moun_out2.anchoredPosition.x <= -1920)
        {
            Moun_out2.anchoredPosition = new Vector2(Moun_out1.anchoredPosition.x + 1920, Moun_out1.anchoredPosition.y);
        }


        if (Moun_In1.anchoredPosition.x <= -1920)
        {
            Moun_In1.anchoredPosition = new Vector2(Moun_In2.anchoredPosition.x + 1920, Moun_In2.anchoredPosition.y);
        }

        if (Moun_In2.anchoredPosition.x <= -1920)
        {
            Moun_In2.anchoredPosition = new Vector2(Moun_In1.anchoredPosition.x + 1920, Moun_In1.anchoredPosition.y);
        }


        if (Tree_1.anchoredPosition.x <= -Length)
        {
            Tree_1.anchoredPosition = new Vector2(Tree_2.anchoredPosition.x + Length, Tree_2.anchoredPosition.y);
            //ImageTree1.anchoredPosition = new Vector2(-400, Random.Range(50, 200));
            //ImageTree2.anchoredPosition = new Vector2(680, Random.Range(50, 200));
            ImageTree1.anchoredPosition = new Vector2(-400, 188);
            ImageTree2.anchoredPosition = new Vector2(680, 159);
        }

        if (Tree_2.anchoredPosition.x <= -Length)
        {
            Tree_2.anchoredPosition = new Vector2(Tree_1.anchoredPosition.x + Length, Tree_1.anchoredPosition.y);
            //ImageTree3.anchoredPosition = new Vector2(-400, Random.Range(50, 200));
            //ImageTree4.anchoredPosition = new Vector2(680, Random.Range(50, 200));
            ImageTree3.anchoredPosition = new Vector2(-400, 188);
            ImageTree4.anchoredPosition = new Vector2(680, 159);
        }


        if (Under_Iten1.anchoredPosition.x <= -Length)
        {
            Under_Iten1.anchoredPosition = new Vector2(Under_Iten2.anchoredPosition.x + Length, Under_Iten2.anchoredPosition.y);

            if (Random.Range(0, 2) == 0)
            {
                ImageCloud1.anchoredPosition = new Vector2(1200, 450);
                Rigi_Cloud1.gameObject.SetActive(true);
                Rigi_QQ1.gameObject.SetActive(false);
                Rigi_Cloud1.velocity = new Vector2(-4f, 4f * tan10);
                TextCloud1.text = RefreshWord();
            }
            else
            {
                ImageQQ1.anchoredPosition = new Vector2(1200, 70);
                Rigi_Cloud1.gameObject.SetActive(false);
                Rigi_QQ1.gameObject.SetActive(true);
                Rigi_QQ1.velocity = new Vector2(-5f, 5f * tan10);
                TextQQ1.text = RefreshWord();
            }
            ImageStone1.SetActive(Random.Range(0, 10) < 3);
        }

        if (Under_Iten2.anchoredPosition.x <= -Length)
        {
            Under_Iten2.anchoredPosition = new Vector2(Under_Iten1.anchoredPosition.x + Length, Under_Iten1.anchoredPosition.y);

            if (Random.Range(0, 2) == 0)
            {
                ImageCloud2.anchoredPosition = new Vector2(1200, 450);
                Rigi_Cloud2.gameObject.SetActive(true);
                Rigi_QQ2.gameObject.SetActive(false);
                Rigi_Cloud2.velocity = new Vector2(-4f, 4f * tan10);
                TextCloud2.text = RefreshWord();
            }
            else
            {
                ImageQQ2.anchoredPosition = new Vector2(1200, 70);
                Rigi_Cloud2.gameObject.SetActive(false);
                Rigi_QQ2.gameObject.SetActive(true);
                Rigi_QQ2.velocity = new Vector2(-5f, 5f * tan10);
                TextQQ2.text = RefreshWord();
            }
            ImageStone2.SetActive(Random.Range(0, 10) < 3);
        }
    }

    public virtual void TouchStone()
    {
        StartCoroutine(Wait());
    }
    public void TouchWord(string word)
    {
        CheckMatch(Current_Word == word);
    }

    IEnumerator Wait()
    {
        isclick = false;
        player_image.sprite = GetS("UISnowMountainTopPlayer3");
        Speed(false);
        Speed1(false);
        yield return new WaitForSeconds(1f);
        isclick = true;
        Speed1(true);
        player_image.sprite = GetS("UISnowMountainTopPlayer1");
        Speed(true);
    }


    private void CheckMatch(bool b = false)
    {
        if (b)
        {
            StopCoroutine("CtrlAutoPlayAudio");
            StartCoroutine(FlyTrue());
        }
        else
        {
            StartCoroutine(FlyFlase());
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

    IEnumerator FlyTrue()
    {
        SpeedPlus();
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

                //                var length = AudioManager.Instance.Play(CurrentAsk.id).clip.length;
                //                yield return new WaitForSeconds(length);
                //                length = AudioManager.Instance.Play(CurrentAsk.answerSound).clip.length;
                //                yield return new WaitForSeconds(length);

                fly = FlyStar(CurrentAsk.id, true);
                fly.OnComplete += () =>
                {
                    audio_play = true;
                    PlayGame();
                };
                break;
            case DataType.Say:
                TextSentence.text = Current_Sentence.Replace("(    )", "(" + Current_Word + ")");

                //                var length1 = AudioManager.Instance.Play(CurrentSay.answerSound).clip.length;
                //                yield return new WaitForSeconds(length1);

                fly = FlyStar(CurrentSay.id, true);
                fly.OnComplete += () =>
                {
                    audio_play = true;
                    PlayGame();
                };
                break;
        }

        yield return new WaitForSeconds(2);
        Speed(true);
        Speed1(true);
    }

    IEnumerator FlyFlase()
    {
        Speed(false);
        Speed1(false);
        yield return new WaitForSeconds(2);
        Speed(true);
        Speed1(true);
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
            if (audio_play)
            {
                yield return wf;
                AudioButtonClick();
            }
            yield return 0;
        }
    }

    #endregion
}