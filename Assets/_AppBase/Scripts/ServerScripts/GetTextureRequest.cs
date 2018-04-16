using System;
using UnityEngine;
using UnityEngine.Networking;

public class GetTextureRequest : BaseRequest
{
    public event Action<Texture2D> OnSuccEvent;
    public event Action OnErrorEvent;

    public void Send(string url)
    {
        //string log = "********Send Request URL Is :" + url;
        //Debug.Log(log);
        IsShowLoading = false;
        Request(UnityWebRequestTexture.GetTexture(url));
    }

    protected override void HandleSucc(UnityWebRequest request)
    {
        //Debug.Log("********Finish Request URL Is :" + m_request.url);
        if (OnSuccEvent != null)
        {
            var tex = DownloadHandlerTexture.GetContent(request);
            Debug.Log("GetTextureRequest Succ!");
            OnSuccEvent(tex);
        }
    }

    protected override void HandleError(string error)
    {
        base.HandleError(error);
        if (OnErrorEvent != null)
            OnErrorEvent();
    }

    protected override void HandleEnd()
    {
        OnSuccEvent = null;
    }
}
