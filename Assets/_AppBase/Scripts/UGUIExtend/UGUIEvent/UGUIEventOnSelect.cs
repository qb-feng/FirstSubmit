using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnSelect : MonoBehaviour, ISelectHandler
{
    public Action<BaseEventData> onSelect;

    public void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(eventData);
    }
}
