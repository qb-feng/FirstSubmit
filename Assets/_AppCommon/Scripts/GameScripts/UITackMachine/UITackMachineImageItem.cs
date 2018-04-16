using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 按键取票机要显示的图片Item
/// </summary>
public class UITackMachineImageItem : UIBaseInitItem
{

    /// <summary>
    /// 供外部数据的初始化函数
    /// 参数1：当前要显示的图片
    /// 参数2：当前要显示的图片的号码
    /// </summary>
    public void Init(Sprite currentSprite, int currentNumber)
    {
        //item初始化
        CurrentSprite.sprite = currentSprite;
        CurrentNumber.SetText(currentNumber.ToString());
    }

    //组件
    private Image CurrentSprite { get { return Get<Image>("Image"); } }//当前要显示的图片
    private TextMeshProUGUI CurrentNumber { get { return Get<TextMeshProUGUI>("NumberText"); } }//当前要显示的数字
}
