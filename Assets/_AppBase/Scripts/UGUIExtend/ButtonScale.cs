using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonScale : MonoBehaviour
{
    public Vector3 scale = new Vector3(0.85f, 0.85f, 0.85f);
    // Use this for initialization
    void Start()
    {
        UGUIEventListener.Get(gameObject).onPointerDown += GoDownClick;
        UGUIEventListener.Get(gameObject).onPointerUp += GoUpClick;
    }

    private void GoUpClick(PointerEventData obj)
    {
        transform.DOScale(Vector3.one, 0.1f);
    }

    private void GoDownClick(PointerEventData obj)
    {
        transform.DOScale(scale, 0.1f);
    }
}
