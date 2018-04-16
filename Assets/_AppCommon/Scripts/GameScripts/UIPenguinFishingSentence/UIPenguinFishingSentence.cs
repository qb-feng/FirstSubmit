using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIPenguinFishingSentence : UIBaseLevelGame
{
    public static UIPenguinFishingSentence Instance { get; set; }

    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        Instance = null;
    }

    /// <summary>
    /// 变量
    /// </summary>
    #region
    private Transform Audio { get { return GetT("Audio"); } }
    public Transform Cake { get { return GetT("Cake"); } }
    private Transform MouseBirth { get { return GetT("MouseBirth"); } }
    private Transform DownStoneAnswer { get { return GetT("DownStoneAnswer"); } }
    //当前老鼠对象
    private GameObject myMouse;
    public RectTransform myRMouse;
    //移动动画
    public Tween myMouseTween;
    //移动路径
    public Vector3[] movePath;
    //移动路径
    private List<Vector3> movePathList;
    //移动路径总长度
    private float disLength;
    //选项列表打乱顺序使用
    private List<GameObject> waitSelectStone;
    //梯子路径
    public Vector3[] ladderPath;
    //单词
    private Transform Word { get { return GetT("Word"); } }
    private Image WordSprite { get { return Get<Image>("WordSprite"); } }
    private Transform WordStonePos { get { return GetT("WordStonePos"); } }
    private Transform WordLadderPos { get { return GetT("WordLadderPos"); } }
    private List<string> wordSort;
    private int wordLength;
    //陈述句
    private string[] saySentenceS;
    private List<string> sayMapWordL;
    private string saySentence;
    private string sayWord;
    //Ask
    private Transform Ask { get { return GetT("Ask"); } }
    private Transform AskStonePosUp { get { return GetT("AskStonePosUp"); } }
    private Transform AskLadderRightPos { get { return GetT("AskLadderRightPos"); } }
    public Transform AskStonePosDown { get { return GetT("AskStonePosDown"); } }
    private Transform AskLadderLeftPos { get { return GetT("AskLadderLeftPos"); } }
    private string[] sAskUpSentence;
    private string[] sAskDownSentence;
    private string sAskUpWord;
    private string sAskDownWord;
    public Vector3[] RightLadderPath;
    public Vector3[] LeftLadderPath;
    public bool UpTureDownFlase = true;

    private int SentenceWordAmount;
    private List<Transform> AllTextList;
    #endregion

    public override void Refresh()
    {
        UGUIEventListener.Get(Audio).onPointerClick = OnAudioClick;
    }

    private void OnAudioClick(PointerEventData t)
    {
        switch (StartData.dataType)
        {
            case DataType.Word:
                CurrentWord.PlaySound();
                break;
            case DataType.Say:
                CurrentSay.PlayAnswerSound();
                break;
            case DataType.Ask:
                CurrentAsk.PlaySentenceSound();
                break;
        }
    }
    public override void PlayGame()
    {
        if (IsGameEnd)
            return;
        InitAll();

        saySentence = CurrentSay.yes + CurrentSay.no;
        saySentenceS = saySentence.Split('_');
        SentenceWordAmount = saySentenceS.Length;

        if (SentenceWordAmount > 0 && SentenceWordAmount <=4)
        {
            SetShortSay();
        }
        else
        {
            SetLongSay();
        }
       
    }
    private void InitAll()
    {
        WordStonePos.ClearAllChild();
        MouseBirth.ClearAllChild();
        DownStoneAnswer.ClearAllChild();
        AskStonePosUp.ClearAllChild();
        AskStonePosDown.ClearAllChild();
        UpTureDownFlase = true;
        if (myMouseTween != null)
        {
            myMouseTween.Kill();
        }
    }


    private void SetShortSay()
    {
        Word.gameObject.SetActive(true);
        Ask.gameObject.SetActive(false);
        CurrentSay.PlayAnswerSound();
        WordSprite.sprite = CurrentSay.MapWordSprite;

        for (int i = 0; i < 3; i++)
        {
            CreateUIItem("UIPenguinFishingStone", WordStonePos).AddComponent<UIPenguinFishingSentenceStone>().InitWord("", true);
        }
        for (int i = 0; i < saySentenceS.Length; i++)
        {
            CreateUIItem("UIPenguinFishingStone", WordStonePos).AddComponent<UIPenguinFishingSentenceStone>().InitWord(saySentenceS[i], false);
        }
        for (int i = 0; i < 3; i++)
        {
            CreateUIItem("UIPenguinFishingStone", WordStonePos).AddComponent<UIPenguinFishingSentenceStone>().InitWord("", true);
        }
        StartCoroutine(ShortMouseStar());

    }
    private void SetLongSay()
    {

        Word.gameObject.SetActive(false);
        Ask.gameObject.SetActive(true);
        AskLadderLeftPos.gameObject.SetActive(false);
        AskLadderRightPos.gameObject.SetActive(false);
        CurrentSay.PlayAnswerSound();
        //生成第一层石块        
        for (int i = 0; i < 3; i++)
        {
            CreateUIItem("UIPenguinFishingStone", AskStonePosUp).AddComponent<UIPenguinFishingSentenceStone>().InitWord("", true);
        }
        for (int i = 0; i < 4; i++)
        {
            CreateUIItem("UIPenguinFishingStone", AskStonePosUp).AddComponent<UIPenguinFishingSentenceStone>().InitWord(saySentenceS[i], false);
        }
        for (int i = 0; i < 3; i++)
        {
            CreateUIItem("UIPenguinFishingStone", AskStonePosUp).AddComponent<UIPenguinFishingSentenceStone>().InitWord("", true);
        }
        //生成第二层石块
        for (int i = 0; i < 2; i++)
        {
            CreateUIItem("UIPenguinFishingStone", AskStonePosDown).AddComponent<UIPenguinFishingSentenceStone>().InitWord("", true);
        }
        for (int i = 4; i < saySentenceS.Length; i++)
        {
            CreateUIItem("UIPenguinFishingStone", AskStonePosDown).AddComponent<UIPenguinFishingSentenceStone>().InitWord(saySentenceS[i], false);
        }
        for (int i = 0; i < 4; i++)
        {
            CreateUIItem("UIPenguinFishingStone", AskStonePosDown).AddComponent<UIPenguinFishingSentenceStone>().InitWord("", true);
        }

        StartCoroutine(LongMouseStar());

    }
    //SetWord
    #region

    IEnumerator ShortMouseStar()
    {
        yield return new WaitForSeconds(0);
        yield return new WaitForSeconds(0);
        //生成选项
        waitSelectStone = new List<GameObject>();
        for (int i = 0; i < WordStonePos.childCount; i++)
        {
            if (WordStonePos.GetChild(i).GetComponent<UIPenguinFishingSentenceStone>().isWork == false)
            {
                waitSelectStone.Add(WordStonePos.GetChild(i).gameObject);
            }
        }
        waitSelectStone = Utility.RandomSortList(waitSelectStone);
        for (int i = 0; i < waitSelectStone.Count; i++)
        {
            Instantiate(waitSelectStone[i], DownStoneAnswer).AddComponent<UIPenguinFishingSentenceStoneSelect>();
        }
        //梯子路径
        ladderPath = new Vector3[WordLadderPos.childCount];
        for (int i = 0; i < WordLadderPos.childCount; i++)
        {
            ladderPath[i] = WordLadderPos.GetChild(i).position;
        }
        //创建老鼠
        myMouse = CreateUIItem("UIPenguinFishingMouse", MouseBirth);
        Vector3 tempMousePos = WordStonePos.GetChild(0).position;
        tempMousePos.y += 0.12f;
        myMouse.transform.position = tempMousePos;
        myMouse.AddComponent<UIPenguinFishingSentenceMouse>();
        myRMouse = (RectTransform)myMouse.transform;

        //进行寻路     
        CreatePath();
        MouseMoveRefresh();
    }
    #endregion
    //SetSay
    #region
    void SetSay()
    {
        Word.gameObject.SetActive(true);
        Ask.gameObject.SetActive(false);
        CurrentSay.PlayAnswerSound();
        WordSprite.sprite = CurrentSay.MapWordSprite;

        List<ConfigSayLibraryModel> list = new List<ConfigSayLibraryModel>();
        sayMapWordL = new List<string>();

        list = m_currentUnitAllSayList;
        for (int i = 0; i < list.Count; i++)
        {
            sayMapWordL.Add(list[i].MapWordString);
        }
        sayMapWordL.Remove(CurrentSay.MapWordString);
        sayMapWordL = Utility.RandomSortList(sayMapWordL);

        //防止关键词为词组进行替换
        saySentence = CurrentSay.yes + CurrentSay.no;
        saySentence = saySentence.ToLower();
        saySentence = saySentence.Replace(",", "");
        saySentence = saySentence.Replace(".", "");
        saySentence = saySentence.Replace("!", "");
        sayWord = CurrentSay.MapWordString.Replace(' ', '_');
        string tempString = sayWord.Replace('_', ' ');
        saySentence = saySentence.Replace(sayWord, tempString);
        saySentenceS = saySentence.Split('_');

        //生成石块
        for (int i = 0; i < 2; i++)
        {
            CreateUIItem("UIPenguinFishingStone", WordStonePos).AddComponent<UIPenguinFishingStone>().InitWord("", true);
        }
        for (int i = 0; i < saySentenceS.Length; i++)
        {
            CreateUIItem("UIPenguinFishingStone", WordStonePos).AddComponent<UIPenguinFishingStone>().InitWord(saySentenceS[i], saySentenceS[i] != CurrentSay.MapWordString);
        }
        for (int i = 0; i < 4; i++)
        {
            CreateUIItem("UIPenguinFishingStone", WordStonePos).AddComponent<UIPenguinFishingStone>().InitWord("", true);
        }

        StartCoroutine(SayMouseStar());
    }
    IEnumerator SayMouseStar()
    {
        yield return new WaitForSeconds(0);
        yield return new WaitForSeconds(0);

        //生成选项
        waitSelectStone = new List<GameObject>();
        for (int i = 0; i < WordStonePos.childCount; i++)
        {
            if (WordStonePos.GetChild(i).GetComponent<UIPenguinFishingStone>().isWork == false)
            {
                waitSelectStone.Add(WordStonePos.GetChild(i).gameObject);
            }
        }
        waitSelectStone.Add(Instantiate(waitSelectStone[0]));
        waitSelectStone.Add(Instantiate(waitSelectStone[0]));

        waitSelectStone[1].GetComponent<UIPenguinFishingStone>().answerStr = sayMapWordL[0];
        waitSelectStone[1].GetComponent<UIPenguinFishingStone>().StoneText.text = sayMapWordL[0];
        waitSelectStone[2].GetComponent<UIPenguinFishingStone>().answerStr = sayMapWordL[1];
        waitSelectStone[2].GetComponent<UIPenguinFishingStone>().StoneText.text = sayMapWordL[1];

        waitSelectStone = Utility.RandomSortList(waitSelectStone);
        for (int i = 0; i < waitSelectStone.Count; i++)
        {
            Instantiate(waitSelectStone[i], DownStoneAnswer).AddComponent<UIPenguinFishingStoneSelect>();
        }

        //梯子路径
        ladderPath = new Vector3[WordLadderPos.childCount];
        for (int i = 0; i < WordLadderPos.childCount; i++)
        {
            ladderPath[i] = WordLadderPos.GetChild(i).position;
        }


        //创建老鼠
        myMouse = CreateUIItem("UIPenguinFishingMouse", MouseBirth);
        Vector3 tempMousePos = WordStonePos.GetChild(0).position;
        tempMousePos.y += 0.12f;
        myMouse.transform.position = tempMousePos;
        myMouse.AddComponent<UIPenguinFishingMouse>();
        myRMouse = (RectTransform)myMouse.transform;

        //进行寻路     
        CreatePath();
        MouseMoveRefresh();
    }
    #endregion
    //SetTalk
    #region
    private void SetAsk()
    {
        Word.gameObject.SetActive(false);
        Ask.gameObject.SetActive(true);
        AskLadderLeftPos.gameObject.SetActive(false);
        AskLadderRightPos.gameObject.SetActive(false);
        CurrentAsk.PlaySentenceSound();
        sAskUpSentence = CurrentAsk.ask.Split('_');
        string tempSentence = CurrentAsk.yes + CurrentAsk.no;
        sAskDownSentence = tempSentence.Split('_');
        sAskUpWord = sAskUpSentence[Random.Range(0, sAskUpSentence.Length)];
        sAskDownWord = sAskDownSentence[Random.Range(0, sAskDownSentence.Length)];

        //生成第一层石块        
        for (int i = 0; i < 2; i++)
        {
            CreateUIItem("UIPenguinFishingStone", AskStonePosUp).AddComponent<UIPenguinFishingStone>().InitWord("", true);
        }
        for (int i = 0; i < sAskUpSentence.Length; i++)
        {
            CreateUIItem("UIPenguinFishingStone", AskStonePosUp).AddComponent<UIPenguinFishingStone>().InitWord(sAskUpSentence[i], sAskUpSentence[i] != sAskUpWord);
        }
        for (int i = 0; i < 4; i++)
        {
            CreateUIItem("UIPenguinFishingStone", AskStonePosUp).AddComponent<UIPenguinFishingStone>().InitWord("", true);
        }
        //生成第二层石块
        for (int i = 0; i < 2; i++)
        {
            CreateUIItem("UIPenguinFishingStone", AskStonePosDown).AddComponent<UIPenguinFishingStone>().InitWord("", true);
        }
        for (int i = 0; i < sAskDownSentence.Length; i++)
        {
            CreateUIItem("UIPenguinFishingStone", AskStonePosDown).AddComponent<UIPenguinFishingStone>().InitWord(sAskDownSentence[i], sAskDownSentence[i] != sAskDownWord);
        }
        for (int i = 0; i < 4; i++)
        {
            CreateUIItem("UIPenguinFishingStone", AskStonePosDown).AddComponent<UIPenguinFishingStone>().InitWord("", true);
        }


        StartCoroutine(LongMouseStar());
    }
    IEnumerator LongMouseStar()
    {
        yield return new WaitForSeconds(0);
        Vector3 tempLadderVec = AskLadderLeftPos.position;
        tempLadderVec.x = AskStonePosDown.GetChild(1).position.x;
        AskLadderLeftPos.position = tempLadderVec;

        tempLadderVec = AskLadderRightPos.position;
        tempLadderVec.x = AskStonePosUp.GetChild(AskStonePosUp.childCount - 2).position.x;
        AskLadderRightPos.position = tempLadderVec;

        AskLadderLeftPos.gameObject.SetActive(true);
        AskLadderRightPos.gameObject.SetActive(true);
        yield return new WaitForSeconds(0);
        //生成选项
        waitSelectStone = new List<GameObject>();
        for (int i = 0; i < AskStonePosUp.childCount; i++)
        {
            if (AskStonePosUp.GetChild(i).GetComponent<UIPenguinFishingSentenceStone>().isWork == false)
            {
                waitSelectStone.Add(AskStonePosUp.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < AskStonePosDown.childCount; i++)
        {
            if (AskStonePosDown.GetChild(i).GetComponent<UIPenguinFishingSentenceStone>().isWork == false)
            {
                waitSelectStone.Add(AskStonePosDown.GetChild(i).gameObject);
            }
        }       

        waitSelectStone = Utility.RandomSortList(waitSelectStone);
        for (int i = 0; i < waitSelectStone.Count; i++)
        {
            Instantiate(waitSelectStone[i], DownStoneAnswer).AddComponent<UIPenguinFishingSentenceStoneSelect>();
        }

        //梯子路径
        RightLadderPath = new Vector3[AskLadderRightPos.childCount];
        for (int i = 0; i < AskLadderRightPos.childCount; i++)
        {
            RightLadderPath[i] = AskLadderRightPos.GetChild(i).position;
        }
        LeftLadderPath = new Vector3[AskLadderLeftPos.childCount];
        for (int i = 0; i < AskLadderLeftPos.childCount; i++)
        {
            LeftLadderPath[i] = AskLadderLeftPos.GetChild(i).position;
        }

        //创建老鼠
        myMouse = CreateUIItem("UIPenguinFishingMouse", MouseBirth);
        Vector3 tempMousePos = AskStonePosUp.GetChild(0).position;
        tempMousePos.y += 0.12f;
        myMouse.transform.position = tempMousePos;
        myMouse.AddComponent<UIPenguinFishingSentenceMouse>();
        myRMouse = (RectTransform)myMouse.transform;

        //进行寻路     
        AskCreatePath(UpTureDownFlase);
        AskMouseMoveRefresh();
    }
    #endregion
    /// <summary>
    /// 创建老鼠移动路径
    /// </summary>
    void CreatePath()
    {
        movePathList = new List<Vector3>();
        for (int i = 0; i < WordStonePos.childCount; i++)
        {
            if (WordStonePos.GetChild(i).GetComponent<UIPenguinFishingSentenceStone>().isWork)
            {
                movePathList.Add(WordStonePos.GetChild(i).position);
            }
            else
            {
                break;
            }
        }
        movePath = new Vector3[movePathList.Count];
        for (int i = 0; i < movePathList.Count; i++)
        {
            movePath[i] = movePathList[i];
            movePath[i].y += 0.12f;
        }
    }
    public void AskCreatePath(bool UpOrDown)
    {
        if (UpOrDown)
        {
            movePathList = new List<Vector3>();
            for (int i = 0; i < AskStonePosUp.childCount; i++)
            {
                if (AskStonePosUp.GetChild(i).GetComponent<UIPenguinFishingSentenceStone>().isWork)
                {
                    movePathList.Add(AskStonePosUp.GetChild(i).position);
                }
                else
                {
                    break;
                }
            }
            movePath = new Vector3[movePathList.Count];
            for (int i = 0; i < movePathList.Count; i++)
            {
                movePath[i] = movePathList[i];
                movePath[i].y += 0.12f;
            }
        }
        else
        {
            movePathList = new List<Vector3>();
            for (int i = AskStonePosDown.childCount - 1; i > 0; i--)
            {
                if (AskStonePosDown.GetChild(i).GetComponent<UIPenguinFishingSentenceStone>().isWork)
                {
                    movePathList.Add(AskStonePosDown.GetChild(i).position);
                }
                else
                {
                    break;
                }
            }
            movePath = new Vector3[movePathList.Count];
            for (int i = 0; i < movePathList.Count; i++)
            {
                movePath[i] = movePathList[i];
                movePath[i].y += 0.12f;
            }
        }
    }

    /// <summary>
    /// 每次从开始点刷新路径
    /// </summary>
    public void MouseMoveRefresh()
    {
        disLength = 0;

        for (int i = 1; i < movePath.Length; i++)
        {
            disLength += Vector3.Distance(movePath[i - 1], movePath[i]);
        }
        myMouseTween = myMouse.transform.DOPath(movePath, disLength / 3.5f).SetOptions(true).OnWaypointChange(MyCallback);

        myMouseTween.onComplete += () =>
        {
            CreatePath();
            MouseMoveRefresh();
        };
    }
    public void AskMouseMoveRefresh()
    {
        disLength = 0;

        for (int i = 1; i < movePath.Length; i++)
        {
            disLength += Vector3.Distance(movePath[i - 1], movePath[i]);
        }
        myMouseTween = myMouse.transform.DOPath(movePath, disLength / 3.5f).SetOptions(true).OnWaypointChange(MyCallback);

        myMouseTween.onComplete += () =>
        {
            AskCreatePath(UpTureDownFlase);
            AskMouseMoveRefresh();
        };
    }

    /// <summary>
    /// 路径回调
    /// </summary>
    /// <param name="waypointIndex"></param>
    void MyCallback(int waypointIndex)
    {
        if (UpTureDownFlase)
        {
            if (waypointIndex == movePath.Length - 1)
            {
                myRMouse.eulerAngles = new Vector3(0, 180, 0);
            }
            if (waypointIndex == 0)
            {
                myRMouse.eulerAngles = Vector3.zero;
            }
        }
        else
        {
            if (waypointIndex == 0)
            {
                myRMouse.eulerAngles = new Vector3(0, 180, 0);
            }
            if (waypointIndex == movePath.Length - 1)
            {
                myRMouse.eulerAngles = Vector3.zero;
            }
        }
    }
    /// <summary>
    /// 飞星判断
    /// </summary>
    /// <param name="FlyOrNo"></param>
    public void DecideStar(bool FlyOrNo)
    {
        if (FlyOrNo)
        {
            switch (StartData.dataType)
            {
                case DataType.Word:
                    CurrentWord.PlaySound();
                    UITopBarStarFly fly = FlyStar(true, true);
                    fly.OnComplete += () =>
                    {
                        PlayGame();
                    };
                    break;
                case DataType.Say:
                    if (SentenceWordAmount > 0 && SentenceWordAmount <= 4)
                    {
                        EndShowSayText();
                    }
                    else
                    {
                        EndShowAskText();
                    }
                    StartCoroutine(playCurrentSay());
                    break;
                case DataType.Ask:
                    StartCoroutine(playCurrentTalk());
                    break;

            }
        }
        else
        {
            FlyStar(false, false);
        }
    }
    IEnumerator playCurrentSay()
    {
        var length = AudioManager.Instance.Play(CurrentSay.answerSound).clip.length;
        yield return new WaitForSeconds(length);
        FlyStar(true, false).OnComplete += () =>
        {
            PlayGame();
        };
    }
    IEnumerator playCurrentTalk()
    {
        var length = AudioManager.Instance.Play(CurrentAsk.id).clip.length;
        yield return new WaitForSeconds(length);
        length = AudioManager.Instance.Play(CurrentAsk.answerSound).clip.length;
        yield return new WaitForSeconds(length);
        FlyStar(true, false).OnComplete += () =>
        {
            PlayGame();
        };
    }
    private void EndShowSayText()
    {
        AllTextList = new List<Transform>();
        for (int i = 0; i < WordStonePos.childCount; i++)
        {
            AllTextList.Add(WordStonePos.GetChild(i).GetChild(0).GetChild(0));
        }
        for (int i = 0; i < DownStoneAnswer.childCount; i++)
        {
            if (DownStoneAnswer.GetChild(i).GetComponent<UIPenguinFishingSentenceStone>().isRightShow)
            {
                AllTextList.Add(DownStoneAnswer.GetChild(i).GetChild(0).GetChild(0).transform);
            }
        }
        for (int i = 0; i < AllTextList.Count; i++)
        {
            AllTextList[i].DOScale(2f, 1.5f).From();
        }
    }
    private void EndShowAskText()
    {
        AllTextList = new List<Transform>();
        for (int i = 0; i < AskStonePosUp.childCount; i++)
        {
            AllTextList.Add(AskStonePosUp.GetChild(i).GetChild(0).GetChild(0));
        }
        for (int i = 0; i < AskStonePosDown.childCount; i++)
        {
            AllTextList.Add(AskStonePosDown.GetChild(i).GetChild(0).GetChild(0));
        }
        for (int i = 0; i < DownStoneAnswer.childCount; i++)
        {
            if (DownStoneAnswer.GetChild(i).GetComponent<UIPenguinFishingSentenceStone>().isRightShow)
            {
                AllTextList.Add(DownStoneAnswer.GetChild(i).GetChild(0).GetChild(0).transform);
            }
        }
        for (int i = 0; i < AllTextList.Count; i++)
        {
            AllTextList[i].DOScale(2f, 1.5f).From();
        }
    }
}
