using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewNestMove : MonoBehaviour
{
    public enum DragDirection
    {
        Horizontal,
        Vertical,
    }

    private ScrollRect m_scrollView;
    private ScrollRect m_parentScrollView;
    private DragDirection m_dragDirection;
    private bool m_horizontalDrag;
    private bool m_verticalDrag;

    void Start()
    {
        m_scrollView = GetComponent<ScrollRect>();
        if (m_scrollView.horizontal)
            m_dragDirection = DragDirection.Horizontal;
        if (m_scrollView.vertical)
            m_dragDirection = DragDirection.Vertical;
        UGUIEventListener.Get(m_scrollView).onInitializePotentialDrag = OnInitializePotentialDrag;
        UGUIEventListener.Get(m_scrollView).onBeginDrag = OnScrollViewBeginDrag;
        UGUIEventListener.Get(m_scrollView).onDrag = OnScrollViewDrag;
        UGUIEventListener.Get(m_scrollView).onEndDrag = OnScrollEndDrag;
    }

    private void OnInitializePotentialDrag(PointerEventData data)
    {
        var parents = GetComponentsInParent<ScrollRect>();
        foreach (var item in parents)
        {
            if (!item.Equals(m_scrollView))
                m_parentScrollView = item;

        }
        ExecuteEvents.Execute(m_parentScrollView.gameObject, data, ExecuteEvents.initializePotentialDrag);
    }

    private void OnScrollViewBeginDrag(PointerEventData data)
    {
        switch (m_dragDirection)
        {
            case DragDirection.Horizontal:
                if (Mathf.Abs(data.delta.x) < Mathf.Abs(data.delta.y))
                {
                    m_verticalDrag = true;
                    m_scrollView.horizontal = false;
                    ExecuteEvents.Execute(m_parentScrollView.gameObject, data, ExecuteEvents.beginDragHandler);
                }
                break;
            case DragDirection.Vertical:
                if (Mathf.Abs(data.delta.x) >= Mathf.Abs(data.delta.y))
                {
                    m_horizontalDrag = true;
                    m_scrollView.vertical = false;
                    ExecuteEvents.Execute(m_parentScrollView.gameObject, data, ExecuteEvents.beginDragHandler);
                }
                break;
        }
    }

    private void OnScrollViewDrag(PointerEventData data)
    {
        switch (m_dragDirection)
        {
            case DragDirection.Horizontal:
                if (m_verticalDrag)
                {
                    ExecuteEvents.Execute(m_parentScrollView.gameObject, data, ExecuteEvents.dragHandler);
                }
                break;
            case DragDirection.Vertical:
                if (m_horizontalDrag)
                {
                    ExecuteEvents.Execute(m_parentScrollView.gameObject, data, ExecuteEvents.dragHandler);
                }
                break;
        }

    }

    private void OnScrollEndDrag(PointerEventData data)
    {
        switch (m_dragDirection)
        {
            case DragDirection.Horizontal:
                if (m_verticalDrag)
                {
                    m_verticalDrag = false;
                    m_scrollView.horizontal = true;
                    ExecuteEvents.Execute(m_parentScrollView.gameObject, data, ExecuteEvents.endDragHandler);
                }
                break;
            case DragDirection.Vertical:
                if (m_horizontalDrag)
                {
                    m_horizontalDrag = false;
                    m_scrollView.vertical = true;
                    ExecuteEvents.Execute(m_parentScrollView.gameObject, data, ExecuteEvents.endDragHandler);
                }
                break;
        }

    }
}
