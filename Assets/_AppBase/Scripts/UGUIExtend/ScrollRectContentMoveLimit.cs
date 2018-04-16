using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRectOffset))]
public class ScrollRectContentMoveLimit : MonoBehaviour
{
    public float top;
    public float bottom;
    public float left;
    public float right;

    private Vector2? limitTop = null;
    private Vector2? limitBottom = null;
    private Vector2? limitLeft = null;
    private Vector2? limitRight = null;

    private ScrollRect m_scroll;
    private ScrollRectOffset m_offset;

    void Start()
    {
        m_scroll = GetComponentInParent<ScrollRect>();
        m_offset = GetComponentInParent<ScrollRectOffset>();
    }

    // Update is called once per frame
    void Update()
    {
        var offset = m_offset.Offset;
        Debug.Log("offset : " + offset);
        var rt = (RectTransform)transform;
        if (offset.y > top)
        {
            if (limitTop == null)
                limitTop = rt.anchoredPosition;
            else
            {
                rt.anchoredPosition = (Vector2)limitTop;
                m_scroll.velocity = Vector2.zero;
            }
        }
        if (offset.y < -bottom)
        {
            if (limitBottom == null)
                limitBottom = rt.anchoredPosition;
            else
            {
                rt.anchoredPosition = (Vector2)limitBottom;
                m_scroll.velocity = Vector2.zero;
            }
        }
        if (offset.x > right)
        {
            if (limitRight == null)
                limitRight = rt.anchoredPosition;
            else
            {
                rt.anchoredPosition = (Vector2)limitRight;
                m_scroll.velocity = Vector2.zero;
            }
        }
        if (offset.x < -left)
        {
            if (limitLeft == null)
                limitLeft = rt.anchoredPosition;
            else
            {
                rt.anchoredPosition = (Vector2)limitLeft;
                m_scroll.velocity = Vector2.zero;
            }
        }
    }
}
