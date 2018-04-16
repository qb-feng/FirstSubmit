using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
public class UICatchBaby : UIBaseLevelGame
{
    private bool audio_play = true;
    private GameObject ImageLaba { get { return Get("ImageLaba"); } }
    private RectTransform ImageBili { get { return Get<RectTransform>("ImageBili"); } }
    private TextMeshProUGUI TextSentence { get { return Get<TextMeshProUGUI>("TextSentence"); } }
    private TextMeshProUGUI TextWord1 { get { return Get<TextMeshProUGUI>("TextWord1"); } }
    private TextMeshProUGUI TextWord2 { get { return Get<TextMeshProUGUI>("TextWord2"); } }
    private TextMeshProUGUI TextWord3 { get { return Get<TextMeshProUGUI>("TextWord3"); } }
    private GameObject BtnLeft { get { return Get("BtnLeft"); } }
    private GameObject BtnRight { get { return Get("BtnRight"); } }
    private GameObject BtnConfirm { get { return Get("BtnConfirm"); } }
    private RectTransform ImageWord1 { get { return Get<RectTransform>("ImageWord1"); } }
    private RectTransform ImageWord2 { get { return Get<RectTransform>("ImageWord2"); } }
    private RectTransform ImageWord3 { get { return Get<RectTransform>("ImageWord3"); } }
    private GameObject TextBg1 { get { return Get("TextBg1"); } }
    private GameObject TextBg2 { get { return Get("TextBg2"); } }
    private GameObject TextBg3 { get { return Get("TextBg3"); } }
    private RectTransform ImageExit { get { return Get<RectTransform>("ImageExit"); } }
    public Image ImageCatch { get { return Get<Image>("ImageCatch"); } }
    private RectTransform CatchRect;
    private List<string> Words = new List<string>();
    private string Current_Word;//不管是 单词 还是句子都用这个。
    private string Current_Sentence;
    private float speed = 200;
    private bool Horizontal_bool = false;//是否在往下抓娃娃，抓到了就不能左右移动
    private bool catching_bool = true;//是否正在抓娃娃
    public bool catched_bool = false;// 是否已经抓到了娃娃，抓到了只能放下娃娃

