using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static readonly Dictionary<string, GameObject> s_instances = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (s_instances.ContainsKey(gameObject.name))
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            s_instances[gameObject.name] = gameObject;
        }
    }
}