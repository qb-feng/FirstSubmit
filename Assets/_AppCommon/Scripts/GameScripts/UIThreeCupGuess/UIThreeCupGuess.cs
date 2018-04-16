using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


#region 换皮模型定义
public class UIThreeBoxGuess : UIThreeCupGuess { }
#endregion

public class UIThreeCupGuess : UIBaseLevelGame
{
    private string m_cupGuessPrefix = "ImageCupGuess";
    public string m_cupPrefix = "ImageCup";
    private string m_guessPrefix = "ImageGuess";
    private string m_bezierUpPrefix = "BezierUp";
    private string m_bezierDownPrefix = "BezierDown";
    public RectTransform Question { get { return GetR("Question"); } }
    public GameObject Answer1 { get { return Get("Answer1"); } }
    public GameObject Answer2 { get { return Get("Answer2"); } }
    public Image ImageAnswer1 { get { return Get<Image>("ImageAnswer1"); } }
    public Image ImageAnswer2 { get { return Get<Image>("ImageAnswer2"); } }
    public TextMeshProUGUI TextAnswer1 { get { return Get<TextMeshProUGUI>("TextAnswer1"); } }
    public TextMeshProUGUI TextAnswer2 { get { return Get<TextMeshProUGUI>("TextAnswer2"); } }
    private RectTransform GuideHand1 { get { return GetR("ImageGuideHand1"); } }
    private RectTransform GuideHand2 { get { return GetR("ImageGuideHand2"); } }
    private Image Bg { get { return Get<Image>("ImageBg"); } }
    public TextMeshProUGUI AskText { get { return Get<TextMeshProUGUI>("AskText"); } }

    private GameObject m_currentGuideHande;
    private bool m_onceGuideHand = false;

    int currentIndex = 0;


    public class CupGuessModel
    {
        public RectTransform cupGuess;
        public RectTransform cup;
        public Image guess;
        //public string guessWord;
        public string guessAudio;
        public string guessAskAudio;
        //public string guessAsk;
        public ConfigAskLibraryModel askModel;
        public ConfigWordLibraryModel wordModel;
    }

    private int m_randomMoveNum = 3;
    private int m_randomCurrentNum = 0;
    private int m_cacheFirstRandom = 0;
    private int m_cacheSecondRandom = 0;

    public List<CupGuessModel> m_cacheCupGuessList = new List<CupGuessModel>();
    private CupGuessModel m_specificCupGuess;
    public ConfigAskLibraryModel CupGuessAsk { get { return m_specificCupGuess.askModel; } }
    public ConfigWordLibraryModel CupGuessWord { get { return m_specificCupGuess.wordModel; } }
    public List<ConfigAskLibraryModel> m_cacheGuessAsks = new List<ConfigAskLibraryModel>();
    public List<ConfigWordLibraryModel> m_cacheGuessWords = new List<ConfigWordLibraryModel>();

