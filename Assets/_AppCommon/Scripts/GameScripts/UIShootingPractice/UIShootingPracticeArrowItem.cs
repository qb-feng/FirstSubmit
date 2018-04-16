using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 箭
/// </summary>
public class UIShootingPracticeArrowItem : UIBaseInitItem
{
    public static string CurrentModeName = "UIShootingPracticeArrowItem(Clone)";//当前箭的名字
    /// <summary>
    /// 供外部数据访问的初始化函数
    /// 参数1：箭射击出去的回调函数
    /// 参数2：弓的左弦
    /// 参数3：弓的右弦
    /// 参数4：弓的中心
    /// </summary>
    public void Init(Action<bool, Sprite> resultBackFunc, RectTransform leftLine, RectTransform rightLine, RectTransform center)
    {
        ResultBackFunction = resultBackFunc;
        LeftLine = leftLine;
        RightLine = rightLine;
        Center = center;
        centerWordPosition = Center.position;

        //箭尾的拖拽参数初始化
        UGUIEventListener.Get(ArrowTail).onBeginDrag += OnBeginDrag;//开始拖拽
        UGUIEventListener.Get(ArrowTail).onDrag += OnDrag;//拖拽中
        UGUIEventListener.Get(ArrowTail).onEndDrag += OnEndDrag;//拖拽结束
    }

    //变量
    private Action<bool, Sprite> ResultBackFunction;//回调函数
    private RectTransform Center;//弓中心
    private RectTransform LeftLine;//弓左弦
    private RectTransform RightLine;//弓右弦
    private Sprite CurrentAtkSprite = null;//当前箭射中的图片


    //组件
    private RectTransform CurrentMode { get { return GetR(CurrentModeName); } }//当前箭的模型
    private Rigidbody2D CurrentModeRigbdy { get { return Get<Rigidbody2D>(CurrentModeName); } }//当前箭的刚体
    private GameObject ArrowTail { get { return Get("ArrowTail"); } }//箭尾
    private RectTransform arrowTailTransform;
    private RectTransform ArrowTailTransform
    {
        get
        {
            if (arrowTailTransform == null)
                arrowTailTransform = ArrowTail.GetComponent<RectTransform>();
            return arrowTailTransform;
        }
    }

    //参数
    private Vector2 beginDragMouseWodPos;//鼠标在一开始拖拽箭尾时的位置
    private Vector2 centerWordPosition;//弓的中心的世界位置
    private Vector2 arrowRefreshLocaPos;//箭重置时的本地坐标
    private Vector2 arrowRefreshWodPos;//箭重置时的世界坐标
    private Vector2 dir;//当前箭的方向
    private float runForce = 0.05f;//当前箭的飞行的力大小
    private float RefreshDis = 1f;//箭射出去的无效距离
    private float leftRightCanDis = 0.5f;//左右两边的误差 - 让箭可以射出去
    private float destoryArrayTime = 3f;//销毁箭的时间
    private string TargetName = "ShowImage";//要射击的目标的名字

    void Start()
    {
        arrowRefreshLocaPos = CurrentMode.localPosition;
        arrowRefreshWodPos = CurrentMode.position;
       // CurrentModeRigbdy.angularDrag = 0;
        CurrentModeRigbdy.drag = 0;
        RefreshDis = Mathf.Abs(Mathf.Abs(arrowRefreshWodPos.y) - Mathf.Abs(UIShootingPractice.instance.GetButtomWorldY)) * 0.5f;


    }

