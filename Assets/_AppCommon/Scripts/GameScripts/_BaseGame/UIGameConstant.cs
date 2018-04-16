using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 游戏常量定义
/// </summary>
public class UIGameConstant
{

}

/// <summary>
/// 游戏枚举定义
/// </summary>
public enum UIGameName
{
    //从这开始定义任务游戏关卡
    UITurnCardGame = 1,
    //UIBirdEatWord = 2,
    UIShootingWords = 2,//替换飞翔的小鸟
    UIThreeCupGuess = 4,
    // UIDesktopCar = 5,
    UIDesktopCarPre = 5,
    //UITrainWord = 6,
    //UISelectAOrAn = 7,
    //UILookTent = 8,//帐篷游戏替换为窗户
    UILookWindow = 8,//
    UISpinningPlate = 9,//
    //UIWhatNext = 10,
    //UISongPicture = 11,
    UICutThing = 12,//切水果
    //UIBigShake = 13,
    UIBigShakeFruit = 13,
    UIVoiceRecognition = 14,
    //UIDrawShape = 15,
    //UITwoByTwo = 16,
    //UIBigBen = 17,
    //UITODO = 18,
    UILaboratory = 19,
    UIConveyor = 20,
    UIDropBall = 21,
    UIWhitewashing = 22,
    UISupermarketShopping = 23,
    UIColourFruit = 24,
    UIEyesight = 25,
    //UIColorJuicer = 26,
    UILaboratoryColor = 27,
    //UIColourFruitMixture = 28,
    UISharkEatFish = 29,
    UIThreeBoxGuess = 30,
    UITurnCardGame2 = 31,
    //UITrainWord2 = 32,
    UICutThingBread = 33,
    UITrainWordNumber = 35,
    UIEyesightNum = 36,
    //UISupermarketShoppingUnit3Lesson2 = 41,
    //UIEyesightUnit4Lesson1 = 43,
    //UIBigShake = 44,
    //UIEyesightUnit4Lesson4 = 45,
    UITrainWordCommon = 52,
    UIKitchen = 53,
    UIDropBall4Row = 54,
    //UILittleGirl = 55,
    //UILittleBoy = 56,
    //UISequence = 57,
    //UISelectPreposition = 58,
    //UIMatchAnswer = 59,
    //UIWashAnimal = 60,
    UIRiver = 61,
    //UIThreeGroup = 62,//三只杯子分组出现
    //UISongPicture7 = 68,
    //UISongPicture8 = 69,
    //UICryAndSmile = 70,
    //UICryAndSmileU7L2 = 71,
    //UIYesOrNo = 72,
    //UILookForAnimals = 73,
    //UILookForCar = 74,
    UIPuzzleGame = 78,
    UISongPicture = 81,// 所有的歌谣都用这个id
    //UIEyesightBook0Unit3Lesson2 = 86,
    //UIColorJuicerS1 = 87,
    //UISequenceS1U3L1 = 88,
    //UISelectNumber = 91,
    UIPenguinSkate = 92,//企鹅滑冰
    //UIBambooRaft = 94,
    //UIMyFeatures = 112,
    UIPolarExplore = 120, // 极地探险
    UIBolyShoot = 121, // 射箭游戏
    UIBolySend = 122, // 寄信游戏
    UIHavePhoto = 123, // 洗照片游戏
    UIWatchTV = 124, // 看电视游戏
    UIPronunciationSelect = 125, // 自然拼读
    UISafeBox = 126, // 保险箱（单词）
    UIMakeClothes = 127, // 衣服加工厂
    UIArtMuseum = 128, // 艺术馆
    UISafeBoxNew = 129, // 新保险箱（句子）
    UIBeach = 130, // 海滩
    UIBottleMessage = 131, // 瓶中信
    UIBottleMessageNew = 132, // 新瓶中信（补空缺）
    UICatchBaby = 133, // 娃娃机
    UIMiracleDoor = 134, // 神奇传送门
    UITangram = 135,// 七巧板
    UIFlyCar = 136,// 极品飞车
    UICupid = 137, // 小丘比特
    UIDuckHunter = 138, // 鸭子猎手
    UIColoursLamp = 139, // 彩色聚光灯
    UISeaSnakeBead = 140, // 海蛇宝珠
    UISnowMountainTop = 141, // 雪山之巅
    UIPetCoffee = 142, // 萌宠咖啡
    UIStarThrough = 143, // 星际穿越
    UISuperChameleon = 150,//超级变色龙
    UIRayReflect = 151,//光合实验室
    UICandyMania = 152,//糖果工坊
    UICandySeesaw = 153,//糖果跷跷板
    UIYellowDuck = 154,  //小黄鸭
    UILineBall = 155,     //小球进洞
    UIHitMole = 156,      //欢乐打地鼠
    UICityHunter = 157,   //城市猎人
    UIAntSoldier = 158,   //蚂蚁雄兵
    UIRunningFrog = 159,  //奔跑吧青蛙
    UICatJumping = 160,   //喵喵喵
    UICollideBall = 161,  //碰撞球
    UISpellStump = 162, //拼树墩
    UIDeliciousPizza = 163,   //美味披萨
    UIMouseRun = 164, //蚂蚁快跑
    UIMouseRunSentence = 165,     //蚂蚁快跑考拼句子
    UIDogJumping = 166,   //汪汪汪
    UIJackAndPea = 167,   //杰克和豌豆
    UIPenguinFishing = 168,   //企鹅捕鱼
    UIPenguinFishingSentence = 169,   //企鹅捕鱼考拼句子

