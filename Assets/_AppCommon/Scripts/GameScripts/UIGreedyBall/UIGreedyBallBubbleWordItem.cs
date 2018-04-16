using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using DG.Tweening;
/// <summary>
/// 带文字的泡泡球
/// </summary>
public class UIGreedyBallBubbleWordItem : UIGreedyBallBubbleItem
{
    protected new const string NAME = "UIGreedyBallBubbleWordItem(Clone)";
    //组件
    private TextMeshProUGUI WordText { get { return Get<TextMeshProUGUI>("WordText"); } }//单词显示text
    private GameObject BubbleGo { get { return Get(NAME); } }//泡泡本体

    private CircleCollider2D bubbleCollider = null;//泡泡碰撞器
    private CircleCollider2D BubbleCollider
    {
        get
        {
            if (bubbleCollider == null)
            {
                bubbleCollider = BubbleGo.GetComponent<CircleCollider2D>();
            }
            return bubbleCollider;
        }
    }

    //重写的组件
    protected override RectTransform BubbleImage { get { return GetR(NAME); } }//泡泡的大小
    protected override Rigidbody2D BubbleRigbdy { get { return Get<Rigidbody2D>(NAME); } }//泡泡刚体

    //内部私有数据
    private Vector2 onBeginDragPosition;//保存泡泡被开始拖拽时的位置（世界坐标）
    private Vector2 onDragMousePositon;//泡泡被拖拽时鼠标的位置
    private float onEndDragToResetTime = 1f;//拖拽结束后泡泡返回到起初位置的时间
    private bool IsEat = false;//当前泡泡是否被吃掉了

    //供外部访问的数据
    public string CurrentWord { get; private set; }//当前泡泡显示的单词
    public Vector2 CurrentPositon { get { return BubbleImage.position; } }//当前泡泡的位置
    private Transform WordTextGo { get { return GetT("WordText"); } }//单词显示text
    public float CurrentBubbleRadius { get; private set; }//当前单词

    /// <summary>
    /// 外部初始化方法
    /// </summary>
    public override void Init(bool isWordBubble, float bubbleRadius = 0, string currentWord = null, Action<UIGreedyBallBubbleItem, DragStatue> resultBackFunc = null,string CurrentModeName = "UIGreedyBall")
    {
            base.Init(false,bubbleRadius,null,null,CurrentModeName);
            resultBackFunction = resultBackFunc;//注册回调方法
            CurrentWord = currentWord;
            //为泡泡添加拖拽事件
            UGUIEventListener.Get(BubbleGo).onBeginDrag = (drag) =>
            {
                resultBackFunction(this, DragStatue.DragBegin);
                onBeginDragPosition = BubbleImage.position;
            };
            UGUIEventListener.Get(BubbleGo).onDrag = BubbleOnDrag;
            UGUIEventListener.Get(BubbleGo).onEndDrag = BubbleOnEndDrag;

            //显示文字
            WordText.SetText(CurrentWord);

            //不可旋转
            BubbleRigbdy.angularDrag = 10000;
        

    }
    /// <summary>
    /// 泡泡的拖拽结束事件
    /// </summary>
    /// <param name="t"></param>
    private void BubbleOnEndDrag(UnityEngine.EventSystems.PointerEventData t)
    {
        resultBackFunction(this, DragStatue.DragEnd);
        if (IsEat)
            return;
        //计算出当前泡泡运动的方向
        Vector2 dir = (new Vector2(BubbleImage.position.x, BubbleImage.position.y) - onBeginDragPosition).normalized;

        //TODO 将泡泡Tween动画送回原处，然后施加一个力
        ChangeBubbleIsTrigger(false);//关闭触发器效果
        ChangeRigidbodyForce(true);//开启刚体效果

        BubbleImage.DOMove(onBeginDragPosition, onEndDragToResetTime).onComplete = () =>
        {
            AddforceRandom(true, dir.x, dir.y);//施加一个新的力
        };
    }

    /// <summary>
    /// 泡泡的拖拽事件
    /// </summary>
    /// <param name="t"></param>
    private void BubbleOnDrag(UnityEngine.EventSystems.PointerEventData t)
    {

        ChangeRigidbodyForce(false);//关闭刚体效果
        ChangeBubbleIsTrigger(true);//开启触发器效果
        onDragMousePositon = UIManager.Instance.WorldCamera.ScreenToWorldPoint(Input.mousePosition);
        BubbleImage.position = onDragMousePositon;

        //TODO 触发回调函数 - 表明当前单词正处于拖拽中
        resultBackFunction(this, DragStatue.DragStay);
    }

    /// <summary>
    /// 改变泡泡的作用力：true表示开启，false表示关闭 - 默认是关闭
    /// </summary>
    private void ChangeRigidbodyForce(bool statue = false)
    {
        if (!statue)
        {
            BubbleRigbdy.drag = 10000;
            BubbleRigbdy.angularDrag = 10000;
        }
        else
        {
            BubbleRigbdy.drag = 0;
           // BubbleRigbdy.angularDrag = 0;
        }
    }
    /// <summary>
    /// 修改泡泡是否开启触发器：true 开启，false，不开启，默认为开启
    /// </summary>
    /// <param name="statue"></param>
    private void ChangeBubbleIsTrigger(bool statue = true)
    {
        BubbleCollider.isTrigger = statue;
    }

    /// <summary>
    /// 吃掉泡泡调用该方法 - 返回要被吃掉的单词物体
    /// 参数1：嘴巴的位置
    /// 参数2：吃掉该泡泡要的时间
    /// </summary>
    public Transform EatBubble(Vector3 wordPosition,float eatTime = 1f) 
    {
        //将泡泡变小
        IsEat = true;
        BubbleRigbdy.drag = 10000;
        BubbleRigbdy.angularDrag = 10000;
        BubbleImage.DOMove(wordPosition, eatTime);
        BubbleImage.DOScale(0, eatTime).onComplete = () => { Destroy(gameObject); };//3s的时间将泡泡缩小然后销毁自己
        return WordTextGo;
    }
}
