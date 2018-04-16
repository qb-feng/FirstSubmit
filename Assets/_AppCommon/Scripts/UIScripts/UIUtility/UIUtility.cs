using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum TipsType
{
    /// <summary>
    /// 提示弹窗出来后一会自动关闭
    /// </summary>
    AutoHide,
    /// <summary>
    /// 只有一个关闭按钮的提示弹窗
    /// </summary>
    OneButton,
    /// <summary>
    /// 有确认按钮和关闭按钮的提示弹窗
    /// </summary>
    TwoButton,
}

public enum SreenArea
{
    None,
    LeftTop,
    LeftBottom,
    RightTop,
    RightBottom,
}

public class UIUtility
{
    /// <summary>
    /// 调用提示弹窗方法
    /// </summary>
    /// <param name="tips"></param>
    public static void Tips(string tips, string title = null, string btnText = null)
    {
        Tips(tips, TipsType.AutoHide, title, btnText);
    }

    public static void Tips(string tips, TipsType type, string title = null, string btnText = null)
    {
        Tips(tips, type, null, title, btnText);
    }

    public static void Tips(string tips, TipsType type, Action<PointerEventData> confirm, string title = null, string btnText = null)
    {
        Tips(tips, type, confirm, null, title, btnText);
    }

    public static void Tips(string tips, TipsType type, Action<PointerEventData> confirm, Action<PointerEventData> cancel, string title = null, string btnText = null)
    {
        var newUI = UIBaseInit.CreateUI<UITipsWindow>();
        if (newUI != null)
        {
            newUI.gameObject.SetActive(true);
            var refresh = newUI.GetComponent<IUIRefresh>();
            if (refresh != null)
                refresh.Refresh(tips, type, confirm, cancel, title, btnText);
        }
        else
        {
            Debug.LogError("Next Popup UI : UITipsWindow Is Null! Please Check!");
        }
    }

    public static SreenArea AreaClick(Vector3 worldPoint)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPoint);
        if (screenPoint.x < Screen.width / 2f && screenPoint.y >= Screen.height / 2f)
            return SreenArea.LeftTop;
        if (screenPoint.x < Screen.width / 2f && screenPoint.y < Screen.height / 2f)
            return SreenArea.LeftBottom;
        if (screenPoint.x >= Screen.width / 2f && screenPoint.y >= Screen.height / 2f)
            return SreenArea.RightTop;
        if (screenPoint.x >= Screen.width / 2f && screenPoint.y < Screen.height / 2f)
            return SreenArea.RightBottom;
        return SreenArea.None;
    }

    /// <summary>
    /// 自适应创建知识点选项方法, 比如学习成就界面
    /// </summary>
    /// <typeparam name="T1">horizontal 的预制体名字</typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="horizontalParent">horizontalItem 的脚本名字</param>
    /// <param name="count">item的数量</param>
    /// <param name="maxLength">横向显示的范围</param>
    /// <param name="onItemInit">当创建后初始化脚本回调</param>
    public static void FlexibleCreateItem<T>(Transform horizontalParent, string horizontalName, int count, int maxLength, Action<T, int> onItemInit)
        where T : Component
    {
        GameObject horizontalOption = null;
        RectTransform rt = null;
        Action action = () =>
        {
            horizontalOption = UIBaseInit.CreateUIItem(horizontalName, horizontalParent);
            rt = horizontalOption.transform as RectTransform;
        };
        action();
        for (int i = 0; i < count; i++)
        {
            var wordItemCp = UIBaseInit.CreateUIItem<T>(rt);
            wordItemCp.name = i.ToString();
            wordItemCp.transform.SetSiblingIndex(i);
            if (onItemInit != null)
                onItemInit(wordItemCp, i);

            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

            if (rt.sizeDelta.x > maxLength)
            {
                action();
                wordItemCp.transform.SetParent(rt);
            }
        }
    }
}
