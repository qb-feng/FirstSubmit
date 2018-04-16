using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

/// <summary>
/// 游戏模型 - 开心速递
/// </summary>
public class UIHappyExpress : UIDataGameBase
{

    //组件
    private Transform TrashCan { get { return GetT("TrashCan"); } }//垃圾桶
    private Transform Point1 { get { return GetT("Point1"); } }//1号点
    private Transform Point2 { get { return GetT("Point2"); } }//2号点
    private Transform Point3 { get { return GetT("Point3"); } }//3号点
    private Transform[] points;
    private Transform StartPoint { get { return GetT("StartPoint"); } }//快递的入口起点
    private Transform EndPoint { get { return GetT("EndPoint"); } }//快递的出口终点
    private Transform PackePiont { get { return GetT("PackePiont"); } }//快递的打包点
    private Transform PackeTool { get { return GetT("PackeTool"); } }//快递的打包工具
    private GameObject PackeObject { get { return Get("PackeObject"); } }//快递打包工具下的打包好的包裹
    private Transform Bg { get { return GetT("Bg"); } }

    //变量
    private Action<Sprite, Transform, Action<bool, Sprite, Transform, Action>> resultBackFunc;//结果回调函数
    private Sprite selectTrueSprite;//选择正确的图集
    private Sprite slelectFalseSprite;//选择错误的图集
    private bool result;//当前的选择结果

    //数据
    private List<UIHappyExpressGoodItem> items = new List<UIHappyExpressGoodItem>();//当前子item
    private List<UIHappyExpressGoodItem> fasleItems = new List<UIHappyExpressGoodItem>();//当前不正确的items
    private float handleItemTime = 0.5f;//处理item数据的tween动画时间
    private float packeToolRunTime = 1f;//打包器上来的时间
    private float packeToolMaxLocalY = -100f;//打包器上来的最大高度
    private float packeToolMinLocalY = -560f;//打包器上来的最低高度
    private float packObjectWorldY = 0;//打包器的位置Y


    //游戏入口
    protected override void RefreshData()
    {
        base.RefreshData();
        items.Clear();
        fasleItems.Clear();

    }

    public override void Refresh()
    {
        base.IsHaveAuioPlayer = true;
        base.Refresh();
        //数据初始化
        points = new Transform[] { Point1, Point2, Point3 };
        resultBackFunc = ResultBackFunc;
        selectTrueSprite = GetS("UIHappyExpressOK");
        slelectFalseSprite = GetS("UIHappyExpressFalse");
        PackeObject.SetActive(false);

        //TODO 清空子节点
        foreach (var v in points)
            v.ClearAllChild();

        packObjectWorldY = PackeObject.transform.position.y;

    }
    public override void PlayGame()
    {
        base.PlayGame();
        if (IsGameEnd)
        {
            OnGameEnd();
            return;
        }
        //显示当前正确项文字
        ShowText.SetText(CurrentWord.word);
        PlayAudio(true);

        //获取数据
        var falseData = base.GetFalseConfigWordRandomList(CurrentWord);//得到混淆项

        //设置数据
        //得到当前的正确项下标
        int trueIndex = UnityEngine.Random.Range(0, points.Length);
        UIHappyExpressGoodItem item = null;
        for (int i = 0; i < points.Length; ++i)
        {
            item = CreateUIItem<UIHappyExpressGoodItem>(points[i]);
            if (trueIndex == i)
            {
                item.Init(CurrentWord.sprite, packObjectWorldY, resultBackFunc, StartPoint.position.x, EndPoint.position.x);
            }
            else
            {
                var v = falseData[UnityEngine.Random.Range(0, falseData.Count)];
                item.Init(v.sprite, packObjectWorldY, resultBackFunc, StartPoint.position.x, EndPoint.position.x);
                falseData.Remove(v);
            }
            items.Add(item);
        }
    }

    /// <summary>
    /// 结果回调函数
    /// 参数1：点击的item的图片
    /// 参数2：点击的item的transform
    /// 参数3：回调函数
    /// </summary>
    private void ResultBackFunc(Sprite itemSprite, Transform itemParentTransform, Action<bool, Sprite, Transform, Action> itemResultBack2)
    {
        result = CurrentWord.sprite == itemSprite;//判断结果
        float targetWordX = itemParentTransform.position.x;//点击的item的目标点的位置
        ChangeAllItemClickStatue(false);//关闭所有item的单击事件
        if (result)
        {
            //结果正确
            //打包点移动到对应的位置
            DOTween.To(() => PackePiont.position.x, x => PackePiont.position = new Vector3(x, PackePiont.position.y, 0), targetWordX, handleItemTime).onComplete = () =>
            {
                //打包器上来
                PackeTool.GetChild(0).gameObject.SetActive(false);
                PackeTool.DOLocalMoveY(packeToolMaxLocalY, packeToolRunTime).onComplete = () =>
                {
                    itemResultBack2(result, selectTrueSprite, PackePiont, ResultBackFunc2);
                };

            };
        }
        else
        {
            //结果错误
            //垃圾回收器移动到对应的位置
            DOTween.To(() => TrashCan.position.x, x => TrashCan.position = new Vector3(x, TrashCan.position.y, 0), targetWordX, handleItemTime).onComplete = () =>
            {
                itemResultBack2(result, slelectFalseSprite, PackePiont, ResultBackFunc2);
            };
        }
    }

    /// <summary>
    /// 结果的二次回调函数
    /// </summary>
    private void ResultBackFunc2()
    {
        if (result)
        {
            //结果正确 - 此时说明item已经打包好了并且放置在打包点了
            //显示已打包好的包裹
            PackeTool.GetChild(0).gameObject.SetActive(true);

            //把包裹带下去
            PackeTool.DOLocalMoveY(packeToolMinLocalY, packeToolRunTime).onComplete = () =>
           {
               //继续下一关
               FlyStar(true, true).OnComplete += () =>
               {
                   PlayGame();
               };

               //其余item移动出去
               for (int i = 0; i < items.Count; ++i)
               {
                   if (items[i] != null)
                   {
                       fasleItems.Add(items[i]);
                       items[i].GoOut(ResultBackFunc3);
                   }
               }
           };

        }
        else
        {
            //结果错误 - 此时说明item已经被垃圾回收期回收了
            FlyStar(false, true).OnComplete += () => { ChangeAllItemClickStatue(true); };//开启其余item的单击事件;
        }

    }
    /// <summary>
    /// 结果的三次回调函数
    /// </summary>
    private void ResultBackFunc3()
    {

    }

    /// <summary>
    /// 修改所有item的单击状态
    /// </summary>
    /// <param name="statue"></param>
    private void ChangeAllItemClickStatue(bool statue)
    {
        foreach (var v in items)
            v.ChangeClickStatue(statue);
    }
}
