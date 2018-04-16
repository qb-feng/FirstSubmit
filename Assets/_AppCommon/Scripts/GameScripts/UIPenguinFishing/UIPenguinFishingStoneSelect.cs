using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class UIPenguinFishingStoneSelect : UIBaseSelectCtrl
{

    private Image StoneImage { get { return Get<Image>("StoneImage"); } }
    public TextMeshProUGUI StoneText { get { return Get<TextMeshProUGUI>("StoneText"); } }

    private Image myImage;
    private Color standard;
    private Rigidbody2D myRigidbody2D;
    private string ansString;

    void Awake()
    {
        GetComponent<Image>().enabled = false ;
        standard = new Color(255, 255, 255, 255);
        StoneImage.color = standard;
        standard = new Color(0, 0, 0, 255);
        StoneText.color = standard;
        myRigidbody2D=this.gameObject.AddComponent<Rigidbody2D>();
        myRigidbody2D.gravityScale = 0;
        ansString = GetComponent<UIPenguinFishingStone>().answerStr;

    }
    protected override void Start()
    {
        StartCoroutine(createNeedWait());
       
    }

    protected override void RighMatchAction()
    {
       
    }

    protected override void WrongMatchAction()
    {
       
    }

    protected override bool GetMatch()
    {
        if (m_cacheCollider.transform.name == "UIPenguinFishingStone(Clone)")
        {
            if (m_cacheCollider.transform.GetComponent<UIPenguinFishingStone>().answerStr == ansString)
            {
                m_cacheCollider.transform.GetComponent<UIPenguinFishingStone>().isWork = true;
                GetComponent<UIPenguinFishingStone>().isRightShow = true;
                return true;
            }
            else
            {
                UIPenguinFishing.Instance.DecideStar(false);
                return false;               
            }
        }
        else 
        {      
            return false;           
        }       
    }
    IEnumerator createNeedWait()
    {
        yield return new WaitForSeconds(0);
        yield return new WaitForSeconds(0);
        base.Start();
        UGUIEventListener.Get(gameObject).onPointerUp += d =>
        {
            m_rigidbody2d.bodyType = RigidbodyType2D.Kinematic;
            m_rigidbody2d.simulated = true;

        };
    }
}
