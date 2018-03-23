using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaiDuVoice
{
    public class SiginBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
                return _instance;
            }
        }
    }
}
