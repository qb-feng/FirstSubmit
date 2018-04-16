using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


#region 换皮模型定义
public class UITurnCardGame2 : UITurnCardGame { }
#endregion

public class UITurnCardGame : UIBaseLevelGame
{
    public Text LabelTotalMoves { get { return Get<Text>("LabelTotalMoves"); } }
    public Text LabelBestMoves { get { return Get<Text>("LabelBestMoves"); } }
    private GameObject BtnBack { get { return Get("SpriteBack"); } }
    public GameObject GoContainerCard { get { return Get("ContainerCard"); } }


    private int m_pairCount = 1;
    private int m_maxPairCount = 4;
    public GameObject goGuideHand;

    private TurnCardGameControllerUGUI m_turnCardGameCtrl { get { return Get<TurnCardGameControllerUGUI>(); } }

    public static UITurnCardGame Instance { get; set; }

    void Awake()
    {
        Instance = this;
        m_turnCardGameCtrl.PairFinishActionEvent += PairFinishActionEvent;
    }

    void OnDestroy()
    {
        Instance = null;
        m_turnCardGameCtrl.PairFinishActionEvent -= PairFinishActionEvent;
    }

    public override void Refresh()
    {
        m_turnCardGameCtrl.pairCount = m_pairCount;
        m_turnCardGameCtrl.SetCardFront(m_randomWordList);
    }

    public override void PlayGame()
    {
        if (goGuideHand != null)
        {
            Destroy(goGuideHand);
        }
        if (IsGameEnd)
            return;

        DoNext();
    }

    private void PairFinishActionEvent()
    {
        //UITopBarBlack.Instance.AddOneStar();
        var cps = GetComponentsInChildren<CardProperties>();
        var temp = new List<ConfigWordLibraryModel>();
        var ids = new List<string>();//集合ID
        foreach (var item in cps)
        {
            bool find = false;
            foreach (var word in temp)
            {
                if (word.id == item.Word.id)
                    find = true;
                ids.Add(word.id);
            }
            if (!find)
                temp.Add(item.Word);
        }
        var fly = FlyStar(ids, true);
        fly.OnComplete += () =>
        {
            PlayGame();
        };
    }

    private void DoNext()
    {
        if (m_pairCount <= m_maxPairCount)
        {
            m_pairCount++;
            m_turnCardGameCtrl.bAgain = true;
            m_turnCardGameCtrl.pairCount = m_pairCount <= m_maxPairCount ? m_pairCount : m_maxPairCount;
            m_pairCount = m_pairCount <= m_maxPairCount ? m_pairCount : m_maxPairCount;
        }
    }

    public bool GetIsFirstPlay()
    {
        return IsFirstPlay;
    }

    public void CreatUI(Transform t)
    {
        goGuideHand = CreateUIItem("GuideHandAnimationItem", t);
        goGuideHand.transform.SetAsLastSibling();
        goGuideHand.transform.position = t.position;
    }
}
