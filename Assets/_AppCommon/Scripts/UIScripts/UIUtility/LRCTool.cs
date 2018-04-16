using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System;

public class LrcTool
{
    public class Word
    {
        public float start;
        public float end;
        public string word;
    }

    /// <summary>
    /// 歌曲
    /// </summary>
    public string Title;
    /// <summary>
    /// 艺术家
    /// </summary>
    public string Artist;
    /// <summary>
    /// 专辑
    /// </summary>
    public string Album;
    /// <summary>
    /// 歌词作者
    /// </summary>
    public string LrcBy;
    /// <summary>
    /// 偏移量
    /// </summary>
    public string Offset;

    /// <summary>
    /// key表示时间, value表示歌词
    /// </summary>
    //public Dictionary<double, string> LrcWord = new Dictionary<double, string>();

    //public List<KeyValuePair<double, string>> LrcWordList = new List<KeyValuePair<double, string>>();

    public List<Word> LrcWordList = new List<Word>();

    /// <summary>
    /// 获得歌词信息
    /// </summary>
    /// <param name="LrcPath">歌词路径</param>
    /// <returns>返回歌词信息(Lrc实例)</returns>
    public static LrcTool InitLrc(string LrcPath)
    {
        LrcTool lrc = null;
        using (FileStream fs = new FileStream(LrcPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.Default))
            {
                lrc = InitLrc(sr);
            }
        }
        return lrc;
    }

    /// <summary>
    /// 获得歌词信息
    /// </summary>
    /// <param name="bytes">字节数组</param>
    /// <returns></returns>
    public static LrcTool InitLrc(byte[] bytes)
    {
        LrcTool lrc = null;
        using (Stream stream = new MemoryStream(bytes))
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                lrc = InitLrc(reader);
            }
        }
        return lrc;
    }

    /// <summary>
    /// 获得歌词信息
    /// </summary>
    /// <param name="sr">流读取</param>
    /// <returns></returns>
    public static LrcTool InitLrc(StreamReader sr)
    {
        LrcTool lrc = new LrcTool();
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            if (line.StartsWith("[ti:"))
            {
                lrc.Title = SplitInfo(line);
            }
            else if (line.StartsWith("[ar:"))
            {
                lrc.Artist = SplitInfo(line);
            }
            else if (line.StartsWith("[al:"))
            {
                lrc.Album = SplitInfo(line);
            }
            else if (line.StartsWith("[by:"))
            {
                lrc.LrcBy = SplitInfo(line);
            }
            else if (line.StartsWith("[offset:"))
            {
                lrc.Offset = SplitInfo(line);
            }
            else if (line.StartsWith("horizontal"))
            {
                lrc.LrcWordList.Add(new Word { start = 0, end = 9, word = "horizontal" });
            }
            else if (line.StartsWith("vertical"))
            {
                lrc.LrcWordList.Add(new Word { start = 0, end = 9, word = "vertical" });
            }
            else
            {
                Regex regex = new Regex(@"\[([0-9.:]*)\]+\[([0-9.:]*)\]+(.*)");
                MatchCollection mc = regex.Matches(line);
                double startTime = TimeSpan.Parse("00:" + mc[0].Groups[1].Value).TotalSeconds;
                double endTime = TimeSpan.Parse("00:" + mc[0].Groups[2].Value).TotalSeconds;
                string word = mc[0].Groups[3].Value;
                //lrc.LrcWord.Add(time, word);
                lrc.LrcWordList.Add(new Word { start = (float)startTime, end = (float)endTime, word = word });
            }
        }
        return lrc;
    }

    /// <summary>
    /// 处理信息(私有方法)
    /// </summary>
    /// <param name="line"></param>
    /// <returns>返回基础信息</returns>
    static string SplitInfo(string line)
    {
        return line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
    }
}
