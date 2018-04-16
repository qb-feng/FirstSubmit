using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Text;
/// <summary>
/// 贪吃球的状态
/// </summary>
public enum PlayerStatue
{
    Normal = 0,//正常状态
    EatingBubble,//吃泡泡状态
    EatingSuccess,//吃泡泡成功的状态
}

#region 换皮模型定义
public class UIInfiniteBlankHole : UIGreedyBall { }
#endregion

/// <summary>
/// 游戏模型 - 贪吃阿米巴
/// </summary>
public class UIGreedyBall : UIBaseLevelGame
{
    //组件
    private Transform BubblePoint { get { return GetT("BubblePoint"); } }//泡泡位置点的父物体
    private List<Transform> bubbleAllPoint = new List<Transform>();//泡泡出现的所有位置点 
    private TextMeshProUGUI SentenText1 { get { return Get<TextMeshProUGUI>("SentenText1"); } }//左边句子
    private Transform SentenText2 { get { return GetT("SentenText2"); } }//中间空缺单词
    private TextMeshProUGUI SentenText3 { get { return Get<TextMeshProUGUI>("SentenText3"); } }//右边句子
    private GameObject PlayerAudio { get { return Get("PlayerAudio"); } }//单词播放按钮

    //贪吃球的身体
    private RectTransform Mouth { get { return GetR("Mouth"); } }//嘴巴
    private RectTransform LeftEye { get { return GetR("LeftEye"); } }//左眼
    private RectTransform RightEye { get { return GetR("RightEye"); } }//右眼
    private RectTransform Body { get { return GetR("Body"); } }//身体
    private CircleCollider2D bodyCollider;
    private CircleCollider2D BodyCollider //身体的碰撞器
    {
        get
        {
            if (bodyCollider == null)
                bodyCollider = Body.GetComponent<CircleCollider2D>();
            return bodyCollider;
        }
    }

