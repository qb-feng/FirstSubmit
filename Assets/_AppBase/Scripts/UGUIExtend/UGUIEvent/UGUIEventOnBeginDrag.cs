using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnBeginDrag : MonoBehaviour, IBeginDragHandler
{
    public Action<PointerEventData> onBeginDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null) onBeginDrag(eventData);
    }
}
