using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BaiDuVoice
{
    /// <summary>
    /// 百度语音识别管理器
    /// </summary>
    public class BaiDuAsrManger : SiginBase<BaiDuAsrManger>
    {
        /// <summary>
        /// 当前语音识别引擎的状态
        /// </summary>
        public enum AsrManagerState
        {
            /// <summary>
            /// 引擎没有初始化
            /// </summary>
            NoInit = 0,
            /// <summary>
            /// 引擎初始化失败
            /// </summary>
            InitError = 1,
            /// <summary>
            /// 引擎启动中
            /// </summary>
            Initing = 2,
            /// <summary>
            /// 引擎初始化成功
            /// </summary>
            InitSuccess = 3,
        }

        /// <summary>
        /// 引擎识别的状态
        /// </summary>
        public enum AsrRecognizeState
        {
            /// <summary>
            /// 闲置状态，不在识别
            /// </summary>
            Idle = 0,
            /// <summary>
            /// 识别可以开始
            /// </summary>
            RecognizeStart = 1,
            /// <summary>
            /// 识别中
            /// </summary>
            Recognizeing = 2,
            /// <summary>
            /// 识别结束
            /// </summary>
            RecognizeEnd = 3,
        }


        /// <summary>
        /// 识别完成时返回给用户的数据model
        /// </summary>
        public class RecognizeResultModel
        {
            /// <summary>
            /// 识别结果
            /// </summary>
            public string result;

            /// <summary>
            /// 当前音频的保存路径，如果存在的话
            /// </summary>
            public string audioFilePath;
        }


        #region 私有变量
        private AndroidJavaObject jo = null;//当前sdk需要的安卓类
        private AndroidJavaClass jc = null;//当前sdk需要的安卓类的new出来的一个实例
        private ReceiMessageModel receiMessageModel = null;
        private AsrManagerState currentAsrManagerState = AsrManagerState.NoInit;
        private AsrRecognizeState currentAsrRecoginizeState = AsrRecognizeState.Idle;
        private string currentRecognizeResult = null;//当前识别到的结果
        private Action<RecognizeResultModel> onEndRecognizeResultComplete = null;//识别结束时的结果回掉函数
        private ASR_STARTParams_User currentParams_User = null;//当前传入的用户参数列表
        #endregion

        #region 公有变量
        /// <summary>
        /// 当前语音引擎的状态
        /// </summary>
        public AsrManagerState CurrentAsrState
        {
            get { return currentAsrManagerState; }
            private set { currentAsrManagerState = value; }
        }
        /// <summary>
        /// 当前语音引擎的语音识别状态
        /// </summary>
        public AsrRecognizeState CurrentAsrRecoginizeState
        {
            get { return currentAsrRecoginizeState; }
            private set { currentAsrRecoginizeState = value; }
        }
        /// <summary>
        /// 当前音量
        /// </summary>
        public float CurrentVolume { get; private set; }
        /// <summary>
        /// 当前音量的相对值 0 - 100
        /// </summary>
        public int CurrentVolumePercent { get; private set; }
        #endregion


        #region 项目基本参数
        public const string APP_ID = "10781891";
        public const string API_KEY = "EXbRdegVkeL22Ui1M9c9jfdm";
        public const string SECRET_KEY = "b4c2c87db69c4848cb4ce7c4f5a2418d";
        #endregion


        /// <summary>
        /// 重置数据
        /// </summary>
        private void ResetData()
        {
            CurrentVolume = 0;
            CurrentVolumePercent = 0;
            currentParams_User = null;
        }



        /// <summary>
        /// 初始化方法
        /// </summary>
        public void Init()
        {
            if (CurrentAsrState != AsrManagerState.NoInit && CurrentAsrState != AsrManagerState.InitError)
            {
                Debug.LogWarning("引擎正在启动中或已经启动成功！请勿重复启动！ 引擎状态：" + CurrentAsrState);
                return;
            }

            Debug.Log("引擎开始启动！");
            CurrentAsrState = AsrManagerState.Initing;

            //通知java开始启动
#if UNITY_ANDROID && !UNITY_EDITOR
            if (jc == null || jo == null)
            {
                //得到java中自定义的BaiDuVoiceActivity类
                jc = new AndroidJavaClass("com.example.unitybaiduvoice.BaiDuVoiceActivity");
                //调用BaiDuVoiceActivity类中的静态方法得到一个BaiDuVoiceActivity对象（调用里面的单例一样）
                //GetStatic 是获取静态字段的值
                jo = jc.CallStatic<AndroidJavaObject>("Instance");
            }
            if (jo == null)
            {
                CurrentAsrState = AsrManagerState.InitError;
                Debug.LogError("引擎初始化失败！" + jo);
                return;
            }

            CallAndroidFunction("Init");
#elif UNITY_IOS && !UNITY_EDITOR
            BaiDuAsrIOSFunction.InitBaiDuVoiceManager();
#endif
            CurrentAsrState = AsrManagerState.InitSuccess;
        }

        /// <summary>
        /// 开始录音的方法 参数1：录音开启时的设置参数（不设置的话系统会自动传入），参数2：录音结束后的结果回调 
        /// </summary>
        public void StartRecord(ASR_STARTParams_User startParams = null, Action<RecognizeResultModel> _onEndRecognizeResultComplete = null)
        {
            if (!CheckState())
                return;

            if (CurrentAsrRecoginizeState == AsrRecognizeState.Recognizeing)
            {
                Debug.LogWarning("语音正在识别中，请勿重复发起识别！" + CurrentAsrRecoginizeState);
                return;
            }

            if (startParams == null)
            {
                startParams = new ASR_STARTParams_User(true);
                Debug.LogError("开启录音传入参数为空！传入默认参数" + startParams);
            }

            //重置数据
            ResetData();

            currentParams_User = startParams;
            onEndRecognizeResultComplete = _onEndRecognizeResultComplete;
            CurrentAsrRecoginizeState = AsrRecognizeState.RecognizeStart;

#if UNITY_ANDROID
            string jsonParams = ASR_STARTParams.ChangeCurrentModelToBaiDuVoiceJsonString(currentParams_User);
            Debug.Log("开启录音！传入json字符串参数为：" + jsonParams);
            CallAndroidFunction("StartRecord", jsonParams);
#elif UNITY_IOS
            BaiDuAsrIOSFunction.BaiDu_StartRecordVoice(API_KEY, SECRET_KEY, APP_ID, currentParams_User.outfile, (int)currentParams_User.pid);
#endif
        }

        /// <summary>
        /// 取消本次录音识别的方法
        /// </summary>
        public void CancelRecord()
        {
            if (!CheckState())
                return;

            if (CurrentAsrRecoginizeState != AsrRecognizeState.Recognizeing)
            {
                Debug.LogWarning("语音不在识别中，取消失败！" + CurrentAsrRecoginizeState);
                return;
            }
            CurrentAsrRecoginizeState = AsrRecognizeState.Idle;
            Debug.Log("取消录音!");

#if UNITY_ANDROID
            CallAndroidFunction("CancelRecord");
#elif  UNITY_IOS && !UNITY_EDITOR
            BaiDuAsrIOSFunction.BaiDu_CancleRecordVoice();
#endif
            //重置数据
            ResetData();
        }


        /// <summary>
        /// 停止本次录音，等待识别结果
        /// </summary>
        public void StopRecord()
        {
            if (!CheckState())
                return;

            if (CurrentAsrRecoginizeState != AsrRecognizeState.Recognizeing)
            {
                Debug.LogWarning("语音不在识别中，停止失败！" + CurrentAsrRecoginizeState);
                return;
            }

            CurrentAsrRecoginizeState = AsrRecognizeState.RecognizeEnd;
            Debug.Log("停止录音!");

#if UNITY_ANDROID
            CallAndroidFunction("StopRecord");
#elif  UNITY_IOS && !UNITY_EDITOR
            BaiDuAsrIOSFunction.BaiDu_StopRecordVoice();
#endif
        }

        /// <summary>
        /// 注销掉百度语音引擎
        /// </summary>
        public void Release()
        {
            if (!CheckState())
                return;

            if (currentAsrRecoginizeState == AsrRecognizeState.Recognizeing)
            {
                //如果正在识别中，就取消识别
                CancelRecord();
            }

            Debug.Log("注销引擎!");

#if UNITY_ANDROID
            CallAndroidFunction("Release");
#elif  UNITY_IOS && !UNITY_EDITOR
            BaiDuAsrIOSFunction.BaiDu_ReleaseRecordVoice();
#endif
            currentAsrManagerState = AsrManagerState.NoInit;
            currentAsrRecoginizeState = AsrRecognizeState.Idle;
            jo = null;
            receiMessageModel = null;

            //重置数据
            ResetData();
        }

        /// <summary>
        /// 检查引擎状态
        /// </summary>
        private bool CheckState()
        {
            if (CurrentAsrState != AsrManagerState.InitSuccess)
            {
                Debug.Log("引擎没有成功启动！请先启动引擎！" + CurrentAsrState);
                return false;
            }

            return true;
        }


        #region android与ios的结果处理方法

#if UNITY_ANDROID

        /// <summary>
        /// 调用安卓里面的方法
        /// </summary>
        private void CallAndroidFunction(string functionName, params object[] args)
        {
            if (jo == null)
            {
                Debug.LogError("引擎初始化失败！不能操作！" + jo);
                return;
            }
            Debug.Log("调用安卓普通方法：" + functionName + "参数:" + args);
            jo.Call(functionName, args);
        }
        /// <summary>
        /// 安卓消息回调方法 
        /// </summary>
        private void OnReceiveMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                Debug.LogWarning("安卓回掉消息为空！");
                return;
            }
            //Debug.Log("参数返回为：" + msg);
            receiMessageModel = JsonUtility.FromJson<ReceiMessageModel>(msg);
            if (receiMessageModel == null)
            {
                Debug.LogError("参数返回为空！" + receiMessageModel);
                return;
            }

            //  Debug.Log("参数转换成功！model数据为：" + receiMessageModel.ToString());

            switch (receiMessageModel.name)
            {
                case SpeechConstant.CALLBACK_EVENT_ASR_READY:
                    //引擎准备就绪，可以开始说话  - 开始识别
                    CurrentAsrRecoginizeState = AsrRecognizeState.Recognizeing;
                    currentRecognizeResult = null;//重置识别结果
                    break;


                case SpeechConstant.CALLBACK_EVENT_ASR_END:
                    //识别结束
                    CurrentAsrRecoginizeState = AsrRecognizeState.RecognizeEnd;
                    break;

                case SpeechConstant.CALLBACK_EVENT_ASR_PARTIAL:
                    //识别结果
                    if (CurrentAsrRecoginizeState == AsrRecognizeState.RecognizeEnd)
                    {
                        //TODO 暂时设定为只有识别结束时才去计算识别结果
                        var partialModel = JsonUtility.FromJson<CALLBACK_EVENT_ASR_PARTIAL_Model>(receiMessageModel.parame);

                        if (partialModel == null || partialModel.result_type == CALLBACK_EVENT_ASR_PARTIAL_Model.Result_Type.nlu_result.ToString())
                        {
                            //此时数据在data中
                            Debug.LogWarning("数据存在在data中！暂时没有解析！" + receiMessageModel.data);
                            //TODO 暂时不解析

                        }
                        else if (partialModel.results_recognition != null && partialModel.results_recognition.Length > 0)
                        {
                            currentRecognizeResult = partialModel.results_recognition[0];
                            Debug.Log("结果识别成功！" + currentRecognizeResult);
                        }
                    }

                    break;

                case SpeechConstant.CALLBACK_EVENT_ASR_FINISH:
                    //识别完成 识别结束（可能含有错误信息）
                    var fisnishModel = JsonUtility.FromJson<CALLBACK_EVENT_ASR_FINISH_Model>(receiMessageModel.parame);
                    if (fisnishModel != null)
                    {
                        Debug.LogWarning("识别完成，但是包含错误！ " + fisnishModel.ToString());
                    }

                    //执行回调函数
                    if (onEndRecognizeResultComplete != null)
                    {
                        Debug.LogWarning("执行回调函数：传入结果：" + currentRecognizeResult);
                        if (currentParams_User != null && currentParams_User.accept_audio_data && !string.IsNullOrEmpty(currentParams_User.outfile))
                        {
                            //此时表明玩家设置的音频路径 - 往该文件路径写入头
                            if (System.IO.File.Exists(currentParams_User.outfile))
                            {
                                Debug.LogWarning("往文件中写入文件头：   " + currentParams_User.outfile);
                                Uttility.WriteHeader(currentParams_User.outfile, 16000, 1);//文件头先写进去
                            }
                        }
                        var resultModel = new RecognizeResultModel()
                        {
                            result = currentRecognizeResult,
                            audioFilePath = currentParams_User.outfile,
                        };
                        onEndRecognizeResultComplete(resultModel);
                        onEndRecognizeResultComplete = null;
                    }

                    break;

                case SpeechConstant.CALLBACK_EVENT_ASR_EXIT:
                    //该次识别退出  识别结束，资源释放
                    CurrentAsrRecoginizeState = AsrRecognizeState.Idle;
                    //数据初始化
                    ResetData();
                    break;

                #region 高级回调事件

                case SpeechConstant.CALLBACK_EVENT_ASR_VOLUME:
                    //当前音量回调。必须在开始录音时设置 ACCEPT_AUDIO_VOLUME参数激活
                    var volumeModel = CALLBACK_EVENT_ASR_VOLUME_Model.JavaStringJsonToCurrentModel(receiMessageModel.parame);
                    if (volumeModel != null)
                    {
                        CurrentVolume = volumeModel.volume;
                        CurrentVolumePercent = volumeModel.volume_percent;
                    }
                    break;

                case SpeechConstant.CALLBACK_EVENT_ASR_AUDIO:
                    //PCM音频片段的回调 - 必须在开始录音时设置ACCEPT_AUDIO_DATA 参数激活
                    //数据存在 (data，offset，length)
                    break;

                #endregion
            }
        }

