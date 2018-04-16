using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICatJumpingMove : MonoBehaviour {

    private Rigidbody2D myRigidbody;
    private Rigidbody2D tempCat;
    private BoxCollider2D myBoxCollider;
    private float Speed;
	void Awake () {
        myRigidbody = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
	}	
	
	void Update () {
        //(-5.8, -2.5, 0.0)down左边端点
        if (this.transform.position.x < -5.8f)
        {           
            myRigidbody.velocity = Vector2.zero;
            myRigidbody.velocity = Vector2.right * Speed;
            if (tempCat != null)
            {
                tempCat.velocity = Vector2.zero;
                tempCat.velocity = Vector2.right * Speed;
            }
          
           
        }
        if (this.transform.position.x > 5.8f)
        {          
            myRigidbody.velocity = Vector2.zero;
            myRigidbody.velocity = Vector2.left * Speed;
            if (tempCat != null)
            {
                tempCat.velocity = Vector2.zero;
                tempCat.velocity = Vector2.left * Speed;
            }
           
           
        }
       
	}
    public void Init(int direction,float speed)
    {
        Speed = speed;
        if(direction==1)
        {          
            myRigidbody.velocity = Vector2.left * speed;
        }
        else
        {          
            myRigidbody.velocity = Vector2.right * speed;
        }        
    }
    void OnCollisionEnter2D(Collision2D e)
    {
        tempCat = e.transform.GetComponent<Rigidbody2D>();       
    }
    void OnCollisionExit2D(Collision2D e)
    {
        tempCat = null;     
    }

    void OnTriggerEnter2D(Collider2D e)
    {
        
       
    }
    void OnTriggerStay2D(Collider2D e)
    {
        if (e.GetComponent<Rigidbody2D>().velocity.y < 0)
        {
            myBoxCollider.isTrigger = false;
        }
    }
    void OnTriggerExit2D(Collider2D e)
    {
       
    }
   
    

}
