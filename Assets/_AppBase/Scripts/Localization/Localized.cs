using UnityEngine;
using UnityEngine.UI;

public class Localized : MonoBehaviour
{
    public string key;

    void Awake()
    {
        var cp = GetComponent<Text>();
        if (cp != null && !string.IsNullOrEmpty(key))
            cp.text = Localization.Get(key);
    }
}
