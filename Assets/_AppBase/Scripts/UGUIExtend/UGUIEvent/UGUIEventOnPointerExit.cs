using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnPointerExit : MonoBehaviour, IPointerExitHandler
{
    public Action<PointerEventData> onPointerExit;

    public void OnPointerExit(PointerEventData eventData)
    {
        if (onPointerExit != null) onPointerExit(eventData);
    }
}
