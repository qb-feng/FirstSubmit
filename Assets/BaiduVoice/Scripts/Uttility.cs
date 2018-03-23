using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using NAudio.Wave.WZT;
using NAudio.Wave;

public static class Uttility
{

    /// <summary>
    /// 往音频文件中写入头文件
    /// </summary>
    public static void WriteHeader(string filePath, int sampleRate, int channels)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
            using (var br = new BinaryReader(fs))
            {
                if (System.Text.Encoding.ASCII.GetString(br.ReadBytes(4)) != "RIFF")//WaveInterop.mmioStringToFOURCC("RIFF", 0)
                {
                    fs.Seek(0, SeekOrigin.Begin);

                    Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
                    fs.Write(riff, 0, 4);

                    Byte[] chunkSize = BitConverter.GetBytes(fs.Length + 44 - 8);//头文件总长度44个字节, chunkSize = pcm数据长度 + 44 -8
                    fs.Write(chunkSize, 0, 4);

                    Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
                    fs.Write(wave, 0, 4);

                    Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
                    fs.Write(fmt, 0, 4);

                    Byte[] subChunk1 = BitConverter.GetBytes(16);
                    fs.Write(subChunk1, 0, 4);

                    UInt16 one = 1;//表示PCM

                    Byte[] audioFormat = BitConverter.GetBytes(one);
                    fs.Write(audioFormat, 0, 2);

                    Byte[] numChannels = BitConverter.GetBytes(channels);
                    fs.Write(numChannels, 0, 2);

                    Byte[] sampleRateBytes = BitConverter.GetBytes(sampleRate);
                    fs.Write(sampleRateBytes, 0, 4);

                    Byte[] byteRate = BitConverter.GetBytes(sampleRate * channels * 2); // 其值为通道数×每秒数据位数×每样本的数据位数／8(默认16位)
                    fs.Write(byteRate, 0, 4);

                    UInt16 blockAlign = (ushort)(channels * 2);//其值为通道数×每样本的数据位值／8(默认16位)
                    fs.Write(BitConverter.GetBytes(blockAlign), 0, 2);

                    Byte[] bitsPerSample = BitConverter.GetBytes(16);//每样本的数据位数(默认16位)
                    fs.Write(bitsPerSample, 0, 2);

                    Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
                    fs.Write(datastring, 0, 4);

                    Byte[] subChunk2 = BitConverter.GetBytes(fs.Length * channels);
                    fs.Write(subChunk2, 0, 4);

                }
            }
        }
    }

    /// <summary>
    /// 将Unity的AudioClip数据转化为PCM格式16bit数据
    /// </summary>
    public static byte[] ConvertAudioClipToPCM16(AudioClip clip)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);
        var samples_int16 = new short[samples.Length];

        for (var index = 0; index < samples.Length; index++)
        {
            var f = samples[index];
            samples_int16[index] = (short)(f * short.MaxValue);
        }

        var byteArray = new byte[samples_int16.Length * 2];
        Buffer.BlockCopy(samples_int16, 0, byteArray, 0, byteArray.Length);

        return byteArray;
    }

    /// <summary>
    /// MP3转wav格式
    /// </summary>
    public static void Mp3ToWav(string mp3File, string outputFile)
    {
        //using (Mp3FileReader reader = new Mp3FileReader(mp3File))
        //{
        //    WaveFileWriter.CreateWaveFile(outputFile, reader);
        //}
    }

    public static byte[] GetData(this AudioClip clip)
    {
        var data = new float[clip.samples * clip.channels];

        clip.GetData(data, 0);

        byte[] bytes = new byte[data.Length * 4];
        Buffer.BlockCopy(data, 0, bytes, 0, bytes.Length);

        return bytes;
    }

    public static void SetData(this AudioClip clip, byte[] bytes)
    {
        float[] data = new float[bytes.Length / 4];
        Buffer.BlockCopy(bytes, 0, data, 0, bytes.Length);

        clip.SetData(data, 0);
    }

    public static byte[] GetData16(this AudioClip clip)
    {
        var data = new float[clip.samples * clip.channels];

        clip.GetData(data, 0);

        byte[] bytes = new byte[data.Length * 2];

        int rescaleFactor = 32767;

        for (int i = 0; i < data.Length; i++)
        {
            short value = (short)(data[i] * rescaleFactor);
            BitConverter.GetBytes(value).CopyTo(bytes, i * 2);
        }

        return bytes;
    }

    /// <summary>
    /// 将WAV格式的AudioClip转换成byte[]
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public static byte[] EncodeToWAV(this AudioClip clip)
    {
        byte[] bytes = null;

        using (var memoryStream = new MemoryStream())
        {
            memoryStream.Write(new byte[44], 0, 44);//预留44字节头部信息

            byte[] bytesData = clip.GetData16();

            memoryStream.Write(bytesData, 0, bytesData.Length);

            memoryStream.Seek(0, SeekOrigin.Begin);

            byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            memoryStream.Write(riff, 0, 4);

            byte[] chunkSize = BitConverter.GetBytes(memoryStream.Length - 8);
            memoryStream.Write(chunkSize, 0, 4);

            byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            memoryStream.Write(wave, 0, 4);

            byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            memoryStream.Write(fmt, 0, 4);

            byte[] subChunk1 = BitConverter.GetBytes(16);
            memoryStream.Write(subChunk1, 0, 4);

            UInt16 two = 2;
            UInt16 one = 1;

            byte[] audioFormat = BitConverter.GetBytes(one);
            memoryStream.Write(audioFormat, 0, 2);

            byte[] numChannels = BitConverter.GetBytes(clip.channels);
            memoryStream.Write(numChannels, 0, 2);

            byte[] sampleRate = BitConverter.GetBytes(clip.frequency);
            memoryStream.Write(sampleRate, 0, 4);

            byte[] byteRate = BitConverter.GetBytes(clip.frequency * clip.channels * 2); // sampleRate * bytesPerSample*number of channels
            memoryStream.Write(byteRate, 0, 4);

            UInt16 blockAlign = (ushort)(clip.channels * 2);
            memoryStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

            UInt16 bps = 16;
            byte[] bitsPerSample = BitConverter.GetBytes(bps);
            memoryStream.Write(bitsPerSample, 0, 2);

            byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
            memoryStream.Write(datastring, 0, 4);

            byte[] subChunk2 = BitConverter.GetBytes(clip.samples * clip.channels * 2);
            memoryStream.Write(subChunk2, 0, 4);

            bytes = memoryStream.ToArray();
        }

        return bytes;
    }
}
