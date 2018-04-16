using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnPointerUp : MonoBehaviour, IPointerUpHandler
{
    public Action<PointerEventData> onPointerUp;

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onPointerUp != null) onPointerUp(eventData);
    }
}
