using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UITestShowLevelItem : UIBaseInitItem
{
    public enum DataTypeMeaning
    {
        单词 = 1,
        问句 = 2,
        单句 = 3,
        歌谣 = 4,
        对话 = 5,
        发音 = 6,
    }

    private TMP_Text LevelId { get { return Get<TMP_Text>("LevelId"); } }
    private TMP_Text LevelName { get { return Get<TMP_Text>("LevelName"); } }
    private TMP_Text LevelTeachType { get { return Get<TMP_Text>("LevelTeachType"); } }
    private TMP_Text LevelDataType { get { return Get<TMP_Text>("LevelDataType"); } }

    //private UITestShowLevel m_parent;
    private StartGameDataModel m_gameModel;

    public void Init(UITestShowLevel parent, ConfigLessonLevelModel model)
    {
        //m_parent = parent;
        m_gameModel = new StartGameDataModel()
        {
            gameId = model.GameId,
            dataList = model.GameData,
            dataType = model.GameDataType,
            star = model.star,
            guide = false,
            describe = ConfigGameLibraryModel.GetModel(model.GameId).describe,
            difficulty = model.Difficulty
        };
        InitInfo();
        LevelId.text += string.Format(" (U{0} L{1})", model.id.Split('_')[1], model.id.Split('_')[2]);
    }

    private void InitInfo()
    {
        var gameModel = ConfigGameLibraryModel.GetModel(m_gameModel.gameId);
        LevelId.text += gameModel.id.ToString();
        LevelName.text += gameModel.gameName;
        string gameDataType = ((int)m_gameModel.dataType).ToString();
        LevelDataType.text += string.Format("{0}({1})", gameDataType, ((DataTypeMeaning)m_gameModel.dataType).ToString());
        LevelTeachType.text += gameModel.GetTeachType(gameDataType);
        UGUIEventListener.Get(gameObject).onPointerClick = OnClick;
        transform.name = ((int)m_gameModel.dataType).ToString();       
    }

    public void Init(UITestShowLevel parent, StartGameDataModel gameModel, int index)
    {
        //m_parent = parent;
        m_gameModel = gameModel;
        InitInfo();
        LevelId.text = string.Format("ID : {0}({1})", index, gameModel.gameId);
    }

    private void OnClick(PointerEventData data)
    {
        var gameName = (UIGameName)m_gameModel.gameId;
        UIManager.Instance.Open(System.Type.GetType(gameName.ToString()), m_gameModel);
    }

    public void SetActive(bool isDefined, int gameId)
    {
        gameObject.SetActive(isDefined && m_gameModel.gameId == gameId);
    }
    public void SetActive(string gameName)
    {
        gameObject.SetActive(LevelName.text.Contains(gameName));
    }
    public void SetActive(List<int> gameIdList)
    {
        foreach (var num in gameIdList)
        {
            if (m_gameModel.gameId == num)
            {
                gameObject.SetActive(true);
                return;
            }
        }
        gameObject.SetActive(false);
    }

    public void SetActive(Dictionary<string, int> ID_DtList, int time)
    {
        bool isAct = false;
        //TODO先判断id中是否包含“-”之后协成分先后显示
        foreach (var item in ID_DtList)
        {
            if (item.Key.Contains("-"))
            {
                if (m_gameModel.gameId == int.Parse(item.Key.Split('-')[0]) && m_gameModel.dataType == (DataType)(item.Value))
                {
                    isAct = true;
                }
            }
            else
            {
                if (m_gameModel.gameId == int.Parse(item.Key.Split('-')[0]) && m_gameModel.dataType == (DataType)(item.Value))
                {
                    isAct = true;
                }
            }
        }
        if (time == 1)
            gameObject.SetActive(isAct);
        else
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(true);
            else
                gameObject.SetActive(isAct);            
        }
    }
    public void SecondSetActive(Dictionary<string, int> ID_DtList)
    {
        bool isAct = false;
        //TODO先判断id中是否包含“-”之后协成分先后显示
        foreach (var item in ID_DtList)
        {
            if (item.Key.Contains("-"))
            {
                if (m_gameModel.gameId == int.Parse(item.Key.Split('-')[0]) && m_gameModel.dataType == (DataType)(item.Value))
                {
                    isAct = true;
                }
            }
            else
            {
                if (m_gameModel.gameId == int.Parse(item.Key.Split('-')[0]) && m_gameModel.dataType == (DataType)(item.Value))
                {
                    isAct = true;
                }
            }
        }       
        gameObject.SetActive(isAct);
       
    }

}
