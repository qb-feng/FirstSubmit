using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIConveyor : UIBaseLevelGame
{
    #region 字段 属性 变量
    public GameObject goGuideHand;
    private GameObject ImageLucency { get { return Get("ImageLucency"); } }
    private BoxCollider2D TopCollider { get { return Get<BoxCollider2D>("ImageTop"); } }
    private RectTransform ClampGears { get { return GetR("ClampGears"); } }
    private RectTransform Gears { get { return GetR("Gears"); } }
    private Image ImageWord { get { return Get<Image>("ImageWord"); } }
    //private UIFrameAnimationCtrl GifConveyor { get { return Get<UIFrameAnimationCtrl>("ImageGifConveyor"); } }
    private RectTransform Conveyor { get { return GetR("Conveyor"); } }
    private Tweener m_conveyorTween;

    private int m_matchLetterNum = 0;
    /// <summary>
    /// 字母索引
    /// </summary>
    public int LetterIndex = -1;

    Tweener tween;
    #endregion

    public static UIConveyor Instance { get; set; }

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
        //2016年6月2日15:52:16
        //白建新:添加考察图案的点击发声和点击物体变大效果
        UGUIEventListener.Get(ImageWord).onPointerDown = d =>
        {
            if (!tween.IsPlaying())
            {
                GraphEffect(ImageWord.gameObject);
            }
            MatchWordSound(CurrentWord.word);
        };

        m_conveyorTween = Conveyor.DOAnchorPosX(-3000, 8).SetLoops(-1, LoopType.Restart);
        m_conveyorTween.Pause();
    }

    public override void PlayGame()
    {
        if (goGuideHand != null)
        {
            Destroy(goGuideHand);
        }
        if (IsGameEnd)
            return;

        LetterIndex = 0;
        TopCollider.enabled = false;
        ImageLucency.SetActive(true);
        ImageWord.sprite = CurrentWord.sprite;
        CreateClampAndGears();
        TweenWordMove();
        this.WaitSecond(() =>
        {
            AudioManager.Instance.Play("FruitDrop");//
        }, 2f);
    }
    /// <summary>
    /// 播放单词的音效
    /// </summary>
    /// <param name="strWord"></param>
    public void MatchWordSound(string strWord)
    {
        if (!string.IsNullOrEmpty(strWord))
        {            
            CurrentWord.PlaySound();
        }
    }
    /// <summary>
    /// 物体变大特效
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public Tweener GraphEffect(GameObject go)
    {
        tween = go.transform.DOScale(Vector3.one * 1.2f, 0.4f).SetLoops(2, LoopType.Yoyo);
        return tween;
    }

    private GameObject secondgo = null;

    public void Secondchar()
    {
        secondgo.GetComponent<UIConveyorGearSelectCtrl>().IsFirst(ClampGears.GetChild(1).GetChild(0).transform);
    }
    private void CreateClampAndGears()
    {
        Utility.ClearChild(ClampGears);
        string s = CurrentWord.word.Replace(" ", "");
        var letters = s.ToCharArray();
        var originals = letters;
        char first = letters[0];//首字母
        char second = letters[1];//次字母
        GameObject firstgo = null;
        letters = Utility.RandomSortList(letters);
        for (int i = 0; i < letters.Length; i++)
        {
            GameObject go1 = CreateUIItem("UIConveyorGear", Gears);
            var letter = go1.GetComponentInChildren<TextMeshProUGUI>();
            letter.text = letters[i].ToString();
            if (letters[i] == first)
            {
                firstgo = go1;
            }
            else if (letters[i] == second)
            {
                secondgo = go1;
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(Gears);
        Gears.GetComponent<HorizontalLayoutGroup>().enabled = false;

        this.WaitSecond(() =>
        {
            for (int i = 0; i < originals.Length; i++)
            {
                GameObject go = CreateUIItem("UIConveyorClamp", ClampGears.transform);
                var cp = go.AddComponent<UIConveyorClamp>();
                cp.Init(originals[i], i,StartData.difficulty);
            }
        }, 1f);

        StartCoroutine(WaitTwo(firstgo));
    }

    IEnumerator WaitTwo(GameObject go)
    {
        yield return new WaitForSeconds(2);
        if (IsFirstPlay)
        {
            isfirstPlay = true;
            go.GetComponent<UIConveyorGearSelectCtrl>().IsFirst(ClampGears.GetChild(0).GetChild(0).transform);
        }
    }

    public bool isfirstPlay = false;//为了在item中判断是否为第一次
    private Tweener TweenWordMove()
    {
        //GifConveyor.enabled = true;
        m_conveyorTween.Play();
        float distance = Screen.currentResolution.width / 2f;
        var tween = ImageWord.rectTransform.DOAnchorPosX(-distance, 3f).SetEase(Ease.Linear);
        tween.OnComplete(() =>
        {
            //GifConveyor.enabled = false;
            m_conveyorTween.Pause();
            TweenClampMove().OnComplete(() =>
            {
                MatchWordSound(CurrentWord.word);
                GraphEffect(ImageWord.gameObject);
                TopCollider.enabled = true;
                ImageLucency.SetActive(false);
            });
        });
        return tween;
    }

    private Tweener TweenWordGo()
    {
        //GifConveyor.enabled = true;
        m_conveyorTween.Play();
        float distance = Screen.currentResolution.width + ImageWord.rectTransform.rect.width / 2;
        var tween = ImageWord.rectTransform.DOAnchorPosX(-distance, 3f).SetEase(Ease.Linear);
        return tween;
    }

    private Tweener TweenClampMove()
    {
        var tween = ClampGears.DOAnchorPosY(-ClampGears.rect.height, 1f);
        return tween;
    }

    public Tweener TweenClampBack()
    {
        var tween = ClampGears.DOAnchorPosY(0, 1f);
        return tween;
    }

    public void AddMatchNum()
    {
        m_matchLetterNum++;
        string s = CurrentWord.word.Replace(" ", "");
        if (m_matchLetterNum == s.Length)
        {
            m_matchLetterNum = 0;
            var cps = ClampGears.GetComponentsInChildren<UIConveyorClamp>();
            var sequence = DOTween.Sequence();
            foreach (var item in cps)
            {
                sequence.Insert(0, item.TweenGearScale());
            }
            CurrentWord.PlaySound();
            sequence.AppendCallback(() =>
            {
                //UITopBarBlack.Instance.AddOneStar();
                FlyStar(true);
                TweenClampBack();
                TweenWordGo().OnComplete(() =>
                {
                    var temp = ImageWord.rectTransform.anchoredPosition;
                    temp.x = ImageWord.rectTransform.rect.width / 2;
                    ImageWord.rectTransform.anchoredPosition = temp;
                    Gears.GetComponent<HorizontalLayoutGroup>().enabled = true;
                    PlayGame();
                });
            });
        }
    }


    /// <summary>
    /// 获取当前需要匹配的字母
    /// </summary>
    /// <param name="strWallWord"></param>
    /// <param name="iCurrentIndx"></param>
    /// <returns></returns>
    public string GetCurrentWord(string strWallWord, int iCurrentIndx)
    {
        string temp = strWallWord;
        if (iCurrentIndx >= temp.Length)
        {
            return "";
        }
        string word = temp.Substring(LetterIndex, 1);
        if (word.Equals(" "))
        {
            word = temp.Substring(++LetterIndex, 1);
        }
        if (word != "")
        {
            return word;
        }
        return "";
    }
    /// <summary>
    /// 设置所有
    /// </summary>
    /// <param name="currIndex"></param>
    public void SetCurrentIndx(int currIndex)
    {
        LetterIndex = currIndex;
    }

    public void StartActive(Transform t)
    {
        //currentTransformt = t;
        StartCoroutine(TextActive(t));
    }

    //private Transform currentTransformt;
    public void Stop()
    {
        StopAllCoroutines();
    }
    IEnumerator TextActive(Transform t)
    {
        t.gameObject.SetActive(!t.gameObject.activeSelf);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(TextActive(t));
    }
}
