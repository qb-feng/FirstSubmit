using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.IO;

public static class ExtendUtility
{
    /// <summary>
    /// 扩展Transform类设置位置X值
    /// </summary>
    /// <param name="t"></param>
    /// <param name="x"></param>
    public static void SetP_x(this Transform t, float x)
    {
        Vector3 temp = new Vector3(x, t.position.y, t.position.z);
        t.position = temp;
    }

    public static void SetLP_x(this Transform t, float x)
    {
        Vector3 temp = new Vector3(x, t.localPosition.y, t.localPosition.z);
        t.localPosition = temp;
    }

    /// <summary>
    /// 扩展Transform类设置位置Y值
    /// </summary>
    /// <param name="t"></param>
    /// <param name="x"></param>
    public static void SetP_y(this Transform t, float y)
    {
        Vector3 temp = new Vector3(t.position.x, y, t.position.z);
        t.position = temp;
    }

    public static void SetLP_y(this Transform t, float y)
    {
        Vector3 temp = new Vector3(t.localPosition.x, y, t.localPosition.z);
        t.localPosition = temp;
    }

    /// <summary>
    /// 扩展Transform类设置位置Z值
    /// </summary>
    /// <param name="t"></param>
    /// <param name="x"></param>
    public static void SetP_z(this Transform t, float z)
    {
        Vector3 temp = new Vector3(t.position.x, t.position.y, z);
        t.position = temp;
    }

    public static void SetLP_z(this Transform t, float z)
    {
        Vector3 temp = new Vector3(t.localPosition.x, t.localPosition.y, z);
        t.localPosition = temp;
    }

    public static void ResetLocal(this Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }

    public static void ClearAllChild(this Transform t)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            GameObject.Destroy(t.GetChild(i).gameObject);
        }
        t.DetachChildren();
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[1];
            do provider.GetBytes(box);
            while (!(box[0] < n * (Byte.MaxValue / n)));
            int k = (box[0] % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public delegate void InvokeAction();
    public static void WaitSecond(this MonoBehaviour mono, InvokeAction action, float delay)
    {
        mono.StartCoroutine(DelayAction(action, delay));
    }

    public static void WaitOneFrame(this MonoBehaviour mono, InvokeAction action)
    {
        mono.StartCoroutine(DelayOneFrameAction(action, 1));
    }

    public static void WaitFrames(this MonoBehaviour mono, InvokeAction action, int frames)
    {
        mono.StartCoroutine(DelayOneFrameAction(action, frames));
    }

    private static IEnumerator DelayOneFrameAction(InvokeAction action, int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return 0;
        }
        action();
    }

    private static IEnumerator DelayAction(InvokeAction action, float time)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    public static string GetFullHierarchy(this Transform t)
    {
        string path = t.name;
        while (t.parent)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    public static T LoadAsset<T>(this AssetBundle ab) where T : UnityEngine.Object
    {
        string[] names = ab.GetAllAssetNames();
        var asset = ab.LoadAsset<T>(names[0]);
        return asset;
    }

    // Only useful before .NET 4
    public static void CopyTo(this Stream input, Stream output)
    {
        byte[] buffer = new byte[16 * 1024]; // Fairly arbitrary size
        int bytesRead;

        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, bytesRead);
        }
    }
}
