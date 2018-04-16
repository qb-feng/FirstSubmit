using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class WZTool : MonoBehaviour
{
    [MenuItem("Tools/Convert Folder To Text")]
    private static void ConvertOneFolderFilesNameToOneText()
    {
        string dir = EditorUtility.OpenFolderPanel("选择一个文件件", Application.dataPath, "*.*");
        var files = Directory.GetFiles(dir, "*.prefab");
        var fs = new FileStream(Path.Combine(dir, "_allName.txt"), FileMode.Create);
        var sw = new StreamWriter(fs);
        foreach (var file in files)
        {
            sw.WriteLine(Path.GetFileName(file));
        }
        sw.Close();
        fs.Close();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Generate Config Json")]
    private static void GenerateConfigJson()
    {
        List<string> files = new List<string>();
        var path = PathEditorWindowBase.GetConfigPath("Excel2JsonConfig");
        var config = AssetDatabase.LoadAssetAtPath<PathConfigScriptable>(path);
        if (config != null)
        {
            foreach (var item in config.configList)
            {
                string configDirectory = Application.dataPath + item.loadPath.TrimStart("Assets".ToCharArray()) + "/Resources/Config";
                files.AddRange(Directory.GetFiles(configDirectory, "*.json"));
            }
        }

        var configList = new List<ConfigModel>();
        foreach (var file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            if (fileName.StartsWith("Config"))
            {
                var configModel = new ConfigModel();
                configModel.name = fileName;
                var splits = Application.version.Split('.');
                int versionNumbers = int.Parse(splits[0]) * 1000 + int.Parse(splits[1]) * 100 + int.Parse(splits[2]) * 10;
                configModel.version = versionNumbers;
                configList.Add(configModel);
            }
        }
        string versionJson = Utility.ArrayToJson(configList);
        string versionPath = Path.Combine(Application.dataPath, "_AppProduct/Config/Resources/Config/ConfigModel.json");
        File.WriteAllText(versionPath, versionJson);
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Convert Picture Format", false, 9)]
    private static void ConvertPictureFormat()
    {
        foreach (var selectAsset in Selection.assetGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(selectAsset);
            if (!string.IsNullOrEmpty(path))
            {
                FindFilesChangeSettings(path, "*.jpg", TextureImporterFormat.ETC2_RGB4);
                FindFilesChangeSettings(path, "*.png", TextureImporterFormat.ETC2_RGBA8);
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("转换完毕: " + System.DateTime.Now.ToLongTimeString());
    }

    private static void FindFilesChangeSettings(string path, string pattern, TextureImporterFormat compress)
    {
        var assetImportList = new List<AssetImporter>();
        var filesList = new List<string>();
        filesList.AddRange(Directory.GetFiles(path, pattern, SearchOption.AllDirectories));
        foreach (var file in filesList)
        {
            var assetImporter = SetPlatFormatSettings(file, compress, "Android", "iPhone");
            if (assetImporter)
                assetImportList.Add(assetImporter);
        }

        foreach (var item in assetImportList)
        {
            item.SaveAndReimport();
        }
    }

    private static AssetImporter SetPlatFormatSettings(string path, TextureImporterFormat format, params string[] platformList)
    {
        bool dirty = false;
        var texImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        var texSettings = new TextureImporterSettings();
        texImporter.ReadTextureSettings(texSettings);
        if (texSettings.mipmapEnabled != false)
            dirty = true;
        texSettings.mipmapEnabled = false;
        if (dirty)
            texImporter.SetTextureSettings(texSettings);

        foreach (var platform in platformList)
        {
            var platformSettings = texImporter.GetPlatformTextureSettings(platform);
            if (platformSettings.overridden != true)
                dirty = true;
            platformSettings.overridden = true;
            if (platformSettings.format != format)
                dirty = true;
            platformSettings.format = format;
            if (dirty)
                texImporter.SetPlatformTextureSettings(platformSettings);
        }

        if (dirty)
        {
            return texImporter;
        }
        return null;
    }

    [MenuItem("Tools/Find Component")]
    private static void FindComponent()
    {
        int startIndex = 0;

        string[] guids = AssetDatabase.FindAssets("t:Prefab");

        EditorApplication.update = delegate ()
        {
            string guid = guids[startIndex];
            string path = AssetDatabase.GUIDToAssetPath(guid);

            bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", guid, (float)startIndex / (float)guids.Length);

            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var cp = go.GetComponentInChildren<AspectRatioFitter>();//指定搜索脚本
            if (cp)
            {
                Debug.Log(path, go);
            }

            startIndex++;
            if (isCancel || startIndex >= guids.Length)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
                startIndex = 0;
                Debug.Log("匹配结束");
            }
        };
    }
}
