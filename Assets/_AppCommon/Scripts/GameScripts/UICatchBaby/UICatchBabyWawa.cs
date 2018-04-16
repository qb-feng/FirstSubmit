using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class UICatchBabyWawa : MonoBehaviour
{
    private RectTransform rect;
    private UICatchBaby Model;
    //private float speed = 200;
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Init(UICatchBaby model)
    {
        Model = model;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.name)
        {
            case "ImageCatch":
                transform.SetParent(collision.transform.GetChild(0));
                rect.anchoredPosition = new Vector2(0, 0);
                Model.current_wawa = transform;
                Model.CatchToUp();
                break;
//            case "ImageUnder":
//                Model.ToUnder();
//                Model.catched_bool = false;
//                Model.ImageCatch.sprite = Model.GetS("UICatchBabyjiqi");
//                break;
            case "ImageExit":
                Model.catched_bool = false;
                Model.ToExit();
                Model.ImageCatch.sprite = Model.GetS("UICatchBabyjiqi");
                break;
        }

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.name)
        {
            case "ImageUnderItem":
                Model.ToUnder();
                Model.catched_bool = false;
                Model.ImageCatch.sprite = Model.GetS("UICatchBabyjiqi");
                break;
        }
    }
}