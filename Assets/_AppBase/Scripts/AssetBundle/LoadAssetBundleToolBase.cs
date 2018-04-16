using UnityEngine;
using System.Collections.Generic;
using Tangzx.ABSystem;

public class LoadAssetBundleToolBase : MonoBehaviour
{
    public class CacheModel
    {
        public AssetBundle ab;
        public List<Object> assets;

        public T Load<T>() where T : Object
        {
            var asset = ab.LoadAsset<T>();
            assets.Add(asset);
            return asset;
        }

        public void Remove(Object asset)
        {
            foreach (var item in assets)
            {
                if (item == asset)
                {
                    assets.Remove(asset);
                    break;
                }
            }
        }
    }

    private static Dictionary<string, CacheModel> s_cache = new Dictionary<string, CacheModel>();

    [SerializeField]
    private bool m_isOnDisableUnload;

    protected Dictionary<string, List<Object>> m_temporary = new Dictionary<string, List<Object>>();

    // Use this for initialization
    protected virtual void Awake()
    {

    }

    void OnDestroy()
    {
        if (!m_isOnDisableUnload)
            UnLoadAB();
    }

    void OnDisable()
    {
        if (m_isOnDisableUnload)
            UnLoadAB();
    }

    private void UnLoadAB()
    {
        foreach (var item in m_temporary)
        {
            Remove(item.Key, item.Value);
        }
        m_temporary.Clear();
    }

    protected CacheModel Cache(string path)
    {
        if (s_cache.ContainsKey(path))
        {
            return s_cache[path];
        }
        else
        {
            var model = new CacheModel()
            {
                ab = AssetBundleManager.Instance.LoadSN(path),
                assets = new List<Object>()
            };
            s_cache.Add(path, model);
            return model;
        }
    }

    private void Remove(string path, List<Object> assets)
    {
        if (s_cache.ContainsKey(path))
        {
            var model = s_cache[path];
            foreach (var item in assets)
            {
                model.Remove(item);
            }
            if (model.assets.Count == 0)
            {
                model.ab.Unload(true);
                s_cache.Remove(path);
            }
        }
    }
}
