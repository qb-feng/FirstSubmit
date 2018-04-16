using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using Spine.Unity;
public class UISuperChameleonWordNiko : UIBaseInit
{
    public TextMeshProUGUI Text { get { return Get<TextMeshProUGUI>("Text"); } }
    public RectTransform LadyBirdImage { get { return GetR("Ladybird"); } }

    public bool isClick = false;
    public bool isRight = false;
    public int answerIndex = -1;

 

    public RectTransform ladyBirdPositin;

    public Vector2 StopVec;

    public void Init(string t,int index,RectTransform parent,float speed)
    {
        Text.text = t;
        UGUIEventListener.Get(gameObject).onPointerClick = ClickText;
        answerIndex = index;
        ladyBirdPositin = parent;
        LadyBirdImage.GetComponent<SkeletonGraphic>().timeScale = speed;
        Text.color = new Color(0, 0, 0, 1);
     
    }   

    private void ClickText(PointerEventData data)
    {       
        if (!isClick)
        {           
            UISuperChameleon.Instance.SetUpItemText(Text.text,this);
            UISuperChameleon.Instance.SetTongue(ladyBirdPositin.name);
        }               
    }

    public void IsActive()
    {
        if (isRight)
            gameObject.SetActive(false);
    }

    public void IsFalseAnswer()
    {
        StartCoroutine(FalseAnswer());
    }
    IEnumerator FalseAnswer()
    {
        yield return new WaitForSeconds(0.6f);
        Vector2 tempVec = new Vector2(30, 0);
        
        DOTween.To(() => { return LadyBirdImage.anchoredPosition; }, v => { LadyBirdImage.anchoredPosition = v; }, tempVec, 1f).From();
    }
    public void IsTrueAnswer()
    {
        StartCoroutine(TrueAnswer(ladyBirdPositin));
    }
    IEnumerator TrueAnswer(RectTransform rt)
    {
        switch (ladyBirdPositin.name)
        {
            case "DownItem1":                
                StopVec = new Vector2(-258, -289);
                break;
            case "DownItem2":
                StopVec = new Vector2(-618, -288);
                break;
            case "DownItem3":
                StopVec = new Vector2(-982, -291);
                break;
            case "DownItem4":
                StopVec = new Vector2(-439,-68);
                break;
            case "DownItem5":
                StopVec = new Vector2(-798,-70);
                break;
            case "DownItem6":
                StopVec = new Vector2(-620, 152);
                break;
        }
        yield return new WaitForSeconds(0.6f);

        float t = Vector2.Distance(LadyBirdImage.anchoredPosition, StopVec);

        var temp = DOTween.To(() => { return LadyBirdImage.anchoredPosition; }, v => { LadyBirdImage.anchoredPosition = v; }, StopVec, t/2500f);
      
        yield return new WaitForSeconds(0.5f);
        temp.OnComplete(DestroyLadybird);
    }
    public void DestroyLadybird()
    {
        this.gameObject.SetActive(false);
    }
}
