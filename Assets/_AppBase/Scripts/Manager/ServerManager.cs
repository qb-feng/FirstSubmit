using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public static event Action<MsgModel> OnHandleMsgEvent;

    public static T GetRequest<T>(GameObject go) where T : Component
    {
        T request = go.GetComponent<T>();
        if (request == null)
        {
            request = go.AddComponent<T>();
        }
        return request;
    }

    public static void HandleMsg(MsgModel msg)
    {
        if (msg != null)
        {
            if (OnHandleMsgEvent != null)
                OnHandleMsgEvent(msg);
        }
    }
}
