using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIRiver : UIBaseLevelGame
{

    private BoxCollider2D TopCollider { get { return Get<BoxCollider2D>("ImageTop"); } }
    private RectTransform ClampGears { get { return GetR("ClampGears"); } }
    private RectTransform Gears { get { return GetR("Gears"); } }
    private GameObject ImageLucency { get { return Get("ImageLucency"); } }
    public Image ImageWord { get { return Get<Image>("ImageWord"); } }

    private int m_matchLetterNum = 0;
    /// <summary>
    /// 溪流声音
    /// </summary>
    private AudioSource m_waterSound;

    private string current_word;

    /// <summary>
    /// 字母索引
    /// </summary>
    public int LetterIndex = -1;
    Tweener tween;
    public static UIRiver Instance { get; set; }

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
        Destroy(m_waterSound.gameObject);
    }

    public override void Refresh()
    {
        SetTraySprite();
        UGUIEventListener.Get(ImageWord).onPointerDown = d =>
        {
            if (!tween.IsPlaying())
            {
                GraphEffect(ImageWord.gameObject);
            }
            MatchWordSound(CurrentWord.word);
        };
        m_waterSound = AudioManager.Instance.Play("SoundOfWater");
        m_waterSound.loop = true;
    }

    public override void PlayGame()
    {
        if (IsGameEnd)
            return;

        LetterIndex = 0;
        TopCollider.enabled = false;
        ImageLucency.SetActive(true);
        ImageWord.sprite = CurrentWord.sprite;
        TweenWordMove();
        CreateClampAndGears();
        //this.Invoke(() =>
        //{
        //    AudioManager.Instance.Play("FruitDrop");//掉落声音
        //}, 2f);

    }
    /// <summary>
    /// 替换漂流的托盘
    /// </summary>
    public virtual void SetTraySprite()
    {

    }
    /// <summary>
    /// 播放单词的音效
    /// </summary>
    /// <param name="strWord"></param>
    public void MatchWordSound(string strWord)
    {
        if (!string.IsNullOrEmpty(strWord))
        {
            AudioManager.Instance.Play(strWord);
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
    private void CreateClampAndGears()
    {
        Utility.ClearChild(ClampGears);
        current_word = CurrentWord.word.Replace(" ", "");
        var letters = current_word.ToCharArray();
        letters = Utility.RandomSortList(letters);
        for (int i = 0; i < letters.Length; i++)
        {
            GameObject go = CreateUIItem("UIRiverClamp", ClampGears.transform);
            var cp = go.AddComponent<UIRiverClamp>();
//            cp.Init(CurrentWord.word[i], i);
            cp.Init(current_word[i], i,StartData.difficulty);
            go = CreateUIItem("UIRiverGear", Gears);
            var letter = go.GetComponentInChildren<TextMeshProUGUI>();
            letter.text = letters[i].ToString();
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(Gears);
        Gears.GetComponent<HorizontalLayoutGroup>().enabled = false;
    }

    private Tweener TweenWordMove()
    {
        float distance = Screen.currentResolution.width / 2f;
        var tween = ImageWord.rectTransform.DOAnchorPosX(-distance, 3f).SetEase(Ease.Linear);
        tween.OnComplete(() =>
        {
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
//        if (m_matchLetterNum == CurrentWord.word.Length)
        if (m_matchLetterNum == current_word.Length)
        {
            m_matchLetterNum = 0;
            var cps = ClampGears.GetComponentsInChildren<UIRiverClamp>();
            var sequence = DOTween.Sequence();
            foreach (var item in cps)
            {
                sequence.Insert(0, item.TweenGearScale());
            }
            CurrentWord.PlaySound();
            sequence.AppendCallback(() =>
            {
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
}
