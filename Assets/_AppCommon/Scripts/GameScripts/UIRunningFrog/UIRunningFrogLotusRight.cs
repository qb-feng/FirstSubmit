using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class UIRunningFrogLotusRight : UIBaseInit
{

    public Transform Bubble { get { return GetT("Bubble"); } }
    public TextMeshProUGUI LotusText { get { return Get<TextMeshProUGUI>("LotusText"); } }
    public Transform RightFrog { get { return GetT("RightFrog"); } }
    public Transform Window { get { return GetT("Window"); } }
    public TextMeshProUGUI WindowText { get { return Get<TextMeshProUGUI>("WindowText"); } }
    public Image WindowImage { get { return Get<Image>("WindowImage"); } }


	void Awake () {      
        UGUIEventListener.Get(this.transform).onPointerClick = OnClickLotus;       
        Bubble.gameObject.SetActive(false);
        RightFrog.gameObject.SetActive(false);
        Window.gameObject.SetActive(false);
        WindowImage.gameObject.SetActive(false);
        WindowText.gameObject.SetActive(false);
	}
    private void OnClickLotus(PointerEventData data)
    {
        if (UIRunningFrog.Instance.FirstClick)
        {
            UIRunningFrog.Instance.FirstClick = false;           
            UIRunningFrog.Instance.MoveCamera(this.transform);
           
            if (this.transform == UIRunningFrog.Instance.originFrog.transform)
            {
                switch (UIRunningFrog.Instance.StartData.dataType)
                {
                    case DataType.Word:
                        StartWord();
                        break;
                    case DataType.Ask:
                        break;
                    case DataType.Pronunciation:
                        break;
                    case DataType.Say:
                        break;
                }                
            }
            else
            {
                StartCoroutine(waitLeft());
            }
        }
    }
    public void StartWord()
    {       
        UIRunningFrog.Instance.CurrentWord.PlaySound();
        WindowImage.sprite = UIRunningFrog.Instance.CurrentWord.sprite;
        //WindowText.text = UIRunningFrog.Instance.Current_Answer;              
        //Window.gameObject.SetActive(true);              
        //WindowText.DOFade(0, 2f);               
        Window.gameObject.SetActive(true);
        WindowImage.gameObject.SetActive(true);
        WindowImage.SetNativeSize();

        WindowImage.DOFade(0, 0);
        Window.GetComponent<Image>().DOFade(0, 0);

        Window.GetComponent<Image>().DOFade(1, 1).onKill=()=>{
            Window.GetComponent<Image>().DOFade(0, 1);
        };
        WindowImage.DOFade(1, 1).onKill = () =>
        {
            WindowImage.DOFade(0, 1).onKill = () =>
            {

                Window.gameObject.SetActive(false);
                UIRunningFrog.Instance.FirstClick = true;
            };
            
        };
    }
    IEnumerator waitLeft()
    {
        yield return new WaitForSeconds(3f);
        UIRunningFrog.Instance.FirstClick = true;
    }
	void Update () {
		
	}
    public void ShowFrog()
    {
        RightFrog.gameObject.SetActive(true);
    }
    public void InitText(string text)
    {    
        Bubble.gameObject.SetActive(true);
        LotusText.text = text;
    }
    public void HideText()
    {
        Bubble.gameObject.SetActive(false);
    }
    public void HideFrog()
    {
        RightFrog.gameObject.SetActive(false);
    }
}
