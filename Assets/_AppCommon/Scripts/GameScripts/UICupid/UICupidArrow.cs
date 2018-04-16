using UnityEngine;
using System.Collections;
public class UICupidArrow : UIBaseInitItem
{
    private UICupid Model;
    public void Init(UICupid model)
    {
        Model = model;
        GetComponent<Rigidbody2D>().velocity = new Vector2(20, 0);

        transform.SetParent(Model.transform);
        
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
//        Debug.Log(Vector2.Angle(Vector2.right, GetComponent<Rigidbody2D>().velocity));
        GetComponent<Rigidbody2D>().rotation = -Vector2.Angle(Vector2.right, GetComponent<Rigidbody2D>().velocity);
    }
}