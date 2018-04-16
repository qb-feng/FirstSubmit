using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class UITestShowLevelClassify : UIBaseInit, IUIRefresh, IUIPopup
{

    /// <summary>
    /// 单选框父节点
    /// </summary>
    private Transform SelectContent { get { return GetT("SelectContent"); } }
    /// <summary>
    /// 知识点按钮
    /// </summary>
    private Transform Knowledge { get { return GetT("Knowledge"); } }
    /// <summary>
    /// 知识点按钮开关-防止反复创建浪费资源（创建后进行显示和隐藏，按Tag显示，达到降低消耗）
    /// </summary>
    private Image KnowledgeSwitch { get { return Get<Image>("KnowledgeSwitch"); } }
    /// <summary>
    /// 教研类型按钮
    /// </summary>
    private Transform Teaching { get { return GetT("Teaching"); } }
    /// <summary>
    /// 教研类型按钮开关
    /// </summary>
    private Image TeachingSwitch { get { return Get<Image>("TeachingSwitch"); } }
    /// <summary>
    /// 重置按钮
    /// </summary>
    private Transform ResetButton { get { return GetT("ResetButton"); } }
    /// <summary>
    /// 确定按钮
    /// </summary>
    private Transform ConfirmButton { get { return GetT("ConfirmButton"); } }
    private Image Close { get { return Get<Image>("Close"); } }
    private Image Bg { get { return Get<Image>("Bg"); } }
    private bool TrueIsKnowledgeFlaseIsTeaching;
    /// <summary>
    /// 判断当前已经勾选的条件
    /// </summary>
    private List<string> SelectTogleList = new List<string>();
    /// <summary>
    /// work主页面
    /// </summary>
    private UITestShowLevel testShowLevel;
    /// <summary>
    /// 添加过滤选项元素
    /// </summary>
    public Action<string> CurrentSelectToggleAdd;
    /// <summary>
    /// 删除过滤选项元素
    /// </summary>
    public Action<string> CurrentSelectToggleDel;

    /// <summary>
    /// 字典Key为搜类类型，value为所有ID的值“|”相隔
    /// </summary>
    private Dictionary<string, string> classifyKnowledgeDic;
    /// <summary>
    /// 字典Key为搜类类型，value为所有ID的值“|”相隔
    /// </summary>
    private Dictionary<string, string> classifyTeachingDic;
    /// <summary>
    /// 所有知识点标签合集
    /// </summary>
    private List<string> classifyKnowledgeList;
    /// <summary>
    /// 所有教研类型标签合集
    /// </summary>
    private List<string> classifyTeachingList;

    void Start()
    {
        UGUIEventListener.Get(Knowledge).onPointerClick += OnClickKnowledge;
        UGUIEventListener.Get(Teaching).onPointerClick += OnClickTeaching;
        UGUIEventListener.Get(ResetButton).onPointerClick += OnClickReset;
        UGUIEventListener.Get(ConfirmButton).onPointerClick += OnClickConfirm;
        UGUIEventListener.Get(Close).onPointerClick += OnClickClose;
        UGUIEventListener.Get(Bg).onPointerClick += OnClickClose;
        CurrentSelectToggleAdd += AddSelectGameObject;
        CurrentSelectToggleDel += DelSelectGameObject;
        //创建查询字典
        CreateDicToSearch();
        //创建分类选项标签
        CreateClassifyItem();
        //默认第一次进来选择知识点分类
        TrueIsKnowledgeFlaseIsTeaching = true; ;
        ClickClassify(TrueIsKnowledgeFlaseIsTeaching);

    }


    void OnDestory()
    {
        CurrentSelectToggleAdd = null;
        CurrentSelectToggleDel = null;
    }

    public void Refresh(params object[] data)
    {
        if (data.Length <= 0)
            return;
        else
        {
            testShowLevel = (UITestShowLevel)data[0];
        }
    }

    private void AddSelectGameObject(string t)
    {
        if (SelectTogleList != null)
        {
            if (!SelectTogleList.Contains(t))
                SelectTogleList.Add(t);
        }
    }
    private void DelSelectGameObject(string t)
    {
        if (SelectTogleList != null)
        {
            if (SelectTogleList.Contains(t))
                SelectTogleList.Remove(t);
        }
    }

    /// <summary>
    /// 点击知识点按钮事件
    /// </summary>
    /// <param name="t"></param>
    private void OnClickKnowledge(UnityEngine.EventSystems.PointerEventData t)
    {
        if (TrueIsKnowledgeFlaseIsTeaching)
            return;
        else
        {
            TrueIsKnowledgeFlaseIsTeaching = true;
            ClickClassify(TrueIsKnowledgeFlaseIsTeaching);
        }
    }

    /// <summary>
    /// 点击教研类型按钮事件
    /// </summary>
    /// <param name="t"></param>
    private void OnClickTeaching(UnityEngine.EventSystems.PointerEventData t)
    {
        if (!TrueIsKnowledgeFlaseIsTeaching)
            return;
        else
        {
            TrueIsKnowledgeFlaseIsTeaching = false;
            ClickClassify(TrueIsKnowledgeFlaseIsTeaching);
        }
    }

    /// <summary>
    /// 进行过滤显示
    /// </summary>
    /// <param name="isKonewldgeOrTeaching"></param>
    private void ClickClassify(bool trueIsKnowledgeFlaseIsTeaching)
    {
        //SelectTogleList.Clear();
        KnowledgeSwitch.enabled = trueIsKnowledgeFlaseIsTeaching;
        TeachingSwitch.enabled = !trueIsKnowledgeFlaseIsTeaching;
        foreach (Transform child in SelectContent)
        {
            //显示知识点分类选项
            if (trueIsKnowledgeFlaseIsTeaching)
            {
                if (child.tag == "Knowledge")
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
            //显示教研类型分类选项
            else
            {
                if (child.tag == "Teaching")
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
        //TODO进行排序
        List<Transform> activeItems = new List<Transform>();
        foreach (Transform item in SelectContent)
        {
            activeItems.Add(item);
        }
        //名字排序
        activeItems = activeItems.OrderBy(o => int.Parse(o.name)).ToList();
        for (int i = 0; i < activeItems.Count; i++)
        {
            activeItems[i].SetSiblingIndex(i + 1);
        }
        //长度排序
        List<List<Transform>> myList = new List<List<Transform>>();
        for (int i = 1; i <= 5; i++)
        {
            List<Transform> myListChild = new List<Transform>();
            for (int j = 0; j < SelectContent.childCount; j++)
            {
                if (SelectContent.GetChild(j).name == i.ToString())
                {
                    myListChild.Add(SelectContent.GetChild(j));
                }
            }
            myList.Add(myListChild);
        }
        for (int i = 0; i < myList.Count; i++)
        {
            activeItems = new List<Transform>();
            foreach (Transform jj in myList[i])
            {
                activeItems.Add(jj);
            }
            activeItems = activeItems.OrderBy(j => j.transform.GetComponent<UITestShowLevelClassifyItem>().ClassifyText.text.Length).ToList();
            for (int j = 0; j < activeItems.Count; j++)
            {
                activeItems[j].SetSiblingIndex((i + 1)*100+j);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)SelectContent);
    }

    /// <summary>
    /// 点击重置按钮事件
    /// </summary>
    /// <param name="t"></param>
    private void OnClickReset(UnityEngine.EventSystems.PointerEventData t)
    {
        SetToggleIson(false);
        SelectTogleList.Clear();
    }

    /// <summary>
    /// 设置所有Toggle是否全选or清除
    /// </summary>
    /// <param name="isOn"></param>
    private void SetToggleIson(bool bIsOn)
    {
        foreach (Transform child in SelectContent)
        {
            if (child.GetComponent<Toggle>() != null)
            {
                child.GetComponent<Toggle>().isOn = bIsOn;
            }
        }
    }
    /// <summary>
    /// 点击提交按钮事件
    /// </summary>
    /// <param name="t"></param>
    private void OnClickConfirm(UnityEngine.EventSystems.PointerEventData t)
    {

        #region 条件且关系过滤 （未使用暂时留存）
        /*
        List<string> typeName = new List<string>();
        List<int> typeId = new List<int>();
        List<List<int>> typeIds = new List<List<int>>();
        foreach (var item in SelectTogleList)
        {
            if (item.GetComponent<UITestShowLevelClassifyItem>() != null)
            {
                typeName.Add(item.GetComponent<UITestShowLevelClassifyItem>().ClassifyText.text);
                Debug.LogError(item.GetComponent<UITestShowLevelClassifyItem>().ClassifyText.text);
            }
        }
        //遍历标签中的ID
        //根据知识点分类
        if (TrueIsKnowledgeFlaseIsTeaching)
        {
            foreach (var name in typeName)
            {
                typeId = new List<int>();
                foreach (var item in classifyKnowledgeDic.Keys)
                {
                    if (name == item)
                    {
                        var idString = classifyKnowledgeDic[item].Split('|');                       
                        foreach (var id in idString)
                        {
                            if (id.Equals(""))
                            {
                                continue;
                            }
                            if (typeId.Contains(int.Parse(id)))
                            {
                                continue;
                            }
                            else
                            {
                                typeId.Add(int.Parse(id));
                            }
                        }
                    }
                }
                typeIds.Add(typeId);
            }
            
        }
        else
        {
            foreach (var name in typeName)
            {
                typeId = new List<int>();
                foreach (var item in classifyTeachingDic.Keys)
                {
                    if (name == item)
                    {
                        var idString = classifyTeachingDic[item].Split('|');
                        foreach (var id in idString)
                        {
                            if (id.Equals(""))
                            {
                                continue;
                            }
                            if (typeId.Contains(int.Parse(id)))
                            {
                                continue;
                            }
                            else
                            {
                                typeId.Add(int.Parse(id));
                            }
                        }
                    }
                }
                typeIds.Add(typeId);
            }
        }        
        //这几个表中找相同的ID
        List<int> wantedInt = new List<int>();
        //进行重复判断用字典key为ID,value为次数
        Dictionary<int,int> repeatNum = new Dictionary<int,int> ();
        //List<int> c = b.Except(a).ToList();
        int repeat = typeIds.Count;
        foreach (List<int> listInt in typeIds)
        {
            foreach (var id in listInt)
            {
                if (repeatNum.ContainsKey(id))
                {
                    int tempIndex = repeatNum[id] + 1;
                    repeatNum[id] = tempIndex;
                }
                else
                {
                    repeatNum.Add(id, 0);
                }                
            }
        }
        foreach (var a in repeatNum)
        {
            if (a.Value == repeat - 1)
            {
                wantedInt.Add(a.Key);               
            }
           
        }       
        testShowLevel.OnClassifySelect(wantedInt);*/
        #endregion

        #region 多选全部显示并排序 dateType为隐形过滤条件
        List<string> typeNames = new List<string>();
        foreach (var item in SelectTogleList)
        {
            typeNames.Add(item);
        }

        //testShowLevel.OnClassifySelect(wantedInt);
        /*    
         * private Dictionary<string, string> classifyKnowledgeDic;   
         * private Dictionary<string, string> classifyTeachingDic;    
         * private List<string> classifyKnowledgeList;    
         * private List<string> classifyTeachingList;
         */

        //如果有单元词汇or功能句型进行特殊筛选，如果没有就分批次显示
        //confirmList是准备要提交的数据，由于筛选需要游戏ID和DateType两项，故字典中Key为游戏ID,Value为DateType
        List<Dictionary<string, int>> confirmKnowledgeList = new List<Dictionary<string, int>>();
        List<Dictionary<string, int>> confirmTeachingList = new List<Dictionary<string, int>>();
        List<Dictionary<string, int>> confirmMixList = new List<Dictionary<string, int>>();
        foreach (var typeName in typeNames)
        {
            Dictionary<string, int> tempDic_Id_DT = new Dictionary<string, int>();
            if (typeName.Split('_')[1] == "Knowledge")
            {
                //TODO知识点
                if (typeName.Split('_')[0] == "单元词汇")
                {
                    //获取当前所选标签游戏ID集合
                    var typeIds = classifyKnowledgeDic[typeName.Split('_')[0]].Split('|');
                    //根据ID获取DateType,单一则取，多种只取dateType为1的
                    foreach (var gameId in typeIds)
                    {
                        foreach (var item in ConfigManager.Get<ConfigGameLibraryModel>())
                        {
                            if (item.id == int.Parse(gameId))
                            {
                                tempDic_Id_DT.Add(gameId, 1);
                            }
                        }
                    }
                }
                else if (typeName.Split('_')[0] == "功能句型")
                {
                    //获取当前所选标签游戏ID集合
                    var typeIds = classifyKnowledgeDic[typeName.Split('_')[0]].Split('|');
                    //根据ID获取DateType,单一则取，多种只取dateType为1的
                    foreach (var gameId in typeIds)
                    {
                        foreach (var item in ConfigManager.Get<ConfigGameLibraryModel>())
                        {
                            if (item.id == int.Parse(gameId))
                            {
                                if (item.dataType.Contains("|"))
                                {
                                    var DTarry = item.dataType.Split('|');
                                    foreach (var dt in DTarry)
                                    {
                                        if (dt == "1")
                                        {
                                            continue;
                                        }
                                        else if (dt == "2")
                                        {
                                            tempDic_Id_DT.Add(gameId + "-2", 2);
                                        }
                                        else if (dt == "3")
                                        {
                                            tempDic_Id_DT.Add(gameId + "-3", 3);
                                        }
                                    }
                                }
                                else
                                {
                                    tempDic_Id_DT.Add(gameId + "-" + item.dataType, int.Parse(item.dataType));
                                }
                            }
                        }
                    }
                }
                else if (typeName.Split('_')[0] == "自然拼读")
                {
                    //获取当前所选标签游戏ID集合
                    var typeIds = classifyKnowledgeDic[typeName.Split('_')[0]].Split('|');
                    //根据ID获取DateType,单一则取，多种只取dateType为1的
                    foreach (var gameId in typeIds)
                    {
                        foreach (var item in ConfigManager.Get<ConfigGameLibraryModel>())
                        {
                            if (item.id == int.Parse(gameId))
                            {
                                if (item.dataType == "1")
                                {
                                    tempDic_Id_DT.Add(gameId + "-1", 1);
                                }
                                else
                                {
                                    tempDic_Id_DT.Add(gameId + "-6", 6);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //获取当前所选标签游戏ID集合
                    var typeIds = classifyKnowledgeDic[typeName.Split('_')[0]].Split('|');
                    //根据ID获取DateType,单一则取，多种只取dateType为1的
                    foreach (var gameId in typeIds)
                    {
                        foreach (var item in ConfigManager.Get<ConfigGameLibraryModel>())
                        {
                            if (item.id == int.Parse(gameId))
                            {
                                tempDic_Id_DT.Add(gameId, int.Parse(item.dataType));
                            }
                        }
                    }
                }
                confirmKnowledgeList.Add(tempDic_Id_DT);
            }
            else
            {
                //TODO教研类型                
                //获取当前所选标签游戏ID集合
                var typeIds = classifyTeachingDic[typeName.Split('_')[0]].Split('|');
                //根据ID获取DateType,单一则取，多种只取dateType为1的
                foreach (var gameId in typeIds)
                {
                    foreach (var item in ConfigManager.Get<ConfigGameLibraryModel>())
                    {
                        if (item.id == int.Parse(gameId))
                        {
                            if (item.teachingType.Contains("|"))
                            {
                                var TTarry = item.teachingType.Split('|');
                                int tempIndex = 0;
                                for (int i = 0; i < TTarry.Length; i++)
                                {
                                    if (TTarry[i] == typeName.Split('_')[0])
                                    {
                                        tempIndex = i;
                                    }
                                }
                                tempDic_Id_DT.Add(gameId, int.Parse(item.dataType.Split('|')[tempIndex]));
                            }
                            else
                            {
                                tempDic_Id_DT.Add(gameId, int.Parse(item.dataType));
                            }
                        }
                    }
                }
                confirmTeachingList.Add(tempDic_Id_DT);
            }
        }

        if (confirmTeachingList.Count == 0)
            testShowLevel.OnClassifySelect(confirmKnowledgeList);
        else if (confirmKnowledgeList.Count == 0)
            testShowLevel.OnClassifySelect(confirmTeachingList);
        else
            testShowLevel.OnClassifySelect(confirmKnowledgeList, confirmTeachingList);


        //         foreach (Dictionary<string, int> item in confirmTeachingList)
        //         {
        //             foreach (var it in item)
        //             {
        //                 Debug.LogError("游戏ID："+it.Key +"游戏DateType" +it.Value);
        //             }
        //             Debug.LogError("***************");
        //         }
        //        testShowLevel.OnClassifySelect(confirmList);
        #endregion

        UIManager.Instance.Close();
    }
    /// <summary>
    /// 点击关闭按钮事件
    /// </summary>
    /// <param name="t"></param>
    private void OnClickClose(UnityEngine.EventSystems.PointerEventData t)
    {
        UIManager.Instance.Close();
    }


    /// <summary>
    /// 创建所有分类选项
    /// </summary>
    private void CreateClassifyItem()
    {
        for (int i = 0; i < classifyKnowledgeList.Count; i++)
        {
            CreateUIItem<UITestShowLevelClassifyItem>(SelectContent).InitClassify(classifyKnowledgeList[i], "Knowledge", this);
        }
        for (int j = 0; j < classifyTeachingList.Count; j++)
        {
            CreateUIItem<UITestShowLevelClassifyItem>(SelectContent).InitClassify(classifyTeachingList[j], "Teaching", this);
        }
    }

    /// <summary>
    /// 创建查询字典
    /// </summary>
    private void CreateDicToSearch()
    {
        //新增查询字典
        classifyKnowledgeDic = new Dictionary<string, string>();
        classifyTeachingDic = new Dictionary<string, string>();
        classifyKnowledgeList = new List<string>();
        classifyTeachingList = new List<string>();
        string tempIdStr = "";
        //统计知识点
        foreach (var item in ConfigManager.Get<ConfigGameLibraryModel>())
        {
            if (item.sortType.Contains("|"))
            {
                var itemSplit = item.sortType.Split('|');
                for (int i = 0; i < itemSplit.Length; i++)
                {
                    if (classifyKnowledgeList.Contains(itemSplit[i]))
                        continue;
                    else
                        classifyKnowledgeList.Add(itemSplit[i]);
                }
            }
            else
            {
                if (classifyKnowledgeList.Contains(item.sortType))
                    continue;
                else
                    classifyKnowledgeList.Add(item.sortType);
            }
        }
        //将知识点和相应ID添加进字典中
        foreach (var knowledgeType in classifyKnowledgeList)
        {
            tempIdStr = "";
            foreach (var item in ConfigManager.Get<ConfigGameLibraryModel>())
            {
                if (item.sortType.Contains(knowledgeType))
                {
                    tempIdStr += item.id + "|";
                }
            }
            if (tempIdStr[tempIdStr.Length - 1] == '|')
            {
                tempIdStr = tempIdStr.Remove(tempIdStr.Length - 1);
            }
            classifyKnowledgeDic.Add(knowledgeType, tempIdStr);
        }

        //统计教研类型       
        tempIdStr = "";
        foreach (var item in ConfigManager.Get<ConfigGameLibraryModel>())
        {
            if (item.teachingType.Contains("|"))
            {
                var itemSplit = item.teachingType.Split('|');
                for (int i = 0; i < itemSplit.Length; i++)
                {
                    if (classifyTeachingList.Contains(itemSplit[i]))
                        continue;
                    else
                        classifyTeachingList.Add(itemSplit[i]);
                }
            }
            else
            {
                if (item.teachingType == null)
                {
                    continue;
                }
                else if (item.teachingType == "")
                {
                    continue;
                }
                else
                {
                    if (classifyTeachingList.Contains(item.teachingType))
                        continue;
                    else
                        classifyTeachingList.Add(item.teachingType);
                }
            }
        }
        //将教研类型和相应ID添加进字典中
        foreach (var teachingType in classifyTeachingList)
        {
            tempIdStr = "";
            foreach (var item in ConfigManager.Get<ConfigGameLibraryModel>())
            {
                if (item.teachingType.Contains(teachingType))
                {
                    tempIdStr += item.id + "|";
                }
            }
            if (tempIdStr[tempIdStr.Length - 1] == '|')
            {
                tempIdStr = tempIdStr.Remove(tempIdStr.Length - 1);
            }
            classifyTeachingDic.Add(teachingType, tempIdStr);
        }

    }
}
