using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// 游戏模型 - 176 打靶
/// </summary>
public class UIShootingPractice : UIBaseLevelGame
{
    //供外部访问的数据
    public static UIShootingPractice instance = null;//单例 ~ 可怕~~
    void OnDestroy()
    {
        instance = null;
    }
    public float GetButtomWorldY { get { return Buttom.position.y; } }//得到当前视图最底部的y值

    //游戏组件
    private Transform Arrows { get { return GetR("Arrows"); } }//箭筒
    private List<GameObject> arrows = new List<GameObject>();//箭筒里所有的箭

    private RectTransform WampCenter { get { return GetR("WampCenter"); } }//武器中心 - 箭放置的位置
    private RectTransform WampLeft { get { return GetR("WampLeft"); } }//箭的左弦
    private RectTransform WampRight { get { return GetR("WampRight"); } }//箭的右弦
    private Transform Practices { get { return GetT("Practices"); } }//所有敌人点的父物体
    private GameObject AudioSource { get { return Get("AudioSource"); } }//播音器
    private Transform Buttom { get { return GetT("Buttom"); } }//游戏的最底部
    private TextMeshProUGUI ShowText { get { return Get<TextMeshProUGUI>("ShowText"); } }//当前显示的单词


    //数据
    private List<ConfigWordLibraryModel> falseWords = new List<ConfigWordLibraryModel>();//当前单词的混淆项
    private float playerStaringTime = 2f;//玩家的准备时间

    //变量
    private Vector3 WampLeftRefreshLocalPositon = Vector3.zero;//箭左弦的重置位置
    private Vector3 WampRightRefreshLocalPositon = Vector3.zero;//箭右弦的重置位置
    private Vector2 WampLeftRightRefreshSizeDelate = Vector3.zero;//箭左右弦重置的大小
    private AudioSource asource;//播音器
    private int CurrentPracticeMaxNumber = 0;//当前活着的敌人的最大数量
    private int CurrentPracticeResultBackNum = -3;//当前敌人返回的次数
    private List<UIShootingPracticePracticeItem> CurrentPractices = new List<UIShootingPracticePracticeItem>();//当前活着的所有敌人
    private List<Sprite> CurrentPracticesSprites = new List<Sprite>();//当前活着的敌人的显示的图片
    private int CurrentPracticeRefreshNum = -3;//敌人次数的初始化次数
    /// <summary>
    /// 数据重置
    /// </summary>
    private void ResatData()
    {
        falseWords.Clear();
        //当前活着的敌人的变量进行初始化
        CurrentPracticeMaxNumber = Practices.childCount;
        CurrentPracticeRefreshNum = -CurrentPracticeMaxNumber;
        CurrentPracticeResultBackNum = CurrentPracticeRefreshNum;
        CurrentPractices.Clear();
        CurrentPracticesSprites.Clear();
    }

    /// 游戏开始
    public override void Refresh()
    {
        instance = this;
        base.Refresh();
        //初始化箭筒里所有的箭
        for (int i = 0; i < Arrows.childCount; ++i)
        {
            arrows.Add(Arrows.GetChild(i).gameObject);
        }
        //博放器初始化
        UGUIEventListener.Get(AudioSource).onPointerClick += (click) =>
        {
            asource = CurrentWord.PlaySound();
        };
    }
    public override void PlayGame()
    {
        ResatData();
        if (IsGameEnd)
            return;
        //找到当前单词的混淆项
        string currenWordId = CurrentWord.id.Substring(0, 5);//当前单元
        foreach (var v in ConfigManager.Get<ConfigWordLibraryModel>())
        {
            if (v.id.Substring(0, 5).Equals(currenWordId))
                falseWords.Add(v);
        }
        //处理混淆项
        falseWords.Remove(CurrentWord);
        Utility.RandomSortList(falseWords);

        //随机出正确单词的item
        int trueIndex = Random.Range(0, Practices.childCount);
        //设置敌人item
        for (int i = 0; i < Practices.childCount; ++i)
        {
            Practices.GetChild(i).ClearAllChild();
            if (i == trueIndex)
            {
                CreateUIItem<UIShootingPracticePracticeItem>(Practices.GetChild(i)).Init(CurrentWord.sprite, playerStaringTime, OnPracticeResultBack);
                continue;
            }
            CreateUIItem<UIShootingPracticePracticeItem>(Practices.GetChild(i)).Init(falseWords[i].sprite, playerStaringTime, OnPracticeResultBack);
        }
        //创建箭 - 在准备时间过后创建
        Invoke("CreateArrow", playerStaringTime);

        //显示当前单词
        ShowText.SetText(CurrentWord.word);

        //播放读音
        if (asource != null && asource.isPlaying)
            asource.Stop();
        asource = CurrentWord.PlaySound();
    }

