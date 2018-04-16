using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnEndDrag : MonoBehaviour, IEndDragHandler
{
    public Action<PointerEventData> onEndDrag;

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null) onEndDrag(eventData);
    }
}
