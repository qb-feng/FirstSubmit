using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 持久化存储枚举命名
/// </summary>
public enum PersistentName
{
    GameLocalData,
    OnceActionData,
    UserInfo,
    ConfigData,
    ExploreRandomY,
    SuperSpell,
    InsideCode, // 内测二维码
    MessageMarquee,//消息跑马灯
    UserSettings,//用户设置
    OrderData,
    //    UICatchGoods,// BOBODAO接单词

    GuidePro,
    MessageGameStartData,
}

public class PersistentManager : BaseInstance<PersistentManager>
{
    /** 后缀常量字符 */
    public static string SavePath
    {
        get
        {
            return Application.persistentDataPath + "/";
        }
    }

    public static string StreamingAssetsPath
    {
        get
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                return Application.streamingAssetsPath + "/";
            }
#endif
            return "file://" + Application.streamingAssetsPath + "/";
        }
    }

    public static void Delete(PersistentName fileName)
    {
        string path = SavePath + fileName.ToString();
        Debug.Log(path);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static void SaveByName(PersistentName fileName, string data, bool encode = true, bool hash = false)
    {
        SaveByName(fileName.ToString(), data, encode);
    }

    /// <summary>
    /// 本地存储数据
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="data"></param>
    public static void SaveByName(string fileName, string data, bool encode = true, bool hash = false)
    {
        string path = SavePath + fileName;
        SaveByPath(path, data, encode, hash);
    }

    public static void SaveByPath(string fullPath, string data, bool encode = true, bool hash = false)
    {
        string fileName = Path.GetFileNameWithoutExtension(fullPath);
        string directory = Path.GetDirectoryName(fullPath);
        if (hash)
            fileName = Tangzx.ABSystem.HashUtil.Get(fileName);
        fullPath = Path.Combine(directory, fileName);
        Debug.Log(fullPath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        else
        {
            bool exist = Directory.Exists(directory);
            if (!exist)
            {
                Directory.CreateDirectory(directory);
            }
        }
        FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        sw.Flush();
        sw.BaseStream.Seek(0, SeekOrigin.Begin);
        sw.Write(encode ? Base64Convert.Encode(data) : data);
        sw.Close();
    }

    public static void SaveStream(string path, byte[] bytes)
    {
        bool exist = Directory.Exists(path);
        if (!exist)
        {
            Directory.CreateDirectory(path);
        }
        File.WriteAllBytes(path, bytes);
    }

    /// <summary>
    /// 加载本地缓存
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string LoadByName(PersistentName fileName, bool encode = true, bool hash = false)
    {
        return LoadByName(fileName.ToString(), encode);
    }

    public static string LoadByName(string fileName, bool encode = true, bool hash = false)
    {
        string path = SavePath + fileName;
        return LoadByPath(path, encode, hash);
    }

    public static string LoadByPath(string fullPath, bool encode = true, bool hash = false)
    {
        string fileName = Path.GetFileNameWithoutExtension(fullPath);
        string directory = Path.GetDirectoryName(fullPath);
        if (hash)
            fileName = Tangzx.ABSystem.HashUtil.Get(fileName);
        fullPath = Path.Combine(directory, fileName);
        byte[] bytes = LoadStream(fullPath);
        if (bytes == null) return "";
        string data = Encoding.UTF8.GetString(bytes);
        return encode ? Base64Convert.Decode(data) : data;
    }

    public static byte[] LoadStream(string filePath)
    {
        //Debug.Log(filePath);
        if (!File.Exists(filePath)) return null;
        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read);
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, (int)fs.Length);
        fs.Close();
        return bytes;
    }

    /// <summary>
    /// 泛型T不能是 集合 Array 或List 或Dictionary等, 必须是自己定义的实体类, 且类标记属性[System.Serializable].
    /// 实体类不能用属性, 必须用字段
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static T LoadObj<T>(PersistentName fileName)
    {
        string json = LoadByName(fileName);
        //Debug.Log(fileName.ToString() + json);
        if (!string.IsNullOrEmpty(json))
            return JsonUtility.FromJson<T>(json);
        return default(T);
    }

    /// <summary>
    /// 泛型T不能是 集合 Array 或List 或Dictionary等, 必须是自己定义的实体类, 且类标记属性[System.Serializable].
    /// 实体类不能用属性, 必须用字段
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <param name="obj"></param>
    public static void SaveObj<T>(PersistentName fileName, T obj)
    {
        string json = JsonUtility.ToJson(obj);
        Debug.Log(fileName.ToString() + json);
        SaveByName(fileName, json);
    }

    public static List<T> LoadList<T>(PersistentName fileName)
    {
        string json = LoadByName(fileName);
        Debug.Log(fileName.ToString() + json);
        return Utility.ArrayFromJson<T>(json);
    }

    public static void SaveList<T>(PersistentName fileName, List<T> list)
    {
        string json = Utility.ArrayToJson(list);
        Debug.Log(fileName.ToString() + json);
        SaveByName(fileName, json);
    }

    /// <summary>
    /// 从StreamingAssets目录下移动文件到PersistentDataPath路径下
    /// </summary>
    /// <param name="fileNames"></param>
    public void FromStreamingMoveToPersistent(params string[] fileNames)
    {
        foreach (string fileName in fileNames)
        {
            StartCoroutine(GetFileFromStreamingAssets(fileName));
        }
    }

    private IEnumerator GetFileFromStreamingAssets(string fileName)
    {
        WWW www = new WWW(StreamingAssetsPath + fileName);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("Get StreamingAssets Error");
        }
        if (www.isDone)
        {
            string path = SavePath + fileName;
            File.WriteAllBytes(path, www.bytes);
        }
        www.Dispose();
    }
}
