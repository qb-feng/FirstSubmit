using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnPointerClick : MonoBehaviour, IPointerClickHandler
{
    public Action<PointerEventData> onPointerClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onPointerClick != null) onPointerClick(eventData);
    }
}
