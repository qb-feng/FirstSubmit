using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Spine.Unity;

public class UISuperChameleon : UIBaseLevelGame
{
    public static UISuperChameleon Instance;
    void Awake()
    {
        Instance = this;
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    private GameObject Audio { get { return Get("Audio"); } }
    private RectTransform Chameleon { get { return Get<RectTransform>("Chameleon"); } }
    private RectTransform DownItem1 { get { return GetR("DownItem1"); } }
    private RectTransform DownItem2 { get { return GetR("DownItem2"); } }
    private RectTransform DownItem3 { get { return GetR("DownItem3"); } }
    private RectTransform DownItem4 { get { return GetR("DownItem4"); } }
    private RectTransform DownItem5 { get { return GetR("DownItem5"); } }
    private RectTransform DownItem6 { get { return GetR("DownItem6"); } }

    private RectTransform Tongue { get { return GetR("Tongue"); } }

    private RectTransform UpImageWord { get { return GetR("UpImageWord"); } }
   

    private List<UISuperChameleonSentenceNiko> UpSentenceList;

    private List<UISuperChameleonWordNiko> DownWordList;

    //private  Vector2 tempVec = new Vector2(0, 0);

    //难度设置：1_随机生成2个单词，2_随机生成4个单词，3_随机生成6个单词
    private int difficultIndex;
    //存储正确的单词
    private string[] wordsRight;
    //存储干扰项
    private string[] wordsWrong;

    //存储句子
    //private string sentence;
    //存储当前选择单词
    private string currentSelectWord;

    //需要填写的空单词
    List<string> indexRightWord;
    //需要填写的干扰空单词
    List<string> indexWrongWord;

    //正确单词位置标记
    List<int> index;

    //存储正确的单词
    private List<string> wordRightlist;
    //存储干扰项
    private List<string> wordWronglist;  

    //private bool isChameleonEnd = false;

    private bool audioIsPlay;

    public override void Refresh()
    {
        base.Refresh();
        UGUIEventListener.Get(Audio).onPointerClick = OnClickAudio;
        UGUIEventListener.Get(Chameleon).onPointerClick = OnClickChameleon;
        UpSentenceList = new List<UISuperChameleonSentenceNiko>();
        DownWordList = new List<UISuperChameleonWordNiko>();
        wordRightlist = new List<string>();
        wordWronglist = new List<string>();
        indexRightWord = new List<string>();
        indexWrongWord = new List<string>();
        index = new List<int>();
    }
    private void OnClickChameleon(PointerEventData data)
    {
        Chameleon.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(1);
        Chameleon.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(1, "touch", false);
        
    }
       
    private void OnClickAudio(PointerEventData data)
    {
        if (audioIsPlay)
            return;
        StartCoroutine(PlayAskAudio());
    }

    IEnumerator PlayAskAudio()
    {
        audioIsPlay = true;
        float length = 1;
       
        length = AudioManager.Instance.Play(CurrentAsk.id).clip.length;
      
        yield return new WaitForSeconds(length);

        if (CurrentAsk.yesSound != null)
        {
            
            length = AudioManager.Instance.Play(CurrentAsk.yesSound).clip.length;
         
        }
        else if (CurrentAsk.noSound != null)
        {
           
           length = AudioManager.Instance.Play(CurrentAsk.noSound).clip.length;
          
        }
        yield return new WaitForSeconds(length);
        audioIsPlay = false;

        /*
        if (CurrentAsk.yesSound != null)
        {
            AudioManager.Instance.Play(CurrentAsk.noSound);
        }
         */
        
    }
    //瓢虫位置随机函数
    private RectTransform RandomladybirdLocation(){    
        RectTransform[]  RandomLocalcation = new RectTransform[6]{DownItem1,DownItem2,DownItem3,DownItem4,DownItem5,DownItem6};       

        int randomIndex = Random.Range(0,6);
        return RandomLocalcation[randomIndex];
    }

    public override void PlayGame()
    {
        Chameleon.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "idel", true);
        if (IsGameEnd)
        {       
            return;
        }
       

        DownItem1.ClearAllChild();
        DownItem2.ClearAllChild();
        DownItem3.ClearAllChild();
        DownItem4.ClearAllChild();
        DownItem5.ClearAllChild();
        DownItem6.ClearAllChild();

        UpImageWord.ClearAllChild();

        UpSentenceList.Clear();
        DownWordList.Clear();
        wordRightlist.Clear();
        wordWronglist.Clear();

        string sAsk = CurrentAsk.ask;
        string sYesOrNo = CurrentAsk.yes + '_' + CurrentAsk.no;
        wordsRight = sAsk.Split('_');
        wordsWrong = sYesOrNo.Split('_');
        //sentence = sAsk.Replace('_', ' ');
       
        StartCoroutine(PlayAskAudio());

        //游戏难度
        difficultIndex = 3;

        for (int i = 0; i < wordsRight.Length; i++)
        {
            if (!string.IsNullOrEmpty(wordsRight[i]))
                wordRightlist.Add(wordsRight[i]);
        }
        for (int i = 0; i < wordsWrong.Length; i++)
        {
            if (!string.IsNullOrEmpty(wordsWrong[i]))
                wordWronglist.Add(wordsWrong[i]);
        }       

        switch (difficultIndex)
        {
            case 1:                               
                index.Add( Random.Range(0, wordRightlist.Count-1));
                indexRightWord.Add(wordRightlist[index[0]]);
                indexWrongWord.Add(wordWronglist[index[0]]);
                wordRightlist[index[0]] = "____";
                break;
            case 2:                             
                index.Add(Random.Range(0, wordRightlist.Count - 1));
                index.Add(Random.Range(0, wordRightlist.Count - 1));
                while (index[0] == index[1]) {
                    index[1] = Random.Range(0, wordRightlist.Count - 1);                    
                }
                //index按大小排序
                if (index[0] > index[1])
                {
                    int tempIndex = index[0];
                    index[0] = index[1];
                    index[1] = tempIndex;
                }
                indexRightWord.Add(wordRightlist[index[0]]);
                indexWrongWord.Add(wordWronglist[index[0]]);
                indexRightWord.Add(wordRightlist[index[1]]);
                indexWrongWord.Add(wordWronglist[index[1]]);
                wordRightlist[index[0]] = "____";
                wordRightlist[index[1]] = "____";
                break;
            case 3:
                int[] tempRandom = new int[wordRightlist.Count];
                
                for (int a = 0; a < wordRightlist.Count; a++)
                {
                    tempRandom[a] = a;
                }
                
                for (int b = 0; b < difficultIndex; b++)
                {
                    int temp = Random.Range(0, tempRandom.Length-1);
                    while (tempRandom[temp] == -1)
                    {
                        temp = Random.Range(0, tempRandom.Length - 1);
                    }
                    index.Add(tempRandom[temp]);
                    tempRandom[temp] = -1;
                }             
                //index大小排序
                for (int i = 0; i < index.Count - 1; i++)                {
                    for (int j = 0; j < index.Count - 1 - i; j++)
                    {
                        if (index[j] > index[j + 1])
                        {
                            int temp = index[j + 1];
                            index[j + 1] = index[j];
                            index[j] = temp;
                        }
                    }
                }
                indexRightWord.Add(wordRightlist[index[0]]);
                indexWrongWord.Add(wordWronglist[index[0]]);
                indexRightWord.Add(wordRightlist[index[1]]);
                indexWrongWord.Add(wordWronglist[index[1]]);
                indexRightWord.Add(wordRightlist[index[2]]);
                indexWrongWord.Add(wordWronglist[index[2]]);
                wordRightlist[index[0]] = "____";
                wordRightlist[index[1]] = "____";
                wordRightlist[index[2]] = "____";
                break;
        }

        for (int i = 0; i < wordRightlist.Count; i++)
        {            
            var up = CreateUIItem<UISuperChameleonSentenceNiko>(UpImageWord);
            UpSentenceList.Add(up);
            up.Init(wordRightlist[i]);
        }

        for (int i = 0; i < difficultIndex * 2; i++)
        {
            RectTransform temp = RandomladybirdLocation();
            if (temp.childCount >= 1)
            {
                i = i - 1;
            }
            else
            {
                var down = CreateUIItem<UISuperChameleonWordNiko>(temp);

                DownWordList.Add(down);

                switch (difficultIndex)
                {
                    case 1: 
                        down.Init((i + 1) % 2 == 0 ? indexRightWord[i - 1] : indexWrongWord[i], i / 2,temp,Random.Range(2,8)/10f);
                        break;
                    case 2:
                        down.Init((i % 2 == 0) ? indexRightWord[(i + 1) / 2] : indexWrongWord[(i + 1) / 4], i / 2, temp, Random.Range(2, 8) / 10f);
                        break;
                    case 3:
                        down.Init((i % 2 == 0) ? indexRightWord[(i + 1) / 2] : indexWrongWord[(i + 1) / 3], i / 2, temp, Random.Range(2, 8) / 10f);
                        break;
                }
              
            }          
        }        

    }


