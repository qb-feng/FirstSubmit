using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaiDuVoice
{

    #region ios端与android端共用数据类型*****************************************************************************************
    ////回调事件的数据类
    [System.Serializable]
    public class ReceiMessageModel
    {
#if UNITY_ANDROID
        /// <summary>
        /// 事件名字
        /// </summary>
        public string name;
        /// <summary>
        /// 事件参数
        /// </summary>
        public string parame;
        /// <summary>
        /// 数据 该数据其实为byte[] 类型通过base64转换成字符串的，需要使用时需要反向转换
        /// </summary>
        public string data;
        /// <summary>
        /// 数据开始的index
        /// </summary>
        public int offect;
        /// <summary>
        /// 数据长度
        /// </summary>
        public int length;

        /// <summary>
        /// 将data数据
        /// </summary>
        /// <returns></returns>
        public byte[] GetData()
        {
            return System.Convert.FromBase64String(data);
        }

        public override string ToString()
        {
            return string.Format("名字：{0}   参数：{1}   数据：{2}    数据开始的index：{3}    数据长度：{4}", name, parame, data, offect, length);
        }
#elif UNITY_IOS
        /// <summary>
        /// IOS返回的事件名(对应事件枚举)
        /// </summary>
        public int workStatus;

        /// <summary>
        /// IOS暂时返回的事件参数
        /// </summary>
        public string data;

#endif
    }

    /// <summary>
    /// 用户启动的参数
    /// </summary>
    public class ASR_STARTParams_User
    {
        public ASR_START_pid_Type pid;
        public ASR_START__decoder_Type decoder;
        public ASR_START__vad_Type vad;
        public ASR_START__vadendpointtimeout_Type vad_endpoint_timeout;

        /// <summary>
        /// 是否需要语音音频数据回调，开启后有CALLBACK_EVENT_ASR_AUDIO事件  需要 true 不需要，false
        /// </summary>
        public bool accept_audio_data;

        /// <summary>
        /// 保存识别过程产生的录音文件, 该参数需要开启accept_audio_data后生效  填写文件路径 如： Application.persistentDataPath + "/myAudio.wav";
        /// </summary>
        public string outfile;

        /// <summary>
        /// 是否需要语音音量数据回调，开启后有CALLBACK_EVENT_ASR_VOLUME事件回调  需要 true 不需要，false
        /// </summary>
        public bool accept_audio_volume;


        /// <summary>
        /// 是否需要设置默认值 - 默认为不需要
        /// </summary>
        public ASR_STARTParams_User(bool defaultValue = false)
        {
            if (defaultValue)
            {
                SetDefaultValue();
            }
        }

        /// <summary>
        /// 如果不想设置参数：调用该方法可以设置默认参数
        /// </summary>
        public void SetDefaultValue()
        {
            this.pid = ASR_START_pid_Type.RecognitionEnglish_Short;
            this.decoder = ASR_START__decoder_Type.OffLine;
            this.vad = ASR_START__vad_Type.touch;
            this.vad_endpoint_timeout = ASR_START__vadendpointtimeout_Type.GreaterThan0;

            accept_audio_data = true;
            outfile = Application.persistentDataPath + "/myAudio.wav";

            accept_audio_volume = false;
        }


    }

    /// <summary>
    /// 启动百度语音时的参数列表：注意：需要转换成json串传给java
    /// </summary>
    [System.Serializable]
    public class ASR_STARTParams
    {
        public int pid;
        public int decoder;
        public string vad;
        public int vad_endpoint_timeout;

        /// <summary>
        /// 是否需要语音音频数据回调，开启后有CALLBACK_EVENT_ASR_AUDIO事件  需要 true 不需要，false
        /// </summary>
        public bool accept_audio_data;

        /// <summary>
        /// 保存识别过程产生的录音文件, 该参数需要开启accept_audio_data后生效  填写文件路径
        /// </summary>
        public string outfile;

        /// <summary>
        /// 是否需要语音音量数据回调，开启后有CALLBACK_EVENT_ASR_VOLUME事件回调  需要 true 不需要，false
        /// </summary>
        public bool accept_audio_volume;

#if UNITY_ANDROID
        /// <summary>
        /// 将用户定义的ASR_STARTParams_User model转换成传给百度语音的json字符串
        /// </summary>
        public static string ChangeCurrentModelToBaiDuVoiceJsonString(ASR_STARTParams_User userParams)
        {
            var asrParams = new ASR_STARTParams();

            asrParams.pid = (int)userParams.pid;
            asrParams.decoder = (int)userParams.decoder;
            asrParams.vad = userParams.vad.ToString();
            asrParams.vad_endpoint_timeout = (int)userParams.vad_endpoint_timeout;

            asrParams.accept_audio_data = userParams.accept_audio_data;
            asrParams.outfile = userParams.outfile;
            asrParams.accept_audio_volume = userParams.accept_audio_volume;

            string jsonString = JsonUtility.ToJson(asrParams);

            //替换掉里面的字段：
            jsonString = jsonString.Replace("accept_audio_data", SpeechConstant.ACCEPT_AUDIO_DATA);
            jsonString = jsonString.Replace("accept_audio_volume", SpeechConstant.ACCEPT_AUDIO_VOLUME);
            jsonString = jsonString.Replace("vad_endpoint_timeout", SpeechConstant.VAD_ENDPOINT_TIMEOUT);

            return jsonString;
        }

        /// <summary>
        /// 将百度语音库返回的json数据转换成ASR_STARTParams_User Model
        /// </summary>
        public static ASR_STARTParams_User ChangeBaiDuVoiceJsonStringToCurrentModel(string jsonString)
        {
            //替换掉里面的字段：
            jsonString.Replace(SpeechConstant.ACCEPT_AUDIO_DATA, "accept_audio_data");
            jsonString.Replace(SpeechConstant.ACCEPT_AUDIO_VOLUME, "accept_audio_volume");
            jsonString.Replace(SpeechConstant.VAD_ENDPOINT_TIMEOUT, "vad_endpoint_timeout");

            var dataModel = JsonUtility.FromJson<ASR_STARTParams>(jsonString);
            var userModel = new ASR_STARTParams_User();

            userModel.pid = (ASR_START_pid_Type)((int)(dataModel.pid));
            userModel.decoder = (ASR_START__decoder_Type)((int)(dataModel.decoder));
            userModel.vad = (ASR_START__vad_Type)System.Enum.Parse(typeof(ASR_START__vad_Type), dataModel.vad); ;

            userModel.vad_endpoint_timeout = (ASR_START__vadendpointtimeout_Type)(int)dataModel.vad_endpoint_timeout;
            userModel.accept_audio_data = dataModel.accept_audio_data;
            userModel.outfile = dataModel.outfile;
            userModel.accept_audio_volume = dataModel.accept_audio_volume;

            return userModel;
        }

#endif
    }

    #region startVoice 参数类型
    /// <summary>
    /// 开始模型时的pid类型:推荐使用1736
    /// </summary>
    public enum ASR_START_pid_Type
    {
        /// <summary>
        ///  1736	英语	搜索模型(支持短句)	无标点	不支持	
        /// </summary>
        RecognitionEnglish_Short = 1736,

        /// <summary>
        /// 1737	英语	输入法模型(支持长句)	可以有标点	不支持
        /// </summary>
        RecognitionEnglish_Long = 1737,
    }

    /// <summary>
    /// 离在线的并行策略:推荐使用 2
    /// </summary>
    public enum ASR_START__decoder_Type
    {
        /// <summary>
        /// 纯在线(默认)
        /// </summary>
        OnLine = 0,
        /// <summary>
        /// 离线, 离在线融合(在线优先)，离线命令词功能需要开启这个选项。
        /// </summary>
        OffLine = 2,
    }

    /// <summary>
    /// 语音活动检测， 根据静音时长自动断句 : 推荐使用1，手动停止
    /// </summary>
    public enum ASR_START__vad_Type
    {
        /// <summary>
        /// 新一代VAD，各方面信息优秀，推荐使用。
        /// </summary>
        dnn = 0,

        /// <summary>
        /// 关闭语音活动检测。注意关闭后不要开启长语音。适合用户自行控制音频结束，如按住说话松手停止的场景。
        /// 功能等同于60s限制的长语音。需要手动调用ASR_STOP停止录音
        /// </summary>
        touch = 1,
    }

    /// <summary>
    /// 静音超时断句及长语音 - 推荐使用800
    /// </summary>
    public enum ASR_START__vadendpointtimeout_Type
    {
        /// <summary>
        /// 开启长语音。即无静音超时断句。手动调用ASR_STOP停止录音。 请勿和VAD=touch联用！
        /// </summary>
        Open = 0,

        /// <summary>
        /// >0（毫秒），默认800ms 不开启长语音。开启VAD尾点检测，即静音判断的毫秒数。建议设置800ms-3000ms
        /// </summary>
        GreaterThan0 = 800,
    }
    #endregion

    #endregion

    #region android数据类型*******************************************************************************************************

#if UNITY_ANDROID
    /// <summary>
    /// 语音事件标志符
    /// </summary>
    public class SpeechConstant
    {
        // Field descriptor #6 Ljava/lang/String;
        public const string ASR_START = "asr.start";

        // Field descriptor #6 Ljava/lang/String;
        public const string ASR_STOP = "asr.stop";

        // Field descriptor #6 Ljava/lang/String;
        public const string ASR_CANCEL = "asr.cancel";

        // Field descriptor #6 Ljava/lang/String;
        public const string ASR_KWS_LOAD_ENGINE = "asr.kws.load";

        // Field descriptor #6 Ljava/lang/String;
        public const string ASR_KWS_UNLOAD_ENGINE = "asr.kws.unload";

        // Field descriptor #6 Ljava/lang/String;
        public const string ASR_UPLOAD_WORDS = "asr.upload.words";

        // Field descriptor #6 Ljava/lang/String;
        public const string ASR_UPLOAD_CANCEL = "asr.upload.cancel";

        // Field descriptor #6 Ljava/lang/String;
        public const string WAKEUP_START = "wp.start";

        // Field descriptor #6 Ljava/lang/String;
        public const string WAKEUP_STOP = "wp.stop";

        // Field descriptor #6 Ljava/lang/String;
        public const string WAKEUP_LOAD_ENGINE = "wp.load";

        // Field descriptor #6 Ljava/lang/String;
        public const string WAKEUP_UNLOAD_ENGINE = "wp.unload";

        // Field descriptor #6 Ljava/lang/String;
        public const string ASR_UPLOAD_CONTRACT = "asr.upload.contract";

        // Field descriptor #6 Ljava/lang/String;
        public const string UPLOADER_START = "uploader.start";

        // Field descriptor #6 Ljava/lang/String;
        public const string UPLOADER_CANCEL = "uploader.cancel";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_READY = "asr.ready";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_BEGIN = "asr.begin";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_AUDIO = "asr.audio";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_VOLUME = "asr.volume";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_END = "asr.end";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_PARTIAL = "asr.partial";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_FINISH = "asr.finish";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_EXIT = "asr.exit";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_CANCEL = "asr.cancel";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_ERROR = "asr.error";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_LOADED = "asr.loaded";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_UNLOADED = "asr.unloaded";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_SERIALNUMBER = "asr.sn";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_LOG = "asr.log";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_UPLOAD_COMPLETE = "asr.upload.complete";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_ASR_LONG_SPEECH = "asr.long-speech.finish";

        // Field descriptor #6 Ljava/lang/String;
        public const string ASR_CALLBACk_NAME = "ASR.callback";

        // Field descriptor #6 Ljava/lang/String;
        public const string WAKEUP_CALLBACK_NAME = "WAKEUP.callback";

        // Field descriptor #6 Ljava/lang/String;
        public const string UPLOAD_CALLBACK_NAME = "UPLOAD.callback";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_ASR_STATUS = "cb.asr.status.int";

        // Field descriptor #6 Ljava/lang/String;
        public const string strCALLBACK_ASR_LEVEL = "cb.asr.level.int";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_ASR_RESULT = "cb.asr.result.string";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_WAK_STATUS = "cb.wak.status.int";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_WAK_RESULT = "cb.wak.result.string";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_ERROR_DOMAIN = "cb.error.domain.int16_t";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_ERROR_CODE = "cb.error.code.int16_t";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_ERROR_DESC = "cb.error.desc.string";

        // Field descriptor #6 Ljava/lang/String;
        public const string SOUND_START = "sound_start";

        // Field descriptor #6 Ljava/lang/String;
        public const string SOUND_END = "sound_end";

        // Field descriptor #6 Ljava/lang/String;
        public const string SOUND_SUCCESS = "sound_success";

        // Field descriptor #6 Ljava/lang/String;
        public const string SOUND_ERROR = "sound_error";

        // Field descriptor #6 Ljava/lang/String;
        public const string SOUND_CANCEL = "sound_cancel";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_WAKEUP_STARTED = "wp.enter";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_WAKEUP_READY = "wp.ready";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_WAKEUP_STOPED = "wp.exit";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_WAKEUP_LOADED = "wp.loaded";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_WAKEUP_UNLOADED = "wp.unloaded";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_WAKEUP_ERROR = "wp.error";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_WAKEUP_SUCCESS = "wp.data";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_WAKEUP_AUDIO = "wp.audio";

        // Field descriptor #6 Ljava/lang/String;
        public const string CALLBACK_EVENT_UPLOAD_FINISH = "uploader.finish";

        // Field descriptor #6 Ljava/lang/String;
        public const string LOG_LEVEL = "log_level";

        // Field descriptor #6 Ljava/lang/String;
        public const string LANGUAGE = "language";

        // Field descriptor #6 Ljava/lang/String;
        public const string CONTACT = "contact";

        // Field descriptor #6 Ljava/lang/String;
        public const string VAD = "vad";

        // Field descriptor #6 Ljava/lang/String;
        public const string VAD_MFE = "mfe";

        // Field descriptor #6 Ljava/lang/String;
        public const string VAD_MODEL = "model-vad";

        // Field descriptor #6 Ljava/lang/String;
        public const string VAD_DNN = "dnn";

        // Field descriptor #6 Ljava/lang/String;
        public const string VAD_TOUCH = "touch";

        // Field descriptor #6 Ljava/lang/String;
        public const string SAMPLE_RATE = "sample";

        // Field descriptor #6 Ljava/lang/String;
        public const string PAM = "decoder-server.pam";

        // Field descriptor #6 Ljava/lang/String;
        public const string NLU = "nlu";

        // Field descriptor #6 Ljava/lang/String;
        public const string PROP = "prop";

        // Field descriptor #141 Z
        public static bool PUBLIC_DECODER;

        // Field descriptor #6 Ljava/lang/String;
        public const string IN_FILE = "infile";

        // Field descriptor #6 Ljava/lang/String;
        public const string AUDIO_MILLS = "audio.mills";

        // Field descriptor #6 Ljava/lang/String;
        public const string AUDIO_SOURCE = "audio.source";

        // Field descriptor #6 Ljava/lang/String;
        public const string OUT_FILE = "outfile";

        // Field descriptor #6 Ljava/lang/String;
        public const string ACCEPT_AUDIO_DATA = "accept-audio-data";

        // Field descriptor #6 Ljava/lang/String;
        public const string ACCEPT_AUDIO_VOLUME = "accept-audio-volume";

        // Field descriptor #6 Ljava/lang/String;
        public const string APP_KEY = "key";

        // Field descriptor #6 Ljava/lang/String;
        public const string SECRET = "secret";

        // Field descriptor #6 Ljava/lang/String;
        public const string URL = "decoder-server.url";

        // Field descriptor #6 Ljava/lang/String;
        public const string PID = "pid";

        // Field descriptor #6 Ljava/lang/String;
        public const string APP_NAME = "decoder-server.app";

        // Field descriptor #6 Ljava/lang/String;
        public const string URL_NEW = "https://vse.baidu.com/v2";

        // Field descriptor #6 Ljava/lang/String;
        public const string URL_OLD = "https://vse.baidu.com/echo.fcgi";

        // Field descriptor #6 Ljava/lang/String;
        public const string DEC_TYPE = "dec-type";

        // Field descriptor #6 Ljava/lang/String;
        public const string DECODER = "decoder";

        // Field descriptor #6 Ljava/lang/String;
        public const string ASR_VAD_RES_FILE_PATH = "vad.res-file";

        // Field descriptor #6 Ljava/lang/String;
        public const string VAD_ENDPOINT_TIMEOUT = "vad.endpoint-timeout";

        // Field descriptor #6 Ljava/lang/String;
        public const string ASR_OFFLINE_ENGINE_GRAMMER_FILE_PATH = "grammar";

        // Field descriptor #6 Ljava/lang/String;
        public const string ASR_OFFLINE_ENGINE_DAT_FILE_PATH = "asr-base-file-path";

        // Field descriptor #6 Ljava/lang/String;
        public const string ASR_OFFLINE_ENGINE_LICENSE_FILE_PATH = "license-file-path";

        // Field descriptor #6 Ljava/lang/String;
        public const string SLOT_DATA = "slot-data";

        // Field descriptor #6 Ljava/lang/String;
        public const string DISABLE_PUNCTUATION = "disable-punctuation";

        // Field descriptor #6 Ljava/lang/String;
        public const string KWS_TYPE = "kws-type";

        // Field descriptor #6 Ljava/lang/String;
        public const string APP_ID = "appid";

        // Field descriptor #6 Ljava/lang/String;
        public const string DEV = "dev";

        // Field descriptor #6 Ljava/lang/String;
        public const string WP_DAT_FILEPATH = "wakeup_dat_filepath";

        // Field descriptor #6 Ljava/lang/String;
        public const string WP_WORDS_FILE = "kws-file";

        // Field descriptor #6 Ljava/lang/String;
        public const string WP_WORDS = "words";

        // Field descriptor #6 Ljava/lang/String;
        public const string ENABLE_HTTPDNS = "enable-httpdns";

        // Field descriptor #6 Ljava/lang/String;
        public const string WP_VAD_ENABLE = "wp.vad_enable";

        // Field descriptor #6 Ljava/lang/String;
        public const string WP_ENGINE_LICENSE_FILE_PATH = "license-file-path";
    }

    #region 回调事件参数params里面的 jsonmodel类
    /// <summary>
    /// 回调事件名：CALLBACK_EVENT_ASR_FINISH 的jsonmodel
    /// </summary>
    [System.Serializable]
    public class CALLBACK_EVENT_ASR_FINISH_Model
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int errorCode;

        /// <summary>
        /// 描述
        /// </summary>
        public string desc;

        public override string ToString()
        {
            return string.Format("错误码：{0}   错误描述：{1}", errorCode, desc);
        }
    }
    /// <summary>
    /// 回调事件名：CALLBACK_EVENT_ASR_PARTIAL 的jsonmodel
    /// </summary>
    [System.Serializable]
    public class CALLBACK_EVENT_ASR_PARTIAL_Model
    {
        /// <summary>
        /// 解析后的识别结果。如无特殊情况，请取第一个结果
        /// </summary>
        public string[] results_recognition;

        /// <summary>
        /// partial_result = 临时识别结果           final_result = 最终结果，长语音每一句都有一个最终结果    nlu_result = 语义结果，在final_result后回调。语义结果的内容在(data，offset，length中）
        /// </summary>
        public string result_type;

        public string best_result;
        public int error;

        public enum Result_Type
        {
            partial_result = 1,
            final_result = 2,
            /// <summary>
            /// 语义结果的内容在(data，offset，length中）
            /// </summary>
            nlu_result = 3,
        }
    }
    /// <summary>
    /// 当前音量回调的json字符串参数的model
    /// </summary>
    [System.Serializable]
    public class CALLBACK_EVENT_ASR_VOLUME_Model
    {
        /// <summary>
        /// 当前音量
        /// </summary>
        public float volume;

        /// <summary>
        /// 当前音量的相对值 （0-100）
        /// </summary>
        public int volume_percent;

        /// <summary>
        /// 将java传入的百度语音回调json字符串转成当前model  必须用此方法转！！！
        /// </summary>
        public static CALLBACK_EVENT_ASR_VOLUME_Model JavaStringJsonToCurrentModel(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                Debug.LogWarning("jsonStirng is null   !!!!");
                return null;
            }
            jsonString = jsonString.Replace("volume-percent", "volume_percent");
            return JsonUtility.FromJson<CALLBACK_EVENT_ASR_VOLUME_Model>(jsonString);
        }
    }
    #endregion

