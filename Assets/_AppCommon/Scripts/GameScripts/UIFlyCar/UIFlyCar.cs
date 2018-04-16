using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
public class UIFlyCar : UIBaseLevelGame
{
    private GameObject ImageLaba { get { return Get("ImageLaba"); } }
    private GameObject ImageTrack1 { get { return Get("ImageTrack1"); } }
    private GameObject ImageTrack2 { get { return Get("ImageTrack2"); } }
    private RectTransform ImageLine1 { get { return Get<RectTransform>("ImageLine1"); } }
    private RectTransform ImageLine2 { get { return Get<RectTransform>("ImageLine2"); } }
    private RectTransform ImageBg1 { get { return Get<RectTransform>("ImageBg1"); } }
    private RectTransform ImageBg2 { get { return Get<RectTransform>("ImageBg2"); } }
    protected RectTransform ImageCar { get { return Get<RectTransform>("ImageCar"); } }
    private RectTransform LinePos1 { get { return Get<RectTransform>("LinePos1"); } }
    private RectTransform LinePos2 { get { return Get<RectTransform>("LinePos2"); } }
    private RectTransform ImageBlue1 { get { return Get<RectTransform>("ImageBlue1"); } }
    private RectTransform ImageBlue2 { get { return Get<RectTransform>("ImageBlue2"); } }
    protected Image ImageCarIcon { get { return Get<Image>("ImageCarIcon"); } }
    private TextMeshProUGUI TextSentence { get { return Get<TextMeshProUGUI>("TextSentence"); } }
    protected RectTransform ImageZhizhen { get { return Get<RectTransform>("ImageZhizhen"); } }
    protected GameObject ImageWarn { get { return Get("ImageWarn"); } }
    private Rigidbody2D RigiLine1;
    private Rigidbody2D RigiLine2;
    private Rigidbody2D RigiImageBg1;
    private Rigidbody2D RigiImageBg2;
    private float height;
    private List<string> Words = new List<string>();
    private string Current_Word;//不管是 单词 还是句子都用这个。
    private string Current_Sentence;
    //private float speed = 300;
    private AudioSource bgm;
    private bool audio_play = true;
    public override void Refresh()
    {
        RigiLine1 = ImageLine1.GetComponent<Rigidbody2D>();
        RigiLine2 = ImageLine2.GetComponent<Rigidbody2D>();
        RigiImageBg1 = ImageBg1.GetComponent<Rigidbody2D>();
        RigiImageBg2 = ImageBg2.GetComponent<Rigidbody2D>();

        height = ImageBg2.rect.height;
        ImageBg2.anchoredPosition = new Vector2(0, height);
        UGUIEventListener.Get(ImageLaba).onPointerClick = AudioButtonClick;

        bgm = AudioManager.Instance.Play("Engine", destroy: false);
        bgm.loop = true;
    }

