using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnCancel : MonoBehaviour, ICancelHandler
{
    public Action<BaseEventData> onCancel;

    public void OnCancel(BaseEventData eventData)
    {
        if (onCancel != null) onCancel(eventData);
    }
}
