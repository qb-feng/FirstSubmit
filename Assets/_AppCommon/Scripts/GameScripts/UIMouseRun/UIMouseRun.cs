using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;

public class UIMouseRun : UIBaseLevelGame
{
    public static UIMouseRun Instance { get; set; }

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
        switch (StartData.dataType)
        {
            case DataType.Word:
                SetWord();
                break;
            case DataType.Say:
                SetSay();
                break;
            case DataType.Ask:
                SetAsk();
                break;
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
    //SetWord
    #region
    private void SetWord()
    {
        Word.gameObject.SetActive(true);
        Ask.gameObject.SetActive(false);
        CurrentWord.PlaySound();
        WordSprite.sprite = CurrentWord.sprite;
        char[] temp = new char[CurrentWord.word.Length];
        wordSort = new List<string>();
        temp = CurrentWord.word.ToCharArray();
        for (int i = 0; i < temp.Length; i++)
        {
            wordSort.Add(temp[i].ToString());
        }
        while (wordSort.Contains(" "))
        {
            wordSort.Remove(" ");
        }
        //生成石块
        for (int i = 0; i < 2; i++)
        {
            CreateUIItem("UIMouseRunStone", WordStonePos).AddComponent<UIMouseRunStone>().InitWord("", true);
        }
        for (int i = 0; i < wordSort.Count; i++)
        {
            CreateUIItem("UIMouseRunStone", WordStonePos).AddComponent<UIMouseRunStone>().InitWord(wordSort[i], false);
        }
        for (int i = 0; i < 4; i++)
        {
            CreateUIItem("UIMouseRunStone", WordStonePos).AddComponent<UIMouseRunStone>().InitWord("", true);
        }

        StartCoroutine(WordMouseStar());
    }
    IEnumerator WordMouseStar()
    {
        yield return new WaitForSeconds(0);
        yield return new WaitForSeconds(0);

        //生成选项
        waitSelectStone = new List<GameObject>();
        for (int i = 0; i < WordStonePos.childCount; i++)
        {
            if (WordStonePos.GetChild(i).GetComponent<UIMouseRunStone>().isWork == false)
            {
                waitSelectStone.Add(WordStonePos.GetChild(i).gameObject);
            }
        }
        waitSelectStone = Utility.RandomSortList(waitSelectStone);
        for (int i = 0; i < waitSelectStone.Count; i++)
        {
            Instantiate(waitSelectStone[i], DownStoneAnswer).AddComponent<UIMouseRunStoneSelect>();
        }

        //梯子路径
        ladderPath = new Vector3[WordLadderPos.childCount];
        for (int i = 0; i < WordLadderPos.childCount; i++)
        {
            ladderPath[i] = WordLadderPos.GetChild(i).position;
        }


        //创建老鼠
        myMouse = CreateUIItem("UIMouseRunMouse", MouseBirth);
        Vector3 tempMousePos = WordStonePos.GetChild(0).position;
        tempMousePos.y += 0.12f;
        myMouse.transform.position = tempMousePos;
        myMouse.AddComponent<UIMouseRunMouse>();
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
            CreateUIItem("UIMouseRunStone", WordStonePos).AddComponent<UIMouseRunStone>().InitWord("", true);
        }
        for (int i = 0; i < saySentenceS.Length; i++)
        {            
            //CreateUIItem("UIMouseRunStone", WordStonePos).AddComponent<UIMouseRunStone>().InitWord(saySentenceS[i], saySentenceS[i] != CurrentSay.MapWordString);
            CreateUIItem("UIMouseRunStone", WordStonePos).AddComponent<UIMouseRunStone>().InitWord(saySentenceS[i], !saySentenceS[i].Contains(CurrentSay.MapWordString));
        }
        for (int i = 0; i < 4; i++)
        {
            CreateUIItem("UIMouseRunStone", WordStonePos).AddComponent<UIMouseRunStone>().InitWord("", true);
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
            if (WordStonePos.GetChild(i).GetComponent<UIMouseRunStone>().isWork == false)
            {
                waitSelectStone.Add(WordStonePos.GetChild(i).gameObject);
            }
        }
        waitSelectStone.Add(Instantiate(waitSelectStone[0]));
        waitSelectStone.Add(Instantiate(waitSelectStone[0]));

        waitSelectStone[1].GetComponent<UIMouseRunStone>().answerStr = sayMapWordL[0];
        waitSelectStone[1].GetComponent<UIMouseRunStone>().StoneText.text = sayMapWordL[0];
        waitSelectStone[2].GetComponent<UIMouseRunStone>().answerStr = sayMapWordL[1];
        waitSelectStone[2].GetComponent<UIMouseRunStone>().StoneText.text = sayMapWordL[1];

        waitSelectStone = Utility.RandomSortList(waitSelectStone);
        for (int i = 0; i < waitSelectStone.Count; i++)
        {
            Instantiate(waitSelectStone[i], DownStoneAnswer).AddComponent<UIMouseRunStoneSelect>();
        }

       

        //梯子路径
        ladderPath = new Vector3[WordLadderPos.childCount];
        for (int i = 0; i < WordLadderPos.childCount; i++)
        {
            ladderPath[i] = WordLadderPos.GetChild(i).position;
        }


        //创建老鼠
        myMouse = CreateUIItem("UIMouseRunMouse", MouseBirth);
        Vector3 tempMousePos = WordStonePos.GetChild(0).position;
        tempMousePos.y += 0.12f;
        myMouse.transform.position = tempMousePos;
        myMouse.AddComponent<UIMouseRunMouse>();
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
            CreateUIItem("UIMouseRunStone", AskStonePosUp).AddComponent<UIMouseRunStone>().InitWord("", true);
        }
        for (int i = 0; i < sAskUpSentence.Length; i++)
        {
            CreateUIItem("UIMouseRunStone", AskStonePosUp).AddComponent<UIMouseRunStone>().InitWord(sAskUpSentence[i], sAskUpSentence[i] != sAskUpWord);
        }
        for (int i = 0; i < 4; i++)
        {
            CreateUIItem("UIMouseRunStone", AskStonePosUp).AddComponent<UIMouseRunStone>().InitWord("", true);
        }
        //生成第二层石块
        for (int i = 0; i < 2; i++)
        {
            CreateUIItem("UIMouseRunStone", AskStonePosDown).AddComponent<UIMouseRunStone>().InitWord("", true);
        }
        for (int i = 0; i < sAskDownSentence.Length; i++)
        {
            CreateUIItem("UIMouseRunStone", AskStonePosDown).AddComponent<UIMouseRunStone>().InitWord(sAskDownSentence[i], sAskDownSentence[i] != sAskDownWord);
        }
        for (int i = 0; i < 4; i++)
        {
            CreateUIItem("UIMouseRunStone", AskStonePosDown).AddComponent<UIMouseRunStone>().InitWord("", true);
        }
        

