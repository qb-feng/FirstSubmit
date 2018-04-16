using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AtlasToPrefab : MonoBehaviour
{
    //[MenuItem("AtlasMaker/AtlasMaker")]
    //private static void MakeAtlas()
    //{
    //    string spriteDir = Application.dataPath + "/Resources";

    //    if (!Directory.Exists(spriteDir))
    //    {
    //        Directory.CreateDirectory(spriteDir);
    //    }

    //    GameObject go = new GameObject("AtlasSprite");
    //    AtlasSprite test1 = go.AddComponent<AtlasSprite>();

    //    List<DirectoryInfo> dirInfos = new List<DirectoryInfo>();
    //    GetAllDirList(Application.dataPath + "/Atlas", dirInfos);
    //    foreach (DirectoryInfo dirInfo in dirInfos)
    //    {
    //        FileInfo[] pngFiles = dirInfo.GetFiles();
    //        foreach (FileInfo pngFile in pngFiles)
    //        {
    //            if (pngFile.Extension.Equals(".png") || pngFile.Extension.Equals(".jpg")) { }
    //            else
    //                continue;
    //            string allPath = pngFile.FullName;
    //            string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
    //            Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath);
    //            foreach (Object item in sprites)
    //            {
    //                if (item is Sprite)
    //                    test1.atlas.Add((Sprite)item);
    //            }
    //        }
    //    }
    //    string fullPath = spriteDir + "/AtlasSprite.prefab";
    //    string prefabPath = fullPath.Substring(fullPath.IndexOf("Assets"));
    //    PrefabUtility.CreatePrefab(prefabPath, go);
    //    GameObject.DestroyImmediate(go);
    //    Debug.Log("Atlas Make Finish!");
    //}

    //public static void GetAllDirList(string dir, List<DirectoryInfo> al)
    //{
    //    DirectoryInfo di = new DirectoryInfo(dir);
    //    DirectoryInfo[] diA = di.GetDirectories();
    //    for (int i = 0; i < diA.Length; i++)
    //    {
    //        al.Add(diA[i]);
    //        GetAllDirList(diA[i].FullName, al);
    //    }
    //}
}