    /// <summary>
    /// 箭拖拽开始
    /// </summary>
    private void OnBeginDrag(UnityEngine.EventSystems.PointerEventData t)
    {
        Debug.Log("开始拖拽！");
        //得到鼠标的初始位置
        beginDragMouseWodPos = UIManager.Instance.WorldCamera.ScreenToWorldPoint(Input.mousePosition);

    }
    /// <summary>
    /// 箭拖拽中
    /// </summary>
    private void OnDrag(UnityEngine.EventSystems.PointerEventData t)
    {
        //获得此时鼠标的位置
        Vector2 dragMousePos = UIManager.Instance.WorldCamera.ScreenToWorldPoint(Input.mousePosition);
        //分析该位置
        if (dragMousePos.x > RightLine.position.x + leftRightCanDis || dragMousePos.x < LeftLine.position.x - leftRightCanDis)//超出弓的范围 - 没效果
            return;
        if (dragMousePos.y >= beginDragMouseWodPos.y)//往上拉，没效果
            return;
        if (dragMousePos.y < UIShootingPractice.instance.GetButtomWorldY)//超出屏幕外了，没效果
            return;
        //得到当前箭的方向
        dir = centerWordPosition - dragMousePos;
        //得到当前箭和Y轴的夹角
        int leftOrRight = dragMousePos.x >= centerWordPosition.x ? 1 : -1;
        float angle = Vector2.Angle(dir, Vector2.up) * leftOrRight;

        //旋转箭的方向
        CurrentMode.localRotation = Quaternion.Euler(0, 0, angle);
        //设置箭的长度
        CurrentMode.position = dragMousePos;
        //TODO 设置两根弦
        SetWampLine();
    }

    /// <summary>
    /// 箭拖拽结束
    /// </summary>
    private void OnEndDrag(UnityEngine.EventSystems.PointerEventData t)
    {
        //获得此时鼠标的位置
        Vector2 dragMousePos = UIManager.Instance.WorldCamera.ScreenToWorldPoint(Input.mousePosition);
        if ((beginDragMouseWodPos.y - dragMousePos.y) < RefreshDis)
        {
            //TODO 距离无效 - 不能射出去，箭重置, 弦重置
            CurrentMode.localPosition = arrowRefreshLocaPos;
            CurrentMode.localRotation = Quaternion.identity;
            UIShootingPractice.instance.RefreshWampLeftRightLine();//重置弦
        }
        else
        {
            //达到射出去的有效距离 - 箭射出
            //TODO 
            CurrentModeRigbdy.AddForce(dir * runForce, ForceMode2D.Force);
            //消除所有拖拽事件
            UGUIEventListener.Get(ArrowTail).onBeginDrag -= OnBeginDrag;
            UGUIEventListener.Get(ArrowTail).onDrag -= OnDrag;
            UGUIEventListener.Get(ArrowTail).onEndDrag -= OnEndDrag;
            //弦重新回到原处
            UIShootingPractice.instance.RefreshWampLeftRightLine();//重置弦

            //n秒后自动销毁该箭
            Invoke("DestoryThis", destoryArrayTime);
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collider) 
    {
        Transform colliderTrasform = collider.transform;
        //TODO 
        if (colliderTrasform.name.Equals(TargetName)) 
        {
            CurrentModeRigbdy.drag = 100000;//箭停住
            CurrentAtkSprite = colliderTrasform.parent.parent.GetComponent<UIShootingPracticePracticeItem>().OnAttack(CurrentMode.position, CurrentMode.rotation);
            base.CancelInvoke("DestoryThis");
            DestoryThis();
        }
    }


    /// <summary>
    /// 设置两根弦
    /// </summary>
    private void SetWampLine()
    {
        float mainCanvasScale = UIManager.Instance.UIRootHorizontal.transform.localScale.x;
        //左弦
        Vector2 dir = LeftLine.position - ArrowTailTransform.position;
        float angle = Vector2.Angle(dir, Vector2.left);
        LeftLine.localRotation = Quaternion.Euler(0, 0, -angle);
        LeftLine.sizeDelta = new Vector2(dir.magnitude / mainCanvasScale, LeftLine.sizeDelta.y);
        //右弦
        dir = RightLine.position - ArrowTailTransform.position;
        angle = Vector2.Angle(dir, Vector2.right);
        RightLine.localRotation = Quaternion.Euler(0, 0, angle);
        RightLine.sizeDelta = new Vector2(dir.magnitude / mainCanvasScale, RightLine.sizeDelta.y);
    }

    /// <summary>
    /// 自动销毁函数
    /// </summary>
    private void DestoryThis()
    {
        if (CurrentAtkSprite == null)
        {
            ResultBackFunction(false, null);
        }
        else 
        {
            ResultBackFunction(true, CurrentAtkSprite);
        }
        Destroy(gameObject);
    }
}
