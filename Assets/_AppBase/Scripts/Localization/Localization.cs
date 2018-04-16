using UnityEngine;
using System.Collections.Generic;

public class Localization
{
    public class KeyValue
    {
        public string key;
        public string value;
    }

    private const string path = "Localization/";
    private static Dictionary<string, string> s_dic = new Dictionary<string, string>();

    private static string s_language = "";
    public static string Language
    {
        get { return s_language; }
        set
        {
            s_language = value;
            TextAsset text = Resources.Load<TextAsset>(path + value);
            if (text != null)
                s_dic = new ByteReader(text).ReadDictionary();
        }
    }

    public static string Get(string key)
    {
        if (s_dic.ContainsKey(key))
        {
            return s_dic[key];
        }
        else
        {
            Debug.LogWarning("Key : " + key + " is not found! Please check!!");
            return key;
        }
    }
}
