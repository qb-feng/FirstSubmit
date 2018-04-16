using UnityEngine;
using System.Collections.Generic;

public class PathConfigScriptable : ScriptableObject
{
    [System.Serializable]
    public class ConvertModel
    {
        public string loadPath;
        public string savePath;
    }

    public List<ConvertModel> configList = new List<ConvertModel>();
}
