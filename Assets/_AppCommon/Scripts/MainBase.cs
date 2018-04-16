using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using Tangzx.ABSystem;
using System.Collections.Generic;

public class MainBase : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void GameMain()
    {
        Action init = () =>
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
            Localization.Language = SystemLanguage.Chinese.ToString();
            var manager = Instantiate(Resources.Load<GameObject>(typeof(Manager).Name));
            manager.name = typeof(Manager).Name;
            DontDestroyOnLoad(manager);
            var abManger = manager.GetComponent<AssetBundleManager>();
            abManger.Init(() =>
            {
                UpdateAssetBundleManager.Instance.InitDepFileHelper(() =>
                {
                    if (SceneManager.GetActiveScene().name.Equals("Main"))
                        MessageHandler.CreateInstance();
                });
            });
        };
        if (SceneManager.GetActiveScene().name.Equals("Main"))
        {
            init();
            UIManager.Instance.Open(typeof(UISplash));
        }
        else if (SceneManager.GetActiveScene().name.Equals("Work"))
        {
            init();
            UIManager.Instance.Open(typeof(UITestShowLevel), "game");
            //UIManager.Instance.Open(typeof(UIWork));
        }
    }
}
