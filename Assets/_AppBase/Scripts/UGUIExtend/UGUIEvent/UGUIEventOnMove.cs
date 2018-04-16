using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventOnMove : MonoBehaviour, IMoveHandler
{
    public Action<AxisEventData> onMove;

    public void OnMove(AxisEventData eventData)
    {
        if (onMove != null) onMove(eventData);
    }
}
