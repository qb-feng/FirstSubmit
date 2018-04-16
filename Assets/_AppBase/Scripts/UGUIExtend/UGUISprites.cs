using UnityEngine;
using System.Collections.Generic;

public class UGUISprites : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> m_spritesList;

    private Dictionary<string, Sprite> m_dicSprites;

    void Awake()
    {
        m_dicSprites = new Dictionary<string, Sprite>();
        foreach (var item in m_spritesList)
        {
            if (item == null)
            {
                Debug.LogError("UGUISprites Exist Null Sprite!!! Please Check!!!");
                continue;
            }
            if (m_dicSprites.ContainsKey(item.name))
                Debug.LogError("Contain Same Name Sprite : " + item.name);
            else
                m_dicSprites.Add(item.name, item);
        }
        m_spritesList.Clear();
    }


    public Sprite Get(string spriteName)
    {
        if (m_dicSprites.ContainsKey(spriteName))
            return m_dicSprites[spriteName];
        else
        {
            Debug.LogError("The Sprite : " + spriteName + " Is Not Exist");
            return null;
        }
    }
}
