using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoadBgAssetBundleTool : LoadAssetBundleToolBase
{
    [SerializeField]
    private List<Image> m_bgs;

    [SerializeField]
    private List<string> m_bgsName;

    // Use this for initialization
    protected override void Awake()
    {
        for (int i = 0; i < m_bgs.Count; i++)
        {
            string path = m_bgsName[i];
            var model = Cache(path);
            var sprite = model.Load<Sprite>();
            if (m_temporary.ContainsKey(path))
            {
                var assets = m_temporary[path];
                assets.Add(sprite);
            }
            else
            {
                var assets = new List<Object>();
                assets.Add(sprite);
                m_temporary.Add(path, assets);
            }
            m_bgs[i].sprite = sprite;
        }
    }
}
