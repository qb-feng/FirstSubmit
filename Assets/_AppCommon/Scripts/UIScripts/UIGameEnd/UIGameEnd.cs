using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class UIGameEnd : UIBaseInit, IUIPopup, IUIRefresh
{
    private List<string> m_finishTips = new List<string> { "Perfect", "Great", "Good" };
    private GameObject BtnFinish { get { return Get("ImageFinish"); } }
    private GameObject Center { get { return Get("ImageCenter"); } }
    private TMP_Text StarNum { get { return Get<TMP_Text>("StarNum"); } }

    // Use this for initialization
    void Start()
    {
        //UGUIEventListener.Get(BtnFinish).onPointerClick = BtnFinishClick;
        Center.transform.DOScale(1, 1).SetEase(Ease.OutBack).SetDelay(0.2f);
        AudioManager.Instance.Play("SecondTimeGameEnd");
    }

    private void BtnFinishClick(PointerEventData t)
    {
        UIManager.Instance.Close();
        //UIManager.Instance.Back();
    }

    public void Refresh(params object[] data)
    {
        int num = (int)data[0];
        StarNum.text = num.ToString() + " Star";
        var action = (System.Action<PointerEventData>)data[1];
        UGUIEventListener.Get(BtnFinish).onPointerClick += action;
    }
}