    UIJournal = 170,//航海日记
    UIDeepSee = 171,//深海探秘
    UISuperCrane = 172,//超级吊车
    UISupermanHitMonster = 173,//超人打小怪
    UIGreedyBall = 174,//贪吃阿米巴
    UIInfiniteBlankHole = 175,//无限黑洞 - 换皮174
    UIShootingPractice = 176,//打靶
    UIHiveQuest = 177,//蜂巢探秘
    UITackMachine = 178,//按键取票机
    UINinjiaAicient = 179,//忍者无敌
    UIRevolvingRestaurant = 180,//旋转餐厅
    UISomersaultCloud = 181,//筋斗云
    UIOuterSpaceUFO = 182,//外太空UFO
    UIDuckSwim = 183,//小鸭子游啊游 - 换皮UIDesktopCarPre
    UIPotionsClass = 184,//魔药课
    UIHappyExpress = 185,//开心速递
    UIStirSkewers = 186,//旺火烤串 - 换皮大震荡UIBigShakeFruit
    UIWraith = 187,//幻影战机 - 换皮极品飞车UIFlyCar
    UICutWatermelon = 188,//切西瓜 - 换皮切水果UICutThing
    UIHandUpRedLight = 189,//红灯高挂 - 换皮大震荡UIBigShakeFruit
    UICreamSoup = 190,//奶油蘑菇场 - 换皮魔药课UIPotionsClass
    UIFacsimileNumber = 191,
    UIWhiteboardPuzzle = 192,//自然拼读白板游戏
    UIHandWritten = 193,

    UILianliankanletters = 194,//字母连连看 - 换皮拼树墩UISpellStump
    UISquirrelAndFood = 195,  //松鼠储备粮-换皮拼树墩UISpellStump
    UIEarthquakeComing = 196, //地震来了-换皮大震荡UIBigShakeFruit
    UIArtGallery = 197,       //美术馆-换皮博物馆UIArtMuseum
    UIDownhillBike = 198,     //下山单车-换皮雪山之巅UISnowMountainTop
    UITeachingPresent = 201,//教师端 - 教学单词呈现模型（只用来给老师当ppt使用的）lesson1
    UITeachingPresentAskType = 202,//教师端 - 教学句子呈现模型（只用来给老师当ppt使用的）lesson2
    UITeachingPresentPhonicsType = 203,//教师端 - 教学发音呈现模型（lesson3）
    UITeachingPresentReadingType = 204,//教师端 - 教学阅读呈现模型（lesson4）

    //练习模型
    UIWordPractiveVoicePicture = 500,//单词音图
    UIWordPracticePictureWord = 501,//单词图形
    UIWordPracticeVoiceWord = 502,//单词音形
    UIWordPracticeWordPicture = 503,//单词形图
    UIWordPracticeVoiceWordHead = 504,//听听单词选择首字母
    UIWordPracticeWordMeaning = 506,//单词英中匹配
    UIWordPracticeLine = 508,//单词连线
    UIWordPracticeVoicePictureLine = 509,//音图连线
    UIWordPractiveWordMatchLetter = 510,//字母配对
    UIWordPracticeSpeech = 511,//单词跟读
    UIListenImage = 513,//听音判断
    UIListenWord = 514,//听词判断
    UIWordJudge = 515,//单词判断
    UIWordPracticeSort = 516,//单词排序
    UIWordSortByChinese = 517,//拼单词（根据汉语意思拼)
    UIWordListerSortPicture = 518,//音图排序
    UIWordArrangeSentence = 519,//单词排列成句子
    UIWordPracticeSpell = 520,//单词拼写
    UIImageLack = 521,//看图补缺
    UISentenceVoicePicture = 522,//句子音图
    UISentenceVoiceSentence = 523,//听音识句
    UISenttencePictureSentence = 524,//看图识句
    UISentenceSentencePicture = 525,//看句子识图
    UISentenceSuppleWord = 526,//给句子补充单词
    UISentenceRepeatSentence = 529,//跟读句子
    UISentenceVoiceJudgeSentence = 530,//听录音判断句子
    UISentenceVoiceJudgePicture = 531,//听录音判断图片
    UISentencePictureSuppleWord = 532,//根据图片补充句子单词
    UISentencePictureLine = 533,//将句子和图片连线
    UIPracticeSentenceSpell = 534,//句子排序
    UISentencePictureSortSentence = 535,//根据图片将单词排列成句子
    UIListenSentence = 536,//句子听力
    UISentenceVoiceSortSentence = 538,//听录音排列句子
    UITalkSentence = 550,//对话
    //End,以上只定义游戏界面枚举, 定义其他界面枚举必须定义到最上面
}