    /// <summary>
    ///  下面的item点击了  将文字设置到上面的item上面去
    /// </summary>
    /// <param name="text"></param>
    public void SetUpItemText(string text,UISuperChameleonWordNiko word)
    {
        currentSelectWord = text;
        if (!word.isClick)
        {
            if (word.answerIndex == 0 && currentSelectWord == indexRightWord[0])
            {
                StartCoroutine(InsertWord(word));
            }
            else
            {
                Chameleon.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(1);
                //Debug.LogError("点击错误");
                Chameleon.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(1, "false", false);
                
                word.IsFalseAnswer();
                

            }
        }     
    }
    IEnumerator InsertWord(UISuperChameleonWordNiko w)
    {
        Chameleon.GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(1);
        Chameleon.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(1, "ture", false);        
        w.IsTrueAnswer();
        yield return new WaitForSeconds(1.5f);
        UpSentenceList[index[0]].SetRightText(currentSelectWord);
        var tempChameleon = w;
        tempChameleon.isRight = true;
        w.answerIndex = -1;
        indexRightWord.Remove(currentSelectWord);
        //把原"____"处替换单词
        wordRightlist[index[0]] = currentSelectWord;
        index.Remove(index[0]);
        for (int i = 0; i < difficultIndex * 2; i++)
        {
            if (DownWordList[i].answerIndex != -1)
            {
                DownWordList[i].answerIndex -= 1;
            }
        }
        tempChameleon.IsActive();
        //游戏结束条件indexRightWord.Count为0    
        if (indexRightWord.Count == 0)
        {            
            UITopBarStarFly fly = UISuperChameleon.Instance.FlyStar(UISuperChameleon.Instance.CurrentAsk.id, true);            
            fly.OnComplete += () =>
            {
                PlayGame();
            };
            
        }
    }

