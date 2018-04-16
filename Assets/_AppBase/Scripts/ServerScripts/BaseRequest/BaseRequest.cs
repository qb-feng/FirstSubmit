using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public abstract class BaseRequest : MonoBehaviour
{
    private bool m_isShowLoading = true;

    protected bool IsShowLoading { get { return m_isShowLoading; } set { m_isShowLoading = value; } }

    private int m_timeOut = 0;

    public int TimeOut
    {
        get
        {
            if (m_timeOut == 0)
                m_timeOut = 60;//ConfigVersionModel.GetVersion().timeOut;
            return m_timeOut;
        }
        set
        {
            m_timeOut = value;
        }
    }

    /// <summary>
    /// 请求处理结束回调
    /// </summary>
    public event Action OnPostEnd;

    protected UnityWebRequest m_request;
    protected bool m_isRequesting;

    public bool IsRequesting { get { return m_isRequesting; } }

    protected virtual void Awake()
    {
        OnPostEnd = HandleEnd;
    }

    protected bool CheckCanRequest(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.Log("Url Is Null! Please Check!");
            return false;
        }
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogWarning("No Internet!! Please Check!");
            UILoading.End();
            return false;
        }
        if (m_isRequesting)
        {
            Debug.Log("The Url : " + url + " Is Requesting, Please Wait To End!!");
            return false;
        }
        return true;
    }

    protected void Request(UnityWebRequest request)
    {
        m_request = request;
        DoRequest();
    }

    protected void DoRequest()
    {
        StartCoroutine("YieldRequest");
        StartCoroutine("YieldTimeOut");
        m_isRequesting = true;
    }

    protected IEnumerator YieldTimeOut()
    {
        float begin = Time.time;
        while (true)
        {
            yield return 0;
            if (m_timeOut != -1)
            {
                if (Time.time - begin > 1)
                {
                    if (m_isShowLoading)
                        UILoading.Show();
                }
                if (Time.time - begin > TimeOut)
                    break;
            }
        }
        HandleError(Localization.Get("server_request_timeout"));
        StopRequest();
    }

    protected IEnumerator YieldRequest()
    {
        yield return m_request.SendWebRequest();
        //yield return new WaitForSeconds(2);
        if (!string.IsNullOrEmpty(m_request.error) || m_request.responseCode != 200)
        {
            HandleError(string.Format("error : {0}, responseCode : {1}", m_request.error, m_request.responseCode));
        }
        else
        {
            HandleSucc(m_request);
        }
        StopRequest();
    }

    protected void StopRequest()
    {
        m_isRequesting = false;
        StopAllCoroutines();
        if (m_request != null)
        {
            m_request.Dispose();
            m_request = null;
        }
        if (OnPostEnd != null)
            OnPostEnd();
        OnPostEnd = null;
        UILoading.Close();
        Debug.Log("Destroy Request");
        DestroyImmediate(this);
    }

    /// <summary>
    /// 请求成功
    /// </summary>
    /// <param name="request"></param>
    protected abstract void HandleSucc(UnityWebRequest request);

    /// <summary>
    /// 请求错误
    /// </summary>
    /// <param name="error"></param>
    protected virtual void HandleError(string error)
    {
        string log = "Response Url: {0}, {1} : {2}";
        Debug.LogWarning(string.Format(log, m_request.url, Localization.Get("server_request_error_code"), error));
    }

    /// <summary>
    /// 请求处理结束
    /// </summary>
    protected abstract void HandleEnd();
}
