using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnScroll : MonoBehaviour, IScrollHandler
{
    public Action<PointerEventData> onScroll;

    public void OnScroll(PointerEventData eventData)
    {
        if (onScroll != null) onScroll(eventData);
    }
}
