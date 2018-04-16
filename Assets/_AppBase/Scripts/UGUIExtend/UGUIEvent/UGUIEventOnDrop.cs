using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnDrop : MonoBehaviour, IDropHandler
{
    public Action<PointerEventData> onDrop;

    public void OnDrop(PointerEventData eventData)
    {
        if (onDrop != null) onDrop(eventData);
    }
}
