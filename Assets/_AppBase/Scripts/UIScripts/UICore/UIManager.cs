using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIManager : UIBaseInit
{
    private GameObject m_UIRoot;

    public GameObject UIRoot { get { return m_UIRoot; } }

    private GameObject m_UIRootHorizontal;

    public GameObject UIRootHorizontal { get { return m_UIRootHorizontal; } }

    private GameObject m_UIRootVertical;

    public GameObject UIRootVertical { get { return m_UIRootVertical; } }

    public Camera WorldCamera { get { return UIRoot.GetComponent<Canvas>().worldCamera; } }

    /// <summary>
    /// 观察者列表
    /// </summary>
    private static List<IOnUIChange> m_observersUIChange = new List<IOnUIChange>();
    private static List<IUIOnRefresh> m_observersUIRefresh = new List<IUIOnRefresh>();
    /// <summary>
    /// UI创建工厂
    /// </summary>
    private IUIFactory m_uiFactory = new UIBaseFactory();
    /// <summary>
    /// 当前状态UI
    /// </summary>
    private Type m_currentStatusUI = typeof(UIManager);

    public Type CurrentStatusUI { get { return m_currentStatusUI; } }

    private Type m_previousStatusUI = typeof(UIManager);

    public Type PreviousStatusUI { get { return m_previousStatusUI; } }

    private Type m_nextStatusUI = typeof(UIManager);
    public Type NextStatusUI { get { return m_nextStatusUI; } }

    /// <summary>
    /// 界面跳转堆栈, 用于topbar记录返回的状态, 默认记录跳转界面
    /// 如需不记录下一个跳转界面, 则直接Open界面的时候调用堆栈的Pop方法即可.
    /// </summary>
    private Stack<KeyValuePair<Type, object[]>> m_statusUIStack = new Stack<KeyValuePair<Type, object[]>>();

    public Stack<KeyValuePair<Type, object[]>> StatusUIStack { get { return m_statusUIStack; } }

    private int m_defaultStatusSiblingIndex = 1;
    /// <summary>
    /// 弹窗堆栈
    /// </summary>
    private Stack<Type> m_popupStack = new Stack<Type>();

    public Stack<Type> PopupStack { get { return m_popupStack; } }

    private int m_nextPopupSiblingIndex = 1;

    private ScreenOrientation m_lastScreenOrientation = ScreenOrientation.AutoRotation;

    private DeviceOrientation m_lastLandscapeDeviceOrientation = DeviceOrientation.LandscapeLeft;
    public DeviceOrientation LastLandscapeDeviceOrientation { get { return m_lastLandscapeDeviceOrientation; } }

    private static UIManager s_instance;

    public static UIManager Instance { get { return s_instance; } }

    void Awake()
    {
        s_instance = this;
        name = typeof(UIManager).Name;
        m_UIRootHorizontal = CreateUI("UIRootHorizontal", transform, "");//此处传空表示没有多余的前缀路径
        //m_UIRootVertical = CreateUI("UIRootVertical", transform, "");//此处传空表示没有多余的前缀路径
        SetUIRoot(m_UIRootHorizontal, true, false);
    }

    private void SetUIRoot(GameObject go, bool horizontal, bool vertical)
    {
        m_UIRoot = go;
        m_UIRootHorizontal.SetActive(horizontal);
        //m_UIRootVertical.SetActive(vertical);
    }

    //public void SwitchHorizontal()
    //{
    //    switch (Input.deviceOrientation)
    //    {
    //        case DeviceOrientation.LandscapeLeft:
    //        case DeviceOrientation.LandscapeRight:
    //            m_lastLandscapeDeviceOrientation = Input.deviceOrientation;
    //            break;
    //    }
    //    SetUIRoot(m_UIRootHorizontal, true, false);
    //    if (m_lastScreenOrientation == ScreenOrientation.Portrait)
    //    {
    //        Screen.orientation = Utility.ConvertDeviceOrientation(m_lastLandscapeDeviceOrientation);
    //        StartCoroutine(YieldSetAuto());
    //    }
    //}

    //private IEnumerator YieldSetAuto()
    //{
    //    yield return new WaitForEndOfFrame();
    //    m_lastScreenOrientation = ScreenOrientation.AutoRotation;
    //    Screen.orientation = ScreenOrientation.AutoRotation;
    //    Screen.autorotateToLandscapeLeft = true;
    //    Screen.autorotateToLandscapeRight = true;
    //    Screen.autorotateToPortrait = false;
    //    Screen.autorotateToPortraitUpsideDown = false;
    //}

    //public void SwitchVertical()
    //{
    //    SetUIRoot(m_UIRootVertical, false, true);
    //    if (m_lastScreenOrientation == ScreenOrientation.AutoRotation)
    //    {
    //        m_lastScreenOrientation = ScreenOrientation.Portrait;
    //        Screen.orientation = ScreenOrientation.Portrait;
    //    }
    //}

    /// <summary>
    /// 注册观察者
    /// </summary>
    /// <param name="observer"></param>
    public static void RegisterObserver(IOnUIChange observer)
    {
        if (!m_observersUIChange.Contains(observer))
            m_observersUIChange.Add(observer);
    }

    public static void UnRegisterObserver(IOnUIChange observer)
    {
        if (m_observersUIChange.Contains(observer))
            m_observersUIChange.Remove(observer);
    }

    public static void RegisterObserver(IUIOnRefresh observer)
    {
        if (!m_observersUIRefresh.Contains(observer))
            m_observersUIRefresh.Add(observer);
    }

    public static void UnRegisterObserver(IUIOnRefresh observer)
    {
        if (m_observersUIRefresh.Contains(observer))
            m_observersUIRefresh.Remove(observer);
    }

    public void Open<T>(params object[] data)
    {
        Open(typeof(T), data);
    }

    public void Open(Type ui, params object[] data)
    {
        if (ui.GetInterface("IUIState") != null)
        {
            OpenState(ui, data);
        }
        else if (ui.GetInterface("IUIPopup") != null)
        {
            OpenPopup(ui, data);
        }
    }

    /// <summary>
    /// 状态UI打开接口
    /// </summary>
    /// <param name="nextStatus">状态UI枚举名</param>
    /// <param name="data">传入的数据</param>
    private void OpenState(Type nextStatus, params object[] data)
    {
        //if (nextStatus == m_currentStatusUI)
        //    return;

        m_statusUIStack.Push(new KeyValuePair<Type, object[]>(nextStatus, data));

        GetStatusUI(nextStatus, data);
        Debug.Log("***Go*** From Status : " + m_previousStatusUI.ToString() + "=====>>>>>" + " To Status : " + nextStatus.ToString());
    }

    private void GetStatusUI(Type next, params object[] data)
    {
        m_previousStatusUI = m_currentStatusUI;
        m_nextStatusUI = next;

        foreach (IOnUIChange item in m_observersUIChange)
        {
            item.OnBeforeChange(m_previousStatusUI, next);
            //Debug.Log("On Before Change UI Notify : " + item.ToString() + " Success!!");
        }

        if (m_currentStatusUI != typeof(UIManager))
            m_uiFactory.DeleteUI(m_currentStatusUI, true);

        var nextUI = m_uiFactory.GetUI(next);
        if (nextUI != null)
        {
            foreach (var item in m_observersUIRefresh)
            {
                item.OnBeforeRefresh(m_previousStatusUI, next, nextUI);
            }
            nextUI.gameObject.SetActive(true);
            nextUI.gameObject.transform.SetSiblingIndex(m_defaultStatusSiblingIndex);
            var refresh = nextUI.GetComponent<IUIRefresh>();
            if (refresh != null)
                refresh.Refresh(data);
            foreach (var item in m_observersUIRefresh)
            {
                item.OnAfterRefreshUI(m_previousStatusUI, next, nextUI);
            }
        }
        else
        {
            Debug.LogError("Next Status UI : " + next.ToString() + " Is Null! Please Check!");
        }
        m_currentStatusUI = next;

        foreach (IOnUIChange item in m_observersUIChange)
        {
            item.OnAfterChange(m_previousStatusUI, next, nextUI);
            //Debug.Log("On After Change UI Notify : " + item.ToString() + " Success!!");
        }

        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 状态界面返回
    /// </summary>
    /// <param name="data"></param>
    public void Back(params object[] data)
    {
        if (m_statusUIStack.Count > 0)
        {
            m_statusUIStack.Pop();
            if (m_statusUIStack.Count > 0)
            {
                KeyValuePair<Type, object[]> item = m_statusUIStack.Peek();
                if (data == null || data.Length == 0)
                    data = item.Value;
                GetStatusUI(item.Key, data);
                Debug.Log("***Back*** From Status : " + m_previousStatusUI.ToString() + "<<<<<=====" + " To Status : " + item.Key.ToString());
            }
        }
    }

    /// <summary>
    /// 弹窗UI打开接口
    /// </summary>
    /// <param name="next">弹窗UI枚举名</param>
    /// <param name="data">传入的数据</param>
    private void OpenPopup(Type next, params object[] data)
    {
        if (m_popupStack.Contains(next))
            return;
        //Debug.LogError("Next popup is already existing!! You can not open already popup! Please close current popup to show next.");

        var newUI = m_uiFactory.GetUI(next);
        if (newUI != null)
        {
            m_popupStack.Push(next);
            newUI.gameObject.SetActive(true);
            newUI.gameObject.transform.SetAsLastSibling();
            var refresh = newUI.GetComponent<IUIRefresh>();
            if (refresh != null)
                refresh.Refresh(data);
            var canvas = newUI.gameObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = newUI.gameObject.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingLayerName = "UIPopup";
                canvas.sortingOrder = m_nextPopupSiblingIndex;
            }
            var raycast = newUI.gameObject.GetComponent<GraphicRaycaster>();
            if (raycast == null)
            {
                var gr = newUI.gameObject.AddComponent<GraphicRaycaster>();
                gr.ignoreReversedGraphics = false;
            }

            m_nextPopupSiblingIndex++;
        }
        else
        {
            Debug.LogError("Next Popup UI : " + next.ToString() + " Is Null! Please Check!");
        }
    }

    /// <summary>
    /// 关闭弹窗界面
    /// </summary>
    public void Close()
    {
        if (m_popupStack.Count > 0)
        {
            m_uiFactory.DeleteUI(m_popupStack.Pop());

            if (m_popupStack.Count > 0)
            {
                m_nextPopupSiblingIndex--;
            }
            else
            {
                m_nextPopupSiblingIndex = 1;
            }
        }
    }

    public void CloseAll()
    {
        var popupList = m_popupStack.ToArray();
        foreach (var item in popupList)
        {
            Close();
        }
    }
}
