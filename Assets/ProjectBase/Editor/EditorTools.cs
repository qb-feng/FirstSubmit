using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;

/// <summary>
/// 编辑器工具
/// </summary>
public class EditorTools : MonoBehaviour
{
    [MenuItem("EditorTools/UNZipRarFile")]
    public static void UNZipRarFile()
    {
        //提供对本地和远程进程的访问权限并使你能够启动和停止本地系统进程。
        Process p = new Process();
        //设置要启动的应用程序
        p.StartInfo.FileName = "cmd.exe";

        //是否使用操作系统shell启动
        p.StartInfo.UseShellExecute = false;
        //接收来自调用应用程序的输入信息
        p.StartInfo.RedirectStandardInput = true;
        //输出信息
        p.StartInfo.RedirectStandardOutput = true;
        //输出错误
        p.StartInfo.RedirectStandardError = true;
        //不显示显示程序窗口
        p.StartInfo.CreateNoWindow = false;
        //启动程序
        p.Start();

        //目标路径（要解压的文件路径）
        string targetFilePath = Application.dataPath + "/Plugins/IOS/libBaiduSpeechSDK.a";
        if (File.Exists(targetFilePath))
        {
            Debug(targetFilePath + "文件已不存在！不需要解压了！");
            p.Close();
            return;
        }
        //源路径
        string sourceFilePath = Application.streamingAssetsPath + "/libBaiduSpeechSDK.rar";
        if (!File.Exists(sourceFilePath))
        {
            Debug(sourceFilePath + "源文件不存在！不能解压！");
            p.Close();
            return;
        }

        //dos下的原路径
        string sourceFilePathToDos = sourceFilePath.Replace('/', '\\');
        string targetFilePathToDos = Application.dataPath + "/Plugins/IOS".Replace('/', '\\');


        p.StandardInput.WriteLine("cd " + targetFilePathToDos);
        //p.StandardInput.WriteLine("start winrar a text2222 first.txt");
        p.StandardInput.WriteLine("start winrar x " + sourceFilePathToDos + " " + targetFilePathToDos);

        p.StandardInput.AutoFlush = true;


        //获取输出信息
        string strOutPut = p.StandardOutput.ReadToEnd();

        //等待程序执行完退出进程
        p.WaitForExit();

        //关闭dos命令
        p.Close();
        Debug(strOutPut);
    }

    /// <summary>
    /// 在整个场景的加载前会运行该方法
    /// </summary>
    [UnityEngine.RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void OnGameStart()
    {
        UNZipRarFile();
    }

    public static void Debug(string str)
    {
        UnityEngine.Debug.LogWarning(str);
    }
}
