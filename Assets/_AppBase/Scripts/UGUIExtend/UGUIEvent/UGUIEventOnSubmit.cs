using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnSubmit : MonoBehaviour, ISubmitHandler
{
    public Action<BaseEventData> onSubmit;

    public void OnSubmit(BaseEventData eventData)
    {
        if (onSubmit != null) onSubmit(eventData);
    }
}
