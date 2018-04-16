using UnityEngine.Networking;
using System.IO;
using System;
using UnityEngine;

public class DownloadFileRequeset : BaseRequest
{
    public event Action<string> OnSuccEvent;
    public event Action<string> OnErrorEvent;

    private string m_saveFilePath;

    public void Send(string saveFilePath, string downloadUrl)
    {
        TimeOut = -1;
        m_saveFilePath = saveFilePath;
        Debug.Log("DownloadFileRequeset Begin, Url : " + downloadUrl);
        Request(UnityWebRequest.Get(downloadUrl));
    }

    protected override void HandleSucc(UnityWebRequest request)
    {
        if (!string.IsNullOrEmpty(m_request.error))
        {
            HandleError(m_request.error);
        }
        else
        {
            if (request.downloadHandler.data == null)
            {
                HandleError("Download Data Is Null!");
            }
            else
            {
                Debug.Log("DownloadFileRequeset Succ, SavePath : " + m_saveFilePath);
				string dir = Path.GetDirectoryName (m_saveFilePath);
				if(!Directory.Exists(dir))
				{
					Directory.CreateDirectory (dir);
				}
                File.WriteAllBytes(m_saveFilePath, request.downloadHandler.data);
                if (OnSuccEvent != null)
                    OnSuccEvent(m_saveFilePath);
            }
        }
    }

    protected override void HandleError(string error)
    {
        base.HandleError(error);
        if (OnErrorEvent != null)
            OnErrorEvent(m_saveFilePath);
    }

    protected override void HandleEnd()
    {
        OnSuccEvent = null;
    }
}
