using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITestShowLevelClassifyItem : UIBaseInit {

    public TextMeshProUGUI ClassifyText { get { return Get<TextMeshProUGUI>("ClassifyText"); } }

    private UITestShowLevelClassify mParent;

    private bool isSelect;
	// Use this for initialization
	void Start () 
    {
        UGUIEventListener.Get(gameObject).onPointerClick += OnClickThisToggle;
	}

    /// <summary>
    /// 根据是否选择添加进标签List以便进行检索
    /// </summary>
    /// <param name="t"></param>
    private void OnClickThisToggle(UnityEngine.EventSystems.PointerEventData t)
    {        
        isSelect=gameObject.GetComponent<Toggle>().isOn;
        //Debug.LogError("选择了" + ClassifyText.text + "选项" + "Toggle为" + isSelect);
        if (isSelect)
        {
            //Debug.LogError("添加");
            if (mParent.CurrentSelectToggleAdd != null)
            {
                mParent.CurrentSelectToggleAdd(ClassifyText.text+"_"+transform.tag);            
            }
        }
        else
        {
            //Debug.LogError("删除");
            if (mParent.CurrentSelectToggleDel != null)
            {
                mParent.CurrentSelectToggleDel(ClassifyText.text + "_" + transform.tag);               
            }
        }
        

    }

    public void InitClassify(string classifyText, string classifyTag, UITestShowLevelClassify tempParent)
    {
        ClassifyText.text = classifyText;
        transform.tag = classifyTag;
        mParent = tempParent;
        //TODO名字排序
        if (classifyText.StartsWith("单词"))
        {
            transform.name = "1";
        }
        else if (classifyText.StartsWith("对话"))
        {
            transform.name = "2";
        }
        else if (classifyText.StartsWith("单句"))
        {
            transform.name = "3";
        }
        else if (classifyText.StartsWith("词根"))
        {
            transform.name = "4";
        }
        else
        {
            transform.name = "5";
        }
        
    }
	
	
}
