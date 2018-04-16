using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnDrag : MonoBehaviour, IDragHandler
{
    public Action<PointerEventData> onDrag;

    public void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag(eventData);
    }
}
