using UnityEngine;
using System;
using System.Collections;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource
    {
        public string prefabName;
        public int poolSize = 5;
        public event Func<int, string> prefabNameFunc;

        private bool inited = false;
        public virtual GameObject GetObject(int index)
        {
            int autoCount = 0;
            if (prefabNameFunc != null)
            {
                prefabName = prefabNameFunc(index);
                autoCount = 1;
            }
            if (!inited)
            {
                SG.ResourceManager.Instance.InitPool(prefabName, poolSize);
                inited = true;
            }
            return SG.ResourceManager.Instance.GetObjectFromPool(prefabName, autoCreate: autoCount);
        }

        public virtual void ReturnObject(Transform go)
        {
            go.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
            SG.ResourceManager.Instance.ReturnObjectToPool(go.gameObject);
        }
    }
}