#endif
    #endregion

    #region ios端数据类型**********************************************************************************************************

#if UNITY_IOS
    public enum TBDVoiceRecognitionClientWorkStatus
    {
        /// <summary>
        /// 识别工作开始，开始采集及处理数据
        /// </summary>
        EVoiceRecognitionClientWorkStatusStartWorkIng = 0,
        /// <summary>
        ///  检测到用户开始说话
        /// </summary>
        EVoiceRecognitionClientWorkStatusStart = 1,
        /// <summary>
        /// 本地声音采集结束，等待识别结果返回并结束录音
        /// </summary>
        EVoiceRecognitionClientWorkStatusEnd = 2,
        /// <summary>
        /// 录音数据回调
        /// </summary>          
        EVoiceRecognitionClientWorkStatusNewRecordData = 3,
        /// <summary>
        /// 连续上屏
        /// </summary>
        EVoiceRecognitionClientWorkStatusFlushData = 4,
        /// <summary>
        ///  语音识别功能完成，服务器返回正确结果
        /// </summary>
        EVoiceRecognitionClientWorkStatusFinish = 5,
        /// <summary>
        ///  当前音量回调
        /// </summary>
        EVoiceRecognitionClientWorkStatusMeterLevel = 6,
        /// <summary>
        ///  用户取消
        /// </summary>
        EVoiceRecognitionClientWorkStatusCancel = 7,
        /// <summary>
        /// 发生错误
        /// </summary>
        EVoiceRecognitionClientWorkStatusError = 8,
        /* 离线引擎状态 */
        /// <summary>
        /// 离线引擎加载完成
        /// </summary>
        EVoiceRecognitionClientWorkStatusLoaded = 9,
        /// <summary>
        /// 离线引擎卸载完成
        /// </summary>
        EVoiceRecognitionClientWorkStatusUnLoaded = 10,
        /* CHUNK状态 */
        /// <summary>
        /// CHUNK: 识别结果中的第三方数据
        /// </summary>
        EVoiceRecognitionClientWorkStatusChunkThirdData = 11,
        /// <summary
        ///     // CHUNK: 识别结果中的语义结果
        /// </summary>
        EVoiceRecognitionClientWorkStatusChunkNlu = 12,
        /// <summary>
        ///   // CHUNK: 识别过程结束a
        /// </summary>
        EVoiceRecognitionClientWorkStatusChunkEnd = 13,
        /* LOG */
        /// <summary>
        ///           // Feedback: 识别过程反馈的打点数据
        /// </summary>
        EVoiceRecognitionClientWorkStatusFeedback = 14,
        /* Only for iOS */
        /// <summary>
        ///     // 录音机关闭，页面跳转需检测此时间，规避状态条 (iOS)
        /// </summary>
        EVoiceRecognitionClientWorkStatusRecorderEnd = 15,
        /* LONG SPEECH END */
        /// <summary>
        /// // 长语音结束状态
        /// </summary>
        EVoiceRecognitionClientWorkStatusLongSpeechEnd = 16,
    }
#endif

    #endregion

    #region 百度rest API 工具端数据类型***************************************************************************************
    /// <summary>
    /// 返回的参数
    /// </summary>
    public class AsrResponse
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int err_no;
        /// <summary>
        /// 错误信息描述
        /// </summary>
        public string err_msg;
        public string sn;
        /// <summary>
        /// 识别结果数组，提供1-5 个候选结果， 优先使用第一个结果。utf-8 编码
        /// </summary>
        public string[] result;
    }
    #endregion

}