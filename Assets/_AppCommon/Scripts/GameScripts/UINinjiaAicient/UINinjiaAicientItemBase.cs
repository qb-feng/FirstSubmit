using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

/// <summary>
/// 忍者无敌中要切的西瓜Item的基类
/// </summary>
public class UINinjiaAicientItemBase : UIBaseInitItem
{
    //组件
    private Transform currentItem;//当前Item
    protected Transform CurrentItem
    {
        get
        {
            if (currentItem == null)
                currentItem = transform;
            return currentItem;
        }
    }
    protected GameObject Food { get { return Get("Food"); } }//主背景
    protected TextMeshProUGUI ShowText { get { return Get<TextMeshProUGUI>("ShowText"); } }//当前单词的显示器
    protected GameObject AttackBg { get { return Get("AttackBg"); } }//被攻击时要显示出来的物体
    protected RectTransform Top { get { return GetR("Top"); } }//被攻击时分割出来的上半部分
    protected RectTransform Buttom { get { return GetR("Buttom"); } }//被攻击时分割出来的下半部分

    //参数
    protected bool IsBoombItem = false;//当前item是否是炸弹
    protected string CurrentShowText;//当前显示的内容
    protected float DoLocalMaxY = 1050f;//当前item能飞到的最大高度
    protected float DoLocalMinY = -50f;//当前item飞走的最小高度
    protected float DoLocalOutSpeed = 1f;//竹子掉下去的速度
    protected float DoLocalSpeed = 0f;//当前item飞行的速度（也就是一个来回的时间）
    protected Action<string> ResultBackFunc;//竹子item的回调函数
    protected Vector3 FoodRefreshLocalPositon = new Vector3(0, 0, 0);//item初始化时是本地坐标
    protected bool CurrentItemIsAttack = false;//当前item是否被攻击过了
    protected Vector3 CurrentItemOnAttackRunDis = new Vector3(10f, 10f, 0);//竹子被攻击时移动的距离

    private void Awake()
    {
        Invoke("SetRefreshData", 1f);
    }
    private void SetRefreshData()
    {
        FoodRefreshLocalPositon.x = CurrentItem.localPosition.x;
    }
    /// <summary>
    /// 供外部调用的初始化函数
    /// 参数1：是否是炸弹
    /// 参数2：不是竹子时的食物上显示的单词
    /// 参数3：item飞行的速度（越大越慢）
    /// </summary>
    public UINinjiaAicientItemBase Init(bool isBoombItem, string showText, float flySpeed)
    {

        IsBoombItem = isBoombItem;  
        CurrentShowText = showText;
        DoLocalSpeed = flySpeed * 1.1f;
        DoLocalMaxY = 1050 + UnityEngine.Random.Range(-50f, 400f);
        //坐标初始化
        CurrentItem.localPosition = FoodRefreshLocalPositon;
        //攻击次数初始化
        CurrentItemIsAttack = false;

        //根据是否是炸弹进行初始化
        if (!isBoombItem)
        {
            //不是炸弹时 - 即为普通竹子
            ShowText.SetText(CurrentShowText);
            Food.SetActive(true);//显示背景
            AttackBg.SetActive(false);//将被攻击状态的背景取消激活
        }

        //TODO 开启该item的飞上飞下功能
        positonY = CurrentItem.localPosition.y;
        CurrentItemPlayTween();
        return this;
    }

    float positonY;
    /// <summary>
    /// 当前item开始Tween运动动画 - 参数：是否向上运动，默认是上
    /// </summary>
    private void CurrentItemPlayTween(bool ToUp = true) 
    {
        if (ToUp)
        {
            //往上飞
            CurrentItem.DOLocalMoveY(DoLocalMaxY, DoLocalSpeed).SetLoops(1, LoopType.Incremental).SetEase(Ease.InOutCubic).onComplete += () =>
            {
                CurrentItemPlayTween(false);
            };
        }
        else 
        {
            //往下飞
            CurrentItem.DOLocalMoveY(positonY, DoLocalSpeed).SetLoops(1, LoopType.Incremental).SetEase(Ease.InCubic).onComplete += () =>
            {
                CurrentItemPlayTween();
            };
        }
    }

    /// <summary>
    /// 被攻击时调用的方法
    /// 参数1：被攻击后的回调函数
    /// 参数2：被攻击的角度
    /// </summary>
    public void OnAttack(Action<string> resultBackFunc = null, float attackDirX = 0, float attackDirY = 0)
    {
        if (CurrentItemIsAttack)//被攻击过时直接没反应
            return;

        Vector2 attackDir = new Vector2(attackDirX, attackDirY);//攻击剑光的方向
        CurrentItemIsAttack = true;
        ResultBackFunc = resultBackFunc;
        //关闭tween动画
        CurrentItem.DOKill();

        if (IsBoombItem)
        {
            //炸弹被攻击了 - 爆炸然后直接销毁自己
            CurrentItem.DOScale(2, 1f).onComplete = () =>
            {
                ResultBackFunc(null);//爆炸 - 销毁玩家的连击数
                Destroy(gameObject);
            };
        }
        else
        {
            //竹子被攻击 - 执行本身竹子被攻击的方法
            FoodOnAttack(attackDir);
            ResultBackFunc(CurrentShowText);//将攻击中的单词返回给竹子
        }
    }
    /// <summary>
    /// 竹子被攻击时执行的方法
    /// 参数1：竹子被攻击的角度
    /// </summary>
    /// <param name="attackAngle"></param>
    protected virtual void FoodOnAttack(Vector2 attackDir)
    {
        float attackAngle = 0;//攻击剑光需要调整的角度
        if (attackDir.x > 0)
        {
            attackAngle = Vector2.Angle(attackDir, Vector2.right);
            attackAngle = attackAngle * (attackDir.y > 0 ? 1 : -1);
        }
        else
        {
            attackAngle = Vector2.Angle(attackDir, Vector2.left);
            attackAngle = attackAngle * (attackDir.y > 0 ? -1 : 1);
        }

        Food.SetActive(false);//隐藏背景
        AttackBg.SetActive(true);//显示被攻击的状态
        //设置竹子的上部与下部的角度
        Top.rotation = Quaternion.Euler(0, 0, attackAngle);
        Buttom.rotation = Quaternion.Euler(0, 0, attackAngle);
        //Top.localPosition += CurrentItemOnAttackRunDis;
        //Top.localPosition -= CurrentItemOnAttackRunDis;

        //竹子往下掉 
        CurrentItem.DOLocalMoveY(DoLocalMinY, DoLocalOutSpeed).onComplete = () =>
        {
            //TODO 竹子掉下去后干啥？重新产生一个？
            Init(IsBoombItem, CurrentShowText, DoLocalSpeed);
        };
    }


    private void OnDestroy()
    {
        CurrentItem.DOKill();
    }

}
