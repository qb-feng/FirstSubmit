using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
/// <summary>
/// 射击板 
/// </summary>
public class UIShootingPracticePracticeItem : UIBaseInitItem
{
    /// <summary>
    /// 供外部数据访问的初始化方法
    /// 参数1：要显示的图片
    /// 参数2：玩家的准备时间 ，默认为2s
    /// 参数3：回调函数
    /// </summary>
    public void Init(Sprite currentSprite, float playStartingTime = 2f, Action<bool, UIShootingPracticePracticeItem> resultBackFunc = null)
    {
        CurrentSprite = currentSprite;
        playerStartingTime = playStartingTime;
        if (resultBackFunc != null)
            resultBackFunction = resultBackFunc;
        //数据初始化
        ShowImage.sprite = CurrentSprite;//显示图片
    }

    /// <summary>
    /// 供外部访问 - 当前单词被击中调用的方法
    /// 参数1：箭的世界坐标
    /// 参数2：箭的roation
    /// 返回值：返回当前被击中的单词的Sprite
    /// </summary>
    public Sprite OnAttack(Vector3 arrayWodPos,Quaternion arrayRoate)
    {
        IsOnAttack = true;//标记已经被攻击过
        resultBackFunction(IsOnAttack, this);//将被击中的结果返回给主管理者

        //显示当前单词被击中的图片
        Arrow.gameObject.SetActive(true);
        Arrow.position = arrayWodPos;
        Arrow.rotation = arrayRoate;
        //动画停止
        t1.Kill();
        t2.Kill();
        t3.Kill();
        //遮罩展开
        TopBox.fillAmount = 0;
        ButtomBox.fillAmount = 0;
        //图片碰撞器消失
        ImageBoxCollider.size = Vector2.zero;
        
        return CurrentSprite;
    }

    //组件
    private Image ShowImage { get { return Get<Image>("ShowImage"); } }//显示器
    private Image TopBox { get { return Get<Image>("TopBox"); } }//顶部遮罩
    private Image ButtomBox { get { return Get<Image>("ButtomBox"); } }//底部遮罩
    private Transform Arrow { get { return GetT("Arrow"); } }//被击中时的箭的样子

    private Tweener t1;
    private Tweener t2;
    private Tweener t3;

    private BoxCollider2D imageBoxCollider;//图片显示器的盒子碰撞器
    private BoxCollider2D ImageBoxCollider 
    {
        get 
        {
            if (imageBoxCollider == null)
                imageBoxCollider = ShowImage.GetComponent<BoxCollider2D>();
            return imageBoxCollider;
        }
    }


    //内部数据
    private float showIamgeBoxMaxHeight;//图片显示器的碰撞器的最大高度
    private float boxRunSpeedTime = 2f;//遮罩一来一回需要的时间（时间越短，速度越快）
    private float playerStartingTime = 0;//玩家的准备时间
    private bool IsOnAttack { get; set; }//是否被攻击过的
    private Action<bool, UIShootingPracticePracticeItem> resultBackFunction;//回调函数 - 返回当前敌人是否被击中与当前显示的图片

    //供外部数据访问的变量
    public Sprite CurrentSprite { get; private set; }//当前显示的图片


    public void Start() 
    {
        //数据初始化
        showIamgeBoxMaxHeight = ImageBoxCollider.size.y;
        TopBox.fillAmount = 0;
        ButtomBox.fillAmount = 0;
        Arrow.gameObject.SetActive(false);
        IsOnAttack = false;
        //给玩家2s的准备时间
        Invoke("StartRun", playerStartingTime);    
    }

    private void StartRun() 
    {
        //遮罩开始来回运动
        t1 = DOTween.To(() => TopBox.fillAmount, x => TopBox.fillAmount = x, 1, boxRunSpeedTime).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
        t1.onStepComplete = () =>
        {
            resultBackFunction(IsOnAttack,this);//返回当前遮罩的运动
        };
       t2 = DOTween.To(() => ButtomBox.fillAmount, x => ButtomBox.fillAmount = x, 1, boxRunSpeedTime).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
        //图片的碰撞器高度大小也开始来回的运动
       t3 = DOTween.To(() => ImageBoxCollider.size.y, y => ImageBoxCollider.size = new Vector2(ImageBoxCollider.size.x, y), 0, boxRunSpeedTime).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);
    }



}