    void OnDestroy()
    {
        if (bgm != null && bgm.gameObject != null)
            Destroy(bgm.gameObject);
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
    private UITangramImageItem currentItem;

    void Start()
    {
        CreateText(LinePos2, -700);
        CreateText(LinePos2, 0);
        CreateText(LinePos2, 700);

        SetSpeed(-4);


        UGUIEventListener.Get(ImageTrack1).onPointerDown += d =>
        {
            if (carBool)
            {
                pos = Input.mousePosition;
                isclick = true;
            }
        };
        UGUIEventListener.Get(ImageTrack2).onPointerDown += d =>
        {
            if (carBool)
            {
                pos = Input.mousePosition;
                isclick = true;
            }
        };

        UGUIEventListener.Get(ImageTrack1).onPointerUp += d =>
        {
            isclick = false;
        };
        UGUIEventListener.Get(ImageTrack2).onPointerUp += d =>
        {
            isclick = false;
        };
    }

    protected string RefreshWord()
    {
        Words = Utility.RandomSortList(Words);
        string word = "";
        if (Random.Range(1, 11) < 5)
            word = Current_Word;
        else
            word = Words[0];

        return word;
    }
    protected virtual void CreateText(Transform t, float y)
    {
        int pos_x = Random.Range(0, 3);
        var v = CreateUIItem("UIFlyCarTextItem", t);
        switch (pos_x)
        {
            case 0:
                v.GetComponent<RectTransform>().anchoredPosition = new Vector2(-287, y);
                break;
            case 1:
                v.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
                break;
            case 2:
                v.GetComponent<RectTransform>().anchoredPosition = new Vector2(287, y);
                break;
        }
        v.gameObject.AddComponent<UIFlyCarCtrl>().Init(this, ImageCar, RefreshWord());
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
                break;
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

    private Vector3 pos;
    private Vector3 posCurrent;
    private bool isclick = false;
    private bool carBool = true;
    private int state = 0;
    void Update()
    {
        if (ImageLine1.anchoredPosition.y <= -2270)
        {
            ImageLine1.anchoredPosition = new Vector2(291, ImageLine2.anchoredPosition.y + 2160);
            LinePos1.ClearAllChild();
            CreateText(LinePos1, -700);
            CreateText(LinePos1, 0);
            CreateText(LinePos1, 700);
        }
        if (ImageLine2.anchoredPosition.y <= -2270)
        {
            ImageLine2.anchoredPosition = new Vector2(291, ImageLine1.anchoredPosition.y + 2160);
            LinePos2.ClearAllChild();
            CreateText(LinePos2, -700);
            CreateText(LinePos2, 0);
            CreateText(LinePos2, 700);
        }
        if (ImageBg1.anchoredPosition.y <= -height)
        {
            ImageBg1.anchoredPosition = new Vector2(0, ImageBg2.anchoredPosition.y + height);
        }
        if (ImageBg2.anchoredPosition.y <= -height)
        {
            ImageBg2.anchoredPosition = new Vector2(0, ImageBg1.anchoredPosition.y + height);
        }

        if (isclick)
        {
            posCurrent = Input.mousePosition;

            if (Mathf.Abs(posCurrent.x - pos.x) > 100)
            {
                if (posCurrent.x > pos.x)//向右滑
                {
                    isclick = false;
                    state = 1;
                }
                else// 向左滑
                {
                    isclick = false;
                    state = 2;
                }
            }
        }

        if (state == 1)
        {
            carBool = false;
            float x = ImageCar.anchoredPosition.x + 291;
            curr_pos++;
            if (x >= 582)
            {
                x = 582;
                curr_pos = 2;
            }
            if (curr_pos != last_pos)
            {
                AudioManager.Instance.Play("ChangeLanes");
                last_pos = curr_pos;
            }
            ImageCar.DOAnchorPosX(x, 0.5f).OnComplete(delegate
            {
                carBool = true;
            });
            state = 0;
        }
        else if (state == 2)
        {
            carBool = false;
            float x = ImageCar.anchoredPosition.x - 291;
            curr_pos--;
            if (x <= 0)
            {
                x = 0;
                curr_pos = 0;
            }
            if (curr_pos != last_pos)
            {
                AudioManager.Instance.Play("ChangeLanes");
                last_pos = curr_pos;
            }
            ImageCar.DOAnchorPosX(x, 0.5f).OnComplete(delegate
            {
                carBool = true;
            });
            state = 0;
        }

    }

    private int curr_pos = 1;
    private int last_pos = 1; // 记录之前的位置与当前比较，变了说明变道了
    private bool isnext = true;//为了不让一个单词重复的飞星，必须等到playgame刷新了到下一个单词才能再检测飞星。
    public void Check(string word)
    {
        if (isnext)
        {
            if (word == Current_Word)
            {
                isnext = false;//进来了就不让再检测了。到playgame里再开放
                CheckMatch(true);
                StartCoroutine(WaitRightTwo());
            }
            else
            {
                CheckMatch();
                StartCoroutine(WaitWrongTwo());
            }
        }
    }
    IEnumerator WaitRightTwo()
    {
        SetSpeed(-6);
        ImageBlue1.gameObject.SetActive(true);
        ImageBlue2.gameObject.SetActive(true);
        ImageBlue1.DOScale(0.5f, 0.05f).SetLoops(-1, LoopType.Yoyo);
        ImageBlue2.DOScale(0.5f, 0.05f).SetLoops(-1, LoopType.Yoyo);
        ImageCar.DOAnchorPosY(180, 0.5f);
        ImageZhizhen.DOLocalRotate(new Vector3(0, 0, -40), 0.5f);
        AudioManager.Instance.Play("Speedup");
        yield return new WaitForSeconds(2);
        ImageBlue1.gameObject.SetActive(false);
        ImageBlue2.gameObject.SetActive(false);
        ImageCar.DOAnchorPosY(120, 0.5f);
        ImageZhizhen.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
        SetSpeed(-4);
    }
    protected virtual IEnumerator WaitWrongTwo()
    {
        ImageCarIcon.sprite = GetS("UIFlyCar2");
        SetSpeed(-2);
        ImageZhizhen.DOLocalRotate(new Vector3(0, 0, 40), 0.5f);
        ImageWarn.gameObject.SetActive(true);
        AudioManager.Instance.Play("Brake");
        yield return new WaitForSeconds(2);
        ImageZhizhen.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
        ImageWarn.gameObject.SetActive(false);
        ImageCarIcon.sprite = GetS("UIFlyCar1");
        SetSpeed(-4);
    }

    protected void SetSpeed(float speed_float)
    {
        RigiLine1.velocity = new Vector2(0, speed_float);
        RigiLine2.velocity = new Vector2(0, speed_float);
        RigiImageBg1.velocity = new Vector2(0, speed_float);
        RigiImageBg2.velocity = new Vector2(0, speed_float);
    }

    private void CheckMatch(bool b = false)
    {
        if (b)
        {
            StartCoroutine(FlyTrue());
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
}