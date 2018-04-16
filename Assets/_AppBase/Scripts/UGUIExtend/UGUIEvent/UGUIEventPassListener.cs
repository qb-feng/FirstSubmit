using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UGUIEventPassListener : MonoBehaviour, IPointerClickHandler,
                                                    IPointerDownHandler,
                                                    IPointerUpHandler,
                                                    IPointerEnterHandler,
                                                    IPointerExitHandler,
                                                    IInitializePotentialDragHandler,
                                                    IBeginDragHandler,
                                                    IDragHandler,
                                                    IEndDragHandler,
                                                    IScrollHandler
{

    //把事件透下去
    public void EventPass<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        GameObject current = data.pointerCurrentRaycast.gameObject;
        for (int i = 0; i < results.Count; i++)
        {
            if (current != results[i].gameObject)
            {
                ExecuteEvents.Execute(results[i].gameObject, data, function);
                //RaycastAll后ugui会自己排序，如果你只想响应透下去的最近的一个响应，这里ExecuteEvents.Execute后直接break就行。
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventPass(eventData, ExecuteEvents.pointerClickHandler);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        EventPass(eventData, ExecuteEvents.pointerDownHandler);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EventPass(eventData, ExecuteEvents.pointerUpHandler);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventPass(eventData, ExecuteEvents.pointerEnterHandler);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventPass(eventData, ExecuteEvents.pointerExitHandler);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        EventPass(eventData, ExecuteEvents.initializePotentialDrag);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        EventPass(eventData, ExecuteEvents.beginDragHandler);
    }

    public void OnDrag(PointerEventData eventData)
    {
        EventPass(eventData, ExecuteEvents.dragHandler);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EventPass(eventData, ExecuteEvents.endDragHandler);
    }

    public void OnScroll(PointerEventData eventData)
    {
        EventPass(eventData, ExecuteEvents.scrollHandler);
    }
}