    //数据
    private float mouthNormalRadius = 60f;//嘴巴正常状态半径
    private float mouthEatingBubbleMaxRadius = 150f;//吃泡泡的嘴巴最大半径
    private float mouthEatingSuccessRadius = 40f;//吃泡泡成功的状态半径
    private float mouthChangeStatueTime = 1.5f;//嘴巴改变状态需要的时间
    private float dragBubbleBeginOfMouthDistance = 0;//开始拖拽泡泡时，嘴巴与泡泡的距离
    private int maxWordBubbleNum = 3;//当前单词泡泡的最大个数
    private float bubbleRandomMinRidus = 20f;//普通泡泡的最小半径
    private float bubbleRandomMaxRidus = 40f;//普通泡泡的最大半径
    private float bubbleWordMaxRidus = 80f;//单词泡泡的半径 - 改为跟单词半径一样长
    private float eveLookMoveDis = 3f;//眼睛看到单词移动的距离
    private float bodyAddRadius = 0.1f;//身体每次长大的scalse
    private float bodyCurrentRadius = 0.6f;//身体每次长大后的scale
    private float bodyMaxScale = 1.2f;//身体最大长大到的sclae
    private Vector3 LeftEyeDefulltLocalPositon = Vector3.zero;//左眼睛的默认本地坐标
    private Vector3 RightEyeDefulltLocalPositon = Vector3.zero;//右眼睛的默认本地坐标
    private float bodyBreathTime = 1f;//小胖呼吸的频率（每一个呼吸来回需要的时间）
    private string[] falseLettets = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };//字母混淆项

    //变量
    private string sentenceText1;//句子的前半段
    private string CurrentTrueWord;//当前的答案单词（即空缺句子）
    private string sentenceText3;//句子的后半段
    private List<string> flaseWordList = new List<string>();//混淆项的单词 （包含正确的单词）
    private StringBuilder sb = new StringBuilder();
    private AudioSource asource;//播放器
    private string CurrentModeName;//当前模型预制体的主名字

    void Start()
    {
        LeftEyeDefulltLocalPositon = LeftEye.localPosition;
        RightEyeDefulltLocalPositon = RightEye.localPosition;
    }

    /// <summary>
    /// 数据初始化
    /// </summary>
    private void ResetDada()
    {
        SentenText2.ClearAllChild();//中间段重置

        ChangePlayerStatue(PlayerStatue.Normal);//重回正常状态
        flaseWordList.Clear();
        sb.Remove(0, sb.Length);
    }

    //游戏启动
    public override void Refresh()
    {

        base.Refresh();
        CurrentModeName = gameObject.name.Replace("(Clone)",null);//初始化当前模型的名字

        //初始化泡泡位置点信息
        for (int i = 0; i < BubblePoint.childCount; ++i)
        {
            bubbleAllPoint.Add(BubblePoint.GetChild(i));
        }
        //TODO 初始化泡泡的大小
        Body.localScale = new Vector3(bodyCurrentRadius, bodyCurrentRadius, bodyCurrentRadius);

        //让小胖的肚子动起来 - 宝宝呼吸
        BodyBreath(true);

        //初始化播放器
        UGUIEventListener.Get(PlayerAudio).onPointerClick = (click) => { OnClickAudioPlayer(); };

    }

    public override void PlayGame()
    {
        ResetDada();
        if (IsGameEnd)
            return;
        //得到混淆项单词 及切割问题句子
        switch (StartData.dataType)
        {
            //陈述句
            case DataType.Say:
                //混淆项
                string id = CurrentSay.id.Substring(0, 5);
                foreach (var v in ConfigManager.Get<ConfigWordLibraryModel>())
                {
                    if (v.id.Substring(0, 5).Equals(id) && !v.word.StartsWith(CurrentSay.MapWordString))
                    {
                        flaseWordList.Add(v.word);
                    }
                }
                //问题句子
                string[] sentence = CurrentSay.yes.Split('_');
                for (int i = 0; i < sentence.Length; ++i)
                {
                    if (sentence[i].StartsWith(CurrentSay.MapWordString))
                    {
                        //找到并句子的空缺处 - 设置正确项
                        CurrentTrueWord = sentence[i];
                        sentenceText1 = sb.ToString();
                        sb.Remove(0, sb.Length);
                        continue;
                    }
                    sb.Append(sentence[i]);
                    sb.Append(" ");
                }
                sentenceText3 = sb.ToString();
                break;

            //单词
            case DataType.Word:
                int trueIndex = Random.Range(0, CurrentWord.word.Length);//正确项下标
                while (CurrentWord.word[trueIndex].ToString() == " ")
                {
                    trueIndex = Random.Range(0, CurrentWord.word.Length);//正确项下标
                }
                CurrentTrueWord = CurrentWord.word[trueIndex].ToString();//设置当前空确单词

                //混淆项
                for (int i = 0; i < falseLettets.Length; ++i)
                {
                    flaseWordList.Add(falseLettets[i]);
                }
                //问题句子
                for (int i = 0; i < CurrentWord.word.Length; ++i)
                {
                    if (CurrentWord.word[i].ToString().Equals(CurrentTrueWord))
                    {
                        //找到并句子的空缺处 - 设置正确项
                        sentenceText1 = sb.ToString();
                        sb.Remove(0, sb.Length);
                        continue;
                    }
                    sb.Append(CurrentWord.word[i]);
                }
                sentenceText3 = sb.ToString();
                break;
        }
        //处理混淆项
        flaseWordList.Remove(CurrentTrueWord);
        Utility.RandomSortList(flaseWordList);

        //设置问题句子
        SentenText1.SetText(sentenceText1);
        SentenText3.SetText(sentenceText3);

        //得到所有单词泡泡的下标
        List<int> buddleIndex = new List<int>();
        for (int i = 1; i <= maxWordBubbleNum && i * 2 <= bubbleAllPoint.Count; ++i)
        {
            buddleIndex.Add(Random.Range(2 * (i - 1), i * 2));
        }
        int currentTrueIndex = buddleIndex[Random.Range(0, buddleIndex.Count)];//随机得到正确单词的下标
        int currentFalseIndex = -1;//错误单词的下标
        //创建泡泡Item
        for (int i = 0; i < bubbleAllPoint.Count; ++i)
        {
            bubbleAllPoint[i].ClearAllChild();
            if (buddleIndex.Contains(i))
            {
                float wordLineLength = 0;//单词的长度
                if (i == currentTrueIndex)
                {
                    wordLineLength = (CurrentTrueWord.Length > 5 ? (CurrentTrueWord.Length / 5f) : 1);
                    CreateUIItem<UIGreedyBallBubbleWordItem>(bubbleAllPoint[i]).Init(true, bubbleWordMaxRidus * wordLineLength, CurrentTrueWord, OnDragResultBackFunc, CurrentModeName);//创建正确的单词泡泡
                }

                else
                {
                    wordLineLength = (flaseWordList[++currentFalseIndex].Length > 5 ? (flaseWordList[currentFalseIndex].Length) / 5f : 1);
                    CreateUIItem<UIGreedyBallBubbleWordItem>(bubbleAllPoint[i]).Init(true, bubbleWordMaxRidus * wordLineLength, flaseWordList[currentFalseIndex], OnDragResultBackFunc, CurrentModeName);//创建错误的单词泡泡
                }

            }

            else
                CreateUIItem<UIGreedyBallBubbleItem>(bubbleAllPoint[i]).Init(false, Random.Range(bubbleRandomMinRidus, bubbleRandomMaxRidus), null, null, CurrentModeName);//创建普通泡泡      
        }

        //播放单词的声音
        OnClickAudioPlayer();

    }

    /// <summary>
    /// 小泡泡item被拖拽的回调函数 
    /// 参数1：触发拖拽回调的泡泡
    /// 参数2：触发拖拽回调的泡泡的拖拽状态
    /// </summary>
    private void OnDragResultBackFunc(UIGreedyBallBubbleItem item, DragStatue dragStatue)
    {
        var bubble = item as UIGreedyBallBubbleWordItem;//所拖拽的泡泡
        Vector2 bubblePosition = bubble.CurrentPositon;//泡泡的位置

        switch (dragStatue)
        {
            case DragStatue.DragStay:
                //TODO 持续拖拽时
                ChangePlayerStatue(PlayerStatue.EatingBubble, bubble.CurrentPositon.x, bubble.CurrentPositon.y);
                break;

            case DragStatue.DragBegin:
                //拖拽开始
                dragBubbleBeginOfMouthDistance = Vector3.Distance(bubble.CurrentPositon, Mouth.position);
                //先停止宝宝呼吸的状态
                Debug.Log("宝宝停止呼吸！");
                BodyBreath(false);
                break;

            case DragStatue.DragEnd:
                //TODO 拖拽结束  
                //眼睛还原
                LeftEye.localPosition = LeftEyeDefulltLocalPositon;
                RightEye.localPosition = RightEyeDefulltLocalPositon;

                //首先判断泡泡单词是否吃成功（即是否在嘴里）
                //判断单词与嘴的距离是否小于当前嘴巴的半径 - 单词球的半径
                float mainCanvasScale = UIManager.Instance.UIRootHorizontal.transform.localScale.x;//主Canvas缩放的大小（他会影响子物体的positon！！！！，所以任何字物体需要精确的positon时需要除以这个缩放距离）
                //RectTransformUtility.ScreenPointToWorldPointInRectangle(Mouth, bubblePosition, UIManager.Instance.WorldCamera, out bubbleLocalPositon);
                var dis = Vector2.Distance(Mouth.position, bubblePosition) / mainCanvasScale;//TODO 距离与实际长度距离不同~~后面解决
                if (dis >= 0 && dis < Mouth.sizeDelta.x * 0.5f && bubble.CurrentWord.Equals(CurrentTrueWord))
                {
                    //在嘴里了
                    //其次判断所吃到的单词泡泡是否正确（符合答案），正确则执行拖拽成功的过程，错误执行拖拽失败的过程                             
                    //吃成功了
                    //吃掉泡泡 得到泡泡内的单词物体 并将单词放到指定的位置
                    SetWordText(bubble.EatBubble(Mouth.position));
                    //设置吃成功的状态
                    ChangePlayerStatue(PlayerStatue.EatingSuccess);
                }
                else
                {
                    //没有吃成功
                    //设置为没有吃成功的状态
                    if (dis < Mouth.sizeDelta.x * 0.5f)
                        FlyStar(false, false);
                    ChangePlayerStatue(PlayerStatue.Normal);
                    //再次开启宝宝呼吸的状态
                    BodyBreath(true);
                }
                break;
        }
    }

    /// <summary>
    /// 修改贪吃球的状态：参数1：贪吃球状态 - 默认为正常状态
    /// 参数2，3：要吃的目标的位置（x和y）
    /// </summary>
    private void ChangePlayerStatue(PlayerStatue statue = PlayerStatue.Normal, float targetPositonX = 0, float targetPositonY = 0)
    {
        switch (statue)
        {
            case PlayerStatue.EatingBubble:
                //TODO 进入吃泡泡的状态
                UpdateMouthSizeAndEyesOfTargetPositon(new Vector2(targetPositonX, targetPositonY));
                break;
            case PlayerStatue.Normal:
                DOTween.To(() => Mouth.sizeDelta, x => Mouth.sizeDelta = x, new Vector2(mouthNormalRadius * 2, mouthNormalRadius * 2), mouthChangeStatueTime);
                break;
            case PlayerStatue.EatingSuccess:
                DOTween.To(() => Mouth.sizeDelta, x => Mouth.sizeDelta = x, new Vector2(mouthEatingSuccessRadius * 2, mouthEatingSuccessRadius * 2), mouthChangeStatueTime);
                break;
        }
    }
    /// <summary>
    /// 更新嘴巴大小和眼睛位置 - 根据目标位置与当前位置的距离更新
    /// 参数：目标当前位置（世界坐标）
    /// </summary>
    private void UpdateMouthSizeAndEyesOfTargetPositon(Vector2 targetPositon)
    {
        //改变嘴巴大小
        var distance = dragBubbleBeginOfMouthDistance - Vector2.Distance(targetPositon, Mouth.position);//两者之间的距离
        if (distance > 1f)
        {
            float currentMouthRadius = mouthNormalRadius;//当前状态的嘴巴大小
            float range = distance / dragBubbleBeginOfMouthDistance;//位置的百分比(当前距离占最大距离的百分比)
            float rangeMouthRadius = (mouthEatingBubbleMaxRadius - currentMouthRadius) * range + currentMouthRadius;//同等比例下嘴巴需要的大小
            Mouth.sizeDelta = new Vector2(rangeMouthRadius * 2, rangeMouthRadius * 2);

        }
        //TODO 改变眼睛
        Vector3 dir = targetPositon - new Vector2(LeftEye.position.x, LeftEye.position.y);//方向      
        LeftEye.localPosition = dir * eveLookMoveDis;//将眼睛突出来
        LeftEye.localPosition += LeftEyeDefulltLocalPositon;


        dir = targetPositon - new Vector2(RightEye.position.x, RightEye.position.y);//方向
        RightEye.localPosition = dir * eveLookMoveDis;//将眼睛突出来
        RightEye.localPosition += RightEyeDefulltLocalPositon;
    }

    /// <summary>
    /// 设置吃掉的单词物体到指定的位置：参数：吃掉的单词物体
    /// </summary>
    private void SetWordText(Transform wordText)
    {
        //放到指定物体下面
        wordText.SetParent(SentenText2);

        //TODO 移动动画

        wordText.DOMove(SentenText2.position, 1f).onComplete = () =>
        {
            var v = wordText as RectTransform;
            v.localPosition = Vector2.zero;
            v.localRotation = Quaternion.identity;
            v.anchorMax = Vector2.one;
            v.anchorMin = Vector2.zero;
            v.pivot = new Vector2(0.5f, 0.5f);
            FlyStar(true, false).OnComplete += PlayGame;//飞星
            BodyBroup();
        };
    }

    /// <summary>
    /// 身体长大
    /// </summary>
    private void BodyBroup()
    {
        Debug.Log("长大一次！");
        if (Body.localScale.x <= bodyMaxScale)
            Body.DOScale(Body.localScale.x + bodyAddRadius, 1f).onComplete = () =>
            {
                bodyCurrentRadius += bodyAddRadius;
                //再次开启宝宝呼吸的状态
                BodyBreath(true);
            };
        else 
        {
            //直接开启宝宝呼吸的状态 - 太大了，就不再继续大了~~~
            BodyBreath(true);
        }
    }

    /// <summary>
    /// 宝宝呼吸的方法：参数是停止或者开启宝宝呼吸，默认是开启
    /// </summary>
    /// <param name="statue"></param>
    private void BodyBreath(bool statue = true)
    {
        this.Body.DOKill();
        if (statue)
        {
            //TODO 初始化泡泡的大小
            Body.localScale = new Vector3(bodyCurrentRadius, bodyCurrentRadius, bodyCurrentRadius);
            Body.DOScale(Body.localScale.x - bodyAddRadius * 0.5f, bodyBreathTime).SetLoops(-1, LoopType.Yoyo);
        }
    }

    /// <summary>
    /// 单词播放
    /// </summary>
    private void OnClickAudioPlayer()
    {
        switch (StartData.dataType)
        {
            case DataType.Say:
                asource = CurrentSay.PlayAnswerSound();
                break;
            case DataType.Word:
                asource = CurrentWord.PlaySound();
                break;
        }
    }

}
