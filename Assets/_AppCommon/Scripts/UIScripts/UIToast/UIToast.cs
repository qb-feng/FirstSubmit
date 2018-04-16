using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class UIToast : UIBaseInit
{
    private CanvasGroup Bg { get { return Get<CanvasGroup>(); } }
    public TMP_Text ToastText { get { return Get<TMP_Text>("ToastText"); } }

    void Awake()
    {
        transform.localScale = Vector3.one * 0.5f;
        transform.DOScale(1, 0.1f);
    }

    void Start()
    {
        transform.SetAsLastSibling();
        var tween = DOTween.To(() => { return Bg.alpha; }, v => { Bg.alpha = v; }, 0, 0.3f);
        tween.SetDelay(2.5f).SetDelay(2).onComplete = delegate { Destroy(gameObject); };
    }

    public static void Toast(string content)
    {
        var ui = CreateUI<UIToast>();
        ui.ToastText.text = content;
    }
}
