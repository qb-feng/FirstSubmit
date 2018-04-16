using UnityEngine;
using UnityEngine.UI;

public class UITrainWordCommon : UIBaseLevelGame
{
    public Transform TrainPosition1 { get { return GetT("TrainPosition1"); } }
    public Transform TrainPosition2 { get { return GetT("TrainPosition2"); } }
    public Image ImageWord { get { return Get<Image>("ImageWord"); } }

    /// <summary>
    /// 当前游戏的单词数
    /// </summary>
    public int currentGamewordCount = 0;

    public static UITrainWordCommon Instance { get; set; }

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
        currentGamewordCount = m_randomAskList.Count;
    }

    public override void PlayGame()
    {
        if (IsGameEnd)
            return;

        UITrainWordCommonItem askTrain = CreateTrainCommonItem(TrainPosition1, 0, 1f, CurrentAsk.ask);//初始化问句火车
        //askTrain.SetTrainWord(CurrentAsk.MapWordString);//音频映射
        askTrain.SetTrainWord(CurrentAsk.id);//问句音频qiubin修改
        UITrainWordCommonItem answerTrain = CreateTrainCommonItem(TrainPosition2, 1, 2f, CurrentAsk.answer);//初始化答句火车
        answerTrain.SetTrainWord(CurrentAsk.answerSound);
        //特殊处理对话有复数类型单词对应图片
        ImageWord.sprite =CurrentAsk.MapWordSprite;
    }

    /// <summary>
    /// 创建火车
    /// </summary>
    /// <param name="parent">位置</param>
    /// <param name="colorIndex">颜色</param>
    /// <param name="tweenDelay">间隔时间</param>
    /// <param name="sentence">当前句子</param>
    /// <returns></returns>
    private UITrainWordCommonItem CreateTrainCommonItem(Transform parent, int colorIndex, float tweenDelay, string sentence)
    {
        Utility.ClearChild(parent);
        GameObject go = CreateUIItem("UITrainWordCommonItem", parent);
        //根据句子长度调整火车位置
        int sentenceLength = sentence.Split('_').Length;
        float deviation = 0;//偏移量,矫正位置
        switch (sentenceLength)
        {
            case 3://3节火车
                deviation = 300f;
                break;
            case 4://4节火车
                deviation = 150f;
                break;
            case 5://5节
                deviation = 0f;
                break;
            default:
                break;
        }
        UITrainWordCommonItem cp = go.AddComponent<UITrainWordCommonItem>();
        cp.Init(colorIndex, tweenDelay, m_currentIndex, sentence, deviation);
        return cp;
    }
}
