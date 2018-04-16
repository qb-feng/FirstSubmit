using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class UIRunningFrog : UIBaseLevelGame {
    public static UIRunningFrog Instance{get;set;}
    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        Instance = null;
    }

    private Transform UIRunningFrogRiver0 { get { return GetT("UIRunningFrogRiver0");} }
    private TextMeshProUGUI AnswerText { get { return Get<TextMeshProUGUI>("AnswerText"); } }
    private Image AnswerImage { get { return Get<Image>("AnswerImage"); } }
    //临时变量
    private Transform tempTrans;
    private GameObject tempGame;
    private RectTransform tempRecr;

    //河道地图加载
    private int RiverTotal;
    Vector3 currentPosition;
    private int randomIndex;
    private int randomIndexT;
    private int ClickTota=2;
    private float RiverHight;
    //青蛙记录
   public GameObject originFrog;
   private  GameObject clickFrog;
    //青蛙面前的两个荷叶
   private GameObject targetTransOne;
   private GameObject targetTransTwo;

    //进行记录出题
    private int RiverIndex=-1;
    private int circulationIndex;
    //单词
    private List<string> Words = new List<string>();
    public string Current_Answer;
    public string Current_Sentence;
    private bool audio_play = true;
    
    //点击一次有效
    public bool FirstClick;

    public override void Refresh()
    {
        FirstClick = true;
        RiverHight=this.transform.parent.GetComponent<RectTransform>().sizeDelta.y;
        InitLotus();    
    }
    public override void PlayGame()
    {              
        RiverIndex++;
        if (IsGameEnd)
            return;         
        switch (StartData.dataType)
        {
            case DataType.Word:              
                SetWord();
                break;
            case DataType.Ask:
                //SetAsk();
                break;
            case DataType.Pronunciation:
                //SetPronunciation();
                break;
            case DataType.Say:
                //SetSay();
                break;
        }
    }
   
    private void SetWord()
    {
        StartCoroutine(Hint());
        List<ConfigWordLibraryModel> list = new List<ConfigWordLibraryModel>();
        Words = new List<string>();
        foreach (var item in m_originalWordList)
        {
            list.Add(item);
        }
        for (int i = 0; i < list.Count; i++)
        {
            Words.Add(list[i].word);
        }
        Words.Remove(CurrentWord.word);
        Utility.RandomSortList(Words);
        int RandomAnswer = Random.Range(0, 2);
        Words.Insert(RandomAnswer, CurrentWord.word);

        Transform parent = transform.Find("UIRunningFrogRiver" + RiverIndex);

       
        for (int i = 3; i < 6;i++)
        {
            if (parent.GetChild(i).childCount != 0)
            {
                if (parent.GetChild(i).GetChild(0).GetComponent<UIRunningFrogLotusRight>() != null)
                {
                    targetTransOne = parent.GetChild(i).GetChild(0).gameObject;
                    circulationIndex = i;
                    break;
                }
            }
          
        }      

        for (int i = circulationIndex+1; i < 6; i++)
        {
            if (parent.GetChild(i).childCount != 0)
            {
                if (parent.GetChild(i).GetChild(0).GetComponent<UIRunningFrogLotusRight>() != null)
                {
                    targetTransTwo = parent.GetChild(i).GetChild(0).gameObject;
                    break;
                }
            }
            
        }
         
        Current_Answer = CurrentWord.word;
        targetTransOne.GetComponent<UIRunningFrogLotusRight>().InitText(Words[0]);
        targetTransTwo.GetComponent<UIRunningFrogLotusRight>().InitText(Words[1]);
        ScaleText(targetTransOne, targetTransTwo);
    }
    IEnumerator Hint()
    {
        yield return new WaitForSeconds(0.3f);
        originFrog.GetComponent<UIRunningFrogLotusRight>().StartWord();
    }

    public void MoveCamera(Transform target)
    {        
        if (Mathf.Abs(UIManager.Instance.WorldCamera.transform.position.y-target.position.y)>=0 &&Mathf.Abs(UIManager.Instance.WorldCamera.transform.position.y-target.position.y)<1)
        {
            UIRunningFrog.Instance.FirstClick = false;
            clickFrog = target.gameObject;
            switch (StartData.dataType)
            {
                case DataType.Word:
                    //Right Lotus Leaf
                    if (clickFrog.GetComponent<UIRunningFrogLotusRight>()!=null)
                    {                       
                         if (clickFrog.GetComponent<UIRunningFrogLotusRight>().LotusText.text == Current_Answer)
                         {
                             StartCoroutine(FrogJumpTween(originFrog.GetComponent<UIRunningFrogLotusRight>().RightFrog, clickFrog.transform,true));  
                              //正确飞星星开始下一题
                             UITopBarStarFly fly = FlyStar(true, true);
                             fly.OnComplete += () =>
                             {  
                                 //做一个遍历隐藏所有文字    
                                 for (int i = 3; i < 6; i++)
                                 {
                                     if (clickFrog.transform.parent.parent.GetChild(i).childCount > 0)
                                     {
                                         if (clickFrog.transform.parent.parent.GetChild(i).GetChild(0).GetComponent<UIRunningFrogLotusRight>() != null)
                                         {
                                             clickFrog.transform.parent.parent.GetChild(i).GetChild(0).GetComponent<UIRunningFrogLotusRight>().HideText();
                                         }
                                     }
                                 }
                                 PlayGame();
                             };
                         }
                         else if (clickFrog.GetComponent<UIRunningFrogLotusRight>().LotusText.text == "")
                         {
                             StartCoroutine(FrogJumpTween(originFrog.GetComponent<UIRunningFrogLotusRight>().RightFrog, clickFrog.transform, true));
                         }
                         else
                         {
                             StartCoroutine(FrogJumpTween(originFrog.GetComponent<UIRunningFrogLotusRight>().RightFrog, clickFrog.transform, false));
                         }
                    }
                    //Wrong Lotus Leaf
                    else if (clickFrog.GetComponent<UIRunningFrogLotusWrong>() != null)
                    {
                        StartCoroutine(FrogJumpTween(originFrog.GetComponent<UIRunningFrogLotusRight>().RightFrog, clickFrog.transform, false));                   
                    }
                    else
                    {
                        StartCoroutine(FrogJumpTween(originFrog.GetComponent<UIRunningFrogLotusRight>().RightFrog, clickFrog.transform, false));                   
                    }                    
                    break;
                case DataType.Ask:
                    SetRightMoveCamera();
                    break;
                case DataType.Pronunciation:
                    SetRightMoveCamera();
                    break;
                case DataType.Say:
                    SetRightMoveCamera();
                    break;
            }
            
        }           
        else
        {
            return;
        }
    }
    private IEnumerator FrogJumpTween(Transform self, Transform target,bool On_OFF)
    {
        int dValue = (int)(self.position.x - target.position.x);
        RectTransform selfRect = (RectTransform)self;
        Vector3 originVec = self.position;
        switch (dValue)
        {
            case 0:
                selfRect.eulerAngles = Vector3.zero;
                break;
            case 3:
                selfRect.eulerAngles = new Vector3(0, 0, 48);
                break;
            case 7:
                selfRect.eulerAngles = new Vector3(0, 0, 65.5f);
                break;
            case -3:
                selfRect.eulerAngles = new Vector3(0, 0, 312);
                break;
            case -7:
                selfRect.eulerAngles = new Vector3(0, 0, 294.5f);
                break;
        }
        yield return new WaitForSeconds(0.2f);
        float destence = Vector3.Distance(self.position, target.position);

        self.DOMove(target.position, destence/8f).SetEase(Ease.InBack);

        yield return new WaitForSeconds((destence / 8f)+0.1f);
        
        if (On_OFF)
        {
            SetRightMoveCamera();
            OnFrogShow(target.gameObject);
            //self.position = originVec;
            selfRect.anchoredPosition = Vector2.zero;
            selfRect.eulerAngles = Vector3.zero;
            yield return new WaitForSeconds(0.5f);
            FirstClick = true;
        }
        else
        {
            OffFrogShow(target.gameObject);
            yield return new WaitForSeconds(0.5f);
            //self.position = originVec;
            selfRect.anchoredPosition = Vector2.zero;
            selfRect.eulerAngles = Vector3.zero;
            FirstClick = true;
        }
    }
  
    private void OnFrogShow(GameObject t)
    {
        
        t.GetComponent<UIRunningFrogLotusRight>().RightFrog.gameObject.SetActive(true);      
        t.GetComponent<UIRunningFrogLotusRight>().HideText();
       
        //originFrog.SetActive(false);
        originFrog.GetComponent<UIRunningFrogLotusRight>().HideFrog();
        originFrog = t;
      
    }
    private void OffFrogShow(GameObject t)
    {
        StartCoroutine(OffFrog(t));
    }
    IEnumerator OffFrog(GameObject t)
    {
     
        if (t.GetComponent<UIRunningFrogLotusWrong>() != null)
        {
           //TODO荷叶碎裂出水花
            t.GetComponent<UIRunningFrogLotusWrong>().GetComponent<Image>().DOFade(0, 2f).From();
        }
        originFrog.GetComponent<UIRunningFrogLotusRight>().RightFrog.GetComponent<Image>().DOFade(0, 0.3f).From().SetLoops(3);
        yield return new WaitForSeconds(0.9f);
        //飞错星星
        UITopBarStarFly fly = FlyStar(false, false);
        originFrog.GetComponent<UIRunningFrogLotusRight>().RightFrog.GetComponent<Image>().DOFade(1, 0);
        //SetWrongMoveCamera();
      
    }
    //照相机移动以及加载路面
    private void SetRightMoveCamera()
    {        
        ClickTota++;
        currentPosition = this.transform.position;
        this.transform.DOMoveY((currentPosition.y - 3.333f), 0.5f);
       
        if (ClickTota % 3 == 0)
        {
            tempGame = CreateUIItem("UIRunningFrogRiver", this.transform);
            tempGame.AddComponent<UIRunningFrogRiver>().name = "UIRunningFrogRiver" + RiverTotal;
            tempRecr = (RectTransform)tempGame.transform;
            tempRecr.anchoredPosition = new Vector2(0, RiverHight * ((ClickTota / 3) + 1));
            RiverTotal++;
            if (RiverTotal > 6)
            {
                Destroy(this.transform.GetChild(0).gameObject);
            }
        }       
    }
   
    private void InitLotus()
    {
        //小青蛙
        tempTrans = UIRunningFrogRiver0.GetChild(7);
        originFrog = CreateUIItem("UIRunningFrogLotusRightStar", tempTrans);
        originFrog.AddComponent<UIRunningFrogLotusRight>().ShowFrog();
       
        

        //中间那一排
        randomIndex = Random.Range(3, 6);
        tempTrans = UIRunningFrogRiver0.GetChild(randomIndex);
         CreateUIItem("UIRunningFrogLotusRight", tempTrans).AddComponent<UIRunningFrogLotusRight>();;
        
       
        randomIndexT = Random.Range(3, 6);
        while(randomIndexT == randomIndex)
        {
            randomIndexT = Random.Range(3, 6);
        }
        tempTrans = UIRunningFrogRiver0.GetChild(randomIndexT);
        CreateUIItem("UIRunningFrogLotusRight", tempTrans).AddComponent<UIRunningFrogLotusRight>();
       
    
        //最上边那一排
        randomIndex = Random.Range(0, 3);
        tempTrans = UIRunningFrogRiver0.GetChild(randomIndex);
        CreateUIItem("UIRunningFrogLotusWrong", tempTrans).AddComponent<UIRunningFrogLotusWrong>();
        randomIndexT = Random.Range(0, 3);
        while (randomIndexT == randomIndex)
        {
            randomIndexT = Random.Range(0, 3);
        }
        tempTrans = UIRunningFrogRiver0.GetChild(randomIndexT);
        CreateUIItem("UIRunningFrogLotusRight", tempTrans).AddComponent<UIRunningFrogLotusRight>();

        //加载地图
        tempGame = CreateUIItem("UIRunningFrogRiver", this.transform);
        tempGame.AddComponent<UIRunningFrogRiver>().name = "UIRunningFrogRiver1";
        tempRecr = (RectTransform)tempGame.transform;
      
        tempRecr.anchoredPosition = new Vector2(0, RiverHight);
        RiverTotal = 2;

        AnswerImage.enabled = false;
        AnswerText.enabled = false;
      
    }
    //气泡内文字闪烁
    private void ScaleText(GameObject a,GameObject b)
    {
        a.GetComponent<UIRunningFrogLotusRight>().LotusText.transform.DOScale(1.2f, 2f).From().SetLoops(-1);
        b.GetComponent<UIRunningFrogLotusRight>().LotusText.transform.DOScale(1.2f, 2f).From().SetLoops(-1);
    }
}
