using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonOffset : MonoBehaviour
{
    public GameObject m_target = null;
    public Vector2 offset = new Vector2(0, -5f);
    private bool pressed = false;
    private bool enter = true;
    private Vector2 m_originalAnchoredPosition;
    private RectTransform rect;

    // Use this for initialization
    void Awake()
    {
        rect = (RectTransform)transform;
        m_originalAnchoredPosition = rect.anchoredPosition;
        m_target = m_target == null ? gameObject : m_target;
        UGUIEventListener.Get(m_target).onPointerDown = GoPointDown;
        UGUIEventListener.Get(m_target).onPointerUp = GoPointUp;
        UGUIEventListener.Get(m_target).onPointerEnter = GoPointEnter;
        UGUIEventListener.Get(m_target).onPointerExit = GoPointExit;
    }

    void OnEnable()
    {
        rect.anchoredPosition = m_originalAnchoredPosition;
    }

    private void GoPointDown(PointerEventData arg0)
    {
        pressed = true;
        if (enter)
            SetOffset(1);
    }

    private void GoPointUp(PointerEventData arg0)
    {
        pressed = false;
        if (enter)
            SetOffset(-1);
    }

    private void GoPointEnter(PointerEventData arg0)
    {
        enter = true;
        if (pressed)
            SetOffset(1);
    }

    private void GoPointExit(PointerEventData arg0)
    {
        enter = false;
        if (pressed)
            SetOffset(-1);
    }

    private void SetOffset(int plus)
    {
        RectTransform rt = (RectTransform)transform;
        rt.anchoredPosition += offset * plus;
    }
}
