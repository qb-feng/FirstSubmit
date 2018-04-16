using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIRunningFrogLotusWrong : UIBaseInit {

    public Transform WrongFrog { get { return GetT("WrongFrog"); } }
	
	void Awake () {
        UGUIEventListener.Get(this.transform).onPointerClick = OnClickLotus;
        WrongFrog.GetComponent<Image>().enabled = false;
	}
    private void OnClickLotus(PointerEventData data)
    {
        if (UIRunningFrog.Instance.FirstClick)
        {           
            UIRunningFrog.Instance.MoveCamera(this.transform);
        }
    }

	void Update () {
		
	}
    public void ShowFrog()
    {
        WrongFrog.gameObject.SetActive(true);
        WrongFrog.GetComponent<Image>().enabled = true;      
    } 
}
