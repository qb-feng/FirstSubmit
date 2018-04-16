using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Linq;

public class UITestShowLevel : UIBaseInit, IUIRefresh, IOnUIChange, IUIUnDestroy, IUIState, IUIPopup
{
    private GameObject Back { get { return Get("Back"); } }
    private ScrollRect ScrollView { get { return Get<ScrollRect>("ScrollView"); } }
    private Transform HorizontalBookOption { get { return GetT("HorizontalBookOption"); } }
    private TMP_InputField SearchInput { get { return Get<TMP_InputField>("SearchInput"); } }
    private GameObject Search { get { return Get("Search"); } }
    private GameObject Classify { get { return Get("Classify"); } }

    private string m_levelType;
    /// <summary>
    /// 20180126因过滤需求增加Grid排序回调
    /// </summary>
    private Action sortGrid;

    void Awake()
    {
        UGUIEventListener.Get(Back).onPointerClick = OnClickBack;
        UGUIEventListener.Get(Search).onPointerClick = OnClickSearch;
        UGUIEventListener.Get(Classify).onPointerClick = OnClickClassify;
        UIManager.RegisterObserver(this);
        sortGrid += SortGrid;
        UITopBarBlack.Instance.Init();
        UITopBarBlack.Instance.SetLevelInfo("", LevelType.Game);
        UILoading.Start();
        var request = ServerManager.GetRequest<DownloadFileRequeset>(gameObject);
        request.Send(Application.persistentDataPath + "/abcgame.ab", "http://cdn.boly.abottletree.com/abcgame.ab");
        request.OnSuccEvent += succ =>
        {
            UILoading.End();
        };
    }

    void OnDestory()
    {
        sortGrid = null;
    }

    private void OnClickClassify(PointerEventData t)
    {
        UIManager.Instance.Open(typeof(UITestShowLevelClassify), this);
    }

    private void OnClickBack(PointerEventData data)
    {
        UIManager.Instance.Back();
        //Application.Quit();
        MessageBridge.Instance.ExitUnity();
    }

