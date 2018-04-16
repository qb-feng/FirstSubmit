using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Collections;

public static class Utility
{
    private static Dictionary<string, Color> s_colorDic;
    public static Dictionary<string, Color> ColorDic
    {
        get
        {
            if (s_colorDic == null)
            {
                s_colorDic = new Dictionary<string, Color>();
                s_colorDic.Add("gray", Color.gray);
                s_colorDic.Add("yellow", Color.yellow);
                s_colorDic.Add("white", Color.white);
                s_colorDic.Add("green", Color.green);
                s_colorDic.Add("red", Color.red);
                s_colorDic.Add("blue", Color.blue);
                s_colorDic.Add("pink", new Color(255 / 255f, 192 / 255f, 203 / 255f));
                s_colorDic.Add("black", Color.black);
                s_colorDic.Add("orange", new Color(255 / 255f, 165 / 255f, 0));
                s_colorDic.Add("brown", new Color(165 / 255f, 42 / 255f, 42 / 255f));
            }

            return s_colorDic;
        }
    }

    public static System.Random s_random = new System.Random();

    public static List<T> RandomSortList<T>(List<T> listT)
    {
        if (listT == null)
            return null;
        List<T> temp = null;
        for (int i = 0; i < 10; i++)
        {
            temp = RandomSortListInternal(listT);
        }
        return temp;
    }

    private static List<T> RandomSortListInternal<T>(List<T> listT)
    {
        if (listT == null)
            return null;
        List<T> newList = new List<T>();
        foreach (T item in listT)
        {
            newList.Insert(s_random.Next(newList.Count + 1), item);
        }
        return newList;
    }

    public static T[] RandomSortList<T>(T[] array)
    {
        if (array == null)
            return null;
        List<T> newList = new List<T>();
        newList.AddRange(array);
        return RandomSortList(newList).ToArray();
    }

    public static void ClearChild(Transform t)
    {
        foreach (Transform item in t)
        {
            GameObject.Destroy(item.gameObject);
        }
        t.DetachChildren();
    }

