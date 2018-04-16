using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;

public class UICandySeesawCandy : UIBaseInit {

    private string match;
    private string id;
    private RectTransform temp;
    private RectTransform temp1;

	void Start () {
        temp = (RectTransform)transform;
	}
	
	
	void Update () {
     
	}

   public void Init(string name,string Id)
    {
        match = name;
        id = Id;
    }
    void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.name == match)
        {          
            UITopBarStarFly fly = UICandySeesaw.Instance.FlyStar(id,true);
            fly.OnComplete += () =>
            {
                UICandySeesaw.Instance.PlayGame();
            };
        }
        else if (collider.name == "OutRegion")
        {
            Destroy(this.gameObject);
            CreateUIItem("UICandySeesawCandy", UICandySeesaw.Instance.CandyBox).AddComponent<UICandySeesawCandy>().Init(match, id);
            UICandySeesaw.Instance.BoardUp.eulerAngles = Vector3.zero;
            UICandySeesaw.Instance.BoardDown.eulerAngles = Vector3.zero;
            UICandySeesaw.Instance.BoardUpLeft.eulerAngles = Vector3.zero;
            UICandySeesaw.Instance.BoardUpRight.eulerAngles = Vector3.zero;
            UICandySeesaw.Instance.BoardDownLeft.eulerAngles = Vector3.zero;
            UICandySeesaw.Instance.BoardDownRight.eulerAngles = Vector3.zero;
            UICandySeesaw.Instance.NextWord.enabled = false;
           
        }
        else
        {
            UICandySeesaw.Instance.FlyStar(id, false);
            //fly.OnComplete += () =>
            //{
            //   UICandySeesaw.Instance.PlayGame();
            //};
        }
      
      
    }  


     void OnTriggerExit2D(Collider2D collider)
     {
    
         UICandySeesaw.Instance.NextWord.enabled = true;
         Destroy(this.gameObject);
      }

     void OnCollisionEnter2D(Collision2D collision)
     {
         if (collision.transform.name == "BoardDown")
         {          
             gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0.4f, 0.4f);
             gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;                   
         }
     }
     void OnCollisionStay2D(Collision2D collision)
     {
         temp1 = (RectTransform)collision.transform;
         if (temp1.eulerAngles==Vector3.zero && collision.transform.name=="BoardDown" && temp.eulerAngles.z <=5)
         {
             gameObject.GetComponent<Rigidbody2D>().velocity =Vector2.zero;                     
         }
    
     }

  
   
}
