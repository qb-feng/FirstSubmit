using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIHitMoleReek : UIBaseInit {
    //爆炸烟雾
    private Image ReekUpLeft { get { return Get<Image>("ReekUpLeft"); } }
    private Image ReekUpRight { get { return Get<Image>("ReekUpRight"); } }
    private Image ReekDownLeft { get { return Get<Image>("ReekDownLeft"); } }
    private Image ReekDownRight { get { return Get<Image>("ReekDownRight"); } }
    private Image ReekCenter { get { return Get<Image>("ReekCenter"); } }
    private Image Explode { get { return Get<Image>("Explode"); } }
	// Use this for initialization
	void Start () {
      
        StartCoroutine(ExplodeReek());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator ExplodeReek()
    {
        Explode.DOFade(0.2f, 0.8f);
        ReekUpLeft.DOFade(0.2f, 1.4f);
        ReekUpLeft.transform.DOMove(new Vector3(-5, 3, 0), 1.4f);
        ReekUpRight.DOFade(0.2f, 1.4f);
        ReekUpRight.transform.DOMove(new Vector3(5, 3, 0), 1.4f);
        ReekDownLeft.DOFade(0.2f, 1.4f);
        ReekDownLeft.transform.DOMove(new Vector3(-5, -3, 0), 1.4f);
        ReekDownRight.DOFade(0.2f, 1.4f);
        ReekDownRight.transform.DOMove(new Vector3(5, -3, 0), 1.4f);
        ReekCenter.DOFade(0.2f, 1.4f);
        yield return new WaitForSeconds(0.8f);
        Explode.gameObject.SetActive(false);        
        yield return new WaitForSeconds(0.4f);
        Destroy(this.gameObject);
    }
}
