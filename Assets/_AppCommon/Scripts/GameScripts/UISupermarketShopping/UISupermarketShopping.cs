using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Spine.Unity;
using TMPro;
using System.Collections;

public class UISupermarketShopping : UIBaseLevelGame
{
    #region 属性 字段 变量
    private int ShoppingWordIndex = -1;
    private Tweener GuiderTween;

    private GameObject goGuideHand;
    private GameObject ImageWordPosion { get { return Get("ImageWordPosion"); } }
    /// <summary>
    /// 放置颜色瓶子的位置
    /// </summary>
    private GameObject PacketContainer { get { return Get("PacketContainer"); } }
    /// <summary>
    /// 创建桌子的位置对象
    /// </summary>
    private GameObject TableBottlesPosion { get { return Get("TableBottlesPosion"); } }

    private GameObject UIShoppingWord { get { return Get("UIShoppingWord"); } }
    /// <summary>
    /// 车子主体
    /// </summary>
    private GameObject ImageCarMain { get { return Get("ImageCarMain"); } }
    /// <summary>
    /// 桌子和颜料瓶
    /// </summary>
    public GameObject goTable;
    public GameObject Horizontal;
    private string StrBottleName = "UISupermarketShoppingColorBottle_Common";
    /// <summary>
    /// 文字特效
    /// </summary>
    Tweener tween;
    /// <summary>
    /// 波利购物动画
    /// </summary>
    private SkeletonGraphic ShoopingBoly { get { return Get<SkeletonGraphic>("ShoopingBoly"); } }

    public static ConfigWordLibraryModel[] WordArry;
    public static int[] bottlesPosIndx = { 0, 1, 2 };
    RandomArrayHelper RandomHelp = new RandomArrayHelper();
    public List<ConfigWordLibraryModel> lstArray;
    private List<ConfigWordLibraryModel> ThisTimeToTest;
    #endregion

