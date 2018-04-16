using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIAntSoldierSelect : UIBaseInit {

    public string answerStr;
    public TextMeshProUGUI Text { get { return Get<TextMeshProUGUI>("Text"); } }

	void Start () {
        gameObject.AddComponent<UIAntSoldierSelectCtrl>().Init(UIAntSoldier.Instance.JudgeAnswer(answerStr));   
	}
	
    public void InitWord(string word )
    {
        answerStr = word;
        Text.text = word;
    }
}
