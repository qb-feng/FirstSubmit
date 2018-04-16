using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ConfigManager
{
    private static Dictionary<string, object> s_dicConfigs = new Dictionary<string, object>();

    public static void SaveConfig(string configName, ConfigCacheModel configCache)
    {
        s_dicConfigs.Remove(configName);
        PersistentManager.SaveByName(configName, JsonUtility.ToJson(configCache), true, true);
    }

    public static List<T> Get<T>()
    {
        var configName = typeof(T).Name;
        if (s_dicConfigs.ContainsKey(configName))
            return (List<T>)s_dicConfigs[configName];

        bool isLocalNew = IsLocalVersionHigher(configName);
        var configList = LoadFromCache<T>();
        if (isLocalNew || configList == null)
            configList = LoadFromLocal<T>();
        if (configList == null || configList.Count == 0)
        {
            Debug.LogWarning("Config Object is Null! Please check : " + typeof(T));
            return new List<T>();
        }
        else
        {
            s_dicConfigs.Add(configName, configList);
            return configList;
        }
    }

    public static bool IsLocalVersionHigher(string configName)
    {
        var localConfigList = LoadFromLocal<ConfigModel>();
        var cacheConfigList = LoadFromCache<ConfigModel>();
        if (cacheConfigList == null)
            return true;
        int localVersion = 0;
        foreach (var item in localConfigList)
        {
            if (item.name.Equals(configName))
            {
                localVersion = item.version;
                break;
            }
        }
        int serverVersion = 0;
        foreach (var item in cacheConfigList)
        {
            if (item.name.Equals(configName))
            {
                serverVersion = item.version;
                break;
            }
        }
        return localVersion >= serverVersion;
    }

    private static List<T> LoadFromCache<T>()
    {
        var configName = typeof(T).Name;
        string content = PersistentManager.LoadByName(configName, true, true);
        if (string.IsNullOrEmpty(content))
            return null;
        var cacheModel = JsonUtility.FromJson<ConfigCacheModel>(content);
        var configList = Utility.ArrayFromJson<T>(cacheModel.content);
        return configList;
    }

    protected static List<T> LoadFromLocal<T>(string path = "")
    {
        var configName = typeof(T).Name;
        if (!string.IsNullOrEmpty(path))
            path += "BookConfig/";
        var configText = Resources.Load<TextAsset>("Config/" + path + configName);
        if (configText == null)
        {
            Debug.LogWarning("Config name is wrong! Please check : " + configName);
            return new List<T>();
        }
        var list = Utility.ArrayFromJson<T>(configText.text);
        return list;
    }

    private static List<ConfigModel> LoadConfigList()
    {
        string configName = typeof(ConfigModel).Name;
        if (s_dicConfigs.ContainsKey(configName))
            return (List<ConfigModel>)s_dicConfigs[configName];
        var cacheConfigList = LoadFromCache<ConfigModel>();
        var localConfigList = LoadFromLocal<ConfigModel>();
        if (cacheConfigList == null)
        {
            s_dicConfigs.Add(configName, localConfigList);
            return localConfigList;
        }
        else
        {
            var tempConfigList = new List<ConfigModel>();
            foreach (var localConfig in localConfigList)
            {
                bool find = false;
                foreach (var cacheConfig in cacheConfigList)
                {
                    if (localConfig.name.Equals(cacheConfig.name))
                    {
                        bool isLocalNew = localConfig.version > cacheConfig.version;
                        if (isLocalNew)
                        {
                            tempConfigList.Add(localConfig);
                        }
                        else
                        {
                            string hashConfigName = Tangzx.ABSystem.HashUtil.Get(cacheConfig.name);
                            if (File.Exists(Path.Combine(Application.persistentDataPath, hashConfigName)))
                                tempConfigList.Add(cacheConfig);
                            else
                                tempConfigList.Add(localConfig);
                        }
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    tempConfigList.Add(localConfig);
                }
            }
            s_dicConfigs.Add(configName, tempConfigList);
            return tempConfigList;
        }
    }
}
