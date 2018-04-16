using UnityEngine;
using TMPro;
public class UIFlyCarCtrl : MonoBehaviour
{
    private UIFlyCar Model;
    private float pos_y;
    private RectTransform rect;
    private TextMeshProUGUI word;

    private bool check = true;

    public void Init(UIFlyCar model,RectTransform r,string s)
    {
        Model = model;
        rect = r;
        word = GetComponent<TextMeshProUGUI>();
        word.text = s;
        //        pos_y = Camera.WorldToScreenPoint(transform.position);
        //        var current = UIManager.Instance.WorldCamera.WorldToScreenPoint(transform.position);
        //        RectTransformUtility.RectangleContainsScreenPoint(rect, current, UIManager.Instance.WorldCamera);
    }
//    public void OnCollisionEnter2D(Collision2D collision)
//    {
//        Model.Check(collision.gameObject.GetComponent<TextMeshProUGUI>().text);
//    }

    void FixedUpdate()
    {
        if (check)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rect,
                UIManager.Instance.WorldCamera.WorldToScreenPoint(transform.position), UIManager.Instance.WorldCamera))
            {
                check = false;
                Model.Check(word.text);
            }
        }
        
    }
}