    /// <summary>
    /// json数据必须是以[] 中括号开头和结尾的数组数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    public static List<T> ArrayFromJson<T>(string json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            json = "{\"list\":" + json + "}";
            ArrayJsonDataModel<T> model = JsonUtility.FromJson<ArrayJsonDataModel<T>>(json);
            return model == null ? null : model.list;
        }
        else return null;
    }

    /// <summary>
    /// Unity JsonUtility 不能直接序列化数组或者List, 需要包裹一个对象才可以. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string ArrayToJson<T>(List<T> list)
    {
        ArrayJsonDataModel<T> model = new ArrayJsonDataModel<T>();
        model.list = list;
        string json = JsonUtility.ToJson(model);
        json = json.Substring(0, json.Length - 1);
        json = json.Remove(0, "{\"list\":".Length);
        return json;
    }

    public static float VectorAngle(Vector2 from, Vector2 to)
    {
        Vector3 cross = Vector3.Cross(from, to);
        float angle = Vector2.Angle(from, to);
        return cross.z > 0 ? -angle : angle;
    }

    public static long GetUnixTimeSecond(DateTime time)
    {
        long ecpo = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime().Ticks;
        long ms = 10000000;
        return (time.ToUniversalTime().Ticks - ecpo) / ms;
    }

    public static DateTime GetLocalTimeFromSecond(long time)
    {
        long ecpo = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime().Ticks;
        long ms = 10000000;
        return new DateTime(time * ms + ecpo).ToLocalTime();
    }

    public static DateTime GetLocalTimeFromMillisecond(long time)
    {
        long ecpo = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime().Ticks;
        long ms = 10000;
        return new DateTime(time * ms + ecpo).ToLocalTime();
    }
    /// <summary>
    /// 截取实际长度
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="n">规定长度</param>
    /// <returns></returns>
    public static string StringLimit(string str, int n)
    {
        string temp = string.Empty;
        if (System.Text.Encoding.Default.GetByteCount(str) <= n)//如果长度比需要的长度n小,返回原字符串
        {
            return str;
        }
        else
        {
            int t = 0;
            char[] q = str.ToCharArray();
            for (int i = 0; i < q.Length && t < n; i++)
            {
                if ((int)q[i] >= 0x4E00 && (int)q[i] <= 0x9FA5)//是否汉字
                {
                    temp += q[i];
                    t += 2;
                }
                else
                {
                    temp += q[i];
                    t++;
                }
            }
            return temp;
        }
    }

    public static ScreenOrientation ConvertDeviceOrientation(DeviceOrientation device)
    {
        switch (device)
        {
            case DeviceOrientation.LandscapeLeft:
                return ScreenOrientation.LandscapeLeft;
            case DeviceOrientation.LandscapeRight:
                return ScreenOrientation.LandscapeRight;
            case DeviceOrientation.Portrait:
                return ScreenOrientation.Portrait;
            case DeviceOrientation.PortraitUpsideDown:
                return ScreenOrientation.PortraitUpsideDown;
            default:
                return ScreenOrientation.Unknown;
        }
    }

    public static string CombineUrl(string uri1, string uri2)
    {
        uri1 = uri1.TrimEnd('/');
        uri2 = uri2.TrimStart('/');
        return string.Format("{0}/{1}", uri1, uri2);
    }

    public static List<int> GetRandomSequence(int total, int n)
    {
        //随机总数组  
        int[] sequence = new int[total];
        //取到的不重复数字的数组长度
        int[] output = new int[n];
        for (int i = 0; i < total; i++)
        {
            sequence[i] = i;
        }
        int end = total - 1;
        for (int i = 0; i < n; i++)
        {
            //随机一个数，每随机一次，随机区间-1  
            int num = UnityEngine.Random.Range(0, end + 1);
            output[i] = sequence[num];
            //将区间最后一个数赋值到取到数上  
            sequence[num] = sequence[end];
            end--;
            //执行一次效果如：1，2，3，4，5 取到2  
            //则下次随机区间变为1,5,3,4;  
        }
        return new List<int>(output);
    }

    public static List<T> RandomCombine<T>(T value, List<T> randomList, int count)
    {
        return RandomCombine(new List<T>() { value }, randomList, count);
    }

    public static List<T> RandomCombine<T>(List<T> value, List<T> randomList, int count)
    {
        var temp = new List<T>();
        temp.AddRange(randomList);
        for (int i = 0; i < value.Count; i++)
        {
            temp.Remove(value[i]);
            temp.Insert(i, value[i]);
        }
        var ret = new List<T>();
        for (int i = 0; i < count; i++)
        {
            ret.Add(temp[i]);
        }
        ret = RandomSortList(ret);
        return ret;
    }

    public static Color ColorMix(Color color, Color color_2)
    {
        float[] cv = new float[] { color.r, color.g, color.b, color_2.r, color_2.g, color_2.b, 0, 0, 0 };
        for (int i = 0; i < 3; i++)
        {
            cv[i + 6] = 255 - (255 - cv[i + 0]) * (255 - cv[i + 3]) / 255;
        }
        return new Color(cv[6], cv[7], cv[8]);
    }

    /// <summary>
    /// color 转换hex
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        int a = Mathf.RoundToInt(color.a * 255.0f);
        string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
        return hex;
    }

    /// <summary>
    /// hex转换到color
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static Color HexToColor(string hex)
    {
        byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        float r = br / 255f;
        float g = bg / 255f;
        float b = bb / 255f;
        float a = cc / 255f;
        return new Color(r, g, b, a);
    }

    public static string GetBookId(string id)
    {
        return id.Split('_')[0];
    }

    public static string GetUnitId(string id)
    {
        string bookId = GetBookId(id);
        string unit = id.Split('_')[1];
        return bookId + "_" + unit;
    }

    public static string GetLessonId(string id)
    {
        string unitId = GetUnitId(id);
        string lesson = id.Split('_')[2];
        return unitId + "_" + lesson;
    }
}
