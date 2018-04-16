using System.Collections.Generic;
using UnityEngine;

public enum AtlasName
{
    UICommon,
    UICommon2,
    UIGameIcon,
    UIGameIcon2,
    UIGameType,
    UIGameCommon,
    UIMainInterfaceBG,
}

public class SpritesLoad : MonoBehaviour
{
    private const string s_rootPath = "SpritesLoad/";

    private static Dictionary<AtlasName, Dictionary<string, Sprite>> m_cacheAtlas = new Dictionary<AtlasName, Dictionary<string, Sprite>>();

    private static readonly string[] s_childPath = { "",
                                                   "FrameAnimation/",
                                                   };


    public static Sprite Load(string key, bool warn = true)
    {
        foreach (string path in s_childPath)
        {
            var sprite = Resources.Load<Sprite>(s_rootPath + path + key);
            if (sprite)
                return sprite;
        }
        if (warn)
            Debug.LogWarning("*****The sprite : " + key + " is not found!!!*******");
        return null;
    }

    public static Sprite LoadAtlas(AtlasName atlasName, string spriteName)
    {
        Sprite sprite = null;
        if (m_cacheAtlas.ContainsKey(atlasName))
        {
            var atlas = m_cacheAtlas[atlasName];
            if (atlas.ContainsKey(spriteName))
                sprite = atlas[spriteName];
        }
        else
        {
            foreach (string path in s_childPath)
            {
                var atlas = Resources.LoadAll<Sprite>(s_rootPath + path + atlasName);
                if (atlas != null && atlas.Length > 0)
                {
                    if (!m_cacheAtlas.ContainsKey(atlasName))
                    {
                        m_cacheAtlas.Add(atlasName, new Dictionary<string, Sprite>());
                        foreach (var item in atlas)
                        {
                            m_cacheAtlas[atlasName].Add(item.name, item);
                            if (item.name.Equals(spriteName))
                            {
                                sprite = item;
                            }
                        }
                    }
                }
            }
        }
        if (sprite == null)
            Debug.LogWarning(string.Format("*****The atlas : {0},sprite : {1}, is not found!!!*******", atlasName, spriteName));
        return sprite;
    }
}
