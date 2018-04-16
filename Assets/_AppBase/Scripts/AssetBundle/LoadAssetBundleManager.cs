using UnityEngine;
using System.Collections.Generic;
using Tangzx.ABSystem;
using System;

public class LoadAssetBundleManager : BaseInstance<LoadAssetBundleManager>, IOnUIChange
{
    public class DependModel
    {
        public string bundleName;
        public AssetBundle ab;
        public List<DependModel> dependList;

        public DependModel(string bundleName)
        {
            this.bundleName = bundleName;
            LoadAll();
        }

        protected void LoadAll()
        {
            LoadDepend();
            LoadSelf();
        }

        protected void LoadSelf()
        {
            if (m_abCache.ContainsKey(bundleName))
                ab = m_abCache[bundleName];
            else
            {
                ab = AssetBundleManager.Instance.LoadBN(bundleName);
                if (ab != null)
                    m_abCache.Add(bundleName, ab);
            }
        }

        protected void LoadDepend()
        {
            dependList = new List<DependModel>();
            AssetBundleData data = null;
            var streamingAB = AssetBundleManager.Instance.depInfoReader.GetAssetBundleInfo(bundleName);
            var downloadAB = UpdateAssetBundleManager.Instance.DepAll.GetAssetBundleInfo(bundleName.ToLower());
            if (streamingAB != null)
                data = streamingAB;
            else if (downloadAB != null)
                data = downloadAB;
            if (data != null)
            {
                var dependencies = data.dependencies;
                foreach (var item in dependencies)
                {
                    var model = new DependModel(item);
                    dependList.Add(model);
                }
            }
        }

        public void UnloadAll()
        {
            UnLoadDepend();
            UnLoadSelf();
        }

        protected void UnLoadSelf()
        {
            if (m_abCache.ContainsKey(bundleName))
            {
                m_abCache.Remove(bundleName);
                ab.Unload(true);
            }
        }

        protected void UnLoadDepend()
        {
            foreach (var item in dependList)
            {
                item.UnloadAll();
            }
        }
    }

    public class CacheModel : DependModel
    {
        public Dictionary<string, UnityEngine.Object> assets;

        public CacheModel(string assetName)
            : base(assetName)
        {
            assets = new Dictionary<string, UnityEngine.Object>();
        }

        public T Load<T>() where T : UnityEngine.Object
        {
            if (ab != null)
            {
                string typeName = typeof(T).Name;
                if (assets.ContainsKey(typeName))
                    return assets[typeName] as T;
                var asset = ab.LoadAsset<T>();
                assets.Add(typeName, asset);
                return asset;
            }
            return default(T);
        }
    }

    private static Dictionary<Type, Dictionary<string, CacheModel>> m_cache = new Dictionary<Type, Dictionary<string, CacheModel>>();

    private static Dictionary<string, AssetBundle> m_abCache = new Dictionary<string, AssetBundle>();

    void Awake()
    {
        UIManager.RegisterObserver(this);
    }

    private void OnDestroy()
    {
        UIManager.UnRegisterObserver(this);
    }

    /// <summary>
    /// 根据资源名字加载对应资源(包括所依赖的Assetbundle)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetName">必须传入带后缀的名字, 例如test.prefab</param>
    /// <returns></returns>
    public T Load<T>(string assetName) where T : UnityEngine.Object
    {
        string bundleName = "";
        var streamingAB = AssetBundleManager.Instance.depInfoReader.GetAssetBundleInfoByShortName(assetName.ToLower());
        var downloadAB = UpdateAssetBundleManager.Instance.DepAll.GetAssetBundleInfoByShortName(assetName.ToLower());
        if (streamingAB != null)
            bundleName = streamingAB.fullName;
        else if (downloadAB != null)
            bundleName = downloadAB.fullName;
        if (string.IsNullOrEmpty(bundleName)) return default(T);
        var model = Cache(bundleName);
        return model.Load<T>();
    }

    private CacheModel Cache(string bundleName)
    {
        var nextState = UIManager.Instance.NextStatusUI;
        if (m_cache.ContainsKey(nextState))
        {
            if (m_cache[nextState].ContainsKey(bundleName))
            {
                return m_cache[nextState][bundleName];
            }
            else
            {
                return CacheHelper(nextState, bundleName);
            }
        }
        else
        {
            m_cache.Add(nextState, new Dictionary<string, CacheModel>());
            return CacheHelper(nextState, bundleName);
        }
    }

    private CacheModel CacheHelper(Type state, string bundleName)
    {
        var model = new CacheModel(bundleName);
        m_cache[state].Add(bundleName, model);
        return model;
    }


    public void OnBeforeChange(Type previous, Type next)
    {
        if (m_cache.ContainsKey(previous))
        {
            foreach (var item in m_cache[previous].Values)
            {
                item.UnloadAll();
            }
            m_cache.Remove(previous);
        }
    }

    public void OnAfterChange(Type previous, Type next, UIBaseInit nextUI)
    {
    }
}
