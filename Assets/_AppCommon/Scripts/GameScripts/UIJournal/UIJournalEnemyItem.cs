using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// 航海日记 - 潜水艇敌人item
/// </summary>
public class UIJournalEnemyItem : UIBaseInitItem
{

    /// <summary>
    /// 供外部访问的初始化数据方法
    /// 参数1：当前潜水艇携带的单词
    /// 参数2：潜水艇出现的高度 - 即本地坐标的y值
    /// 参数3：潜水艇速度：几秒内运动一个来回
    /// 参数4：潜水艇是否需要旋转：默认不需要(右侧潜水艇需要旋转)
    /// 参数5：潜水艇需要有等待出场时间吗 = 默认不需要
    /// 参数6：潜水艇默认出场时间间隔
    /// </summary>
    public void Init(string currentWord, float LocalPositonY, float runSpeed, bool isShouldRoate = false, bool isShouldWait = false, float WaitingTime = 0f)
    {
        CurrentWord = currentWord;
        if (LocalPositonY <= -500)
            LocalPositonY = Random.Range(-450, 0);
        ThisTranform.localPosition = new Vector3(LocalPositonY * Random.Range(0, 2) == 0 ? 1 : -1, LocalPositonY, 0);//设置潜水艇高度
        isRoate = isShouldRoate;
        runSpeedTime = runSpeed;

        if (isShouldWait)
        {
            resetDistance *= (1 + WaitingTime / 10);
            Invoke("StartRun", WaitingTime);
        }
        else
        {
            //开始运动
            StartRun();
        }

    }
    void Awake()
    {
        IsOnAttack = false;
        Bomb.localScale = Vector3.zero;//先将爆炸特效隐藏

        UGUIEventListener.Get(Bg).onPointerClick += (c) =>
        {
            MainScript.OpenAutoAtack(transform);
        };

    }

    //组件
    private Transform Bg { get { return GetT("Bg"); } }//潜水艇背景图片 - 用来旋转用的
    private TextMeshProUGUI WordText { get { return Get<TextMeshProUGUI>("WordText"); } }//潜水艇显示的单词
    private Transform ThisTranform { get { return GetT("UIJournalEnemyItem(Clone)"); } }//当前Tranform
    private Transform Bomb { get { return GetT("Bomb"); } }//爆炸特效

    //内部数据
    private bool isRoate = false;//当前潜水艇是否需要旋转
    private float runSpeedTime;//潜水艇运行的速度
    private float resetDistance = 2500f;//潜水艇重置的最大距离
    private UIJournal mainScirpt = null;
    private UIJournal MainScript
    {
        get
        {
            if (mainScirpt == null)
                mainScirpt = GetComponentInParent<UIJournal>();
            return mainScirpt;
        }
    }


    //外部数据
    public string CurrentWord { get; private set; }//当前潜水艇携带的单词
    public bool IsOnAttack { get; private set; }//当前潜艇是否被击中

    /// <summary>
    /// 开启被攻击状态 - 供炸弹访问
    /// </summary>
    public void OpenOnAttackState()
    {
        IsOnAttack = true;
        //结束运动
        ThisTranform.DOKill();
        //产生爆炸效果并且销毁自己
        StartCoroutine(StartBombState(1f));
    }

    //潜水艇开始运行
    private void StartRun()
    {
        WordText.SetText(CurrentWord);//显示单词
        if (isRoate)
        {
            //右侧潜艇
            Bg.Rotate(0, 180, 0);//旋转180
            ThisTranform.DOLocalMoveX(-resetDistance, runSpeedTime).SetLoops(-1, LoopType.Yoyo).onStepComplete = () =>
            {
                Bg.Rotate(0, 180, 0);

            };
        }
        else
        {
            //左侧潜艇
            ThisTranform.DOLocalMoveX(resetDistance, runSpeedTime).SetLoops(-1, LoopType.Yoyo).onStepComplete = () => { Bg.Rotate(0, 180, 0); };
        }
    }

    /// <summary>
    /// 潜水艇被击中的爆炸效果:参数：效果播放时间
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartBombState(float playTime)
    {
        float thisTime = 0;
        Bomb.DOScale(1.3f, 0.5f).SetLoops(-1);
        while (true)
        {
            yield return new WaitForEndOfFrame();
            thisTime += Time.deltaTime;
            if (thisTime > playTime)
                break;
        }

        //爆炸后销毁
        GameObject.DestroyImmediate(gameObject);
    }

}
