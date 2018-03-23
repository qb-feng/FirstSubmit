//
//  BaiDuVoiceManager.m
//  Unity-iPhone
//
//  Created by puhanda on 2018/2/5.
//

#import <Foundation/Foundation.h>
#import "BaiDuVoiceManager.h"
#import "BDSEventManager.h"
#import "BDSASRDefines.h"
#import "BDSASRParameters.h"
#import "BaiDuVoicedelegate.h"

@implementation BaiDuVoiceManager

//单例
static BaiDuVoiceManager* _sharedInstance = nil;

+ (BaiDuVoiceManager*)Instance
{
    @synchronized(self.class)
    {
        if (_sharedInstance == nil) {
            _sharedInstance = [[self.class alloc] init];
        }
        
        return _sharedInstance;
    }
}


//常量
const NSString* API_KEY = @"";
const NSString* SECRET_KEY = @"";
const NSString* APP_ID = @"";

//变量
BDSEventManager* asrEventManager = nil;
NSFileHandle *fileHandler = nil;


//方法
-(void)InitBaiDuVoiceManager
{
    // 创建语音识别对象
    asrEventManager = [BDSEventManager createEventManagerWithName:BDS_ASR_NAME];
    // 设置语音识别代理
    [asrEventManager setDelegate:self];
    
    NSLog(@"ios:初始化成功!");
    
}

-(void)StartRecord:(NSString*)api_key SECRET:(NSString*)secret_key AppID:(NSString*)app_id KeepFile:(NSString*)keep_file PID:(int)pid
{
    NSLog(@"ios:开始录音1!");
    
    API_KEY = api_key;
    SECRET_KEY = secret_key;
    APP_ID = app_id;
    
    NSLog(@"ios:开始录音2!");
    //部分设置
    //设置DEBUG_LOG的级别
    //[asrEventManager setParameter:@(EVRDebugLogLevelTrace) forKey:BDS_ASR_DEBUG_LOG_LEVEL];
    //设置采样率
    [asrEventManager setParameter:@(EVoiceRecognitionRecordSampleRate16K) forKey:BDS_ASR_SAMPLE_RATE];
    //设置开启语义解析,将返回包含语义的json串  true = YES fasle = No
    [asrEventManager setParameter:@(YES) forKey:BDS_ASR_ENABLE_NLU];
    //设置音频文件保存路径
    //[asrEventManager setParameter:keep_file forKey:BDS_ASR_AUDIO_FILE_PATH];
    //识别策略 - 并行模式
    [asrEventManager setParameter:@(EVR_STRATEGY_BOTH) forKey:BDS_ASR_STRATEGY];
    //屏蔽sdk内部设置的AudioSession的Active状态
    [asrEventManager setParameter:@(YES) forKey:BDS_ASR_DISABLE_AUDIO_OPERATION];
    //设置pid
    NSString *pidstring = [NSString stringWithFormat:@"%d",pid];
    [asrEventManager setParameter:pidstring forKey:BDS_ASR_PRODUCT_ID];
    
    // 参数配置：在线身份验证
    [asrEventManager setParameter:@[API_KEY, SECRET_KEY] forKey:BDS_ASR_API_SECRET_KEYS];
    //设置 APPID
    [asrEventManager setParameter:APP_ID forKey:BDS_ASR_OFFLINE_APP_CODE];
    
    //3.4 VAD端点检测，即自动检测音频输入的起始点和结束点，如果需要自行控制识别结束需关闭VAD，请同时关闭服务端VAD与端上VAD：
    // 关闭服务端VAD
    [asrEventManager setParameter:@(NO) forKey:BDS_ASR_ENABLE_EARLY_RETURN];
    // 关闭本地VAD
    [asrEventManager setParameter:@(NO) forKey:BDS_ASR_ENABLE_LOCAL_VAD];
    
    if(keep_file != nil){
        //创建当前音频文件路径格式
        fileHandler = [self createFileHandleWithName:keep_file isAppend:NO];
    }
    
    
    // 发送指令：启动识别
    [asrEventManager sendCommand:BDS_ASR_CMD_START];
    
    
    
    NSLog(@"ios:开始录音!");
}

-(void)CancelRecord
{
    // 发送指令：取消识别
    [asrEventManager sendCommand:BDS_ASR_CMD_CANCEL];
    if(fileHandler != nil)
    {
        [fileHandler closeFile];
        fileHandler = nil;
    }
    NSLog(@"ios:取消录音!");
}

-(void)StopRecord
{
    [asrEventManager sendCommand:BDS_ASR_CMD_STOP];
    if(fileHandler != nil)
    {
        [fileHandler closeFile];
        fileHandler = nil;
    }
    NSLog(@"ios:停止录音!");
}

