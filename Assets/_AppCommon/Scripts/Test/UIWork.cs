using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIWork : UIBaseInit, IUIState
{
    private GameObject GameLevel { get { return Get("GameLevel"); } }
    private GameObject LessonLevel { get { return Get("LessonLevel"); } }
    private GameObject PracticeLevel { get { return Get("PracticeLevel"); } }
    private GameObject CustomLevel { get { return Get("CustomLevel"); } }

    // Use this for initialization
    void Start()
    {
        //UITopBar.Instance.Init();
        UITopBarBlack.Instance.Init();
        UITopBarBlack.Instance.SetLevelInfo("", LevelType.Game);
        UGUIEventListener.Get(GameLevel).onPointerClick = OnClickGameLevel;
        UGUIEventListener.Get(LessonLevel).onPointerClick = OnClickLessonLevel;
        UGUIEventListener.Get(PracticeLevel).onPointerClick = OnClickPracticeLevel;
        UGUIEventListener.Get(CustomLevel).onPointerClick = OnClickCustomLevel;
    }

    private void OnUpdateConfigCompleteEvent()
    {
        //Player.Instance.InitUser("", "");
    }

    private void OnClickGameLevel(PointerEventData data)
    {
        UIManager.Instance.Open(typeof(UITestShowLevel), "game");
    }

    private void OnClickLessonLevel(PointerEventData data)
    {
        UIManager.Instance.Open(typeof(UITestShowLevel), "lesson");
    }

    private void OnClickPracticeLevel(PointerEventData data)
    {
        UIManager.Instance.Open(typeof(UITestShowLevel), "practice");
    }

    private void OnClickCustomLevel(PointerEventData data)
    {
        //单词
        var gameUI = typeof(UIInfiniteBlankHole);
        var dataList = new List<string>()
        {
               "1_8_0_1",
               "1_8_0_2",
               "1_8_0_3",
               "1_8_0_4"
        };
        var gameData = new StartGameDataModel()
        {
            gameId = (int)System.Enum.Parse(typeof(UIGameName), gameUI.Name),
            dataList = dataList,
            dataType = DataType.Word,
            star = dataList.Count,
            guide = false,
        };
        UIManager.Instance.Open(gameUI, gameData);

        //疑问句
        //var gameUI = typeof(UIMouseRun);

        //var dataList = new List<string>() { "105_6_6_2_1",  "102_1_6_2_1",
        //                              };
        //var gameData = new StartGameDataModel()
        //{
        //    gameId = (int)gameUI,
        //    dataList = dataList,
        //    dataType = DataType.Ask,
        //    star = dataList.Count,
        //    guide = false,

        //};
        //UIManager.Instance.Open(gameUI, gameData);

        //陈述句
        //var gameUI = typeof(UIMouseRunSentence);

        //var dataList = new List<string>() { "103_9_2_3_2", "104_2_2_3_2" };
        //var gameData = new StartGameDataModel()
        //{
        //    gameId = (int)gameUI,
        //    dataList = dataList,
        //    dataType = DataType.Say,
        //    star = dataList.Count,
        //    guide = false,

        //};
        //UIManager.Instance.Open(gameUI, gameData);


        //听力
        //var gameUI = typeof(UIAntSoldier);

        //var dataList = new List<string>() { "102_1_3_6_1", "103_8_3_6_1", "101_10_3_6_1","103_6_3_6_1","103_9_3_6_1","104_3_3_6_1"
        //                              };
        //var gameData = new StartGameDataModel()
        //{
        //    gameId = (int)gameUI,
        //    dataList = dataList,
        //    dataType = DataType.Pronunciation,
        //    star = dataList.Count,
        //    guide = false,

        //};
        //UIManager.Instance.Open(gameUI, gameData);

        //情景对话
        //var gameUI = typeof(UIJackAndPea);
        //var dataList = new List<string>() { 
        //       "100_1_4_5_1","100_1_4_5_2","100_1_4_5_3","100_1_4_5_4"};
        //var gameData = new StartGameDataModel()
        //{
        //    gameId = (int)gameUI,
        //    dataList = dataList,
        //    dataType = DataType.Talk,
        //    star = dataList.Count,
        //    guide = false,

        //};
        //UIManager.Instance.Open(gameUI, gameData);

    }
}


