using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
public class UITangramImageItem : UIBaseInitItem
{
    private Image ImageWenhao { get { return Get<Image>("ImageWenhao"); } }
    public TextMeshProUGUI TextWord { get { return Get<TextMeshProUGUI>("TextWord"); } }

    public void SetText(string word,Sprite sp)
    {
        TextWord.text = word;
        ImageWenhao.gameObject.SetActive(false);
        GetComponent<Image>().sprite = sp;
        GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    void Start()
    {
        UGUIEventListener.Get(gameObject).onPointerClick = ButtonConfirm;
    }

    private void ButtonConfirm(UnityEngine.EventSystems.PointerEventData arg0)
    {
        TextWord.text = "";
        ImageWenhao.gameObject.SetActive(true);
        GetComponent<Image>().sprite = null;
        GetComponent<Image>().color = new Color(209f / 255, 209f / 255, 209f / 255, 1);
    }
}