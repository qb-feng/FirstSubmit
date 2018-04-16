using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UICandySeesaw : UIBaseLevelGame {


    public Transform CandyBox { get { return GetT("CandyBox"); } }


    public Transform BoardUp { get { return GetT("BoardUp"); } }
    public Transform BoardUpLeft { get { return GetT("BoardUpLeft"); } }
    public Transform BoardUpRight { get { return GetT("BoardUpRight"); } }


    public Transform BoardDown { get { return GetT("BoardDown"); } }
    public Transform BoardDownLeft { get { return GetT("BoardDownLeft"); } }
    public Transform BoardDownRight { get { return GetT("BoardDownRight"); } }


    private Image Audio { get { return Get<Image>("Audio"); } }
    public Image NextWord { get { return Get<Image>("NextWord"); } }
    private TextMeshProUGUI Word { get { return Get<TextMeshProUGUI>("Word"); } }

    private List<ConfigWordLibraryModel> tempRandomList;
    private int randomIndex;
    public static UICandySeesaw Instance { get; set; }


    //测试Drag用变量
    private Vector3 originalBoardUpLeftPosition;
    private Vector3 originalBoardUpRightPosition;
    private Vector3 originalBoardDownLeftPosition;
    private Vector3 originalBoardDownRightPosition;  
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
        NextWord.enabled = false;
        UGUIEventListener.Get(Audio).onPointerClick = OnClickAudio;
        //UGUIEventListener.Get(BoardUpLeft).onPointerClick = OnClickBoardUpLeft;
        //UGUIEventListener.Get(BoardUpRight).onPointerClick = OnClickBoardUpRight;
        //UGUIEventListener.Get(BoardDownLeft).onPointerClick = OnClickBoardDownLeft;
        //UGUIEventListener.Get(BoardDownRight).onPointerClick = OnClickBoardDownRight;
        UGUIEventListener.Get(NextWord).onPointerClick = OnClickNextWord;

        UGUIEventListener.Get(BoardUpLeft).onDrag = OnDragBoardUpLeft;
        UGUIEventListener.Get(BoardUpRight).onDrag = OnDragBoardUpRight;
        UGUIEventListener.Get(BoardDownLeft).onDrag = OnDragBoardDownLeft;
        UGUIEventListener.Get(BoardDownRight).onDrag = OnDragBoardDownRight;
        originalBoardUpLeftPosition = BoardUpLeft.position;
        originalBoardUpRightPosition=BoardUpRight.position;
        originalBoardDownLeftPosition = BoardDownLeft.position;
        originalBoardDownRightPosition = BoardDownRight.position;
 
    }
    private void OnDragBoardUpLeft(PointerEventData data)
    {       
        var currentWorld = data.pressEventCamera.ScreenToWorldPoint(data.position);
        var originalWorld = data.pressEventCamera.ScreenToWorldPoint(data.pressPosition);
        var TempPos = (Vector2)(originalBoardUpLeftPosition + currentWorld - originalWorld);         
       
        if (TempPos.y - originalBoardUpLeftPosition.y > 0)
            return;       
        BoardUp.localEulerAngles += new Vector3(0, 0, -(TempPos.y - originalBoardUpLeftPosition.y));
        BoardUpLeft.localEulerAngles = -BoardUp.localEulerAngles;
        BoardUpRight.localEulerAngles = -BoardUp.localEulerAngles;    
    
        if (BoardUp.localEulerAngles.z >= 30 &&BoardUp.localEulerAngles.z <180) 
        {           
            BoardUp.localEulerAngles = new Vector3(0, 0, 30);
            BoardUpLeft.localEulerAngles = new Vector3(0, 0, 330);
            BoardUpRight.localEulerAngles = new Vector3(0, 0, 330);
        }       
       
    }
    private void OnDragBoardUpRight(PointerEventData data)
    {        
        var currentWorld = data.pressEventCamera.ScreenToWorldPoint(data.position);
        var originalWorld = data.pressEventCamera.ScreenToWorldPoint(data.pressPosition);
        var TempPos = (Vector2)(originalBoardUpRightPosition + currentWorld - originalWorld);
        if (TempPos.y - originalBoardUpRightPosition.y > 0)
            return;
        BoardUp.localEulerAngles += new Vector3(0, 0, (TempPos.y - originalBoardUpRightPosition.y));
        BoardUpLeft.localEulerAngles = -BoardUp.localEulerAngles;
        BoardUpRight.localEulerAngles = -BoardUp.localEulerAngles;       

        if (BoardUp.localEulerAngles.z <= 330 && BoardUp.localEulerAngles.z>180)
        {
            BoardUp.localEulerAngles = new Vector3(0, 0, 330);
            BoardUpLeft.localEulerAngles = new Vector3(0, 0, 30);
            BoardUpRight.localEulerAngles = new Vector3(0, 0, 30);
        }         
    }
    private void OnDragBoardDownLeft(PointerEventData data)
    {
        var currentWorld = data.pressEventCamera.ScreenToWorldPoint(data.position);
        var originalWorld = data.pressEventCamera.ScreenToWorldPoint(data.pressPosition);
        var TempPos = (Vector2)(originalBoardDownLeftPosition + currentWorld - originalWorld);

        if (TempPos.y - originalBoardDownLeftPosition.y > 0)
            return;
        BoardDown.localEulerAngles += new Vector3(0, 0, -(TempPos.y - originalBoardDownLeftPosition.y));
        BoardDownLeft.localEulerAngles = -BoardDown.localEulerAngles;
        BoardDownRight.localEulerAngles = -BoardDown.localEulerAngles;

        if (BoardDown.localEulerAngles.z >= 30 && BoardDown.localEulerAngles.z < 180)
        {
            BoardDown.localEulerAngles = new Vector3(0, 0, 30);
            BoardDownLeft.localEulerAngles = new Vector3(0, 0, 330);
            BoardDownRight.localEulerAngles = new Vector3(0, 0, 330);
        }       
    }
    private void OnDragBoardDownRight(PointerEventData data)
    {
        var currentWorld = data.pressEventCamera.ScreenToWorldPoint(data.position);
        var originalWorld = data.pressEventCamera.ScreenToWorldPoint(data.pressPosition);
        var TempPos = (Vector2)(originalBoardDownRightPosition + currentWorld - originalWorld);
        if (TempPos.y - originalBoardDownRightPosition.y > 0)
            return;
        BoardDown.localEulerAngles += new Vector3(0, 0, (TempPos.y - originalBoardDownRightPosition.y));
        BoardDownLeft.localEulerAngles = -BoardDown.localEulerAngles;
        BoardDownRight.localEulerAngles = -BoardDown.localEulerAngles;

        if (BoardDown.localEulerAngles.z <= 330 && BoardDown.localEulerAngles.z > 180)
        {
            BoardDown.localEulerAngles = new Vector3(0, 0, 330);
            BoardDownLeft.localEulerAngles = new Vector3(0, 0, 30);
            BoardDownRight.localEulerAngles = new Vector3(0, 0, 30);
        }        
    }


    private void OnClickAudio(PointerEventData data)
    {
        if (CurrentWord.PlaySound().isPlaying)
            return;
        CurrentWord.PlaySound();
    }
    /*
    private void OnClickBoardUpLeft(PointerEventData data)
    {
        BoardUp.eulerAngles += new Vector3(0, 0, 10);
        BoardUpLeft.eulerAngles -= new Vector3(0, 0, 10);
        BoardUpRight.eulerAngles -= new Vector3(0, 0, 10);
    }
    private void OnClickBoardUpRight(PointerEventData data)
    {
        BoardUp.eulerAngles -= new Vector3(0, 0, 10);
        BoardUpLeft.eulerAngles += new Vector3(0, 0, 10);
        BoardUpRight.eulerAngles += new Vector3(0, 0, 10);
    }
    private void OnClickBoardDownLeft(PointerEventData data)
    {
        BoardDown.eulerAngles += new Vector3(0, 0, 10);
        BoardDownLeft.eulerAngles -= new Vector3(0, 0, 10);
        BoardDownRight.eulerAngles -= new Vector3(0, 0, 10);
    }
    private void OnClickBoardDownRight(PointerEventData data)
    {
        BoardDown.eulerAngles -= new Vector3(0, 0, 10);
        BoardDownLeft.eulerAngles += new Vector3(0, 0, 10);
        BoardDownRight.eulerAngles += new Vector3(0, 0, 10);
    }*/
    private void OnClickNextWord(PointerEventData data)
    {
        NextWord.enabled = false;
        BoardUp.eulerAngles = Vector3.zero;
        BoardDown.eulerAngles =Vector3.zero;
        BoardUpLeft.eulerAngles = Vector3.zero;
        BoardUpRight.eulerAngles = Vector3.zero;
        BoardDownLeft.eulerAngles = Vector3.zero;
        BoardDownRight.eulerAngles = Vector3.zero;
        if (tempRandomList[0].word == CurrentWord.word)
        {
            CreateUIItem("UICandySeesawCandy", CandyBox).AddComponent<UICandySeesawCandy>().Init("RightRegion", tempRandomList[0].id);
        }
        else
        {
            CreateUIItem("UICandySeesawCandy", CandyBox).AddComponent<UICandySeesawCandy>().Init("WrongRegion", tempRandomList[0].id);
        }
    }

    public override void PlayGame()
    {
        if (IsGameEnd)
            return;
        NextWord.enabled = false;
        CurrentWord.PlaySound();
        BoardUp.eulerAngles = Vector3.zero;
        BoardDown.eulerAngles = Vector3.zero;
        BoardUpLeft.eulerAngles = Vector3.zero;
        BoardUpRight.eulerAngles = Vector3.zero;
        BoardDownLeft.eulerAngles = Vector3.zero;
        BoardDownRight.eulerAngles = Vector3.zero;
        tempRandomList = new List<ConfigWordLibraryModel>(m_randomWordList);
        tempRandomList.Remove(CurrentWord);
        tempRandomList = Utility.RandomSortList(tempRandomList);
        randomIndex = Random.Range(0, 2);
        tempRandomList.Insert(randomIndex, CurrentWord);

        Word.text = tempRandomList[0].word;
        if (tempRandomList[0].word == CurrentWord.word)
        {
            CreateUIItem("UICandySeesawCandy", CandyBox).AddComponent<UICandySeesawCandy>().Init("RightRegion", tempRandomList[0].id);
        }
        else
        {
            CreateUIItem("UICandySeesawCandy", CandyBox).AddComponent<UICandySeesawCandy>().Init("WrongRegion", tempRandomList[0].id);
        }

        
    }
}
