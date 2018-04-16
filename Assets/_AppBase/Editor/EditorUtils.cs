using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;
using UnityEditor;
using UnityEngine.UI;

public class EditorUtils : MonoBehaviour
{
    private static int commandTotalCount = 0;
    private static int commandCompleteCount = 0;
    private static MonoBehaviour CommandObj;

    public static void InitCommand(int totalCount)
    {
        commandTotalCount = 0;
        commandCompleteCount = 0;
        var go = new GameObject();
        CommandObj = go.AddComponent<Button>();
        ShowLoading(0);
        commandTotalCount = totalCount;
        CommandObj.StartCoroutine(CheckComplete());
    }

    private static void ShowLoading(float progress)
    {
        EditorUtility.DisplayProgressBar("Building", "Waiting...", progress);
    }

    private static IEnumerator CheckComplete()
    {
        while (true)
        {
            if (!RefreshLoading())
            {
                break;
            }
            yield return 0;
        }
    }

    private static bool RefreshLoading()
    {
        ShowLoading(commandCompleteCount / (float)commandTotalCount);
        if (commandCompleteCount == commandTotalCount)
        {
            EditorUtility.ClearProgressBar();
            DestroyImmediate(CommandObj.gameObject);
            return false;
        }
        return true;
    }

    public static void RunCommand(string fileName, string arguments)
    {
        Process p = new Process();
        p.StartInfo.FileName = fileName;
        p.StartInfo.Arguments = arguments;
        p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
        p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
        p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
        p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
        p.StartInfo.CreateNoWindow = true;//不显示程序窗口
        p.Start();//启动程序

        //获取cmd窗口的输出信息
        string output = p.StandardOutput.ReadToEnd();

        p.Exited += (object sender, EventArgs e) => commandCompleteCount++;
        p.WaitForExit();//等待程序执行完退出进程
        p.Close();

        UnityEngine.Debug.Log(output);
    }
}
