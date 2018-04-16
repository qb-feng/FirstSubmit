using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

public class UILookWindow : UIBaseLevelGame
{

    #region 字段 属性 变量
    private Transform Horizontal2 { get { return GetT("Horizontal2"); } }
    private Transform Horizontal3 { get { return GetT("Horizontal3"); } }
    private Transform Horizontal4 { get { return GetT("Horizontal4"); } }
    private GameObject ImageDialog { get { return Get("ImageDialog"); } }
    private Image ImgDialogContent { get { return Get<Image>("ImgDialogContent"); } }
    private TextMeshProUGUI ImgDialogText { get { return Get<TextMeshProUGUI>("ImgDialogText"); } }//当前显示的单词

    private Image ImgDialog { get { return Get<Image>("ImageDialog"); } }

    private string[] m_gameModels = { "0 1", "1 2 3", "0 2 3", "0 1 2 3" };

    private float LastTime;
    GameObject goGuideHand;
    #endregion

    /// <summary>
    /// 游戏模式
    /// </summary>
    public enum GameType
    {
        None = 0,
        /// <summary>
        /// 窗户图片,提示单词
        /// </summary>
        WindowSpritePromptWord = 1,
        /// <summary>
        /// 提示图片，窗户单词
        /// </summary>
        PromptSpriteWindowWord = 2,
    }
    #region 2018年3月29日17:54:25 新增变量
    private GameType currentGameType = GameType.None;//当前的游戏模式
    public GameType CurrentGameType { get { return currentGameType; } }

    #endregion



