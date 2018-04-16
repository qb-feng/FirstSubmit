using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnPointerDown : MonoBehaviour, IPointerDownHandler
{
    public Action<PointerEventData> onPointerDown;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onPointerDown != null) onPointerDown(eventData);
    }
}
