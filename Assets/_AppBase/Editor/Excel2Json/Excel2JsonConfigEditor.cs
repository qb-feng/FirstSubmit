using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

public class Excel2JsonConfigEditor : PathEditorWindowBase
{
    [MenuItem("Excel2Json/Config Panel")]
    static void Open()
    {
        SaveConfigPath = GetConfigPath("Excel2JsonConfig");
        PathConfig = null;
        GetWindow<Excel2JsonConfigEditor>("Excel2Json", true);
    }

    protected override void Build()
    {
        base.Build();
        BuildConfig();
    }

    [MenuItem("Excel2Json/Build Config")]
    static void BuildConfig()
    {
        SaveConfigPath = GetConfigPath("Excel2JsonConfig");
        EditorUtils.InitCommand(PathConfig.configList.Count);
        foreach (var item in PathConfig.configList)
        {
            if (string.IsNullOrEmpty(item.loadPath))
                continue;
            string savePath = item.loadPath + "/Resources/Config/";
            Export2Json(item.loadPath + @"/", savePath);
        }
    }

    private static string toolPath = @"Tools\ExcelTool\excel2json.exe";
    private static string ExcelToolPath = "";

    public static void Export2Json(string loadPath, string savePath)
    {
        loadPath = loadPath.Remove(0, "Assets".Length);
        savePath = savePath.Remove(0, "Assets".Length);
        if (string.IsNullOrEmpty(ExcelToolPath))
            ExcelToolPath = GetExcelToolPath();
        var dir = new DirectoryInfo(Application.dataPath);
        var fromPath = string.Concat(dir, loadPath).Replace('/', '\\');
        if (!Directory.Exists(fromPath))
            return;
        var toPath = string.Concat(dir, savePath).Replace('/', '\\');
        var para = "-e " + fromPath + " -j " + toPath + " -h 1";
        //System.Diagnostics.Process.Start(ExcelToolPath, para);
        EditorUtils.RunCommand(ExcelToolPath, para);
    }


    private static string GetExcelToolPath()
    {
        var dir = new DirectoryInfo(Application.dataPath);
        var rootPath = dir.Parent.FullName.Replace('/', '\\');
        var path = Path.Combine(rootPath, toolPath);
        if (File.Exists(path))
            return path;

        return EditorUtility.OpenFilePanel("Select excel2json.exe", "d:\\", "exe");
    }
}
