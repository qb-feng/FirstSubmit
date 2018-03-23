using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;

namespace BaiDuVoice
{
    /// <summary>
    /// 百度语音Rest API 语音识别工具（在线语言识别）
    /// </summary>
    public class BaiDuResManager : SiginBase<BaiDuResManager>
    {
        /// <summary>
        /// 当前获取的百度语音授权的类型
        /// </summary>
        protected enum TokenFetchStatus
        {
            /// <summary>
            /// //没有申请
            /// </summary>
            NotFetched,

            /// <summary>
            /// //申请中
            /// </summary>
            Fetching,

            /// <summary>
            /// //申请成功
            /// </summary>
            Success,

            /// <summary>
            /// //申请失败
            /// </summary>
            Failed,
        }

        #region 基本数据
        /// <summary>
        /// 用户解析token的json数据
        /// </summary>
        class TokenResponse
        {
            public string access_token = null;
        }
        /// <summary>
        /// 当前百度语音的授权状态
        /// </summary>
        protected TokenFetchStatus CurrentTokenFetchStatus { get; private set; }
        #endregion


        #region 项目基本参数
        public const string APP_ID = "10768340";
        public const string API_KEY = "dwoIh7AS1sknuVT2ztqvTzGP";
        public const string SECRET_KEY = "9d9e5d5a856d13cb170181d1cec91767";
        protected string Token { get; private set; }
        private const string UrlAsr = "https://vop.baidu.com/server_api";
        #endregion

        /// <summary>
        /// 百度语音Rest识别器初始化 - 向百度语音申请授权
        /// </summary>
        public void Init()
        {
            CurrentTokenFetchStatus = TokenFetchStatus.NotFetched;//状态设置为没有申请
            StartCoroutine(GetAccessToken());//申请授权
        }


        /// <summary>
        /// 传入本地单个音频文件路径进行语言识别
        /// </summary>
        public IEnumerator RecoginzeByFile(string filePath, Action<string> onComplete = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("filePath is Null !" + filePath);
                yield break;
            }
            if (filePath.EndsWith(".mp3"))
            {
                string file = filePath.Replace(".mp3", ".wav");
                Uttility.Mp3ToWav(filePath, file);
                //转换成功
                filePath = file;
                Debug.Log("mp3 ->wav格式转换成功！" + filePath);
            }

            //下载文件
            WWW www = new WWW("file:///" + filePath);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                AudioClip audioClip = www.GetAudioClip();//下载本地音频
                GameObject go = new GameObject("audioClip");
                var asource = go.AddComponent<AudioSource>();
                asource.clip = audioClip;
                asource.Play();

