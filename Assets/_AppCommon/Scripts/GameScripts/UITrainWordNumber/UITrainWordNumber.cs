using UnityEngine;
using UnityEngine.UI;

public class UITrainWordNumber : UIBaseLevelGame
{
    public Transform TrainPosition1 { get { return GetT("TrainPosition1"); } }
    public Transform TrainPosition2 { get { return GetT("TrainPosition2"); } }
    public Image ImageWord { get { return Get<Image>("ImageWord"); } }
    private Image Bg { get { return Get<Image>("ImageBg"); } }

    public static UITrainWordNumber Instance { get; set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }
    void OnDestroy()
    {
        Instance = null;
    }

    public override void PlayGame()
    {
        if (IsGameEnd)
            return;
        UITrainWordItemNumAsk firstTrain = CreateTrainItemAsk(TrainPosition1, 0, 1);
        firstTrain.SetTrainWordAsk(CurrentAsk.ask, CurrentAsk.id);
        UITrainWordItemNumAnswer secondTrain = CreateTrainItemAnswer(TrainPosition2, 1, 2f);
        secondTrain.SetTrainWord(CurrentAsk.answer, CurrentAsk.answerSound);
        ImageWord.sprite = CurrentAsk.sprite;
    }
    /// <summary>
    /// 创建问句的火车
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="colorIndex"></param>
    /// <param name="tweenDelay"></param>
    /// <returns></returns>
    private UITrainWordItemNumAsk CreateTrainItemAsk(Transform parent, int colorIndex, float tweenDelay)
    {
        Utility.ClearChild(parent);
        GameObject go = CreateUIItem("UITrainWordItemNumAsk", parent);
        UITrainWordItemNumAsk cp = go.AddComponent<UITrainWordItemNumAsk>();
        cp.Init(colorIndex, tweenDelay, m_currentIndex);
        return cp;
    }
    /// <summary>
    /// 创建答句火车
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="colorIndex"></param>
    /// <param name="tweenDelay"></param>
    /// <returns></returns>
    private UITrainWordItemNumAnswer CreateTrainItemAnswer(Transform parent, int colorIndex, float tweenDelay)
    {
        Utility.ClearChild(parent);
        GameObject go = CreateUIItem("UITrainWordItemNumAnswer", parent);
        UITrainWordItemNumAnswer cp = go.AddComponent<UITrainWordItemNumAnswer>();
        cp.Init(colorIndex, tweenDelay, m_currentIndex);
        return cp;
    }
}