    public string m_answer1Audio;
    public string m_answer2Audio;
    public static UIThreeCupGuess Instance { get; set; }

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
        UGUIEventListener.Get(Answer1).onPointerDown = Answer1PointDown;
        UGUIEventListener.Get(Answer2).onPointerDown = Answer2PointDown;
        UGUIEventListener.Get(Question).onPointerDown = QuestionPointDown;
        for (int i = 1; i < 4; i++)
        {
            var cup = Get<Image>(m_cupPrefix + i);
            cup.SetNativeSize();
        }
    }

    public void QuestionPointDown(UnityEngine.EventSystems.PointerEventData arg0)
    {
        //AudioManager.Instance.Play(GetCurrentQuessionAudio());
        //        AudioManager.Instance.Play(m_specificCupGuess.guessAskAudio);

        if (StartData.dataType == DataType.Ask)
        {
            AudioManager.Instance.Play(m_specificCupGuess.guessAskAudio);
        }
    }

    public void Answer2PointDown(UnityEngine.EventSystems.PointerEventData arg0)
    {
        AudioManager.Instance.Play(m_answer2Audio);
    }

    public void Answer1PointDown(UnityEngine.EventSystems.PointerEventData arg0)
    {
        AudioManager.Instance.Play(m_answer1Audio);
    }

    public override void PlayGame()
    {
        if (IsGameEnd)
            return;


        HideQuestionAndAnswer();
        RefreshCupGuess();
        RefreshGuessContent();
        DoCupAndGuessTween();
    }

    public void RefreshCupGuess()
    {
        m_cacheCupGuessList.Clear();
        for (int i = 1; i < 4; i++)
        {
            CupGuessModel cupGuess = new CupGuessModel();
            cupGuess.cupGuess = GetR(m_cupGuessPrefix + i);
            cupGuess.cup = GetR(m_cupPrefix + i);
            cupGuess.guess = Get<Image>(m_guessPrefix + i);
            m_cacheCupGuessList.Add(cupGuess);
        }
    }

    public virtual void RefreshGuessContent()
    {
        m_cacheGuessAsks.Clear();
        m_cacheGuessWords.Clear();
        int otherIndex = 0;
        int randomOther = 0;
        switch (m_currentIndex)
        {
            case 0:
                currentIndex = 0;
                otherIndex = 1;
                randomOther = UnityEngine.Random.Range(0, 2);
                break;
            case 1:
                currentIndex = 1;
                otherIndex = 2;
                randomOther = UnityEngine.Random.Range(1, 3);
                break;
            case 2:
                currentIndex = 2;
                otherIndex = 3;
                randomOther = UnityEngine.Random.Range(2, 4);
                break;
            case 3:
                currentIndex = 3;
                otherIndex = 4;
                randomOther = UnityEngine.Random.Range(3, 5);
                break;
            case 4:
                currentIndex = 4;
                otherIndex = 5;
                randomOther = UnityEngine.Random.Range(4, 6);
                break;
            case 5:
                currentIndex = 5;
                otherIndex = 4;
                randomOther = UnityEngine.Random.Range(4, 6);
                break;
            default:
                break;
        }
        switch (StartData.dataType)
        {
            case DataType.Word:
                AskText.gameObject.SetActive(false);
                if (m_originalWordList.Count == 2 && m_currentIndex == 1)
                {
                    otherIndex = 0;
                    randomOther = UnityEngine.Random.Range(0, 2);
                }

                //qiubin修改 - 增加index的越界判断
                if (otherIndex >= m_originalWordList.Count)
                {
                    otherIndex = 0;
                }
                if (randomOther >= m_originalWordList.Count) 
                {
                    randomOther = 0;
                }


                m_cacheGuessWords.Add(m_originalWordList[currentIndex]);
                m_cacheGuessWords.Add(m_originalWordList[otherIndex]);
                m_cacheGuessWords.Add(m_originalWordList[randomOther]);

                ImageAnswer1.sprite = m_originalWordList[currentIndex].sprite;
                ImageAnswer2.sprite = m_originalWordList[otherIndex].sprite;

                TextAnswer1.text = m_originalWordList[currentIndex].word;
                TextAnswer2.text = m_originalWordList[otherIndex].word;

                m_answer1Audio = m_originalWordList[currentIndex].word;
                m_answer2Audio = m_originalWordList[otherIndex].word;

                m_cacheGuessWords = Utility.RandomSortList(m_cacheGuessWords);
                for (int i = 0; i < m_cacheCupGuessList.Count; i++)
                {
                    m_cacheCupGuessList[i].guess.sprite = m_cacheGuessWords[i].sprite;
                    m_cacheCupGuessList[i].guessAudio = m_cacheGuessWords[i].word;
                    m_cacheCupGuessList[i].guessAskAudio = m_cacheGuessWords[i].word;
                    m_cacheCupGuessList[i].wordModel = m_cacheGuessWords[i];
                }
                break;
            case DataType.Ask:

                if (m_originalAskList.Count == 2 && m_currentIndex == 1)
                {
                    otherIndex = 0;
                    randomOther = UnityEngine.Random.Range(0, 2);
                }

                if (m_originalAskList.Count == 1)
                {
                    ConfigAskLibraryModel temp = m_currentUnitAllAskList[Random.Range(0, m_currentUnitAllAskList.Count)];
                    while (temp == m_originalAskList[0])
                    {
                        temp = m_currentUnitAllAskList[Random.Range(0, m_currentUnitAllAskList.Count)];
                    }
                    m_originalAskList.Add(temp);
                }

                if (m_originalAskList[currentIndex].MapWordString == m_originalAskList[otherIndex].MapWordString)
                {
                    m_cacheGuessAsks.Add(m_originalAskList[currentIndex]);
                    m_cacheGuessAsks.Add(m_currentUnitAllAskList[0]);
                    m_cacheGuessAsks.Add(m_originalAskList[randomOther]);
                }
                else
                {
                    m_cacheGuessAsks.Add(m_originalAskList[currentIndex]);
                    m_cacheGuessAsks.Add(m_originalAskList[otherIndex]);
                    m_cacheGuessAsks.Add(m_originalAskList[randomOther]);
                }


                if (m_originalAskList[currentIndex].MapWordString == m_originalAskList[otherIndex].MapWordString)
                {
                    ImageAnswer1.sprite = m_originalAskList[currentIndex].MapWordSprite;
                    ImageAnswer2.sprite = m_currentUnitAllAskList[0].MapWordSprite;
                    m_answer1Audio = m_originalAskList[currentIndex].MapWordString;
                    m_answer2Audio = m_currentUnitAllAskList[0].MapWordString;
                    TextAnswer1.text = string.IsNullOrEmpty(m_originalAskList[currentIndex].yes) ? m_originalAskList[currentIndex].no.Replace("_", " ") : m_originalAskList[currentIndex].yes.Replace("_", " ");
                    TextAnswer2.text = string.IsNullOrEmpty(m_currentUnitAllAskList[0].yes) ? m_currentUnitAllAskList[0].no.Replace("_", " ") : m_currentUnitAllAskList[0].yes.Replace("_", " ");
                }
                else
                {
                    ImageAnswer1.sprite = m_originalAskList[currentIndex].MapWordSprite;
                    ImageAnswer2.sprite = m_originalAskList[otherIndex].MapWordSprite;
                    m_answer1Audio = m_originalAskList[currentIndex].MapWordString;
                    m_answer2Audio = m_originalAskList[otherIndex].MapWordString;
                    TextAnswer1.text = string.IsNullOrEmpty(m_originalAskList[currentIndex].yes) ? m_originalAskList[currentIndex].no.Replace("_", " ") : m_originalAskList[currentIndex].yes.Replace("_", " ");
                    TextAnswer2.text = string.IsNullOrEmpty(m_originalAskList[otherIndex].yes) ? m_originalAskList[otherIndex].no.Replace("_", " ") : m_originalAskList[otherIndex].yes.Replace("_", " ");
                }

                m_cacheGuessAsks = Utility.RandomSortList(m_cacheGuessAsks);
                for (int i = 0; i < m_cacheCupGuessList.Count; i++)
                {
                    m_cacheCupGuessList[i].guess.sprite = m_cacheGuessAsks[i].MapWordSprite;
                    m_cacheCupGuessList[i].guessAudio = m_cacheGuessAsks[i].MapWordString;
                    m_cacheCupGuessList[i].guessAskAudio = m_cacheGuessAsks[i].MapWordString;
                    m_cacheCupGuessList[i].askModel = m_cacheGuessAsks[i];
                }
                break;
        }
    }
    /// <summary>
    /// 2017年10月24日14:33:07邱斌修改 -问句模式 - 将盒子盖下
    /// </summary>
    private void AskPlayGoOn()
    {
        Tweener tweenCup = DoCupMoveTween(0f);
        tweenCup.OnComplete(() =>
        {
            DoRandomMove();
        });
    }
    private int random = -1;//正确项下标

    public void DoCupAndGuessTween()
    {
        Tweener tweenCup = DoCupMoveTween(300f);
        tweenCup.OnComplete(() =>
        {
            //问句模式修改 - 2017年10月24日14:30:12邱斌
            if (StartData.dataType == DataType.Ask)
            {
                random = UnityEngine.Random.Range(0, m_cacheCupGuessList.Count);
                List<AudioSource> asource = m_cacheCupGuessList[random].askModel.PlaySentenceSound();
                float sentenceTime = 0;
                foreach (var v in asource)
                    sentenceTime += v.clip.length;
                Invoke("AskPlayGoOn", sentenceTime - askSentenceErrorTime);
                return;
            }


            Tweener tweenGuess = null;
            float delay = 0;
            for (int i = 0; i < m_cacheCupGuessList.Count; i++)
            {
                tweenGuess = DoGuessScaleTween(m_cacheCupGuessList[i].guess.transform, delay);
                string audio = m_cacheCupGuessList[i].guessAudio;
                tweenGuess.OnStart(() => AudioManager.Instance.Play(audio));
                delay += 1.5f;
            }
            tweenGuess.OnComplete(() =>
            {
                tweenCup = DoCupMoveTween(0f);
                tweenCup.OnComplete(() =>
                {
                    DoRandomMove();
                });
            });
        });
    }


    public void HideQuestionAndAnswer()
    {
        Question.gameObject.SetActive(false);
        Answer1.SetActive(false);
        Answer2.SetActive(false);
    }

    private Tweener DoCupMoveTween(float move, float delay = 0)
    {

        Tweener tween = null;
        for (int i = 0; i < m_cacheCupGuessList.Count; i++)
        {
            tween = m_cacheCupGuessList[i].cup.DOAnchorPosY(move, 0.5f).SetDelay(delay);
        }
        return tween;
    }

    private Tweener DoGuessScaleTween(Transform guess, float delay)
    {
        return guess.DOScale(Vector3.one * 1.5f, 0.7f).SetLoops(2, LoopType.Yoyo).SetDelay(delay);
    }

    private void DoRandomMove()
    {
        int randomNum1 = 0;
        int randomNum2 = 0;
        //int upOrDown = Random.Range(1, 2);
        RandomTwoNum(out randomNum1, out randomNum2);
        Transform cupGuessRandom1 = GetT(m_cupGuessPrefix + randomNum1);
        Transform cupGuessRandom2 = GetT(m_cupGuessPrefix + randomNum2);

        ExchangeCupGuessRandom(m_cupGuessPrefix, randomNum1, randomNum2);
        ExchangeCupGuessRandom(m_cupPrefix, randomNum1, randomNum2);
        ExchangeCupGuessRandom(m_guessPrefix, randomNum1, randomNum2);
        SortCupGuessHierarchy();
        //string bezierRandom1 = (upOrDown == 1 ? m_bezierUpPrefix : m_bezierDownPrefix) + (randomNum1 + randomNum2);
        //string bezierRandom2 = (upOrDown == 2 ? m_bezierUpPrefix : m_bezierDownPrefix) + (randomNum1 + randomNum2);
        DrawBezier bezier1 = Get<DrawBezier>(m_bezierDownPrefix + (randomNum1 + randomNum2));
        DrawBezier bezier2 = Get<DrawBezier>(m_bezierUpPrefix + (randomNum1 + randomNum2));
        Vector3[] path1 = bezier1.GetPoints();


        Tweener tween1 = cupGuessRandom1.DOPath(path1, 1.0f, PathType.CatmullRom);
        tween1.SetEase(Ease.InOutCubic);
        tween1.SetDelay(1);
        tween1.OnStart(() =>
        {
            AudioManager.Instance.Play("MoveCup");//移动杯子
        });
        tween1.OnComplete(() =>
        {
            m_randomCurrentNum++;
            if (m_randomCurrentNum < m_randomMoveNum)
                DoRandomMove();
            else
            {
                BeginDoQuestion();
            }

        });
        Vector3[] path2 = bezier2.GetPoints();
        Tweener tween2 = cupGuessRandom2.DOPath(path2, 1.0f, PathType.CatmullRom);

        tween2.SetEase(Ease.InOutCubic);
        tween2.SetDelay(1);
    }

    private void ExchangeCupGuessRandom(string prefix, int random1, int random2)
    {
        GameObject go1 = Get(prefix + random1);
        GameObject go2 = Get(prefix + random2);
        Set(prefix + random1, go2);
        Set(prefix + random2, go1);
    }

    private void SortCupGuessHierarchy()
    {
        for (int i = 1; i < 4; i++)
        {
            GetT(m_cupGuessPrefix + i).SetSiblingIndex(i);
        }
    }

    public virtual string GetCurrentQuessionAudio()
    {
        return CurrentAsk.id;
    }
    private void BeginDoQuestion()
    {
        Question.gameObject.SetActive(true);

        if(StartData.dataType != DataType.Ask)
            random = Random.Range(0, 3);

        m_specificCupGuess = m_cacheCupGuessList[random];
        //        AudioManager.Instance.Play(m_specificCupGuess.guessAskAudio);

        #region qiubin修改
        if (StartData.dataType == DataType.Ask)
        {
            AskText.text = m_specificCupGuess.askModel.ask.Replace("_", " ");
            m_specificCupGuess.askModel.PlayAskSound();//qiubin修改
        }
        else
        {
            AudioManager.Instance.Play(m_specificCupGuess.guessAskAudio);
        }
        #endregion

        float x = m_specificCupGuess.cupGuess.anchoredPosition.x;
        Question.anchoredPosition = new Vector2(x, Question.anchoredPosition.y);
        Answer1.SetActive(true);
        Answer2.SetActive(true);

        Question.DOScale(1.2f, 0.4f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        {
            if (!m_onceGuideHand)
            {
                string answer1Word = Answer1.GetComponentInChildren<TextMeshProUGUI>().text;
                if (CheckMatch(answer1Word))
                {
                    GuideHand1.gameObject.SetActive(true);
                    m_currentGuideHande = GuideHand1.gameObject;
                    GuideHand1.DOMove(Question.position, 2f).SetLoops(-1);
                    GuideHand2.gameObject.SetActive(false);
                }
                else
                {
                    GuideHand2.gameObject.SetActive(true);
                    m_currentGuideHande = GuideHand2.gameObject;
                    GuideHand2.DOMove(Question.position, 2f).SetLoops(-1);
                    GuideHand1.gameObject.SetActive(false);
                }
            }
        });
    }

    public void AnswerRight(System.Action reset, RectTransform answer)
    {
        Question.gameObject.SetActive(false);
        m_currentGuideHande.SetActive(false);
        if (!m_onceGuideHand)
            m_onceGuideHand = true;

        #region qiubin修改-----------------------------------------------------------------------
        if (StartData.dataType == DataType.Ask)
        {

        }
        else
        {
            AudioManager.Instance.Play(m_specificCupGuess.guessAudio);
        }
        #endregion

        Vector2 cacheAnswerPosition = answer.anchoredPosition;
        Tweener tween = m_specificCupGuess.cup.DOAnchorPosY(300f, 0.5f);
        tween.OnUpdate(() =>
        {
            float y = m_specificCupGuess.cup.anchoredPosition.y;
            Vector2 temp = cacheAnswerPosition;
            temp.y += y;
            answer.anchoredPosition = temp;
        }).OnComplete(() =>
        {
            Debug.Log("Guess Right!!");
            tween = m_specificCupGuess.cup.DOAnchorPosY(0f, 0.5f).SetDelay(2f);
            tween.OnComplete(() =>
            {
                reset();
                m_randomCurrentNum = 0;

                if (StartData.dataType != DataType.Ask)//qiubin修改
                    PlayGame();
            });
        });
    }

    public void AnswerWrong(System.Action reset, RectTransform answer)
    {
        GameObject balloon = answer.Find("ImageBalloon").gameObject;
        Sprite originalSprite = balloon.GetComponent<Image>().sprite;
        balloon.GetComponent<UIFrameAnimationCtrl>().enabled = true;
        balloon.transform.GetChild(0).gameObject.SetActive(false);
        answer.GetComponent<Rigidbody2D>().gravityScale = 1;
        Question.gameObject.SetActive(false);
        AudioManager.Instance.Play("BalloonExplosion");//气球爆炸
        Tweener tween = DoCupMoveTween(300f);
        tween.OnComplete(() =>
        {
            Debug.Log("Guess Wrong!!!");
            tween = DoCupMoveTween(0, 2f);
            tween.OnComplete(() =>
            {
                balloon.GetComponent<Image>().sprite = originalSprite;
                balloon.GetComponent<UIFrameAnimationCtrl>().enabled = false;
                balloon.transform.GetChild(0).gameObject.SetActive(true);
                answer.GetComponent<Rigidbody2D>().gravityScale = 0;
                reset();
                m_randomCurrentNum = 0;
                m_currentIndex--;
                PlayGame();
            });
        });

    }

    private void RandomTwoNum(out int first, out int second)
    {
        first = UnityEngine.Random.Range(1, 4);
        second = UnityEngine.Random.Range(1, 4);
        if (first == second || first > second || first == m_cacheFirstRandom && second == m_cacheSecondRandom)
            RandomTwoNum(out first, out second);
        m_cacheFirstRandom = first;
        m_cacheSecondRandom = second;
    }
    /// <summary>
    /// 时间:2016年6月16日15:20:27
    /// </summary>
    /// <param name="content">匹配的字符</param>
    /// <returns></returns>
    public bool CheckMatch(string content)
    {
        switch (StartData.dataType)
        {
            case DataType.Word:
                if (content == m_specificCupGuess.wordModel.word)
                {
                    return true;
                }
                break;
            case DataType.Ask:

                string tempAns = string.IsNullOrEmpty(m_specificCupGuess.askModel.yes) ? m_specificCupGuess.askModel.no.Replace("_", " ") : m_specificCupGuess.askModel.yes.Replace("_", " ");
                //if (content == m_specificCupGuess.askModel.yes.Replace("_", " "))
                if (content == tempAns)
                {
                    return true;
                }
                break;
        }
        return false;
    }

    public void Next(bool b)
    {
        switch (StartData.dataType)
        {
            case DataType.Word:
                FlyStar(b);
                break;
            case DataType.Ask:
                if (b)
                {
                    result = true;
                    m_specificCupGuess.askModel.PlaySentenceSound();//qiubin修改
                    List<AudioSource> asource = m_cacheCupGuessList[random].askModel.PlaySentenceSound();
                    float sentenceTime = 0;
                    foreach (var v in asource)
                        sentenceTime += v.clip.length;
                    Invoke("OnAskNext", sentenceTime - askSentenceErrorTime);
                }
                else 
                {
                    FlyStar(CupGuessAsk.id, b);
                }

                break;
        }
    }
    private bool result = false;//当前结果
    private float askSentenceErrorTime = 3f;//对话句子的误差时间
    /// <summary>
    /// qiubin修改
    /// </summary>
    private void OnAskNext()
    {
        FlyStar(CupGuessAsk.id, true).OnComplete += () =>
        {
            m_cacheCupGuessList.RemoveAt(random);//移除掉当前已选择的
            result = false;
            PlayGame();
        };
    }
}
