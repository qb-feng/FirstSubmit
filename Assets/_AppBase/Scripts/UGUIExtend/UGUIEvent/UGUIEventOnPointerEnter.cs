using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnPointerEnter : MonoBehaviour, IPointerEnterHandler
{
    public Action<PointerEventData> onPointerEnter;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onPointerEnter != null) onPointerEnter(eventData);
    }
}
