using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnUpdateSelected : MonoBehaviour, IUpdateSelectedHandler
{
    public Action<BaseEventData> onUpdateSelected;

    public void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelected != null) onUpdateSelected(eventData);
    }
}
