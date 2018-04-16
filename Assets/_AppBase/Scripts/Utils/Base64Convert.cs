using System;
using System.Security.Cryptography;
using System.IO;
using UnityEngine;

public class Base64Convert
{
    /// <summary>
    /// 秘钥
    /// </summary>
    const string KEY_64 = "OtZ1Fzxd";//注意了，是8个字符，64位
    /// <summary>
    /// 初始化向量
    /// </summary>
    const string IV_64 = "1ZxQcLP0";//可以和上面的不一致 但是一定要是8位

    public static string Encode(string data)
    {
        byte[] byKey = System.Text.Encoding.ASCII.GetBytes(KEY_64);
        byte[] byIV = System.Text.Encoding.ASCII.GetBytes(IV_64);

        DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
        MemoryStream ms = new MemoryStream();
        CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

        StreamWriter sw = new StreamWriter(cst);
        sw.Write(data);
        sw.Flush();
        cst.FlushFinalBlock();
        sw.Flush();
        return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

    }

    public static string Decode(string data)
    {
        byte[] byKey = System.Text.Encoding.ASCII.GetBytes(KEY_64);
        byte[] byIV = System.Text.Encoding.ASCII.GetBytes(IV_64);

        byte[] byEnc;
        try
        {
            byEnc = Convert.FromBase64String(data);
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }

        DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
        MemoryStream ms = new MemoryStream(byEnc);
        CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
        StreamReader sr = new StreamReader(cst);
        return sr.ReadToEnd();
    }
}
