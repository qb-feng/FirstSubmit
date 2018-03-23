using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BaiDuVoice;
using System;
namespace BaiDuVoice
{
    public class Test : MonoBehaviour
    {

        public Button start;
        public Button cancel;
        public Button end;
        public Button dispose;
        public Button init;
        public Text resultText;

        // Use this for initialization
        void Start()
        {
            var s = BaiDuAsrManger.Instance;

            init.onClick.AddListener(() =>
            {
                //初始化
                BaiDuAsrManger.Instance.Init();
            });

            start.onClick.AddListener(() =>
            {
                //使用默认设置
                BaiDuAsrManger.Instance.StartRecord(null, _onEndRecognizeResultComplete: (result) =>
                {
                    resultText.text = result.result;
                    StartCoroutine(YieldPlayMp3(result.audioFilePath, null));
                });

            });
            cancel.onClick.AddListener(() =>
            {
                BaiDuAsrManger.Instance.CancelRecord();
            });
            end.onClick.AddListener(() =>
            {
                BaiDuAsrManger.Instance.StopRecord();
            });
            dispose.onClick.AddListener(() =>
            {
                BaiDuAsrManger.Instance.Release();
            });
        }

        /// <summary>
        /// 下载玩家的录音
        /// </summary>
        private IEnumerator YieldPlayMp3(string filePath, Action complete)
        {
            WWW www = new WWW("file:///" + filePath);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                var audioClip = www.GetAudioClipCompressed();//下载返回经过压缩的音频
                audioClip.name = filePath;

                Debug.LogWarning(filePath + "下载成功！" + "下载音频为:" + audioClip.ToString());

                GameObject go = new GameObject("Audio: " + audioClip.name);
                go.transform.position = Vector3.zero;

                AudioSource source = go.AddComponent<AudioSource>();
                source.clip = audioClip;
                source.volume = 1;
                source.pitch = 1;
                source.Play();

                Debug.LogWarning("播放成功！1111111111111111111111" + source);
                if (complete != null)
                {
                    complete();
                }
            }
            else
            {
                Debug.LogWarning("YieldPlayMp3 Error : " + www.error);
            }
        }

    }
}