    public static UILookWindow Instance { get; set; }

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
    }


    public override void Refresh()
    {
        base.Refresh();
        currentGameType = (GameType)StartData.difficulty;
        Debug.LogWarning("当前游戏难度为：" + currentGameType);
    }

    /// <summary>
    /// 去除重复配置的单词
    /// </summary>
    private List<ConfigWordLibraryModel> RemoveDuplicates(List<ConfigWordLibraryModel> CountinsDuplicates)
    {
        List<ConfigWordLibraryModel> RemoveDuplicatesList = new List<ConfigWordLibraryModel>();
        RemoveDuplicatesList = CountinsDuplicates;
        for (int i = 0; i < RemoveDuplicatesList.Count; i++)  //外循环是循环的次数
        {
            for (int j = RemoveDuplicatesList.Count - 1; j > i; j--)  //内循环是 外循环一次比较的次数
            {
                if (RemoveDuplicatesList[i].word == RemoveDuplicatesList[j].word)//单词一致的去掉
                {
                    RemoveDuplicatesList.RemoveAt(j);
                }

            }
        }
        return RemoveDuplicatesList;
    }


    public override void PlayGame()
    {
        //        FirstTime = Time.time;
        if (goGuideHand != null)
        {
            Destroy(goGuideHand);
        }

        if (IsGameEnd)
            return;

        ImgDialogContent.gameObject.SetActive(false);
        ImgDialog.gameObject.SetActive(false);
        Utility.ClearChild(Horizontal2);
        Utility.ClearChild(Horizontal3);
        Utility.ClearChild(Horizontal4);
        string gameModel;
        if (m_currentIndex > m_gameModels.Length - 1)
        {
            gameModel = m_gameModels[m_gameModels.Length - 1];
        }
        else
            gameModel = m_gameModels[m_currentIndex];

        string[] indexes = gameModel.Split(' ');
        indexes = Utility.RandomSortList(indexes);
        float delay = 0.5f;
        Transform horizontal = null;

        int index_out = 0;
        if (m_currentIndex > m_gameModels.Length - 1)
        {
            index_out = Random.Range(0, indexes.Length);
        }

        for (int i = 0; i < indexes.Length; i++)
        {
            if (indexes.Length == 2)
            {
                horizontal = Horizontal2;
            }
            else if (indexes.Length == 3)
            {
                if (i == 0)
                {
                    horizontal = Horizontal3;
                }
                else if (i == 1 || i == 2)
                {
                    horizontal = Horizontal4;
                }
            }
            else if (indexes.Length == 4)
            {
                if (i == 0 || i == 1)
                {
                    horizontal = Horizontal3;
                }
                else if (i == 2 || i == 3)
                {
                    horizontal = Horizontal4;
                }
            }
            UILookWindowItem itemCp = CreateLookWindow(horizontal);

            ConfigWordLibraryModel config;
            if (m_currentIndex > m_gameModels.Length - 1)
            {
                if (index_out == i)
                    config = m_randomWordList[m_currentIndex];
                else
                    config = m_randomWordList[int.Parse(indexes[i])];
            }
            else
            {
                config = m_randomWordList[int.Parse(indexes[i])];
            }

            itemCp.Init(config);


            itemCp.SetWindowOpen(true, delay);
            itemCp.SetWindowClose(false, delay);
            delay += 1.8f;
            itemCp.SetWindowOpen(false, indexes.Length * 2f);
            itemCp.SetWindowClose(true, indexes.Length * 2f);

        }
        this.WaitSecond(() =>
        {
            SetDialogContent(ImgDialogContent.gameObject);
            ImgDialog.gameObject.SetActive(true);
            var itemCps = GetComponentsInChildren<UILookWindowItem>();
            foreach (var item in itemCps)
            {
                item.SetClick();
            }

        }, delay / 2 + indexes.Length);

    }


    /// <summary>
    /// Create Item
    /// </summary>
    /// <param name="parent">Create Transform</param>
    /// <returns></returns>
    private UILookWindowItem CreateLookWindow(Transform parent)
    {
        GameObject go = CreateUIItem("UILookWindowItem", parent);
        return go.AddComponent<UILookWindowItem>();
    }
    /// <summary>
    ///Match sprite name equals?
    /// </summary>
    /// <param name="sprite">ImageContent sprite name</param>
    /// <returns></returns>
    public bool CheckMatch(string sprite)
    {
        return ImgDialogContent.sprite.name.Equals(sprite);
    }


    /// <summary>
    /// Click DialogContent GameObjectImage Play Audio
    /// </summary>
    /// <param name="arg0"></param>
    private void SetDialogContent(GameObject go)
    {
        go.transform.DOScale(Vector3.one * 1.2f, 0.4f).SetLoops(2, LoopType.Yoyo).OnComplete(delegate
        {
            CurrentLevel(CurrentStar);
        });
        go.SetActive(true);

        go.GetComponent<Image>().sprite = m_randomWordList[m_currentIndex].sprite;
        //根据游戏类型设置当前游戏的模式
        switch (currentGameType)
        {
            case GameType.WindowSpritePromptWord:
                ImgDialogContent.enabled = false;
                ImgDialogText.text = m_randomWordList[m_currentIndex].word;

                break;

            case GameType.PromptSpriteWindowWord:
                ImgDialogContent.enabled = true;
                ImgDialogText.text = null;
                break;
        }
        UGUIEventListener.Get(go).onPointerClick += d =>
        {
            CurrentWord.PlaySound();
            go.transform.DOScale(Vector3.one * 1.2f, 0.4f).SetLoops(2, LoopType.Yoyo);
        };
        CurrentWord.PlaySound();

    }

    void CurrentLevel(int level)
    {
        switch (level)
        {
            case 0:
                int n = 0;
                for (int i = 0; i < Horizontal2.childCount; i++)
                {
                    Image ImageContent = Horizontal2.GetChild(i).GetComponent<UILookWindowItem>().Imagecon;
                    if (CheckMatch(ImageContent.sprite.name))
                    {
                        n = i;
                    }
                }
                GameObject go = Horizontal2.GetChild(n).GetComponent<UILookWindowItem>().WindowCloses;
                if (IsFirstPlay)
                {
                    goGuideHand = CreateUIItem("GuideHandAnimationItem", go.transform);
                    goGuideHand.transform.SetAsLastSibling();
                    goGuideHand.transform.position = go.transform.position;
                }
                else
                {
                    goGuideHand = CreateUIItem("GuideHandAnimationItem", go.transform);
                    goGuideHand.transform.SetAsLastSibling();
                    goGuideHand.transform.position = go.transform.position;
                    goGuideHand.SetActive(false);
                }
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

}
