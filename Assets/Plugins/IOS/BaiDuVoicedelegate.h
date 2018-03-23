//
//  BaiDuVoicedelegate.h
//  Unity-iPhone
//
//  Created by puhanda on 2018/2/6.
//

#ifndef BaiDuVoicedelegate_h
#define BaiDuVoicedelegate_h


#endif /* BaiDuVoicedelegate_h */
#import <Foundation/Foundation.h>

//申明一个代理
@protocol BaiDuVoicedelegate<NSObject>
-(void)VoiceRecognitionClientWorkStatus:(int)workStatus obj:(id)aObj;
@end
