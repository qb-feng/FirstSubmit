using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEditorInternal;

/// <summary>
/// 图集打包工具, 如果图集超过规定的2048 * 2048 则打包图集会失败!!, 
/// 如果在电脑上第一次运行TexturePacker, 则需要打开TexturePack, 统一条款后才可以打包图集, 否则Unity会卡主
/// </summary>
public class TexturePackTool : PathEditorWindowBase
{
    [MenuItem("TexturePacker/Config Panel")]
    static void Open()
    {
        SaveConfigPath = GetConfigPath("TexturePackToolConfig");
        PathConfig = null;
        GetWindow<TexturePackTool>("TexturePackTool", true);
    }

    protected override void Build()
    {
        base.Build();
        BuildConfig();
    }

    [MenuItem("TexturePacker/Atlas Build")]
    static void BuildConfig()
    {
        SaveConfigPath = GetConfigPath("TexturePackToolConfig");
        int totalCount = 0;
        string loadPath = null;
        string savePath = null;
        List<string> atlasList = null;
        List<string> spritesForlderList = null;
        Action<PathConfigScriptable.ConvertModel> action = model =>
        {
            loadPath = model.loadPath.Remove(0, "Assets".Length);
            loadPath = string.Concat(Application.dataPath, loadPath);
            savePath = model.savePath.Remove(0, "Assets".Length);
            savePath = string.Concat(Application.dataPath, savePath);
            atlasList = new List<string>(Directory.GetFiles(savePath));
            spritesForlderList = new List<string>(Directory.GetDirectories(loadPath));
        };
        foreach (var item in PathConfig.configList)
        {
            action(item);
            totalCount += spritesForlderList.Count;
        }

        EditorUtils.InitCommand(totalCount);

        foreach (var item in PathConfig.configList)
        {
            action(item);
            AtlasBuild(savePath, atlasList, spritesForlderList);
        }
        AssetDatabase.Refresh();
    }

    static void AtlasBuild(string savePath, List<string> atlasList, List<string> spritesForlderList)
    {
        foreach (var folder in spritesForlderList)
        {
            string fullSavePath = Path.Combine(savePath, Path.GetFileName(folder));
            string saveSheetPath = fullSavePath + ".tpsheet";
            if (atlasList.Contains(saveSheetPath))
            {
                atlasList.Remove(saveSheetPath);
                atlasList.Remove(saveSheetPath + ".meta");
            }
            string saveTexturePath = fullSavePath + ".png";
            if (atlasList.Contains(saveTexturePath))
            {
                atlasList.Remove(saveTexturePath);
                atlasList.Remove(saveTexturePath + ".meta");
            }
            string format = "--format unity-texture2d";
            string data = "--data " + saveSheetPath;
            string textureFormat = "--texture-format png";
//#if UNITY_IOS || UNITY_ANDROID
//            string pixelFormat = "--opt RGBA4444";
//            string dithering = "--dither-fs-alpha";//fs-alpha
//#endif
            string sheet = "--sheet " + saveTexturePath;
            //string forceSquare = "--force-squared";
            string sizeConstraints = "--size-constraints POT";
            string shapePadding = "--shape-padding 1";
            string borderPadding = "--border-padding 1";
            var commands = new string[] { 
                format,
                data,
                textureFormat,
//#if UNITY_IOS || UNITY_ANDROID
//                pixelFormat,
//                dithering,
//#endif
                sheet,
                //forceSquare,
                sizeConstraints,
                shapePadding,
                borderPadding,
                folder
            };
            RunCommand(commands);
        }

        foreach (var atlas in atlasList)
        {
            File.Delete(atlas);
        }
    }

    private static void RunCommand(params string[] args)
    {
        string fileName = Directory.CreateDirectory(Application.dataPath).Parent.FullName + "/Tools/TexturePacker/Bin/TexturePacker.exe";
        string arguments = "";
        for (int i = 0; i < args.Length; i++)
        {
            arguments += args[i] + " ";
        }
        arguments = arguments.TrimEnd(' ');
        EditorUtils.RunCommand(fileName, arguments);
    }
}