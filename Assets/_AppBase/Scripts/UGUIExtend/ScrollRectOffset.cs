using UnityEngine;
using UnityEngine.UI;

public class ScrollRectOffset : MonoBehaviour
{
    private ScrollRect scroll;
    private RectTransform m_Content;
    private RectTransform viewRect;
    private Bounds m_ContentBounds;
    private Bounds m_ViewBounds;
    private Vector2 offset;
    public Vector2 Offset { get { return offset; } }

    void Start()
    {
        scroll = GetComponentInParent<ScrollRect>();
        if (scroll == null)
            Debug.LogError("Can Not Find ScrollRect! Please Check!!");
        else
        {
            m_Content = scroll.content;
            viewRect = scroll.viewport;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("scroll.velocity : " + scroll.velocity);
        m_ContentBounds = GetBounds();
        m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
        offset = CalculateOffset(Vector2.zero);
        Debug.Log("offset : " + offset);
    }

    private readonly Vector3[] m_Corners = new Vector3[4];
    private Bounds GetBounds()
    {
        if (m_Content == null)
            return new Bounds();

        var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        var toLocal = viewRect.worldToLocalMatrix;
        m_Content.GetWorldCorners(m_Corners);
        for (int j = 0; j < 4; j++)
        {
            Vector3 v = toLocal.MultiplyPoint3x4(m_Corners[j]);
            vMin = Vector3.Min(v, vMin);
            vMax = Vector3.Max(v, vMax);
        }

        var bounds = new Bounds(vMin, Vector3.zero);
        bounds.Encapsulate(vMax);
        return bounds;
    }

    private Vector2 CalculateOffset(Vector2 delta)
    {
        Vector2 offset = Vector2.zero;
        if (scroll.movementType == UnityEngine.UI.ScrollRect.MovementType.Unrestricted)
            return offset;

        Vector2 min = m_ContentBounds.min;
        Vector2 max = m_ContentBounds.max;

        if (scroll.horizontal)
        {
            min.x += delta.x;
            max.x += delta.x;
            if (min.x > m_ViewBounds.min.x)
                offset.x = m_ViewBounds.min.x - min.x;
            else if (max.x < m_ViewBounds.max.x)
                offset.x = m_ViewBounds.max.x - max.x;
        }

        if (scroll.vertical)
        {
            min.y += delta.y;
            max.y += delta.y;
            if (max.y < m_ViewBounds.max.y)
                offset.y = m_ViewBounds.max.y - max.y;
            else if (min.y > m_ViewBounds.min.y)
                offset.y = m_ViewBounds.min.y - min.y;
        }

        return offset;
    }
}
