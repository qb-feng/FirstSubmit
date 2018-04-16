using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnInitializePotentialDrag : MonoBehaviour, IInitializePotentialDragHandler
{
    public Action<PointerEventData> onInitializePotentialDrag;

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (onInitializePotentialDrag != null) onInitializePotentialDrag(eventData);
    }
}
