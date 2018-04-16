using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

/// <summary>
/// 按键取票机的按键item
/// </summary>
public class UITackMachineNumberItem : UIBaseInitItem
{
    /// <summary>
    /// 供外部数据访问的初始化方法
    /// 参数1：当前按键的数字
    /// 参数2：当前按键的回调函数
    /// </summary>
    public void Init(int currentNumber, Action<int> resultBackFunc)
    {
        CurrenNumber = currentNumber;
        ResultBackFunc = resultBackFunc;

        //item初始化
        NumberText.SetText(CurrenNumber.ToString());//显示数字
        //绑定单击事件
        UGUIEventListener.Get(CurrentGamoObject).onPointerClick = (cliclk) =>
        {
            if (!ClickOpenOrClose)
                return;

            if (isOnClick)
                return;
            isOnClick = true;
            CurrentGamoObject.transform.DOScale(clickScaleNumber, clickScaleTime).SetLoops(2, LoopType.Yoyo).onComplete = () =>
            {
                ResultBackFunc(CurrenNumber);
                isOnClick = false;
            };
        };
    }

    //组件
    private GameObject CurrentGamoObject { get { return Get("UITackMachineNumberItem(Clone)"); } }//当前item物体
    private TextMeshProUGUI NumberText { get { return Get<TextMeshProUGUI>("NumberText"); } }//数字显示器

    //变量
    private int CurrenNumber = -1;//当前表示的数字
    private Action<int> ResultBackFunc;//按键的回调函数
    private bool isOnClick = false;//是否处于单击状态
    private float clickScaleNumber = 0.88f;//按键按下去时的sclae
    private float clickScaleTime = 0.1f;//按下去需要的时间

    private bool ClickOpenOrClose = false;//单击事件开启或者关闭
    /// <summary>
    /// 供外部访问，开启或者关闭按钮的单击事件
    /// </summary>
    /// <param name="open"></param>
    public void SetClickStaues(bool open = true) 
    {
        ClickOpenOrClose = open;
    }
}
