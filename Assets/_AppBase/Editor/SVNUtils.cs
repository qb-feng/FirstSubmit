using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SVNUtils
{
//#if !UNITY_EDITOR_OSX
//    private static List<string> drives = new List<string>() { "c:", "d:", "e:", "f:" };
//    private static string svnPath = @"\Program Files\TortoiseSVN\bin\";
//    private static string svnProc = @"TortoiseProc.exe";
//    private static string svnProcPath = "";

//    [MenuItem("Assets/SVN更新 %&e")]
//    public static void UpdateFromSVN()
//    {
//        DoSvnCommond("update", true);
//    }

//    [MenuItem("Assets/SVN提交 %&C")]
//    public static void CommitCurrent()
//    {
//        DoSvnCommond("commit", true);
//    }

//    [MenuItem("Assets/SVN还原 %&t")]
//    public static void RevertFromSVN()
//    {
//        DoSvnCommond("revert", true);
//    }

//    [MenuItem("Assets/SVN添加 %&u")]
//    public static void AddToSVN()
//    {
//        DoSvnCommond("add", true);
//    }

//    [MenuItem("Assets/SVN清理 %&y")]
//    public static void CleanUpFromSVN()
//    {
//        DoSvnCommond("cleanup", false);
//    }

//    [MenuItem("Assets/SVN日志 %&q")]
//    public static void ShowLogFromSVN()
//    {
//        DoSvnCommond("log", false);
//    }

//    private static string GetSvnProcPath()
//    {
//        foreach (var item in drives)
//        {
//            var path = string.Concat(item, svnPath, svnProc);
//            if (File.Exists(path))
//                return path;
//        }
//        return EditorUtility.OpenFilePanel("Select TortoiseProc.exe", "c:\\", "exe");
//    }

//    private static void DoSvnCommond(string commond, bool meta)
//    {
//        string[] guids = Selection.assetGUIDs;
//        if (guids.Length > 0)
//        {
//            string selectPath = "";
//            if (string.IsNullOrEmpty(svnProcPath))
//                svnProcPath = GetSvnProcPath();
//            foreach (var guid in guids)
//            {
//                var path = AssetDatabase.GUIDToAssetPath(guid);
//                if (Directory.Exists(path) || File.Exists(path))
//                {
//                    selectPath += path + "*";
//                    if (meta)
//                    {
//                        string metaPath = path + ".meta";
//                        if (File.Exists(metaPath))
//                            selectPath += metaPath + "*";
//                    }
//                }
//            }
//            //Debug.Log(selectPath);
//            commond = string.Format("/command:{0} /path:\"{1}\"", commond, selectPath);
//            System.Diagnostics.Process.Start(svnProcPath, commond);
//        }
//    }
//#endif
}