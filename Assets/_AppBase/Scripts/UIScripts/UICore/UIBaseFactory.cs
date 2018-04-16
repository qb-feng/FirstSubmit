using UnityEngine;
using System.Collections.Generic;
using System;

public class UIBaseFactory : IUIFactory
{
    protected Dictionary<Type, UIBaseInit> m_pool = new Dictionary<Type, UIBaseInit>();

    public UIBaseInit GetUI(Type ui)
    {
        return CreateUI(ui);
    }

    public delegate void CreateUIEvent(GameObject UI);

    public void DeleteUI(Type ui, bool immediately = false)
    {
        if (m_pool.ContainsKey(ui))
        {
            UIBaseInit currentUI = GetUIFromPool(ui);
            if (currentUI != null)
            {
                IUIUnDestroy destroy = currentUI.GetComponent<IUIUnDestroy>();
                if (destroy != null)
                {
                    currentUI.gameObject.SetActive(false);
                }
                else
                {
                    if (immediately)
                        GameObject.DestroyImmediate(currentUI.gameObject);
                    else
                        GameObject.Destroy(currentUI.gameObject);
                    m_pool.Remove(ui);
                }
            }
        }
        else
        {
            Debug.LogWarning("Delete UI Is Not Exist!! The UI Is : " + ui);
        }
    }

    protected UIBaseInit CreateUI(Type ui)
    {
        var uiBase = GetUIFromPool(ui);
        if (uiBase == null)
        {
            GameObject go = UIBaseInit.CreateUI(ui.Name, null);
            var cp = go.AddComponent(ui);
            uiBase = cp as UIBaseInit;//cp.GetComponent<UIBaseInit>();
            go.transform.SetParent(UIManager.Instance.UIRoot.transform, false);
            m_pool.Add(ui, uiBase);
        }
        return uiBase;
    }

    public UIBaseInit GetUIFromPool(Type ui)
    {
        if (m_pool.ContainsKey(ui))
            if (m_pool[ui] != null && m_pool[ui].gameObject != null)
                return m_pool[ui];
            else
                m_pool.Remove(ui);
        return null;
    }
}