    //判断游戏结束
    private bool IsChameleonEnd()
    {
        //游戏结束条件indexRightWord.Count为0    
        //  UITrainWordCommon.Instance.FlyStar(UITrainWordCommon.Instance.CurrentAsk.id, true)
        return true;
    }

    //设置舌头长度   
    public void SetTongue(string name)
    {
        StartCoroutine(WaitOpen(name));      
    }
    IEnumerator WaitEat()
    {
        yield return new WaitForSeconds(0.3f);
        DOTween.Init();        
        Tongue.DOSizeDelta(new Vector2(0, 20), 0.25f);       
        //float t = Vector2.Distance(Tongue.anchoredPosition, tempVec);
        //Debug.LogError("距离" + t);
        //var temp = DOTween.To(() => { return Tongue.anchoredPosition; }, v => { Tongue.anchoredPosition = v; }, tempVec, t/2500f).From();
        //temp.OnStart(MoveStartReturn);
        //temp.OnComplete(MoveEndReturn);
    }
    IEnumerator WaitOpen(string name)
    {
        yield return new WaitForSeconds(0.3f);
        switch (name)
        {
            case "DownItem1":
                //Tongue.anchoredPosition = new Vector2(-444, -204);
                //Tongue.localScale = new Vector3(11, 1, 1);
                Tongue.DOSizeDelta(new Vector2(370, 20), 0.1f);
                //Tongue.sizeDelta = new Vector2(342, 20);
                Tongue.localEulerAngles = new Vector3(0, 0, 54);

                //tempVec = new Vector2(-267, 40);
                break;
            case "DownItem2":
                //Tongue.anchoredPosition = new Vector2(-625, -246);
                //Tongue.localScale = new Vector3(22, 1, 1);
                Tongue.DOSizeDelta(new Vector2(650, 20), 0.175f);
                //Tongue.sizeDelta = new Vector2(700, 20);
                Tongue.localEulerAngles = new Vector3(0, 0, 27);
                //tempVec = new Vector2(-85, 32);
                break;
            case "DownItem3":
                //Tongue.anchoredPosition = new Vector2(-793, -249);
                //Tongue.localScale = new Vector3(34, 1, 1);
                Tongue.DOSizeDelta(new Vector2(980, 20), 0.26f);
                //Tongue.sizeDelta = new Vector2(900, 20);
                Tongue.localEulerAngles = new Vector3(0, 0, 18);
                //tempVec = new Vector2(90, 41);
                break;
            case "DownItem4":
                //Tongue.anchoredPosition = new Vector2(-531, -148);
                //Tongue.localScale = new Vector3(15, 1, 1);
                Tongue.DOSizeDelta(new Vector2(400, 20), 0.1f);
                //Tongue.sizeDelta = new Vector2(400, 20);
                Tongue.localEulerAngles = new Vector3(0, 0, 11);
                //tempVec = new Vector2(-166, -77);
                break;
            case "DownItem5":
                //Tongue.anchoredPosition = new Vector2(-721, -135);
                //Tongue.localScale = new Vector3(28, 1, 1);
                Tongue.DOSizeDelta(new Vector2(750, 20), 0.2f);
                //Tongue.sizeDelta = new Vector2(800, 20);
                Tongue.localEulerAngles = new Vector3(0, 0, 6f);
                //tempVec = new Vector2(-31, -79);
                break;
            case "DownItem6":
                //Tongue.anchoredPosition = new Vector2(-599, -23);
                //Tongue.localScale = new Vector3(20, 1, 1);
                Tongue.DOSizeDelta(new Vector2(590, 20), 0.15f);
                //Tongue.sizeDelta = new Vector2(600, 20);
                Tongue.localEulerAngles = new Vector3(0, 0, -14);
                //tempVec = new Vector2(-94, -170);
                break;
        }
        StartCoroutine(WaitEat());
    }
    private void MoveEndReturn()
    {
        Tongue.GetComponent<Image>().enabled = false;       
    }
    private void MoveStartReturn()
    {
        Tongue.GetComponent<Image>().enabled = true;
    }
  
   
}