        StartCoroutine(AskMouseStar());
    }
    IEnumerator AskMouseStar()
    {
        yield return new WaitForSeconds(0);
        Vector3 tempLadderVec = AskLadderLeftPos.position;
        tempLadderVec.x = AskStonePosDown.GetChild(1).position.x;
        AskLadderLeftPos.position = tempLadderVec;

        tempLadderVec = AskLadderRightPos.position;
        tempLadderVec.x = AskStonePosUp.GetChild(AskStonePosUp.childCount-2).position.x;
        AskLadderRightPos.position = tempLadderVec;

        AskLadderLeftPos.gameObject.SetActive(true);
        AskLadderRightPos.gameObject.SetActive(true);
        yield return new WaitForSeconds(0);
        //生成选项
        waitSelectStone = new List<GameObject>();
        for (int i = 0; i < AskStonePosUp.childCount; i++)
        {
            if (AskStonePosUp.GetChild(i).GetComponent<UIMouseRunStone>().isWork == false)
            {
                waitSelectStone.Add(AskStonePosUp.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < AskStonePosDown.childCount; i++)
        {
            if (AskStonePosDown.GetChild(i).GetComponent<UIMouseRunStone>().isWork == false)
            {
                waitSelectStone.Add(AskStonePosDown.GetChild(i).gameObject);
            }
        }
        waitSelectStone.Add(Instantiate(waitSelectStone[0]));
        waitSelectStone.Add(Instantiate(waitSelectStone[1]));
        string tempWord = "";
        for (int i = 0; i < sAskUpSentence.Length; i++)
        {
            if (sAskUpSentence[i] != sAskUpWord)
            {
                tempWord = sAskUpSentence[i];
                break;
            }
        }
        waitSelectStone[2].GetComponent<UIMouseRunStone>().answerStr = tempWord;
        waitSelectStone[2].GetComponent<UIMouseRunStone>().StoneText.text = tempWord;
        for (int i = 0; i < sAskDownSentence.Length; i++)
        {
            if (sAskDownSentence[i] != sAskDownWord)
            {
                tempWord = sAskDownSentence[i];
                break;
            }
        }
        waitSelectStone[3].GetComponent<UIMouseRunStone>().answerStr = tempWord;
        waitSelectStone[3].GetComponent<UIMouseRunStone>().StoneText.text = tempWord;

        waitSelectStone = Utility.RandomSortList(waitSelectStone);
        for (int i = 0; i < waitSelectStone.Count; i++)
        {
            Instantiate(waitSelectStone[i], DownStoneAnswer).AddComponent<UIMouseRunStoneSelect>();           
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
        myMouse = CreateUIItem("UIMouseRunMouse", MouseBirth);
        Vector3 tempMousePos = AskStonePosUp.GetChild(0).position;
        tempMousePos.y += 0.12f;
        myMouse.transform.position = tempMousePos;
        myMouse.AddComponent<UIMouseRunMouse>();
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
            if (WordStonePos.GetChild(i).GetComponent<UIMouseRunStone>().isWork)
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
                if (AskStonePosUp.GetChild(i).GetComponent<UIMouseRunStone>().isWork)
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
                if (AskStonePosDown.GetChild(i).GetComponent<UIMouseRunStone>().isWork)
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
                    WordSprite.transform.DOScale(1.5f, 1f).From();
                    UITopBarStarFly fly = FlyStar(true, true);
                    fly.OnComplete += () =>
                    {
                        PlayGame();
                    };
                    break;
                case DataType.Say:
                    EndShowSayText();
                    StartCoroutine(playCurrentSay());
                    break;
                case DataType.Ask:
                    EndShowAskText();
                    StartCoroutine(playCurrentTalk());
                    break;

            }
        }
        else
        {
            FlyStar(false, false);
        }
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
            if (DownStoneAnswer.GetChild(i).GetComponent<UIMouseRunStone>().isRightShow)
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
            if (DownStoneAnswer.GetChild(i).GetComponent<UIMouseRunStone>().isRightShow)
            {
                AllTextList.Add(DownStoneAnswer.GetChild(i).GetChild(0).GetChild(0).transform);
            }
        }
        for (int i = 0; i < AllTextList.Count; i++)
        {
            AllTextList[i].DOScale(2f, 1.5f).From();
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

}