    private void OnClickSearch(PointerEventData data)
    {
        if (!string.IsNullOrEmpty(SearchInput.text))
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(SearchInput.text, @"^\d*$"))
            {
                int gameId = int.Parse(SearchInput.text);
                bool isDefined = System.Enum.IsDefined(typeof(UIGameName), gameId);
                var itemCps = ScrollView.content.GetComponentsInChildren<UITestShowLevelItem>(true);
                foreach (var item in itemCps)
                {
                    item.SetActive(isDefined, gameId);
                }
            }
            else
            {
                string gameName = SearchInput.text;
                var itemCps = ScrollView.content.GetComponentsInChildren<UITestShowLevelItem>(true);
                foreach (var item in itemCps)
                {
                    item.SetActive(gameName);
                }
            }

        }
        else
        {
            var itemCps = ScrollView.content.GetComponentsInChildren<UITestShowLevelItem>(true);
            foreach (var item in itemCps)
            {
                item.gameObject.SetActive(true);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollView.content);
    }

    public void Refresh(params object[] data)
    {
        if (ScrollView.content.childCount > 0)
            return;
        m_levelType = (string)data[0];
        if (m_levelType.Equals("game"))
        {
            HorizontalBookOption.gameObject.SetActive(false);
            int i = 0;
            foreach (var flItem in ConfigManager.Get<ConfigFirstLeapGameModel>())
            {
                if (string.IsNullOrEmpty(flItem.gameId))
                    continue;
                i++;
                if (!System.Enum.IsDefined(typeof(UIGameName), flItem.GameId))
                    continue;
                var type = flItem.GameDataType;
                var testDataModel = ConfigGameTestDataModel.GetTestDataModel(flItem.GameId, (int)type);
                if (testDataModel == null)
                {
                    Debug.Log("testDataModel = null");
                    continue;
                }

                var gameDataModel = new StartGameDataModel()
                {
                    gameId = flItem.GameId,
                    dataType = type,
                    dataList = testDataModel.DataList,
                    star = testDataModel.star,
                    difficulty = flItem.Difficulty,
                };
                var itemCp = CreateUIItem<UITestShowLevelItem>(ScrollView.content);
                itemCp.Init(this, gameDataModel, i);
            }
        }
        else
        {
            HorizontalBookOption.gameObject.SetActive(true);
            InitBookOptionList();
        }
    }

    private void InitBookOptionList()
    {
        HorizontalBookOption.ClearAllChild();
        foreach (var item in ConfigManager.Get<ConfigBookModel>())
        {
            var itemCp = CreateUIItem<UITestShowLevelBookOptionItem>(HorizontalBookOption);
            itemCp.Init(this, item);
            if (item.volume.Equals("2A"))
            {
                InitItemList(item);
            }
        }
    }

    public void InitItemList(ConfigBookModel model)
    {
        ScrollView.content.ClearAllChild();
        foreach (var item in ConfigManager.Get<ConfigLessonLevelModel>())
        {
            if (!item.id.StartsWith(model.bookId))
                continue;

            if (m_levelType == "lesson")
            {
                if (item.id.Split('_')[3] == "T")
                    continue;
            }
            else
            {
                if (item.id.Split('_')[3] != "T")
                    continue;
            }

            var itemCp = CreateUIItem<UITestShowLevelItem>(ScrollView.content);
            itemCp.Init(this, item);
        }
    }

    private List<string> TestGetData<T>(int count = 6)
    {
        var data = new List<string>();
        var field = typeof(T).GetField("id");
        var allWords = ConfigManager.Get<T>();
        var randomList = Utility.GetRandomSequence(allWords.Count, count);
        foreach (var index in randomList)
        {
            data.Add((string)field.GetValue(allWords[index]));
        }
        return data;
    }

    public void OnBeforeChange(Type previous, Type next)
    {
        if (next == typeof(UIWork))
        {
            ScrollView.content.ClearAllChild();
        }
    }

    public void OnAfterChange(Type previous, Type next, UIBaseInit nextUI)
    {
    }

    public void OnClassifySelect(List<int> gameIDList)
    {
        var itemCps = ScrollView.content.GetComponentsInChildren<UITestShowLevelItem>(true);
        foreach (var item in itemCps)
        {
            item.SetActive(gameIDList);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollView.content);
    }

    public void OnClassifySelect(List<Dictionary<string, int>> id_dtList)
    {
        for (int i = 0; i < id_dtList.Count; i++)
        {
            var itemCps = ScrollView.content.GetComponentsInChildren<UITestShowLevelItem>(true);
            foreach (var item in itemCps)
            {
                item.SetActive(id_dtList[i], i + 1);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollView.content);
        }
        if (sortGrid != null)
            sortGrid();
    }
    public void OnClassifySelect(List<Dictionary<string, int>> id_1dtList, List<Dictionary<string, int>> id_2dtList)
    {
        for (int i = 0; i < id_1dtList.Count; i++)
        {
            var itemCps = ScrollView.content.GetComponentsInChildren<UITestShowLevelItem>(true);
            foreach (var item in itemCps)
            {
                item.SetActive(id_1dtList[i], i + 1);
            }
        }
        if (sortGrid != null)
            sortGrid();

        //在符合第一个List的Item上进行判断显示or隐藏
        for (int i = 0; i < id_2dtList.Count; i++)
        {
            var itemCps = ScrollView.content.GetComponentsInChildren<UITestShowLevelItem>(false);
            foreach (var item in itemCps)
            {
                item.SecondSetActive(id_2dtList[i]);
            }
        }
        if (sortGrid != null)
            sortGrid();
    }

    //sortGrid回调函数在协成结束后进行回调
    private void SortGrid()
    {
        List<Transform> activeItems = new List<Transform>();
        var itemCps = ScrollView.content.GetComponentsInChildren<UITestShowLevelItem>(false);
        foreach (var item in itemCps)
        {
            activeItems.Add(item.transform);
        }
        //排序
        activeItems = activeItems.OrderBy(o => int.Parse(o.name)).ToList();
        for (int i = 0; i < activeItems.Count; i++)
        {
            activeItems[i].SetSiblingIndex(i + 1);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollView.content);
    }
}
