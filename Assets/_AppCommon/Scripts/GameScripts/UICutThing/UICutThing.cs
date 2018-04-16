using UnityEngine;
using DG.Tweening;
using System;

#region 换皮模型定义
public class UICutThingBread : UICutThing { }

public class UICutWatermelon : UICutThing { }
#endregion

public class UICutThing : UIBaseLevelGame
{
    private RectTransform Melon { get { return GetR("ImageMelon"); } }
    private RectTransform CutBoard { get { return GetR("ImageCutBoard"); } }
    private UIFrameAnimationCtrl Cuting { get { return Get<UIFrameAnimationCtrl>("ImageCutingAnimation"); } }
    private Transform HorizontalMelonSlices { get { return GetT("HorizontalMelonSlices"); } }
    private Transform HorizontalSelectLetters { get { return GetT("HorizontalSelectLetters"); } }

    private Vector2 m_cutingOriginalPosition;
    private Vector2 m_cutboardTweenXOriginalPosition;
    private float m_cutboardTweenXDistance;
    private Vector2 m_melonTweenYOriginalPosition;
    private float m_melonTweenYDistance;

    private int m_matchLetterCount = 0;
    /// <summary>
    /// 字母索引
    /// </summary>
    public int wallWordIndex = -1;

    public static UICutThing Instance { get; set; }

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
        Cuting.gameObject.SetActive(false);
        m_cutingOriginalPosition = ((RectTransform)Cuting.transform).anchoredPosition;
        m_cutboardTweenXOriginalPosition = CutBoard.anchoredPosition;
        m_cutboardTweenXDistance = m_cutboardTweenXOriginalPosition.x + CutBoard.rect.width / 2 + Screen.currentResolution.width / 2;
        CutBoard.anchoredPosition -= new Vector2(m_cutboardTweenXDistance, 0);
        m_melonTweenYOriginalPosition = Melon.anchoredPosition;
        m_melonTweenYDistance = m_melonTweenYOriginalPosition.y + Melon.rect.height / 2 + Screen.currentResolution.height / 2;
        Melon.anchoredPosition += new Vector2(0, m_melonTweenYDistance);

    }

    public override void PlayGame()
    {
        if (IsGameEnd)
            return;
        //Debug.LogError(StartData.difficulty);
        wallWordIndex = 0;
        ((RectTransform)Cuting.transform).anchoredPosition = m_cutingOriginalPosition;
        m_matchLetterCount = 0;
        Tweener tweenCutBoard = CutBoardTweenRight();
        Tweener tweenMelon = MelonTweenDown();
        tweenMelon.Pause();
        tweenCutBoard.OnComplete(() =>
        {
            tweenMelon.Play().OnComplete(() =>
            {
                CutingAnimation(() =>
                {
                    SetMelonAndCutBoardActive(false);
                    CreateMelonSlices(HorizontalMelonSlices);
                    SetHorizontalAlpha(HorizontalMelonSlices, 1);
                    TweenWord();
                    this.WaitSecond(() =>
                    {
                        TweenEveryMelonSlice(HorizontalMelonSlices, 0).AppendCallback(() =>
                        {
                            CreateSelectLetters();
                            SequenceHorizontal(HorizontalSelectLetters, 1);
                        });
                    }, 2f);
                    this.WaitSecond(() =>
                    {
                        AudioManager.Instance.Play("FruitDrop");//
                    }, 3.5f);
                    SetelonSlicesChiidIsTrigger();
                });
            });
        });
    }

    private void SetelonSlicesChiidIsTrigger()
    {
        for (int i = 0; i < HorizontalMelonSlices.childCount; i++)
        {
            HorizontalMelonSlices.GetChild(i).GetChild(0).GetComponent<Collider2D>().isTrigger = true;
        }
    }
    ///// <summary>
    ///// 掉落到地面效果
    ///// </summary>
    ///// <param name="HorizontalSelectLetters">单词掉落</param>
    //private void DorpSlelectLettersHorizontal(Transform HorizontalSelectLetters)
    //{
    //    HorizontalSelectLetters.gameObject.AddComponent<BoxCollider2D>();
    //    HorizontalSelectLetters.gameObject.AddComponent<Rigidbody2D>();
    //}

    private void SetMelonAndCutBoardActive(bool active)
    {
        CutBoard.gameObject.SetActive(active);
        Melon.gameObject.SetActive(active);
    }

    private Tweener CutBoardTweenRight()
    {
        return CutBoard.DOAnchorPosX(m_cutboardTweenXOriginalPosition.x, 1f).SetEase(Ease.InOutBack);
    }

    private Tweener CutBoardTweenLeft()
    {
        return CutBoard.DOAnchorPosX(-m_cutboardTweenXDistance, 1f).SetEase(Ease.InOutBack);
    }

    private Tweener MelonTweenDown()
    {
        return Melon.DOAnchorPosY(m_melonTweenYOriginalPosition.y, 1f).SetEase(Ease.InOutBack);
    }

    private Tweener MelonTweenUp()
    {
        return Melon.DOAnchorPosY(m_melonTweenYDistance, 1f).SetEase(Ease.OutBounce);
    }

    private void CutingAnimation(Action cutingFinish)
    {
        int cutNum = 0;
        Cuting.gameObject.SetActive(true);
        AudioManager.Instance.Play("cutSound");
        Cuting.LoopOnceFinishAction = () =>
        {
            cutNum++;
            if (cutNum < 4)
                AudioManager.Instance.Play("cutSound");
            RectTransform rt = (RectTransform)Cuting.transform;
            rt.anchoredPosition += new Vector2(120, 0);
            if (cutNum == 4)
            {
                Cuting.LoopOnceFinishAction = null;
                Cuting.gameObject.SetActive(false);
                cutingFinish();
            }
        };
    }

    private void CreateMelonSlices(Transform parent, bool random = false)
    {
        Utility.ClearChild(parent);
        //qiubin修改 - 短语单词留出空格面包
        char[] letters = CurrentWord.word.ToCharArray();
        //char[] letters = CurrentWord.word.Replace(" ", "").ToCharArray();
        if (random)
            letters = Utility.RandomSortList(letters);
        for (int i = 0; i < letters.Length; i++)
        {
            string currentState = UIManager.Instance.CurrentStatusUI.ToString();
            GameObject go = CreateUIItem(currentState + "Slice", parent);
            var cp = go.AddComponent<UICutThingSlice>();
            if (random)
                //go.GetComponentInChildren<Collider2D>().isTrigger = true;
                cp.MelonSlice.gameObject.GetComponent<Collider2D>().isTrigger = true;
            cp.MelonSlice.gameObject.layer = parent.gameObject.layer;
            cp.Init(letters[i], random, i);
        }
        SetHorizontalAlpha(parent, 0);
    }

    private UICutThingSlice[] GetItemCps(Transform parent)
    {
        return parent.GetComponentsInChildren<UICutThingSlice>();
    }

    private void CreateSelectLetters()
    {
        CreateMelonSlices(HorizontalSelectLetters, true);
        var cps = GetItemCps(HorizontalSelectLetters);
        for (int i = 0; i < cps.Length; i++)
        {
            cps[i].MelonSliceDotted.gameObject.SetActive(false);
            //qiubin修改
            var v = cps[i].gameObject.transform.GetChild(0).gameObject.GetComponent<Rigidbody2D>();
            if (v == null)
                v = cps[i].gameObject.transform.GetChild(0).gameObject.AddComponent<Rigidbody2D>();
            v.gravityScale = 1;
            //cps[i].gameObject.transform.GetChild(0).gameObject.AddComponent<Rigidbody2D>();
            //cps[i].gameObject.transform.GetChild(0).gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            //cps[i].gameObject.transform.GetChild(0).gameObject.AddComponent<BoxCollider2D>();
            cps[i].gameObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0f);
            cps[i].gameObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().size = new Vector2(150f, 180f);
            cps[i].gameObject.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }

    private void TweenAlphaHorizontal(Transform parent, float alhpa)
    {
        var cps = GetItemCps(parent);
        foreach (var item in cps)
        {
            item.TweenCanvasGroupAlpha(alhpa);
        }
    }

    private Sequence TweenEveryMelonSlice(Transform parent, float alpha)
    {
        UICutThingSlice[] cps = GetItemCps(parent);
        Sequence sq = DOTween.Sequence();
        for (int i = 0; i < cps.Length; i++)
        {
            sq.Append(cps[i].TweenMelonAlpha(alpha));
            if (StartData.difficulty == 2)
            {
                sq.Append(cps[i].TweenLetterAlpha(alpha));
            }
        }
        return sq;
    }

    private Sequence SequenceHorizontal(Transform parent, float alpha)
    {
        var cps = GetItemCps(parent);
        Sequence sq = DOTween.Sequence();
        for (int i = 0; i < cps.Length; i++)
        {
            sq.Append(cps[i].TweenCanvasGroupAlpha(alpha));
        }
        return sq;
    }

    private void SetHorizontalAlpha(Transform parent, float alpha)
    {
        var cps = GetItemCps(parent);
        foreach (var item in cps)
        {
            item.SetMelonAlpha(alpha);
        }
    }

    public void AddMatchLetterCount()
    {
        string temp = CurrentWord.word;
        if (wallWordIndex > temp.Length)
        {
            return;
        }
        m_matchLetterCount++;
        if (m_matchLetterCount == CurrentWord.word.Length)
        {
            //UITopBarBlack.Instance.AddOneStar();
            FlyStar(true);
            TweenWord().OnComplete(() =>
            {
                Utility.ClearChild(HorizontalMelonSlices);
                Utility.ClearChild(HorizontalSelectLetters);
                SetMelonAndCutBoardActive(true);
                MelonTweenUp().OnComplete(() =>
                {
                    CutBoardTweenLeft().OnComplete(() =>
                    {
                        PlayGame();
                    });
                });
            });
        }
    }

    public void SetCurrentIndx(int currIndex)
    {
        wallWordIndex = currIndex;

    }
    private Tweener TweenWord()
    {
        CurrentWord.PlaySound();
        return HorizontalMelonSlices.DOScale(1.2f, 0.5f).SetLoops(2, LoopType.Yoyo);
    }

    /// <summary>
    /// 获取当前需要涂改的字母
    /// </summary>
    /// <param name="strWallWord"></param>
    /// <param name="iCurrentIndx"></param>
    /// <returns></returns>
    public string GetCurrentWord(string strWallWord, int iCurrentIndx)
    {
        //qiubn修改为短语出现空格
        string temp = strWallWord;
        //string temp = strWallWord.Replace(" ", "");
        if (iCurrentIndx >= temp.Length)
        {
            return "";
        }
        string word = temp.Substring(wallWordIndex, 1);
        return word;

    }
}
