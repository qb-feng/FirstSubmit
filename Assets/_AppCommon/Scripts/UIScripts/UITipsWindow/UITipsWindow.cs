using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

public class UITipsWindow : UIBaseInit, IUIPopup, IUIRefresh
{
    #region Field
    private RectTransform Window { get { return GetR("ImageWindow"); } }
    /// <summary>
    /// 取消按钮
    /// </summary>
    private GameObject BtnCancel { get { return Get("ImageCancel"); } }
    /// <summary>
    /// 确认按钮
    /// </summary>
    private GameObject BtnOK { get { return Get("ImageConfirm"); } }
    /// <summary>
    /// 提示内容
    /// </summary>
    private TextMeshProUGUI TextContent { get { return Get<TextMeshProUGUI>("TextContent"); } }
    /// <summary>
    /// 题目
    /// </summary>
    private TMP_Text TextTitle { get { return Get<TMP_Text>("TextTitle"); } }
    //private Tweener m_tweenOpen;
    #endregion

    public void Refresh(params object[] data)
    {
        OpenPop();
        transform.SetAsLastSibling();
        UGUIEventListener.Get(BtnCancel).onPointerClick = BtnCloseClick;
        UGUIEventListener.Get(BtnOK).onPointerClick = BtnCloseClick;
        TextContent.text = (string)data[0];
        SetTips((TipsType)data[1], data[2] as Action<PointerEventData>, data[3] as Action<PointerEventData>);
        if (!string.IsNullOrEmpty((string)data[4]))
            TextTitle.text = (string)data[4];
        if (!string.IsNullOrEmpty((string)data[5]))
        {
            BtnOK.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (string)data[5];
        }
    }
    #region BtnEvent
    /// <summary>
    /// 关闭按钮响应事件
    /// </summary>
    /// <param name="go"></param>
    private void BtnCloseClick(PointerEventData eventData)
    {
        DestroyImmediate(gameObject);
    }

    private void SetTips(TipsType type, Action<PointerEventData> confirm, Action<PointerEventData> cancel)
    {
        switch (type)
        {
            case TipsType.AutoHide:
                BtnCancel.SetActive(false);
                BtnOK.SetActive(false);
                StartCoroutine(WaitToClose(confirm));
                break;
            case TipsType.OneButton:
                BtnCancel.SetActive(false);
                BtnOK.SetActive(true);
                BtnOK.transform.SetLP_x(0);
                UGUIEventListener.Get(BtnOK).onPointerClick += confirm;
                break;
            case TipsType.TwoButton:
                BtnCancel.SetActive(true);
                BtnOK.SetActive(true);
                UGUIEventListener.Get(BtnOK).onPointerClick += confirm;
                UGUIEventListener.Get(BtnCancel).onPointerClick += cancel;
                break;
        }
    }

    private IEnumerator WaitToClose(Action<PointerEventData> action)
    {
        yield return new WaitForSeconds(3.0f);
        if (action != null)
            action(null);
        DestroyImmediate(gameObject);
    }
    #endregion

    /// <summary>
    /// 打开弹窗动画
    /// </summary>
    /// <returns></returns>
    public Tweener OpenPop()
    {
        var tween = Window.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        return tween;
    }
}
