using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UICityHunterBullet : UIBaseInit
{

    public bool isShoot;
    public Vector2 endPos;
    private Vector2 startPos;
    // Use this for initialization
    void Awake()
    {
        isShoot = false;
        startPos = new Vector2(0, -320);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(startPos, endPos) < 120)
        {
            isShoot = false;
        }
        if (isShoot)
        {            
            transform.GetComponent<Rigidbody2D>().velocity = -(endPos - startPos) / 10;
        }
    }
    void OnBecameInvisible()
    {
        //Destroy(this.gameObject);
        
        if (UICityHunter.Instance.onRefreshBullet != null)
        {
            UICityHunter.Instance.onRefreshBullet();
        }
        //UICityHunter.Instance.CreateBullet();
    }
}

