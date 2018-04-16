using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Spine.Unity;

public class UIYellowDuck : UIBaseLevelGame {

    private Transform Audio { get { return GetT("Audio"); } }
    private Transform Vortex { get { return GetT("Vortex"); } }
    private Transform DuckBirth { get { return GetT("DuckBirth"); } }

    private Transform Terminus { get { return GetT("Terminus"); } }

    public Vector3 DuckTerminus;

    //A*寻路
    public Transform Astar { get { return GetT("Astar"); } }

    //句子生成
    private Transform SentenceRegion { get { return GetT("SentenceRegion"); } }
    public Transform PanelUp { get { return GetT("PanelUp"); } }
    public Transform PanelDown { get { return GetT("PanelDown"); } }
    public string[] sAskArray;
    public List<string> Sentence;
    public List<string> SentenceDuck;
    private List<int> mNumber;
    public bool isEnd;
    public Tween terminusTween;
    //单词位置
    private Transform WordLocation { get { return GetT("WordLocation"); } }
  
        
    public Transform Duck;

    private Transform MouseEndDown;

    //播音
    private bool audio_play = true;

    public static UIYellowDuck Instance { get; set; }

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
        terminusTween=Vortex.DORotate(new Vector3(0, 0, 180), 5f, RotateMode.Fast).SetLoops(-1);
        DuckBirth.ClearAllChild();
        Duck = CreateUIItem("UIYellowDuckDuck", DuckBirth).transform;
        Duck.gameObject.AddComponent<UIYellowDuckDuck>();
        Astar.gameObject.AddComponent<UIYellowDuckGridNiko>().player = Duck ;
        Astar.gameObject.AddComponent<UIYellowDuckFindPathNiko>().player = Duck ;
        DuckTerminus = Terminus.position;
        isEnd = false;
        UGUIEventListener.Get(Audio).onPointerClick = OnAudioPlay;   
    }
    private void OnAudioPlay(PointerEventData data)
    {
        if (audio_play)
        {
            audio_play = false;           
           StartCoroutine(PlayAskAudio());          
        }
    }

    public override void PlayGame()
    {
        if (IsGameEnd)
            return;
        string sAsk = CurrentAsk.ask;
        sAskArray = sAsk.Split('_');
        if (sAskArray.Length > 10)
            PlayGame();
        isEnd = false;
        Duck.position = DuckBirth.position;
        Duck.localScale = Vector3.zero;
        Duck.DOScale(1, 2f).SetEase(Ease.OutElastic);
        PanelUp.ClearAllChild();
        PanelDown.ClearAllChild();
        for (int i = 0; i < WordLocation.childCount; i++)
        {
            WordLocation.GetChild(i).ClearAllChild();
        }
        Sentence = new List<string>();
        mNumber = new List<int>();
        SentenceDuck = new List<string>();

        
        for (int i = 0; i < sAskArray.Length; i++)
        {
            Sentence.Add(sAskArray[i]);
            SentenceDuck.Add(sAskArray[i]); 
        }
        
        for (int i = 0; i < 8; i++)
        {
            mNumber.Add(i);
        }

            if (sAskArray.Length > 0 && sAskArray.Length <= 5)
            {
                for (int i = 0; i < sAskArray.Length; i++)
                {
                    GameObject temp = CreateUIItem("UIYellowDuckSentenceText", PanelUp);
                    temp.GetComponent<TextMeshProUGUI>().text = sAskArray[i];
                    temp.GetComponent<TextMeshProUGUI>().alpha = 0;
                    temp.GetComponent<TextMeshProUGUI>().DOFade(1f, 2f).From().SetLoops(2);
                    temp.transform.DOScale(1.3f, 2f).From().SetLoops(2);
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject temp = CreateUIItem("UIYellowDuckSentenceText", PanelUp);
                    temp.GetComponent<TextMeshProUGUI>().text = sAskArray[i];
                    temp.GetComponent<TextMeshProUGUI>().alpha = 0;
                    temp.GetComponent<TextMeshProUGUI>().DOFade(1f, 2f).From().SetLoops(2);
                    temp.transform.DOScale(1.3f, 2f).From().SetLoops(2);
                }
                for (int j = 5; j < sAskArray.Length; j++)
                {
                    GameObject temp = CreateUIItem("UIYellowDuckSentenceText", PanelDown);
                    temp.GetComponent<TextMeshProUGUI>().text = sAskArray[j];
                    temp.GetComponent<TextMeshProUGUI>().alpha = 0;
                    temp.GetComponent<TextMeshProUGUI>().DOFade(1f, 2f).From().SetLoops(2);
                    temp.transform.DOScale(1.3f, 2f).From().SetLoops(2);
                }
            }

           
        for (int i = 0; i < sAskArray.Length; i++)
        {
            
            int tempIndex = Random.Range(0,mNumber.Count);
            GameObject tGO = CreateUIItem("UIYellowDuckWordText", WordLocation.GetChild(mNumber[tempIndex]));
            mNumber.RemoveAt(tempIndex);

            tempIndex = Random.Range(0, Sentence.Count);
            tGO.GetComponent<TextMeshProUGUI>().text = Sentence[tempIndex];           
            Sentence.RemoveAt(tempIndex);
        }

        StartCoroutine(PlayAskAudio());      



    }

    public void PlayAskAudioDuck()
    {
        StartCoroutine(PlayAskAudio());      
    }
    public void RotatrText()
    {
        SentenceRegion.DOScale(1.5f, 2f).From();
    }
    IEnumerator PlayAskAudio()
    {
        var length = AudioManager.Instance.Play(CurrentAsk.id).clip.length;
        yield return new WaitForSeconds(length);
        //length = AudioManager.Instance.Play(CurrentAsk.answerSound).clip.length;
        //yield return new WaitForSeconds(length);
        audio_play = true;
    }

    void Update()
    {
        if (!isEnd)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //防止点障碍物重新寻路->后加的
                Astar.gameObject.GetComponent<UIYellowDuckGridNiko>().path.Clear();
                
                Vector3 target = UIManager.Instance.WorldCamera.ScreenToWorldPoint(Input.mousePosition);
                //Debug.LogError(target.x - Duck.position.x);
                //鸭子转换方向
                if (target.x - Duck.position.x > 0)
                {
                    Duck.eulerAngles = new Vector3(0, 0, 0);
                }
                else
                {
                    Duck.eulerAngles = new Vector3(0, 180, 0);
                }
                Astar.gameObject.GetComponent<UIYellowDuckFindPathNiko>().FindingPath(Duck.position, target);
                if (Astar.gameObject.GetComponent<UIYellowDuckGridNiko>().path.Count != 0)
                {
                    Vector3[] vecPath = new Vector3[Astar.gameObject.GetComponent<UIYellowDuckGridNiko>().path.Count];
                    for (int i = 0; i < Astar.gameObject.GetComponent<UIYellowDuckGridNiko>().path.Count; i++)
                    {
                        vecPath[i] = Astar.gameObject.GetComponent<UIYellowDuckGridNiko>().path[i]._worldPos;
                    }
                    Duck.GetComponent<UIYellowDuckDuck>().moveDuck(vecPath);
                }
            }
        }
    }

    public void InitDuck()
    {
        //.SetEase(Ease.OutElastic)
            Duck.GetComponent<Image>().DOFade(0.2f, 0.2f).SetLoops(4).OnComplete(()=>{
            Duck.position = DuckBirth.position;
            Duck.GetComponent<Image>().DOFade(1, 0.7f);
            Duck.localScale = Vector3.zero;
            Duck.DORotate(new Vector3(0, 0, 180), 1.2f);
            Duck.DOScale(1, 1.2f).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                Duck.eulerAngles = new Vector3(0, 0, 0);
                isEnd = false;
            });           

        });  
    }

}