                //  byte[] data = Uttility.ConvertAudioClipToPCM16(asource.clip);//将音频转换成PCM16k采样率的data数组
                var data = asource.clip.EncodeToWAV();
                //System.IO.File.Delete(filePath);
                //开始识别
                yield return RecognizeByRaw(data, (d) =>
                {
                    if (onComplete != null && d.result.Length >= 1)
                    {
                        Debug.Log(filePath + " 音频识别结果为：" + d.result[0]);
                        onComplete(d.result[0]);
                    }
                });
            }
            else
            {
                Debug.LogWarning("YieldPlayMp3 Error : " + www.error + "wwwUrl:" + www.url);
            }

        }


        /// <summary>
        /// Json方式发送语音给百度语音服务器识别
        /// 数据需要，base64编码后，数据会增大1/3。
        /// </summary>
        protected IEnumerator RecogiiceByJson(byte[] data, Action<AsrResponse> callback)
        {
            //先检测当前是否获取了百度语音的授权
            yield return PreAction();

            if (CurrentTokenFetchStatus == TokenFetchStatus.Failed)
            {
                Debug.LogError("Token fetched failed, please check your APIKey and SecretKey");
                yield break;
            }
            Debug.LogWarning("开始使用Json格式Post转换语音！");

            //固定参数
            var headers = new Dictionary<string, string> { { "Content-Type", "audio/pcm;rate=16000" } };
            var url = UrlAsr;
            var token = Token;

            //变化参数
            string format = "pcm";
            int rate = 16000;
            int channel = 1;
            string cuid = "915";
            string lan = "en";
            string speech = System.Convert.ToBase64String(data);//音频数据必须经过base64编码  - 注意：base64编码之后，数据会增大1/3
            int len = data.Length;

            //设置参数
            JsonWriter jsonWriter = new JsonWriter();
            jsonWriter.WriteObjectStart();
            jsonWriter.WritePropertyName("format");
            jsonWriter.Write(format);
            jsonWriter.WritePropertyName("rate");
            jsonWriter.Write(rate);
            jsonWriter.WritePropertyName("channel");
            jsonWriter.Write(channel);
            jsonWriter.WritePropertyName("cuid");
            jsonWriter.Write(cuid);
            jsonWriter.WritePropertyName("token");
            jsonWriter.Write(token);
            jsonWriter.WritePropertyName("lan");
            jsonWriter.Write(lan);
            jsonWriter.WritePropertyName("speech");
            jsonWriter.Write(speech);
            jsonWriter.WritePropertyName("len");
            jsonWriter.Write(len);
            jsonWriter.WriteObjectEnd();

            //开始发送
            WWW www = new WWW(url, System.Text.Encoding.Default.GetBytes(jsonWriter.ToString()));
            yield return www;
            if (www.isDone)
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    Debug.Log("语音识别成功！" + www.text);
                    var result = JsonUtility.FromJson<AsrResponse>(www.text);
                    //检测结果
                    if (CheckReuslt(result))
                    {
                        callback(result);
                    }
                }
                else
                {
                    Debug.LogError("语音识别发送出错！" + www.error);
                }
            }

        }


        /// <summary>
        /// raw方式识别语音
        /// </summary>
        protected IEnumerator RecognizeByRaw(byte[] data, Action<AsrResponse> callback)
        {
            //先检测当前是否获取了百度语音的授权
            yield return PreAction();

            if (CurrentTokenFetchStatus == TokenFetchStatus.Failed)
            {
                Debug.LogError("Token fetched failed, please check your APIKey and SecretKey");
                yield break;
            }
            Debug.LogWarning("开始使用raw格式Post转换语音！");

            var uri = string.Format("{0}?lan=en&cuid={1}&token={2}", UrlAsr, SystemInfo.deviceUniqueIdentifier, Token);

            var headers = new Dictionary<string, string> { { "Content-Type", "audio/pcm;rate=16000" } };

            var www = new WWW(uri, data, headers);
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.text);
                var result = JsonUtility.FromJson<AsrResponse>(www.text);
                //检测结果
                if (CheckReuslt(result))
                {
                    callback(result);
                }
            }
            else
                Debug.LogError(www.error);
        }

        private bool CheckReuslt(AsrResponse result)
        {
            if (result == null)
            {
                Debug.LogWarning("result is null ！！！" + result);
                return false;
            }

            if (result.err_no != 0)
            {
                Debug.LogError("返回结果包含错误：错误码：" + result.err_no + "  错误信息：" + result.err_msg);
                return false;
            }
            if (result.result == null || result.result.Length == 0)
            {
                Debug.LogWarning("识别完成！但是没有得到识别结果！");
                return false;
            }
            Debug.Log("识别成功！识别结果如下：");
            foreach (var v in result.result)
            {
                Debug.Log(v);
            }
            return true;

        }

        /// <summary>
        /// 申请百度语音权限
        /// </summary>
        private IEnumerator GetAccessToken()
        {
            Debug.Log("开始申请百度语音授权：Start fetching token...");
            CurrentTokenFetchStatus = TokenFetchStatus.Fetching;

            var uri =
                string.Format(
                    "https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials&client_id={0}&client_secret={1}",
                    API_KEY, SECRET_KEY);
            var www = new WWW(uri);
            yield return www;

            if (string.IsNullOrEmpty(www.error))
            {
                var result = JsonUtility.FromJson<TokenResponse>(www.text);
                Token = result.access_token;
                Debug.Log("申请百度语音授权成功：Token has been fetched successfully");
                CurrentTokenFetchStatus = TokenFetchStatus.Success;
            }
            else
            {
                Debug.LogError(www.error);
                Debug.LogError("申请百度语音授权失败！请检查你的APIKey或者ScretKey是否不正确！：Token was fetched failed. Please check your APIKey and SecretKey");
                CurrentTokenFetchStatus = TokenFetchStatus.Failed;
            }
        }

        /// <summary>
        /// 检测是否获取了百度语音全身
        /// </summary>
        /// <returns></returns>
        protected IEnumerator PreAction()
        {
            if (CurrentTokenFetchStatus == TokenFetchStatus.NotFetched)
            {
                Debug.Log("Token has not been fetched, now fetching...");
                yield return GetAccessToken();
            }

            if (CurrentTokenFetchStatus == TokenFetchStatus.Fetching)
                Debug.Log("Token is still being fetched, waiting...");

            while (CurrentTokenFetchStatus == TokenFetchStatus.Fetching)
            {
                //循环等待 - 获取百度语音权限中
                yield return null;
            }
        }


    }
}
