using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Text;

/// <summary>
/// 按键取票机
/// </summary>
public class UITackMachine : UIDataGameBase
{

    //组件
    private Transform Numbers { get { return GetT("Numbers"); } }//号码总父物体
    private List<Transform> numbersList = new List<Transform>();//所有的号码格子父物体
    private Transform Images { get { return GetT("Images"); } }//图片面板总父物体
    private List<Transform> imagesList = new List<Transform>();//所有的图片面板的父物体

    private Transform UITackMachineTicket { get { return GetT("UITackMachineTicket"); } }//票物体
    private TextMeshProUGUI ShowNumberText { get { return Get<TextMeshProUGUI>("ShowNumberText"); } }//票物体显示数字

    //变量
    private List<UITackMachineNumberItem> allNumberItemList = new List<UITackMachineNumberItem>();//所有的键盘面板
    private List<UITackMachineImageItem> allImageItemList = new List<UITackMachineImageItem>();//所有的图片面板
    private int currentTrueNumber = 0;//当前的正确数字
    private int currentClickNumber = 0;//当前的按下的数字
    private int currentNumberMaxBit = 6;//当前的数组的最大位数
    private bool NumberUseful = false;//按键机是否有用 默认为false没用

    private float TackTicketLocalY = -318f;//出票时票的停顿位置
    private float TackTicketEndLocalY = -2000f;//出票时票的结束位置
    private float TackTicketTime = 3f;//出票时间
    private float TackTicketWaitTime = 1f;//出票后的结果判断时间
    private StringBuilder sb = new StringBuilder();

    //游戏入口
    protected override void RefreshData()
    {
        base.RefreshData();
        UITackMachineTicket.gameObject.SetActive(false);//将票取消激活
        UITackMachineTicket.localPosition = Vector3.zero;
        NumberUseful = false;
        currentClickNumber = 0;
    }

    public override void Refresh()
    {
        base.IsHaveAuioPlayer = true;//有播放器
        base.Refresh();

        //组件初始化
        for (int i = 0; i < Numbers.childCount; ++i)
        {
            numbersList.Add(Numbers.GetChild(i));
        }
        for (int i = 0; i < Images.childCount; ++i)
        {
            imagesList.Add(Images.GetChild(i));
        }
        //创建按钮Item
        for (int i = 0; i < numbersList.Count; ++i)
        {
            allNumberItemList.Add(CreateUIItem<UITackMachineNumberItem>(numbersList[i]));
            allNumberItemList[i].Init(i + 1, NumberResultBackFunc);
        }
        //创建图片item
        for (int i = 0; i < imagesList.Count; ++i)
        {
            allImageItemList.Add(CreateUIItem<UITackMachineImageItem>(imagesList[i]));
        }
    }
    public override void PlayGame()
    {
        base.PlayGame();
        if (IsGameEnd)
        {
            OnGameEnd();
            return;
        }
        //显示当前单词
        ShowText.SetText(CurrentWord.word);
        CurrentWord.PlaySound();
        ShowText.rectTransform.DOScale(1.2f, 0.6f).SetLoops(2, LoopType.Yoyo);

        var falseList = base.GetFalseConfigWordRandomList(CurrentWord);//得到所有的混淆单词
        if (falseList.Count < allImageItemList.Count - 1)
        {
            Debug.LogWarning("混淆项不足！！！");
            PlayGame();
            return;
        }

        //先随机出一个正确的数字和正确项的下标
        int currentMinNumber = (int)Mathf.Pow(10, currentNumberMaxBit - 1);//最小值
        int currentMaxNumber = (int)Mathf.Pow(10, currentNumberMaxBit);//最大值
        currentTrueNumber = RegectNumberHaveZero(Random.Range(currentMinNumber, currentMaxNumber));
        int trueIndex = Random.Range(0, allImageItemList.Count);

        //给所有的图片随机初始化
        int flaseIndex = -1;
        for (int i = 0; i < allImageItemList.Count; ++i)
        {
            if (i == trueIndex)
            {
                allImageItemList[i].Init(CurrentWord.sprite, currentTrueNumber);
                continue;
            }
            else if (i % 2 == 0)
            {
                allImageItemList[i].Init(falseList[++flaseIndex].sprite, RegectNumberHaveZero(Random.Range(currentMinNumber, currentTrueNumber)));
            }
            else
            {
                allImageItemList[i].Init(falseList[++flaseIndex].sprite, RegectNumberHaveZero(Random.Range(currentTrueNumber, currentMaxNumber)));
            }
        }

        //开启按键器可用
        NumberUseful = true;
        SetNumberClickStatues(true);
    }


    /// <summary>
    /// 按键的单击回调事件
    /// 参数1：按钮按下的数字
    /// </summary>
    private void NumberResultBackFunc(int clickNumber)
    {
        if (NumberUseful)
        {
            //按键机可用时
            //得到当前按下的数字
            currentClickNumber = currentClickNumber * 10 + clickNumber;
            //判断数字是否按够
            string numberString = currentClickNumber.ToString();
            if (numberString.Length == currentNumberMaxBit)
            {
                //够了 - 立刻关闭按键机
                NumberUseful = false;
                SetNumberClickStatues(false);
                //出票         
                CreateTicket(numberString);
            }
        }
    }

    /// <summary>
    /// 打印电影票 参数1：要打印的数字
    /// </summary>
    private void CreateTicket(string numberString)
    {
        UITackMachineTicket.gameObject.SetActive(true);//激活
        ShowNumberText.SetText(numberString);//显示

        //执行出票动作
        UITackMachineTicket.DOLocalMoveY(TackTicketLocalY, TackTicketTime).onComplete = () =>
        {
            Invoke("JudgeResult", TackTicketWaitTime);//n秒后判断结果
        };
    }

    /// <summary>
    /// 判断结果
    /// </summary>
    private void JudgeResult()
    {
        //将票迅速飞走
        UITackMachineTicket.DOLocalMoveY(TackTicketEndLocalY, TackTicketWaitTime);
        bool isResult = currentClickNumber == currentTrueNumber;
        //飞星
        FlyStar(isResult, true).OnComplete += () =>
        {
            if (isResult)
            {
                //输入正确
                PlayGame();
            }
            else
            {
                //输入错误
                RefreshData();//重置当前数据
                NumberUseful = true;//重新开启按键效果
                SetNumberClickStatues(true);
            }
        };
    }

    /// <summary>
    /// 改变所有按键单击状态的方法：参数1：打开吗？默认 = true
    /// </summary>
    /// <param name="open"></param>
    private void SetNumberClickStatues(bool open = true)
    {
        foreach (var v in allNumberItemList)
            v.SetClickStaues(open);
    }

    /// <summary>
    /// 剔除一个数中含有的0数字
    /// </summary>
    /// <param name="number"></param>
    private int RegectNumberHaveZero(int number)
    {
        sb.Remove(0,sb.Length);
        sb.Append(number.ToString());
        for (int i = 0; i < sb.Length; ++i)
        {
            if (sb[i] == '0')
                sb[i] = '1';
        }
        return int.Parse(sb.ToString());
    }
}
