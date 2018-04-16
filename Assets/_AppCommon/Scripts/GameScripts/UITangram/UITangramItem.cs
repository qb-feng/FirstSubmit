using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
public class UITangramItem : UIBaseInitItem
{
    public TextMeshProUGUI TextWord { get { return Get<TextMeshProUGUI>("TextWord"); } }
    public Image sp;
    public void Init(string word)
    {
        sp = GetComponent<Image>();

        TextWord.text = word;
        int index = Random.Range(0, 2);
        if (index == 0)
            sp.sprite = GetS("UITangramBlue");
        else
            sp.sprite = GetS("UITangramRed");
    }
}