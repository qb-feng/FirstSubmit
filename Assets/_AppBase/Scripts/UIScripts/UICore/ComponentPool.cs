using UnityEngine;
using System.Collections.Generic;

public class ComponentPool : MonoBehaviour
{
    /// <summary>
    /// 获取组件池, 目的减少GetCommpent调用次数.
    /// </summary>
    protected Dictionary<int, Component> m_pool = new Dictionary<int, Component>();

    public T Get<T>() where T : Component
    {
        return Get<T>(gameObject);
    }

    public T Get<T>(GameObject go) where T : Component
    {
        int instanceID = go.GetInstanceID();
        if (m_pool.ContainsKey(instanceID))
        {
            if (m_pool[instanceID])
                return (T)m_pool[instanceID];
            else
            {
                m_pool.Remove(instanceID);
                Get<T>(go);
            }
        }
        Component component = go.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError(go.name + " has no Component, " + typeof(T) + " is null");
            return null;
        }
        m_pool.Add(instanceID, component);
        return (T)component;
    }
}
