using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UGUIEventListener : MonoBehaviour
{
    public Action<PointerEventData> onPointerEnter
    {
        get
        {
            return Get<UGUIEventOnPointerEnter>().onPointerEnter;
        }
        set
        {
            Get<UGUIEventOnPointerEnter>().onPointerEnter = value;
        }
    }

    public Action<PointerEventData> onPointerExit
    {
        get
        {
            return Get<UGUIEventOnPointerExit>().onPointerExit;
        }
        set
        {
            Get<UGUIEventOnPointerExit>().onPointerExit = value;
        }
    }

    public Action<PointerEventData> onPointerDown
    {
        get
        {
            return Get<UGUIEventOnPointerDown>().onPointerDown;
        }
        set
        {
            Get<UGUIEventOnPointerDown>().onPointerDown = value;
        }
    }

    public Action<PointerEventData> onPointerUp
    {
        get
        {
            return Get<UGUIEventOnPointerUp>().onPointerUp;
        }
        set
        {
            Get<UGUIEventOnPointerUp>().onPointerUp = value;
        }
    }

    public Action<PointerEventData> onPointerClick
    {
        get
        {
            return Get<UGUIEventOnPointerClick>().onPointerClick;
        }
        set
        {
            Get<UGUIEventOnPointerClick>().onPointerClick = value;
        }
    }

    public Action<PointerEventData> onInitializePotentialDrag
    {
        get
        {
            return Get<UGUIEventOnInitializePotentialDrag>().onInitializePotentialDrag;
        }
        set
        {
            Get<UGUIEventOnInitializePotentialDrag>().onInitializePotentialDrag = value;
        }
    }

    public Action<PointerEventData> onBeginDrag
    {
        get
        {
            return Get<UGUIEventOnBeginDrag>().onBeginDrag;
        }
        set
        {
            Get<UGUIEventOnBeginDrag>().onBeginDrag = value;
        }
    }

    public Action<PointerEventData> onDrag
    {
        get
        {
            return Get<UGUIEventOnDrag>().onDrag;
        }
        set
        {
            Get<UGUIEventOnDrag>().onDrag = value;
        }
    }

    public Action<PointerEventData> onEndDrag
    {
        get
        {
            return Get<UGUIEventOnEndDrag>().onEndDrag;
        }
        set
        {
            Get<UGUIEventOnEndDrag>().onEndDrag = value;
        }
    }

    public Action<PointerEventData> onDrop
    {
        get
        {
            return Get<UGUIEventOnDrop>().onDrop;
        }
        set
        {
            Get<UGUIEventOnDrop>().onDrop = value;
        }
    }

    public Action<PointerEventData> onScroll
    {
        get
        {
            return Get<UGUIEventOnScroll>().onScroll;
        }
        set
        {
            Get<UGUIEventOnScroll>().onScroll = value;
        }
    }

    public Action<BaseEventData> onUpdateSelected
    {
        get
        {
            return Get<UGUIEventOnUpdateSelected>().onUpdateSelected;
        }
        set
        {
            Get<UGUIEventOnUpdateSelected>().onUpdateSelected = value;
        }
    }

    public Action<BaseEventData> onSelect
    {
        get
        {
            return Get<UGUIEventOnSelect>().onSelect;
        }
        set
        {
            Get<UGUIEventOnSelect>().onSelect = value;
        }
    }

    public Action<BaseEventData> onDeselect
    {
        get
        {
            return Get<UGUIEventOnDeselect>().onDeselect;
        }
        set
        {
            Get<UGUIEventOnDeselect>().onDeselect = value;
        }
    }

    public Action<AxisEventData> onMove
    {
        get
        {
            return Get<UGUIEventOnMove>().onMove;
        }
        set
        {
            Get<UGUIEventOnMove>().onMove = value;
        }
    }

    public Action<BaseEventData> onSubmit
    {
        get
        {
            return Get<UGUIEventOnSubmit>().onSubmit;
        }
        set
        {
            Get<UGUIEventOnSubmit>().onSubmit = value;
        }
    }
    public Action<BaseEventData> onCancel
    {
        get
        {
            return Get<UGUIEventOnCancel>().onCancel;
        }
        set
        {
            Get<UGUIEventOnCancel>().onCancel = value;
        }
    }

    public T Get<T>() where T : Component
    {
        return Get<T>(gameObject);
    }

    public static T Get<T>(GameObject go) where T : Component
    {
        var cp = go.GetComponent<T>();
        if (cp == null) cp = go.AddComponent<T>();
        return cp;
    }

    public static UGUIEventListener Get(GameObject go)
    {
        return Get<UGUIEventListener>(go);
    }

    public static UGUIEventListener Get(Component c)
    {
        return Get(c.gameObject);
    }
}
