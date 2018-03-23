using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaiDuVoice;

public class RestTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        /*
         * 2018年3月2日17:28:00 测试使用百度语音的restApi：
         * 百度语音识别通过 REST API 的方式给开发者提供一个通用的 HTTP 接口。上传需要完整的录音文件，录音文件时长不超过60s。
         * 
         * 测试过程：将本地音频资源下载解压发送给百度语音，接收百度语音的识别结果，以扩大指定音频的容错率
         * 
         * 测试结果：识别结果与实际相差太大！！！暂时放弃。。。
         */

        BaiDuResManager.Instance.Init();
        string filePath = "D:/qiubin/GitProject/BolyEnglish/Assets/AssetBundles/WordSource/Audio/Book0/Unit1/Lesson1/0_1_1_1_1.wav";
        StartCoroutine(BaiDuResManager.Instance.RecoginzeByFile(filePath, (result) =>
        {
        }));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
