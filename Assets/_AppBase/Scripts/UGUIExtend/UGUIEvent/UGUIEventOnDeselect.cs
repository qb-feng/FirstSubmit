using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnDeselect : MonoBehaviour, IDeselectHandler
{
    public Action<BaseEventData> onDeselect;

    public void OnDeselect(BaseEventData eventData)
    {
        if (onDeselect != null) onDeselect(eventData);
    }
}