-(void)Release
{
    
    
    asrEventManager = nil;
    if(fileHandler != nil)
    {
        [fileHandler closeFile];
        fileHandler = nil;
    }
    NSLog(@"ios:卸载百度语音识别引擎!");
    
}


//百度语音事件处理
- (void)VoiceRecognitionClientWorkStatus:(int)workStatus obj:(id)aObj
{
    
    NSLog(@"ios:收到百度语音回调结果!回调事件名:%d  参数:%@",workStatus,aObj);
    
    
    NSString* dataString = nil;
    switch (workStatus) {
        case EVoiceRecognitionClientWorkStatusNewRecordData: {
            if(fileHandler != nil)
            {
                //音频数据写入指定文件格式
                [fileHandler writeData:(NSData *)aObj];
                NSLog(@"ios:音频数据写入成功!");
            }
            else
            {
                NSLog(@"ios:音频数据写入失败!文件流不存在! %@",fileHandler);
            }
            break;
        }
            
        case EVoiceRecognitionClientWorkStatusFinish:
        {
            //解析识别结果
            dataString = [self DataTOjsonString:aObj];//将结果转成json串
            NSDictionary* dicData = (NSDictionary*)aObj;
            if(dicData != nil){
                //根据key查value
                NSArray* resultDic = [dicData objectForKey:@"results_recognition"];
                if(resultDic != nil && resultDic.count > 0)
                {
                    dataString = resultDic[0];
                }
            }
            break;
        }
    }
    
    //组成返回给unity的参数
    NSString *stringMsgJson =[NSString stringWithFormat:@"{\"workStatus\":%d,\"data\":\"%@\"}",workStatus,dataString];
    NSLog(@"iso:回调给unity的json字符串为:%@",stringMsgJson);
    UnitySendMessage("BaiDuAsrManger", "OnReceiveIOSMessage", [stringMsgJson UTF8String]);
    
}



- (NSFileHandle *)createFileHandleWithName:(NSString *)aFileName isAppend:(BOOL)isAppend {
    NSFileHandle *fileHandle = nil;
    NSString *fileName = aFileName;
    
    int fd = -1;
    if (fileName) {
        if ([[NSFileManager defaultManager] fileExistsAtPath:fileName]&& !isAppend) {
            [[NSFileManager defaultManager] removeItemAtPath:fileName error:nil];
        }
        
        int flags = O_WRONLY | O_APPEND | O_CREAT;
        fd = open([fileName fileSystemRepresentation], flags, 0644);
    }
    
    if (fd != -1) {
        fileHandle = [[NSFileHandle alloc] initWithFileDescriptor:fd closeOnDealloc:YES];
    }
    
    return fileHandle;
}


-(NSString*)DataTOjsonString:(id)object
{
    NSString *jsonString = nil;
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:object
                                                       options:NSJSONWritingPrettyPrinted // Pass 0 if you don't care about the readability of the generated string
                                                         error:&error];
    if (! jsonData) {
        NSLog(@"Got an error: %@", error);
    } else {
        jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
    return jsonString;
}

@end



extern "C"
{
    
    //初始化
    void InitBaiDuVoiceManager()
    {
        [BaiDuVoiceManager.Instance InitBaiDuVoiceManager];
    }
    //开始录音
    void BaiDu_StartRecordVoice(const char* apikeys,const char* seckey,const char* appids,const char* filepaths,int pid)
    {
        NSLog(@"收到unity的启动录音命令!APIKEY为:%d",pid);
        
        NSString *apikey = [NSString stringWithUTF8String:apikeys];
        NSString *secretkey = [NSString stringWithUTF8String:seckey];
        NSString *appid = [NSString stringWithUTF8String:appids];
        NSString *filePath = [NSString stringWithUTF8String:filepaths];
        
        NSLog(@"apikey:%@",apikey);
        NSLog(@"secretkey:%@",secretkey);
        NSLog(@"appid:%@",appid);
        NSLog(@"filePath:%@",filePath);
        
        
        [BaiDuVoiceManager.Instance StartRecord:apikey SECRET:secretkey AppID:appid KeepFile:filePath PID:pid];
    }
    
    
    //取消录音
    void BaiDu_CancleRecordVoice()
    {
        [BaiDuVoiceManager.Instance CancelRecord];
    }
    
    /// <summary>
    /// 结束录音的方法 - 等待识别完成
    /// </summary>
    
    void BaiDu_StopRecordVoice()
    {
        [BaiDuVoiceManager.Instance StopRecord];
        
    }
    
    /// <summary>
    /// 注销识别引擎
    /// </summary>
    void BaiDu_ReleaseRecordVoice()
    {
        [BaiDuVoiceManager.Instance Release];
    }
    
    
    
}



