using UnityEngine;
using System.Collections.Generic;
using System;

public class UIBaseInit : ComponentPool
{
    public const string UIPrefabPath = "Prefab/UIPrefab/";
    public const string UIItemPath = "Prefab/UIPrefab/UIItem/";
    public const string FXPrefabPath = "Prefab/FXPrefab/";

    /// <summary>
    /// 该字典内存放UI预制体上需要控制的元素, 若要控制界面预制体, 需要把界面UI元素tag设置为UIObject
    /// </summary>
    private Dictionary<string, GameObject> m_UI;

    private Dictionary<string, GameObject> UI
    {
        get
        {
            if (m_UI == null)
            {
                UIInit();
                //Debug.Log("###### UIInit : " + name);
            }
            return m_UI;
        }
    }

    public virtual string InitTag { get { return "UIObject"; } }

    public void Set(string ui, GameObject go)
    {
        UI[ui] = go;
        go.name = ui;
    }

    public GameObject Get(string ui)
    {
        if (UI.ContainsKey(ui))
            return UI[ui];
        Debug.LogError(ui + " is not found! Please check!");
        return null;
    }

    public Transform GetT(string ui)
    {
        if (UI.ContainsKey(ui))
            return UI[ui].transform;
        Debug.LogError(ui + " is not found! Please check!");
        return null;
    }

    public RectTransform GetR(string ui)
    {
        if (UI.ContainsKey(ui))
            return (RectTransform)UI[ui].transform;
        Debug.LogError(ui + " is not found! Please check!");
        return null;
    }

    public Sprite GetS(string spriteName)
    {
        var cp = Get<UGUISprites>();
        if (cp != null)
            return cp.Get(spriteName);
        else
        {
            Debug.LogError("UGUISprites Component Is Null!!");
            return null;
        }
    }

    public T Get<T>(string ui) where T : Component
    {
        if (UI.ContainsKey(ui))
            return Get<T>(UI[ui]);
        Debug.LogError(ui + " is not found! Please check!");
        return null;
    }

    public static T CreateUI<T>() where T : Component
    {
        return CreateUI(typeof(T).Name).AddComponent<T>();
    }

    public static T CreateUI<T>(Component parent) where T : Component
    {
        return CreateUI(typeof(T).Name, parent).AddComponent<T>();
    }

    public static GameObject CreateUI(Type name)
    {
        return CreateUI(name.Name);
    }

    public static GameObject CreateUI(string name)
    {
        return CreateUI(name, UIManager.Instance.UIRoot.transform);
    }

    public static GameObject CreateUI(string name, Component parent)
    {
        return CreateUI(name, parent, UIPrefabPath);
    }

    public static GameObject CreateUIItem(string name, Component parent)
    {
        return CreateUI(name, parent, UIItemPath);
    }

    public static T CreateUIItem<T>(Component parent) where T : Component
    {
        return CreateUIItem(typeof(T).Name, parent).AddComponent<T>();
    }

    public static GameObject CreateFX(string name, Component parent = null)
    {
        return CreateUI(name, parent, FXPrefabPath);
    }

#if UNITY_EDITOR
    public static string FindAsset(string fileName, string extension)
    {
        var scriptGUID = UnityEditor.AssetDatabase.FindAssets(fileName);
        foreach (var item in scriptGUID)
        {
            string filePath = UnityEditor.AssetDatabase.GUIDToAssetPath(item);
            if (System.IO.Path.GetFileName(filePath).Equals(fileName + extension))
            {
                return filePath;
            }
        }
        return "";
    }
#endif

    /// <summary>
    /// 克隆界面UI预制体
    /// </summary>
    /// <param name="name">预制体名字</param>
    /// <returns></returns>
    public static GameObject CreateUI(string name, Component parent, string path)
    {
        var prefab = Resources.Load<GameObject>(path + name);
        if (!prefab)
        {
#if UNITY_EDITOR
            string findPath = FindAsset(name, ".prefab");
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(findPath);
#else
        prefab = LoadAssetBundleManager.Instance.Load<GameObject>(name + ".prefab");
#endif
        }
        if (prefab)
        {
            var go = Instantiate<GameObject>(prefab);
            go.transform.SetParent(parent == null ? null : parent.transform, false);
            return go;
        }
        else
        {
            Debug.LogWarning("Clone UI Is Null!! Name : " + name);
            return null;
        }
    }

    /// <summary>
    /// 该方法需要在继承的界面控制脚本中的Awake方法中调用
    /// </summary>
    private void UIInit()
    {
        tag = InitTag;
        m_UI = new Dictionary<string, GameObject>();
        Transform[] trans = GetComponentsInChildren<Transform>(true);
        foreach (Transform item in trans)
        {
            if (item.tag.Equals(InitTag))
            {
                // 注 : 思考 另外一个方式 : m_UI 字典的key可用当前对象所在目录级保存, 
                //只要不存在所有父级名字都相同的即可, 可实现不同名父级下同名子物体
                if (m_UI.ContainsKey(item.name))
                {
                    string tips1 = "Find repeat object name is : " + item.name + "! Please check! The error detail is :";
                    string tips2 = " The Exist object parent is : " + m_UI[item.name].transform.parent.name;
                    string tips3 = ". The Repeat object parent is : " + item.parent.name;
                    Debug.LogError(tips1 + tips2 + tips3);
                    continue;
                }
                m_UI.Add(item.name, item.gameObject);
            }
        }
    }
}
