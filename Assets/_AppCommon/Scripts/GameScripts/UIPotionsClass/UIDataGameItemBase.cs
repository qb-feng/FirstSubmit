using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 游戏模型Item的基类
/// </summary>
public abstract class UIDataGameItemBase : UIBaseInitItem
{
    //公有组件
    protected TextMeshProUGUI ShowText { get { return Get<TextMeshProUGUI>("ShowText"); } }//内容显示器
    protected Image ShowImage { get { return Get<Image>("ShowImage"); } }//图片显示器
    protected GameObject CurrenItemGo //当前Item
    {
        get
        {
            if (currenItemGo == null)
                currenItemGo = gameObject;
            return currenItemGo;
        }
    }
    protected Transform CurrentImtemTransfrom //当前//当前item的transfrom
    {
        get
        {
            if (currentImtemTransfrom == null)
                currentImtemTransfrom = transform;
            return currentImtemTransfrom;
        }
    }
    protected RectTransform CurrentItemRectTransform //当前//当前item的RectTransform
    {
        get
        {
            if (currentItemRectTransform == null)
                currentItemRectTransform = CurrentImtemTransfrom as RectTransform;
            return currentItemRectTransform;
        }
    }

    //变量
    protected string CurrentShowText;//当前显示的文字
    protected Sprite CurrentShowImage;//当前显示的图片
    private GameObject currenItemGo;
    private RectTransform currentItemRectTransform;//当前item的RectTransform
    private Transform currentImtemTransfrom;//当前item的transfrom



    /// <summary>
    /// 供外部访问的初始化方法 - 子类可重写
    /// 参数为可变参数 
    /// 参数1默认为stirng类型的要显示的文本
    /// </summary>
    /// <param name="data"></param>
    public virtual void Init(params object[] data)
    {
        CurrentShowText = data[0] as string;
    }

}