    public static UISupermarketShopping Instance { get; set; }
    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
    }

    void Start()
    {
        ShoopingBoly.AnimationState.Start += delegate
        {
            ShoopingBoly.Skeleton.SetToSetupPose();
        };
    }
    public override void Refresh()
    {
        UGUIEventListener.Get(UIShoppingWord).onPointerDown = d =>
        {
            MatchWordSound(CurrentWord.letter);
        };
        UGUIEventListener.Get(ImageCarMain).onPointerDown = d =>
        {
            MatchWordSound(CurrentWord.letter);
        };

        WordArry = new ConfigWordLibraryModel[m_randomWordList.Count];
        for (int i = 0; i < m_randomWordList.Count; i++)
        {
            WordArry[i] = m_randomWordList[i];
        }
        lstArray = WordArry.ToList();
        WordArry = null;
    }
    /// <summary>
    ///一局游戏开始
    /// </summary>
    public override void PlayGame()
    {
        if (goGuideHand != null)
        {
            Destroy(goGuideHand);
        }
        if (IsGameEnd)
            return;

        //TODO 每次创建物体的时候只创建一个包含currentWord letter的选项和两个干扰项
        ThisTimeToTest = new List<ConfigWordLibraryModel>();
        foreach (var i in lstArray)
        {
            if (i.letter != CurrentWord.letter)
            {
                ThisTimeToTest.Add(i);
            }          
        }
        while (ThisTimeToTest.Count > 2)
        {
            ThisTimeToTest.RemoveAt(0);
        }
        ThisTimeToTest.Add(CurrentWord);
        ThisTimeToTest = Utility.RandomSortList(ThisTimeToTest);

        ShoppingWordIndex = 0;
        //创建桌子
        goTable = CreateUIItem("UISupermarketShoppingTableBottles", TableBottlesPosion.transform);
        Horizontal = goTable.transform.GetChild(0).gameObject;
        if (goTable)
        {
            TableBottlesCome().OnComplete(() =>
            {         
                
                ShoopingBoly.AnimationState.SetAnimation(0, "UP", false);
                ShoopingBoly.AnimationState.Complete += CreatCurrentWord;             
                for (int i = 0; i < 3; i++)
                {
                    var go = CreateUIItem(StrBottleName, Horizontal.transform.GetChild(i).transform);
                    var cp = go.GetComponent<Image>();
                    cp.sprite = ThisTimeToTest[i].sprite;
                }             
            });
        }
    }
    private Tweener TeenGuiderMove(Transform goGuiderParent)
    {
        GuiderTween = goGuiderParent.DOMove(PacketContainer.transform.position, 3f).SetEase(Ease.Linear).SetLoops(-1);
        return GuiderTween;
    }
    private void CreatCurrentWord(Spine.TrackEntry trackEntry)
    {
        ShoopingBoly.AnimationState.Complete -= CreatCurrentWord;
        //创建单词牌
        UIShoppingWord.GetComponentInChildren<TextMeshProUGUI>().text = CurrentWord.letter;
        CurrentWord.PlayLetter();
        wordEff();
    }
    /// <summary>
    /// 播放单词的音效
    /// </summary>
    /// <param name="WordId"></param>
    public void MatchWordSound(string WordId)
    {
        AudioManager.Instance.Play(WordId, false);
        if (!tween.IsPlaying())
            wordEff();
    }

    /// <summary>
    /// 播放文字变化效果
    /// </summary>
    /// <returns></returns>
    private Tweener wordEff()
    {
        tween = UIShoppingWord.GetComponentInChildren<TextMeshProUGUI>().transform.DOScale(1.2f, 0.5f).SetLoops(2, LoopType.Yoyo);
        return tween;
    }
    /// <summary>
    /// 检测匹配正确后放置物品到对应位置,增加星星
    /// </summary>
    /// <param name="objCtrl"></param>
    public void CheckColorMatch(UISupermarketShoppingSelectCtrl objCtrl)
    {
        StartCoroutine(PlayThreeAudioIE(objCtrl));                
    }
    public IEnumerator PlayThreeAudioIE(UISupermarketShoppingSelectCtrl objCtrl)
    {
        
        ShoppingWordIndex++;
        FlyStar(true);
        yield return new WaitForSeconds(AudioManager.Instance.PlayThreeAudio(CurrentWord.letter, CurrentWord.letter, CurrentWord.word));
        GameObject go = CreateUIItem(StrBottleName, PacketContainer.transform);
        var cp = go.GetComponent<Image>();
        cp.sprite = CurrentWord.sprite;
        go.transform.Translate((float)RandomHelp.GetRandomInt(-2, 2) * 0.4f, (float)RandomHelp.GetRandomInt(-3, 1) / 5, 0);
        //播放完成后高兴的动画
        if (goGuideHand != null)
        {
            Destroy(goGuideHand);
            GuiderTween.Kill();
        }
        ShoopingBoly.AnimationState.SetAnimation(0, "DOWN", false);
        ShoopingBoly.AnimationState.Complete += DeleteCurrentWord;

        ShoopingBoly.AnimationState.SetAnimation(0, "HAPPY", false);
        ShoopingBoly.AnimationState.Complete += GoonMove;
        Destroy(objCtrl.gameObject);
      
        //让放有颜色瓶子的台子移动完成之后
        TableBottlesLive().OnComplete(() =>
        {
            for (int i = 0; i < Horizontal.transform.childCount; i++)
            {
                Utility.ClearChild(Horizontal.transform.GetChild(i));
            }
            lstArray.Add(CurrentWord);//补充到集合提供随机数据源
            Destroy(goTable.gameObject);
            PlayGame();
        });
    }
    /// <summary>
    /// 继续购物
    /// </summary>
    /// <param name="state"></param>
    /// <param name="trackIndex"></param>
    /// <param name="loopCount"></param>
    private void GoonMove(Spine.TrackEntry trackEntry)
    {
        ShoopingBoly.AnimationState.Complete -= GoonMove;
        ShoopingBoly.AnimationState.SetAnimation(0, "MOVE", true);
    }
    /// <summary>
    /// 答完之后把当前单词置空
    /// </summary>
    /// <param name="state"></param>
    /// <param name="trackIndex"></param>
    /// <param name="loopCount"></param>
    private void DeleteCurrentWord(Spine.TrackEntry trackEntry)
    {
        ShoopingBoly.AnimationState.Complete -= DeleteCurrentWord;
        UIShoppingWord.GetComponentInChildren<TextMeshProUGUI>().text = "";
    }

    /// <summary>
    /// 颜料瓶子进入
    /// </summary>
    public Tweener TableBottlesCome()
    {
        var tween = goTable.transform.DOMoveX(2, 2);
        return tween;
    }

    /// <summary>
    /// 颜料瓶子离开
    /// </summary>
    public Tweener TableBottlesLive()
    {
        var tween = goTable.transform.DOMoveX(-20, 2);
        return tween;
    }
}