    public Transform current_wawa;//当前抓到的娃娃
    private Transform ImagePos { get { return GetT("ImagePos"); } }//底部不让娃娃掉下去
    public override void Refresh()
    {
        CatchRect = ImageCatch.GetComponent<RectTransform>();
        ImageWord1.gameObject.AddComponent<UICatchBabyWawa>().Init(this);
        ImageWord2.gameObject.AddComponent<UICatchBabyWawa>().Init(this);
        ImageWord3.gameObject.AddComponent<UICatchBabyWawa>().Init(this);
        UGUIEventListener.Get(BtnLeft).onPointerClick = ButtonLeft;
        UGUIEventListener.Get(BtnRight).onPointerClick = ButtonRight;
        UGUIEventListener.Get(BtnConfirm).onPointerClick = ButtonConfirm;
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
    private void ButtonLeft(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (Horizontal_bool) return;

        CatchRect.DOKill();
        float time = (CatchRect.anchoredPosition.x - (-700)) / speed;
        CatchRect.DOAnchorPosX(-700, time);
    }
    private void ButtonRight(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (Horizontal_bool) return;

        CatchRect.DOKill();
        float time = (700 - CatchRect.anchoredPosition.x) / speed;
        CatchRect.DOAnchorPosX(700, time);
    }
    private void ButtonConfirm(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (catched_bool)
        {
//            CatchRect.DOKill();
//            ImageCatch.sprite = GetS("UICatchBabyjiqi1");
//            current_wawa.GetComponent<BoxCollider2D>().isTrigger = false;
//            current_wawa.GetComponent<Rigidbody2D>().isKinematic = false;
            return;
        }

        if (!catching_bool) return;
        catching_bool = false;

        Horizontal_bool = true;

        CatchRect.DOKill();
        ImageCatch.sprite = GetS("UICatchBabyjiqi1");

//        float time = (CatchRect.anchoredPosition.y - (-340)) / speed;
        float height = ImageBili.rect.height;
        float time = height / speed;
        CatchRect.DOAnchorPosY(120 - height, time).OnComplete(delegate
        {
            ImageCatch.sprite = GetS("UICatchBabyjiqi");
            CatchRect.DOAnchorPosY(120, time).OnComplete(delegate
            {
                catching_bool = true;

                Horizontal_bool = false;
            });
        });
    }
    public override void PlayGame()
    {
        if (IsGameEnd)
            return;

        isFirstPlayaudio = true;

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
        
        if (Words.Count >= 3)
        {
            TextWord1.text = Words[0];
            TextWord2.text = Words[1];
            TextWord3.text = Words[2];
        }
        else
        {
            TextWord1.text = Words[0];
            TextWord2.text = Words[1];
            TextWord3.text = "Apple";
        }
        

        ImageWord1.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        ImageWord2.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        ImageWord3.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        ImageWord1.anchoredPosition = new Vector2(-250, 205);
        ImageWord2.anchoredPosition = new Vector2(60, 220);
        ImageWord3.anchoredPosition = new Vector2(470, 290);
        ImageWord1.gameObject.SetActive(true);
        ImageWord2.gameObject.SetActive(true);
        ImageWord3.gameObject.SetActive(true);

        TextSentence.text = Current_Sentence;

        TextSentence.transform.DOScale(1.2f, 0.5f).SetLoops(2, LoopType.Yoyo);
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
        if (list.Count > 3)
        {
            while (list.Count > 3)
            {
                list.Remove(list[Random.Range(0, list.Count)]);
            }
        }

        if (!list.Contains(CurrentAsk))
        {
            list[0] = CurrentAsk;
        }
        list = Utility.RandomSortList(list);

        Words.Add(list[0].MapWordString);
        Words.Add(list[1].MapWordString);
        if (list.Count < 3)
        {
            string s = CurrentAsk.id.Split('_')[0] + CurrentAsk.id.Split('_')[1];
            for (int i = 0; i < ConfigManager.Get<ConfigWordLibraryModel>().Count; i++)
            {
                string w = ConfigManager.Get<ConfigWordLibraryModel>()[i].id;
                if (s == w.Split('_')[0] + w.Split('_')[1])
                {
                    string word = ConfigManager.Get<ConfigWordLibraryModel>()[i].word;
                    if (word != list[0].MapWordString && word != list[1].MapWordString)
                    {
                        Words.Add(word);
                        break;
                    }
                }
            }
        }
        else
        {
            Words.Add(list[2].MapWordString);
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
        if (isFirstPlayaudio)
        {
            isFirstPlayaudio = false;
            TextBg1.SetActive(true);
            yield return new WaitForSeconds(1);
            TextBg2.SetActive(true);
            yield return new WaitForSeconds(1);
            TextBg3.SetActive(true);
            yield return new WaitForSeconds(2);
            TextBg1.SetActive(false);
            TextBg2.SetActive(false);
            TextBg3.SetActive(false);
        }
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
        if (list.Count > 3)
        {
            while (list.Count > 3)
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
        if (list.Count < 3)
        {
            string s = CurrentSay.id.Split('_')[0] + CurrentSay.id.Split('_')[1];
            for (int i = 0; i < ConfigManager.Get<ConfigWordLibraryModel>().Count; i++)
            {
                string w = ConfigManager.Get<ConfigWordLibraryModel>()[i].id;
                if (s == w.Split('_')[0] + w.Split('_')[1])
                {
                    string word = ConfigManager.Get<ConfigWordLibraryModel>()[i].word;
                    if (word != list[0].MapWordString && word != list[1].MapWordString)
                    {
                        Words.Add(word);
                        break;
                    }
                }
            }
        }
        else
        {
            Words.Add(list[2].MapWordString);
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
        Words = Utility.RandomSortList(Words);
        audio_play = false;
        StartCoroutine(PlaySayAudio());
    }

    private bool isFirstPlayaudio = true;//开始播放一遍，后面就不放了。
    IEnumerator PlaySayAudio()
    {
        var length = AudioManager.Instance.Play(CurrentSay.answerSound).clip.length;
        yield return new WaitForSeconds(length);
        audio_play = true;
        if (isFirstPlayaudio)
        {
            isFirstPlayaudio = false;
            TextBg1.SetActive(true);
            yield return new WaitForSeconds(1);
            TextBg2.SetActive(true);
            yield return new WaitForSeconds(1);
            TextBg3.SetActive(true);
            yield return new WaitForSeconds(2);
            TextBg1.SetActive(false);
            TextBg2.SetActive(false);
            TextBg3.SetActive(false);
        }
    }

    /// <summary>
    ///  抓到娃娃开始往上移动
    /// </summary>
    public void CatchToUp()
    {
        CatchRect.DOKill();
        ImageCatch.sprite = GetS("UICatchBabyjiqi");

        switch (current_wawa.name)
        {
            case "ImageWord1":
                CheckCatch(TextWord1.text == Current_Word);
                break;
            case "ImageWord2":
                CheckCatch(TextWord2.text == Current_Word);
                break;
            case "ImageWord3":
                CheckCatch(TextWord3.text == Current_Word);
                break;
        }

    }
    /// <summary>
    ///  是否抓的单词正确
    /// </summary>
    /// <param name="b"></param>
    private void CheckCatch(bool b)
    {
        float time = (120 - CatchRect.anchoredPosition.y) / speed;
        float fall_time = Random.Range(time / 2, time);
        if (!b)
            StartCoroutine(Fall(fall_time));
        CatchRect.DOAnchorPosY(120, time).OnComplete(delegate
        {
            catched_bool = b; // 此时有没有抓到娃娃
            if (b)
            {
                time = (CatchRect.anchoredPosition.x - (ImageExit.anchoredPosition.x)) / speed;
                CatchRect.DOAnchorPosX(ImageExit.anchoredPosition.x, time).OnComplete(delegate
                {
                    catching_bool = true;
                    Horizontal_bool = false;

                    current_wawa.GetComponent<BoxCollider2D>().isTrigger = false;
                    current_wawa.GetComponent<Rigidbody2D>().isKinematic = false;
                    ImageCatch.sprite = GetS("UICatchBabyjiqi1");
                });
            }
            else
            {
                catching_bool = true;
                Horizontal_bool = false;
            }
        });
    }

    IEnumerator Fall(float f)
    {
        yield return new WaitForSeconds(f);
        current_wawa.GetComponent<BoxCollider2D>().isTrigger = false;
        current_wawa.GetComponent<Rigidbody2D>().isKinematic = false;
        CheckMatch();
    }
    /// <summary>
    ///  碰到下面阻挡的底板
    /// </summary>
    public void ToUnder()
    {
        current_wawa.GetComponent<BoxCollider2D>().isTrigger = true;
        current_wawa.GetComponent<Rigidbody2D>().isKinematic = true;
        current_wawa.parent = ImagePos;
        current_wawa = null;
    }

    /// <summary>
    ///  碰到下面接中娃娃的箱子
    /// </summary>
    public void ToExit()
    {
        current_wawa.SetParent(ImagePos);
        
        switch (current_wawa.name)
        {
            case "ImageWord1":
                CheckMatch(TextWord1.text == Current_Word);
                ImageWord1.anchoredPosition = new Vector2(-250, 205);
                break;
            case "ImageWord2":
                CheckMatch(TextWord2.text == Current_Word);
                ImageWord2.anchoredPosition = new Vector2(60, 220);
                break;
            case "ImageWord3":
                CheckMatch(TextWord3.text == Current_Word);
                ImageWord3.anchoredPosition = new Vector2(470, 290);
                break;
        }
        
    }
    
    private void CheckMatch(bool b = false)
    {
        if (b)
        {
            current_wawa.gameObject.SetActive(false);
            current_wawa.GetComponent<BoxCollider2D>().isTrigger = true;
            current_wawa.GetComponent<Rigidbody2D>().isKinematic = true;
            current_wawa = null;
            StartCoroutine(FlyTrue());
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

    IEnumerator FlyTrue()
    {
        audio_play = false;
        UITopBarStarFly fly;
        switch (StartData.dataType)
        {
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