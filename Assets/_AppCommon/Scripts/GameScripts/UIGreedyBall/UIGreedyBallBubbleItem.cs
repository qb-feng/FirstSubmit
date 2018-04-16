using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 拖拽状态
/// </summary>
public enum DragStatue
{
    DragBegin = 1,//拖拽开始
    DragStay,//拖拽持续中
    DragEnd,//拖拽结束
}

/// <summary>
/// 泡泡球Item - 只用来到处跑的那种泡泡球
/// </summary>
public class UIGreedyBallBubbleItem : UIBaseInitItem
{
    protected const string NAME = "UIGreedyBallBubbleItem(Clone)";//当前子物体的名字

    //换皮模式下增加的游戏背景
    private GameObject UIInfiniteBlankHoleImage { get { return Get("UIInfiniteBlankHoleImage"); } }//175无限黑洞增加的游戏背景

    //组件
    protected virtual RectTransform BubbleImage { get { return GetR(NAME); } }//泡泡的大小
    protected virtual Rigidbody2D BubbleRigbdy { get { return Get<Rigidbody2D>(NAME); } }//泡泡刚体

    private Vector2 bubbleForce = Vector2.zero;//当前泡泡前进的方向
    private float[] forceNum = new float[] { 1, -1, 0.5f, -0.5f, 0.25f, -0.25f, 0.75f, -0.75f };//各个力方向的选择
    private float forceMax = 0.005f;//力的大小的上限
    private float forceMin = 0.001f;//力的大小的下限
    private float stayTime = 0;//持续碰撞的时间

    //可被子类修改的数据
    protected Action<UIGreedyBallBubbleItem, DragStatue> resultBackFunction;
    /// <summary>
    /// 供外部数据访问的初始化方法
    /// 参数3：是否是初始化泡泡Item，默认为不是
    /// 参数1：泡泡的半径大小
    /// 参数2：单词泡泡的显示单词
    /// 参数4：单词的回调函数
    /// 参数5：当前item所属的游戏模型预制体的名字- 默认是属于 174贪吃阿米巴
    /// </summary>
    public virtual void Init(bool isWordBubble, float bubbleRadius = 0, string currentWord = null, Action<UIGreedyBallBubbleItem, DragStatue> resultBackFunc = null, string CurrentModeName = "UIGreedyBall")
    {
        BubbleImage.localPosition = Vector3.zero;

        BubbleImage.sizeDelta = new Vector2(bubbleRadius * 2, bubbleRadius * 2);//修改泡泡大小
        BubbleImage.GetComponent<CircleCollider2D>().radius = bubbleRadius;//修改泡泡碰撞器的大小

        switch (CurrentModeName) 
        {
            case "UIInfiniteBlankHole"://如果是无限黑洞模型，则将无限黑洞的背景打开
                UIInfiniteBlankHoleImage.SetActive(true);
                break;
            default:
                //默认 - 关闭除原型174贪吃阿米巴外的任何一个模型的背景
                UIInfiniteBlankHoleImage.SetActive(false);
                break;
        }
    }

    void Start()
    {
        AddforceRandom();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        BubbleRigbdy.AddForce(-bubbleForce * UnityEngine.Random.Range(forceMin, forceMax), ForceMode2D.Force);
        bubbleForce *= -1;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        stayTime += Time.deltaTime;
        if (stayTime >= 1)
        {
            stayTime = 0;
            ResetBubbleForce();
            BubbleRigbdy.AddForce(bubbleForce * UnityEngine.Random.Range(forceMin, forceMax), ForceMode2D.Force);
        }
    }

    /// <summary>
    /// 重置力的方向
    /// </summary>
    private void ResetBubbleForce()
    {
        //初始化时添加一个力
        bubbleForce.x = forceNum[UnityEngine.Random.Range(0, forceNum.Length)];
        bubbleForce.y = forceNum[UnityEngine.Random.Range(0, forceNum.Length)];
    }

    /// <summary>
    /// 随机给泡泡施加一个力
    /// </summary>
    public void AddforceRandom(bool isDircetion = false, float x = 0, float y = 0)
    {
        if (isDircetion)
        {
            bubbleForce.x = x;
            bubbleForce.y = y;
        }

        ResetBubbleForce();
        BubbleRigbdy.AddForce(bubbleForce * UnityEngine.Random.Range(forceMin, forceMax), ForceMode2D.Force);
        BubbleRigbdy.velocity = bubbleForce;
    }
}
