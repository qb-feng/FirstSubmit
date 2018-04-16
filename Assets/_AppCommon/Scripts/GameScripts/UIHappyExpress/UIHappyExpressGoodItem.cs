using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

/// <summary>
/// 开心速递 - 包裹item
/// </summary>
public class UIHappyExpressGoodItem : UIDataGameItemBase
{
    //组件
    private Image Bg { get { return Get<Image>("Bg"); } }//背景


    //参数
    private float seleteFalseMaxLocalY = 1300f;//选择错误时当前物体上天的最大高度
    private float seleteTrueWorldPositionY;//选择正确时当前物体下降的位置的y值
    private float toTargetPositonTime = 3f;//到达目标点的时间
    private static float startPointWorldX = 0;//item起点的x世界坐标
    private static float endPointWorldX = 0;//item终点的x世界坐标
    private bool ClickOk = false;//是否可以单击 - 默认为false
    private float resultTweenerTime = 1f;//结果tween动画的处理时间

    //变量
    private Action<Sprite, Transform, Action<bool, Sprite, Transform, Action>> ResultBackFunc;//结果回调函数

    /// <summary>
    /// 参数0：要显示的图片
    /// 参数1：选择正确时当前物体下降的位置的y值
    /// 参数2：结果回调函数Action(Sprite, Transform,Action(bool, Sprite, Action))
    /// 参数3：item的起点坐标x - 世界系
    /// 参数4：item的终点坐标x - 世界系
    /// </summary>
    public override void Init(params object[] data)
    {
        CurrenItemGo.SetActive(false);

        CurrentShowImage = data[0] as Sprite;
        seleteTrueWorldPositionY = (float)data[1];
        ResultBackFunc = data[2] as Action<Sprite, Transform, Action<bool, Sprite, Transform, Action>>;
        if (startPointWorldX == 0 || endPointWorldX == 0)
        {
            startPointWorldX = (float)data[3];
            endPointWorldX = (float)data[4];
        }

        ///初始化

        //显示图片
        ShowImage.sprite = CurrentShowImage;

        //位置初始化
        CurrentImtemTransfrom.position = new Vector3(startPointWorldX + (CurrentImtemTransfrom.position.x - endPointWorldX), CurrentImtemTransfrom.position.y, 0);
        CurrenItemGo.SetActive(true);
        //开始移动
        Tweener toTargetTween = DOTween.To(() => CurrentImtemTransfrom.position.x, x => CurrentImtemTransfrom.position = new Vector3(x, CurrentImtemTransfrom.position.y), CurrentImtemTransfrom.parent.position.x, toTargetPositonTime);
        toTargetTween.onComplete = () =>
        {
            //到达目标点到添加单击事件
            ClickOk = true;
            UGUIEventListener.Get(ShowImage).onPointerClick += OnPointCick;
        };

    }

    /// <summary>
    /// item的单击事件
    /// </summary>
    private void OnPointCick(UnityEngine.EventSystems.PointerEventData t)
    {
        if (ClickOk)
        {
            ClickOk = false;
            ShowImage.gameObject.SetActive(false);
            ResultBackFunc(CurrentShowImage, CurrentImtemTransfrom.parent, StartResultTween);//得到单击结果并且返回结果处理方法与二次回调方法
        }
    }

    /// <summary>
    /// 结果处理方法与二次回调方法
    /// 参数1：主脚本返回的结果
    /// 参数2：主脚本返回的结果对应的背景图
    /// 参数3：主脚本返回的结果做完后的回调函数
    /// </summary>
    private void StartResultTween(bool result, Sprite resultSprite, Transform parent, Action resultBackFunc2)
    {
        //背景显示
        Bg.sprite = resultSprite;

        Tweener tweener = null;
        if (result)
        {
            //结果正确
            CurrentImtemTransfrom.SetParent(parent);
            tweener = DOTween.To(() => CurrentImtemTransfrom.position.y, y => CurrentImtemTransfrom.position = new Vector3(CurrentImtemTransfrom.position.x, y, 0), seleteTrueWorldPositionY, resultTweenerTime);
        }
        else
        {
            //结果错误
            tweener = DOTween.To(() => CurrentImtemTransfrom.localPosition.y, y => CurrentImtemTransfrom.localPosition = new Vector3(CurrentImtemTransfrom.localPosition.x, y, 0), seleteFalseMaxLocalY, resultTweenerTime);
        }
        //二次回调函数
        tweener.onComplete = () =>
        {
            Destroy(CurrenItemGo);//销毁自己
            resultBackFunc2();
        };
    }

    /// <summary>
    /// 修改单击事件的状态
    /// </summary>
    public void ChangeClickStatue(bool statue)
    {
        this.ClickOk = statue;
    }

    /// <summary>
    /// 子item移出去的方法
    /// </summary>
    /// <param name="resultBackFunc"></param>
    public void GoOut(Action resultBackFunc)
    {
        float targetX = endPointWorldX - Mathf.Abs(CurrentImtemTransfrom.position.x - startPointWorldX);
        CurrentImtemTransfrom.DOMove(new Vector3(targetX, CurrentImtemTransfrom.position.y, 0), toTargetPositonTime).onComplete = () =>
        {
            Destroy(CurrenItemGo);
        };
    }

}
