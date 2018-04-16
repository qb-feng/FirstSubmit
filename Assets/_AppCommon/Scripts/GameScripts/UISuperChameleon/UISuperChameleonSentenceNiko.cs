using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISuperChameleonSentenceNiko : UIBaseInit
{

    public TextMeshProUGUI Text { get { return Get<TextMeshProUGUI>("Text"); } }

    public void Init(string t)
    {
        Text.text = t;
    }

    public void SetRightText(string t)
    {
        Text.text = t;
    }
}