#elif UNITY_IOS
        /// <summary>
        /// ios消息回调方法 
        /// </summary>
        private void OnReceiveIOSMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                Debug.LogWarning("IOS发来了一条空消息!" + msg);
                return;
            }
          //  Debug.Log("unity: 接收到ios发来的的消息:" + msg);

            receiMessageModel = JsonUtility.FromJson<ReceiMessageModel>(msg);

            switch ((TBDVoiceRecognitionClientWorkStatus)receiMessageModel.workStatus)
            {
                case TBDVoiceRecognitionClientWorkStatus.EVoiceRecognitionClientWorkStatusStartWorkIng:
                    //引擎准备就绪，可以开始说话  - 开始识别
                    CurrentAsrRecoginizeState = AsrRecognizeState.Recognizeing;
                    currentRecognizeResult = null;//重置识别结果
                    break;


                case TBDVoiceRecognitionClientWorkStatus.EVoiceRecognitionClientWorkStatusEnd:
                    //识别结束
                    CurrentAsrRecoginizeState = AsrRecognizeState.RecognizeEnd;
                    break;

                case TBDVoiceRecognitionClientWorkStatus.EVoiceRecognitionClientWorkStatusFinish:
                    //返回识别结果
                    if (CurrentAsrRecoginizeState == AsrRecognizeState.RecognizeEnd)
                    {
                        currentRecognizeResult = receiMessageModel.data;

                    }

                    break;

                case TBDVoiceRecognitionClientWorkStatus.EVoiceRecognitionClientWorkStatusError:
                    //识别出错 （含有错误信息）
                    Debug.LogError("百度语音返回错误!错误信息:" + receiMessageModel.data);

                    CurrentAsrRecoginizeState = AsrRecognizeState.RecognizeEnd;
                    break;

                case TBDVoiceRecognitionClientWorkStatus.EVoiceRecognitionClientWorkStatusChunkEnd:

                    //执行回调函数
                    if (onEndRecognizeResultComplete != null)
                    {
                        Debug.LogWarning("执行回调函数：传入结果：" + currentRecognizeResult);
                        if (currentParams_User != null && currentParams_User.accept_audio_data && !string.IsNullOrEmpty(currentParams_User.outfile))
                        {
                            //此时表明玩家设置的音频路径 - 往该文件路径写入头
                            if (System.IO.File.Exists(currentParams_User.outfile))
                            {
                                Debug.LogWarning("往文件中写入文件头：11   " + currentParams_User.outfile);
                                Uttility.WriteHeader(currentParams_User.outfile, 16000, 1);//文件头先写进去
                            }
                        }
                        var resultModel = new RecognizeResultModel() 
                        {
                            result = currentRecognizeResult,
                            audioFilePath = currentParams_User.outfile,
                        };
                        onEndRecognizeResultComplete(resultModel);
                        onEndRecognizeResultComplete = null;

                    }


                    //该次识别退出  识别结束，资源释
                    CurrentAsrRecoginizeState = AsrRecognizeState.Idle;
                    //数据初始化
                    ResetData();
                    break;


                case TBDVoiceRecognitionClientWorkStatus.EVoiceRecognitionClientWorkStatusRecorderEnd:
                    //当前录音机关闭才能执行回调(播放声音)

                    break;
            }

        }

#endif
    }

        #endregion


}
