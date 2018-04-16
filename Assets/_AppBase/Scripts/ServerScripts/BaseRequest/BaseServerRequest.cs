using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class BaseServerRequest : BaseRequest
{
    /// <summary>
    /// 上传资源进度回调
    /// </summary>
    //public event Action<float> UploadProgressPercentEvent;

    void OnDisable()
    {
        //StopRequest();
    }

    protected void PostRequest(string url, string fieldName, string fileName, byte[] bytes, string d = "")
    {
        if (!CheckCanRequest(url))
            return;
        string log = "********Send Request URL Is :" + url;
        log += ", fieldName : " + fileName;
        log += ", fileName : " + fileName;
        log += ", bytes : " + bytes;
        Debug.Log(log);
        var model = new RequestDataModel()
        {
            id = Player.Instance.Id,
            t = Player.Instance.Token,
            d = d
        };
        string json = JsonUtility.ToJson(model);
        WWWForm form = new WWWForm();
        form.AddField("d", json);
        if (bytes != null && fieldName != null)
        {
            form.AddBinaryData(fieldName, bytes, fileName);
        }
        Request(UnityWebRequest.Post(url, form));
    }

    protected void GetRequest(string url)
    {
        Request(UnityWebRequest.Get(url));
    }

    protected void PostRequest(string url, string d = "")
    {
        string id = "";//boly58350ca60916443194ebfc79
        string t = "";
        //if (Player.Instance.IsUserModelUsable)
        {
            id = Player.Instance.Id;
            t = Player.Instance.Token;
        }
        PostRequest(url, id, t, d);
    }

    protected void PostRequest(string url, string id, string t, string d)
    {
        var model = new RequestDataModel()
        {
            id = id,
            t = t,
            d = d
        };
        string postData = JsonUtility.ToJson(model);
        Request(url, postData);
    }

    protected void Request(string url, string postData = "")
    {
        if (!CheckCanRequest(url))
            return;
        string log = "********Send Request URL Is :" + url + ">>>postData : " + postData;
        Debug.Log(log);
        WWWForm form = new WWWForm();
        form.AddField("d", postData);
        Request(UnityWebRequest.Post(url, form));
    }

    protected override void HandleSucc(UnityWebRequest request)
    {
        if (string.IsNullOrEmpty(request.downloadHandler.text))
        {
            HandleError("Content Empty");
        }
        else
        {
            ResponseDataModel dataModel = null;
            try
            {
                dataModel = JsonUtility.FromJson<ResponseDataModel>(request.downloadHandler.text);
            }
            catch (Exception)
            {
            }
            if (dataModel == null)
            {
                HandleError("Response Data Convert Failed!!!!!");
            }
            else
            {
                if (dataModel.s.Equals(0))
                {
                    Debug.Log("********Finish Request URL Is :" + m_request.url + " >>> TranData Is: " + request.downloadHandler.text);
                    if (dataModel.d == null)
                        dataModel.d = "";
                    HandleSucc(dataModel.d);
                    HandleMsg(dataModel.msg);
                }
                else
                {
                    HandleError(dataModel.s.ToString());
                }
            }
        }
    }

    /// <summary>
    /// 请求处理成功
    /// </summary>
    /// <param name="data"></param>
    protected abstract void HandleSucc(string data);

    protected virtual void HandleMsg(MsgModel msg)
    {
        ServerManager.HandleMsg(msg);
    }
}