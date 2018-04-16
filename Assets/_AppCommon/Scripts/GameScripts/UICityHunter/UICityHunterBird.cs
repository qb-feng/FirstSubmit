using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class UICityHunterBird : UIBaseInit {

    private Transform BirdRope { get { return GetT("BirdRope"); } }
    private Image Bird { get { return Get<Image>("Bird"); } }
    private Image Rope0 { get { return Get<Image>("Rope0"); } }
    private Image Rope1 { get { return Get<Image>("Rope1"); } }
    private Image Rope2 { get { return Get<Image>("Rope2"); } }
 
    public TextMeshProUGUI WordText { get { return Get<TextMeshProUGUI>("WordText"); } }

    
    public void Init(string word,string ropeName,string birdName)
    {
        BirdRope.gameObject.SetActive(false);
        Rope0.sprite = GetS(ropeName);
        Rope1.sprite = GetS(ropeName);
        Rope2.sprite = GetS(ropeName);
        Bird.sprite = GetS(birdName);
        Bird.SetNativeSize();
        WordText.text = word;       
    }
    public void BeHit()
    {
       
        switch (this.transform.parent.name)
        {
            case("BirdPos0"):
                Bird.transform.DOMove(new Vector3(-8, 5, 0), 1f);              
                break;
            case ("BirdPos1"):
                Bird.transform.DOMoveY(5, 1);
                break;
            case ("BirdPos2"):
                Bird.transform.DOMove(new Vector3(8, 5, 0), 1f);
                break;
        }
        Bird.DOFade(0.2f, 1f);
        Bird.transform.DORotate(new Vector3(0, 0, 180), 0.7f);
        Bird.transform.DOScale(0.2f, 1f);
        UICityHunter.Instance.CurrentWord.PlaySound();        
        StartCoroutine(RopeDown());
        UITopBarStarFly fly = UICityHunter.Instance.FlyStar(true, true);
        UICityHunter.Instance.CloseInvoke();
        fly.OnComplete += () =>
        {
            UICityHunter.Instance.PlayGame();
        };
       
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.gameObject.GetComponent<UICityHunterBullet>().isShoot)
        {
            if (WordText.text == UICityHunter.Instance.CurrentWord.word)
            {
                other.transform.GetComponent<MeshRenderer>().enabled = false;
                Destroy(other.gameObject);
                BeHit();
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
        }              
    }
    IEnumerator RopeDown()
    {
        yield return new WaitForSeconds(0);
        BirdRope.gameObject.SetActive(true);
        BirdRope.DOMoveY(-5, 1f);
        BirdRope.DOScale(0.2f, 1f);       
        Rope0.DOFade(0, 1.2f);
        Rope1.DOFade(0, 1.2f);
        Rope2.DOFade(0, 1.2f);
       
       
    }
}