    /// <summary>
    /// 敌人的回调函数 
    /// 参数1：敌人是否被击中
    /// 参数2：敌人
    /// </summary>
    private void OnPracticeResultBack(bool isOnAttack, UIShootingPracticePracticeItem currenItem)
    {
        if (!isOnAttack)
        {
            //未被击中
            //将该敌人添加到存活敌人的列表中,同时将该图集添加到存活敌人的图集表中
            if (!CurrentPractices.Contains(currenItem))
            {
                CurrentPractices.Add(currenItem);
                CurrentPracticesSprites.Add(currenItem.CurrentSprite);
            }

            ++CurrentPracticeResultBackNum;//回调次数+1

            if (CurrentPracticeResultBackNum == CurrentPracticeMaxNumber * 2)
            {
                //每四次交换一次图片
                //此时为所有敌人的闭合状态
                //随机交换敌人图片
                RandomSortList(ref CurrentPracticesSprites);
                for (int i = 0; i < CurrentPractices.Count; ++i)
                {
                    CurrentPractices[i].Init(CurrentPracticesSprites[i], playerStaringTime);
                }
                CurrentPracticeResultBackNum = 0;
            }

        }
        else
        {
            //被击中了
            --CurrentPracticeMaxNumber;//还存活的敌人数量-1
            CurrentPractices.Remove(currenItem);//将之从存活列表中移除
            CurrentPracticesSprites.Remove(currenItem.CurrentSprite);//将之从存活列表中的图集移除
        }
    }


    /// <summary>
    /// 创建一支新箭
    /// </summary>
    private void CreateArrow()
    {
        //取箭
        GetArrowForArrows();
        //还原弓弦
        RefreshWampLeftRightLine();
        //搭箭
        CreateUIItem<UIShootingPracticeArrowItem>(WampCenter).Init(OnArrowResultBack, WampLeft, WampRight, WampCenter);
    }
    /// <summary>
    /// 箭的回调函数
    /// 参数1：箭是否击中物体，击中返回true，没有击中返回false
    /// 参数2：箭击中物体的图片精灵
    /// </summary>
    private void OnArrowResultBack(bool isHit, Sprite hitSprite = null)
    {
        //TODO
        if (!isHit)
        {
            //重新创建一支箭 - 没有打中
            CreateArrow();
        }
        else
        {
            //打中了
            if (hitSprite == CurrentWord.sprite)
            {
                //打中正确了的
                FlyStar(true, true).OnComplete += PlayGame;
            }
            else
            {
                //没有打中正确的 - 重新创建一支箭
                FlyStar(false, false).OnComplete += CreateArrow;
            }
        }
    }

    /// <summary>
    /// 重置弓的左右弦
    /// </summary>
    public void RefreshWampLeftRightLine()
    {
        if (WampLeftRefreshLocalPositon == Vector3.zero)
        {
            SetRefreshData();
        }
        WampLeft.localPosition = WampLeftRefreshLocalPositon;
        WampRight.localPosition = WampRightRefreshLocalPositon;
        WampLeft.sizeDelta = WampLeftRightRefreshSizeDelate;
        WampRight.sizeDelta = WampLeftRightRefreshSizeDelate;
        WampLeft.localRotation = Quaternion.identity;
        WampRight.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// 从箭筒中拿一支箭
    /// </summary>
    private void GetArrowForArrows()
    {
        for (int i = 0; i < arrows.Count; ++i)
        {
            if (arrows[i].activeSelf)
            {
                arrows[i].SetActive(false);
                if (i == arrows.Count - 1)
                {
                    //TODO 此时已经拿到最后一支箭了，因此1s后重置所有的箭
                    Invoke("RefrshArrows", 1f);
                }
                return;
            }
        }
    }
    /// <summary>
    /// 初始化重置所有的箭
    /// </summary>
    private void RefrshArrows()
    {
        foreach (var v in arrows)
            v.SetActive(true);
    }

    /// <summary>
    /// 弓弦数据初始化
    /// </summary>
    private void SetRefreshData()
    {
        //初始化箭左右弦重置变量
        WampLeftRefreshLocalPositon = WampLeft.localPosition;
        WampRightRefreshLocalPositon = WampRight.localPosition;
        WampLeftRightRefreshSizeDelate = WampLeft.sizeDelta;
    }

    /// <summary>
    /// 随机打乱一个数组
    /// </summary>
    private static void RandomSortList<T>(ref List<T> list)
    {
        List<int> randomSeries = new List<int>();//随机序列 - 用来随机间单词打乱的一个序列
        GetRandomSeries(randomSeries, 0, list.Count);
        List<T> temp = new List<T>();
        for (int i = 0; i < list.Count; ++i) 
        {
            temp.Add(list[randomSeries[i]]);
        }
        list = temp;
        temp = null;
    }


    /// <summary>
    /// 得到一个随机序列保存在RandomSeries数组中
    /// </summary>
    private static void GetRandomSeries(List<int> randomSeries, int startIndex, int endIndex)
    {
        if (startIndex >= endIndex)
            return;

        int result = Random.Range(startIndex, endIndex);
        randomSeries.Add(result);

        GetRandomSeries(randomSeries, startIndex, result);
        GetRandomSeries(randomSeries, result + 1, endIndex);
    }
}
