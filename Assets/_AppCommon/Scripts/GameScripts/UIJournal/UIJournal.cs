using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System.Text;

/// <summary>
/// 游戏模型：航海日记
/// </summary>
public class UIJournal : UIBaseLevelGame
{
    ///组件
    //波浪
    private Transform Seawve1 { get { return GetT("Seawve1"); } }//波浪1
    private Transform Seawve1_1 { get { return GetT("Seawve1_1"); } }//波浪1_1
    private Transform Seawve2 { get { return GetT("Seawve2"); } }//波浪2
    private Transform Seawve2_1 { get { return GetT("Seawve2_1"); } }//波浪2_1
    private Transform Seawve3 { get { return GetT("Seawve3"); } }//波浪3
    private Transform Seawve3_1 { get { return GetT("Seawve3_1"); } }//波浪3_1

    //玩家控制组件
    private Transform PlayerShip { get { return GetT("PlayerShip"); } }//战舰
    private GameObject LeftTouch { get { return Get("LeftTouch"); } }//左方向键
    private GameObject RightTouch { get { return Get("RightTouch"); } }//右方向键
    private GameObject Attack { get { return Get("Attack"); } }//攻击键
    private Transform AttackPoint { get { return GetT("AttackPoint"); } }//攻击口位置
    private Transform Wamp { get { return GetT("Wamp"); } }//弹药库- 武器的存储位置
    private GameObject PlayerAudio { get { return Get("PlayerAudio"); } }//声音播放器

    //显示组件
    private Image PlayerPhoto { get { return Get<Image>("PlayerPhoto"); } }//玩家的显示头像
    private Image PlayerPhotoTrue { get { return Get<Image>("PlayerPhotoTrue"); } }//玩家开心的显示头像
    private Image PlayerPhotoFalse { get { return Get<Image>("PlayerPhotoFalse"); } }//玩家不开心的显示头像
    private TextMeshProUGUI SentenceText { get { return Get<TextMeshProUGUI>("SentenceText"); } }//句子text
    private Transform EnemyLeft { get { return GetT("EnemyLeft"); } }//左侧潜艇出现位置
    private Transform EnemyRight { get { return GetT("EnemyRight"); } }//右侧潜艇出现位置
    private GameObject PlayerShipLeft { get { return Get("PlayerShipLeft"); } }//潜艇在左边的方向的图片
    private GameObject PlayerShipRight { get { return Get("PlayerShipRight"); } }//潜艇在右边的方向的图片
    private GameObject TopBg { get { return Get("TopBg"); } }//顶部背景
    private Image WordBg { get { return Get<Image>("WordBg"); } }//单词图片
    private GameObject WordBgGo { get { return Get("WordBgGo"); } }//单词背景

    ///内部变量
    //波浪
    private Vector3 seawveResetPosition = new Vector3(1920, 0, 0);//波浪重置点
    private float seawveStartResetXPositon = -1920f;//波浪开启重置点的x轴位置
    private float seawveSpeed1 = 5f;//波浪1运行的速度 - 越小越快
    private float seawveSpeed2 = 7f;//波浪2运行的速度
    private float seawveSpeed3 = 10f;//波浪3运行的速度

    //玩家控制
    private float playerShipSpeed = 10f;//战舰的运行速度
    private bool ShipRunCtrl = true;//控制战舰是否可以运动
    private float runLeftDistance = -600f;//战舰的最大左边移动距离
    private float runRightDistance = 600f;//战舰的最大右边移动距离
    private float attackCDTime = 1f;//战舰的攻击cd时间
    private float attackNowCDTime = 0;//当前剩余攻击cd时间
    private AudioSource audioSource;//播放器
    private List<AudioSource> askAudioSources = null;//句子播放器数组

    //潜水艇
    private float enemySpaceDistanceY = 295f;//战舰的单个y轴上间隔距离


    /// 数据
    private string currentTrueWord;//当前正确的单词
    private string currentSentence;//当前需要显示的句子（完整的句子）
    private StringBuilder stringBuider = new StringBuilder();
    private string[] currentSentenceArray;//当前句子

    private int maxFalseWordNum = 2;//当前混淆单词的最大数量
    private List<string> currentFalseWord = new List<string>();//当前的单词选项
    private float shipSwingDis = 10f;//战船上下摇摆的距离
    private float shipSwingTime = 1.5f;//战船上下摇摆的时间


    void Awake()
    {
        //开启波浪运行
        SeawveStartRun();
        //初始化玩家控制事件
        PlayerCtrlInit();


        Attack.SetActive(false);
        //初始化玩家攻击事件
        //UGUIEventListener.Get(Attack).onPointerClick = OnClickAttack;

        //去掉播放按钮 - 改为自动播放
        PlayerAudio.SetActive(false);
        ////初始化声音播放事件
        //UGUIEventListener.Get(PlayerAudio).onPointerClick = (click) =>
        //{
        //    PlayerAudioMusic();
        //};
    }

    //启动
    public override void Refresh()
    {
        base.Refresh();
        PlayerShipRight.SetActive(false);
        //开启船上下浮动的动画
        float shipY = PlayerShip.localPosition.y;
        PlayerShip.DOLocalMoveY(shipY + shipSwingDis, shipSwingTime).SetLoops(-1, LoopType.Yoyo);

    }

    /// <summary>
    /// 变量初始化
    /// </summary>
    private void ResetData()
    {
        currentFalseWord.Clear();
        stringBuider.Remove(0, stringBuider.Length);
        EnemyLeft.ClearAllChild();
        EnemyRight.ClearAllChild();
        //玩家表情重置
        PlayerPhotoTrue.gameObject.SetActive(false);
        PlayerPhotoFalse.gameObject.SetActive(false);
        PlayerPhoto.gameObject.SetActive(false);

        //关闭自动播放协程
        StopCoroutine("CtrlAutoPlayAudio");
    }

    public override void PlayGame()
    {
        ResetData();
        if (IsGameEnd)
            return;

        string currentSentenceId = null;//当前句子的id所在的单元！！！注意是单元
        //得到新数据
        switch (base.StartData.dataType)
        {
            case DataType.Ask:
                //执行新版对话练习操作
                //string askString = CurrentAsk.ask + "_" + CurrentAsk.answer;
                //InitAskPlay(askString);
                //return;
                string askString = CurrentAsk.ask + "_?" + CurrentAsk.answer;
                currentSentenceArray = askString.Split('_');
                currentTrueWord = CurrentAsk.MapWordString;//映射单词              
                currentSentenceId = CurrentAsk.id.Substring(0, 5);
                Debug.Log(CurrentAsk.id);

                //得到当前句子单元的所有单词 - TODO 混淆项 - 以后需要再修改
                foreach (var everyWord in ConfigManager.Get<ConfigWordLibraryModel>())
                {
                    if (currentSentenceId.Equals(everyWord.id.Substring(0, 5)))
                    {
                        currentFalseWord.Add(everyWord.word);
                    }
                }

                break;

            case DataType.Say:
                currentSentenceArray = CurrentSay.yes.Split('_');
                currentTrueWord = CurrentSay.MapWordString;//映射单词
                currentSentenceId = CurrentSay.id.Substring(0, 5);
                Debug.Log(CurrentSay.id);

                //得到当前句子单元的所有单词 - TODO 混淆项  - 以后修改
                foreach (var everyWord in ConfigManager.Get<ConfigWordLibraryModel>())
                {
                    if (currentSentenceId.Equals(everyWord.id.Substring(0, 5)))
                    {
                        currentFalseWord.Add(everyWord.word);
                    }
                }

                break;
            case DataType.Word:
                //单词模式
                currentSentenceArray = new string[] { };
                TopBg.SetActive(false);
                this.WordBgGo.SetActive(true);
                currentTrueWord = CurrentWord.word;
                WordBg.sprite = CurrentWord.sprite;
                foreach (var v in m_originalWordList)
                    currentFalseWord.Add(v.word);
                break;
        }

        if (currentFalseWord.Contains(currentTrueWord))
        {
            currentFalseWord.Remove(currentTrueWord);
        }
        //混淆单词数组整理
        Utility.RandomSortList(currentFalseWord);//将该混淆单词数组随机排序

        //数据解析 - 得到当前隐藏单词已经显示的句子
        for (int i = 0; i < currentSentenceArray.Length; ++i)
        {
            if (currentSentenceArray[i].StartsWith(currentTrueWord, System.StringComparison.InvariantCulture))
            {
                //判断答案是否带标点符号 - 带的话就移除
                currentTrueWord = currentSentenceArray[i];
                char lastCurrentTrueWord = currentTrueWord[currentTrueWord.Length - 1];
                if (lastCurrentTrueWord < 65 || lastCurrentTrueWord > 122 || (lastCurrentTrueWord > 90 && lastCurrentTrueWord < 97))
                {
                    currentTrueWord = currentTrueWord.Remove(currentTrueWord.Length - 1, 1);
                }
                stringBuider.Append("_______");
                continue;
            }
            stringBuider.Append(currentSentenceArray[i]);
            stringBuider.Append(" ");
        }
        if (stringBuider != null && stringBuider.Length > 0)
        {
            stringBuider.Remove(stringBuider.Length - 1, 1);
            currentSentence = stringBuider.ToString();
        }


        //显示句子和初始化敌军
        SentenceText.SetText(currentSentence);
        int trueIndex = Random.Range(0, maxFalseWordNum);//正确选项下标
        for (int i = 0; i < maxFalseWordNum && i < currentFalseWord.Count; ++i)
        {
            if (trueIndex == i)
            {
                //正确选项设置
                currentFalseWord[i] = currentTrueWord;
            }

            if (i % 2 == 0)
            {
                //初始化左侧敌军战舰
                CreateUIItem<UIJournalEnemyItem>(EnemyLeft).Init(currentFalseWord[i], -1 * i * enemySpaceDistanceY, Random.Range(6, 12));

            }
            else
            {
                //初始化右侧敌军战舰
                CreateUIItem<UIJournalEnemyItem>(EnemyRight).Init(currentFalseWord[i], -1 * i * enemySpaceDistanceY, Random.Range(5, 12), true);
            }
        }

        //播放音频
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
        if (askAudioSources != null)
        {
            foreach (var v in askAudioSources)
            {
                if (v != null)
                    v.Stop();
            }

            askAudioSources.Clear();
        }


        PlayerAudioMusic();// 播放声音
        StartCoroutine("CtrlAutoPlayAudio");

        isCanAtack = true;//可以攻击了

    }

    #region 新版对话类型游戏模型 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private string[] currentAskWords;//当前问句类型的所有单词数组
    private int currentAskTrueWordIndex = 0;//当前问句类型需要轰炸的单词在数组中的下标
    private string askString;
    private void InitAskPlay(string askStrings)
    {
        askString = askStrings;
        currentAskTrueWordIndex = 0;
        CreateAskWordUIItem();

        //播放音频
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
        PlayerAudioMusic();// 播放声音
    }
    /// <summary>
    /// 创建战舰
    /// </summary>
    private void CreateAskWordUIItem()
    {
        currentAskWords = askString.Split('_');
        //表情重置
        // PlayerPhotoTrue.gameObject.SetActive(false);
        //PlayerPhotoFalse.gameObject.SetActive(false);

        //当前需要轰炸的单词显示
        currentTrueWord = currentAskWords[currentAskTrueWordIndex];
        SentenceText.SetText(currentTrueWord);

        EnemyLeft.ClearAllChild();
        EnemyRight.ClearAllChild();
        enemySpaceDistanceY = 500 / currentAskWords.Length;

        //随机出现鱼雷
        randomSeries.Clear();
        GetRandomSeries(0, currentAskWords.Length);
        for (int j = 0; j < randomSeries.Count; ++j)
        {
            int i = randomSeries[j];
            Debug.Log(randomSeries[j]);
            if (i % 2 == 0)
            {
                //初始化左侧敌军战舰              
                CreateUIItem<UIJournalEnemyItem>(EnemyLeft).Init(currentAskWords[i], -1 * j * enemySpaceDistanceY, Random.Range(5, 8) + i, false, true, i);

            }
            else
            {
                //初始化右侧敌军战舰
                CreateUIItem<UIJournalEnemyItem>(EnemyRight).Init(currentAskWords[i], -1 * j * enemySpaceDistanceY, Random.Range(5, 8) + i, true, true, i);
            }
        }
    }

    #endregion

    void Update()
    {
        attackNowCDTime -= Time.deltaTime;
    }
    /// <summary>
    /// 玩家的攻击单击事件
    /// </summary>
    private void OnClickAttack()
    {
        if (attackNowCDTime <= 0)
        {
            attackNowCDTime = attackCDTime;//重新进入cd
            //创建一个新攻击鱼雷
            CreateUIItem<UIJournalWampItem>(Wamp).Init(AttackPoint.position, currentTrueWord, OnAttackResultBackFunc);
        }
    }

    /// <summary>
    /// 鱼雷攻击的回调事件：参数：是否击中目标：true 击中目标 ,false 目标击中错误
    /// </summary>
    private void OnAttackResultBackFunc(bool isAttack)
    {
        isCanAtack = !isAttack;

        //击中目标 - 显示对应的表情
        // PlayerPhotoTrue.gameObject.SetActive(isAttack);
        //PlayerPhotoFalse.gameObject.SetActive(!isAttack);

        #region 新版对话游戏方式 - 现在已经不需要
        //if (StartData.dataType == DataType.Ask)
        //{
        //    if (isAttack)
        //    {
        //        ++currentAskTrueWordIndex;
        //        if (currentAskTrueWordIndex >= currentAskWords.Length)
        //        {
        //            //全部击杀完毕
        //            FlyStar(isAttack, false).OnComplete += () => PlayGame();
        //        }
        //        else
        //        {
        //            Invoke("CreateAskWordUIItem", 2f);//重新创建第二批战舰
        //        }
        //    }
        //    else
        //    {
        //        FlyStar(isAttack, false).OnComplete += () =>
        //        {
        //            //表情重置
        //            PlayerPhotoTrue.gameObject.SetActive(false);
        //            PlayerPhotoFalse.gameObject.SetActive(false);
        //        };
        //    }

        //    return;
        //}
        #endregion

        string dataId = null;
        switch (StartData.dataType)
        {
            case DataType.Word:
                dataId = CurrentWord.id;
                break;
            case DataType.Say:
                dataId = CurrentSay.id;
                break;
            case DataType.Ask:
                dataId = CurrentAsk.id;
                break;
        }

        // 飞星一颗！
        FlyStar(dataId, isAttack).OnComplete += () =>
        {
            if (isAttack)
            {
                PlayGame();//下一关               
            }
            else
            {
                //表情重置
                //  PlayerPhotoTrue.gameObject.SetActive(false);
                //  PlayerPhotoFalse.gameObject.SetActive(false);
            }
        };
    }


    /// <summary>
    /// 波浪运动
    /// </summary>
    private void SeawveStartRun()
    {
        //波浪运动初始化
        //波浪1
        Seawve1.DOLocalMoveX(seawveStartResetXPositon, seawveSpeed1).onComplete = () =>
        {
            seawveResetPosition.y = Seawve1.localPosition.y;
            Seawve1.localPosition = seawveResetPosition;
            Seawve1.DOLocalMoveX(seawveStartResetXPositon, seawveSpeed1 * 2).SetLoops(-1);
        };
        Seawve1_1.DOLocalMoveX(seawveStartResetXPositon, seawveSpeed1 * 2).SetLoops(-1);

        //波浪2
        Seawve2.DOLocalMoveX(seawveStartResetXPositon, seawveSpeed2).onComplete = () =>
        {
            seawveResetPosition.y = Seawve2.localPosition.y;
            Seawve2.localPosition = seawveResetPosition;
            Seawve2.DOLocalMoveX(seawveStartResetXPositon, seawveSpeed2 * 2).SetLoops(-1);
        };
        Seawve2_1.DOLocalMoveX(seawveStartResetXPositon, seawveSpeed2 * 2).SetLoops(-1);

        //波浪3
        Seawve3.DOLocalMoveX(seawveStartResetXPositon, seawveSpeed3).onComplete = () =>
        {
            seawveResetPosition.y = Seawve3.localPosition.y;
            Seawve3.localPosition = seawveResetPosition;
            Seawve3.DOLocalMoveX(seawveStartResetXPositon, seawveSpeed3 * 2).SetLoops(-1);
        };
        Seawve3_1.DOLocalMoveX(seawveStartResetXPositon, seawveSpeed3 * 2).SetLoops(-1);
    }

    /// <summary>
    /// 玩家摇杆控制初始化
    /// </summary>
    private void PlayerCtrlInit()
    {
        LeftTouch.SetActive(false);
        RightTouch.SetActive(false);
        return;

        //方向键事件
        //左方向键
        UGUIEventListener.Get(LeftTouch).onPointerDown = (click) =>
        {
            CtrlShipRun(true, true);
        };
        UGUIEventListener.Get(LeftTouch).onPointerUp = (click) =>
        {
            CtrlShipRun(false);
        };
        //右方向键
        UGUIEventListener.Get(RightTouch).onPointerDown = (click) =>
        {
            CtrlShipRun(true, false);
        };
        UGUIEventListener.Get(RightTouch).onPointerUp = (click) =>
        {
            CtrlShipRun(false);
        };
    }

    /// <summary>
    /// 控制船的运动  参数1：停止还是运动     参数2：  方向 true 为左边 
    /// </summary>
    private void CtrlShipRun(bool actice, bool dirctror = true)
    {
        if (!actice)
        {
            //停船
            ShipRunCtrl = false;
            StopCoroutine("StartPlayerShipRun");
            return;
        }

        if (dirctror)
        {
            //左行
            ShipRunCtrl = true;
            //显示船的图片
            PlayerShipLeft.SetActive(true);
            PlayerShipRight.SetActive(false);
            playerShipSpeed = -Mathf.Abs(playerShipSpeed);
            StopCoroutine("StartPlayerShipRun");
            StartCoroutine("StartPlayerShipRun");
        }
        else
        {
            //右行
            ShipRunCtrl = true;
            PlayerShipLeft.SetActive(false);
            PlayerShipRight.SetActive(true);
            playerShipSpeed = Mathf.Abs(playerShipSpeed);
            StopCoroutine("StartPlayerShipRun");
            StartCoroutine("StartPlayerShipRun");
        }


    }


    /// <summary>
    /// 开启战舰运动的协程
    /// </summary>
    IEnumerator StartPlayerShipRun()
    {
        while (true && ShipRunCtrl)
        {
            yield return new WaitForFixedUpdate();
            if (playerShipSpeed > 0)
            {
                if (PlayerShip.localPosition.x <= runRightDistance)
                {
                    PlayerShip.Translate(new Vector3(playerShipSpeed, 0, 0) * Time.deltaTime);
                }
                else
                {
                    PlayerShip.localPosition = new Vector3(runRightDistance, PlayerShip.localPosition.y, 0);
                    break;
                }
            }
            else
            {
                if (PlayerShip.localPosition.x >= runLeftDistance)
                {
                    PlayerShip.Translate(new Vector3(playerShipSpeed, 0, 0) * Time.deltaTime);
                }
                else
                {
                    PlayerShip.localPosition = new Vector3(runLeftDistance, PlayerShip.localPosition.y, 0);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 播放当前句子的音频
    /// </summary>
    private void PlayerAudioMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
            return;
        switch (StartData.dataType)
        {
            case DataType.Ask:
                if (CheckIsPlayAskAudio())
                {
                    return;
                }
                askAudioSources = CurrentAsk.PlaySentenceSound();

                break;
            case DataType.Say:
                audioSource = CurrentSay.PlayAnswerSound();
                break;
            case DataType.Word:
                audioSource = CurrentWord.PlaySound();
                break;
        }
    }

    private List<int> randomSeries = new List<int>();//随机序列 - 用来随机间单词打乱的一个序列
    /// <summary>
    /// 得到一个随机序列保存在RandomSeries数组中
    /// </summary>
    private void GetRandomSeries(int startIndex, int endIndex)
    {
        if (startIndex >= endIndex)
            return;

        int result = Random.Range(startIndex, endIndex);
        randomSeries.Add(result);

        GetRandomSeries(startIndex, result);
        GetRandomSeries(result + 1, endIndex);
    }





    #region 2018年3月30日11:54:24 励步需求修改部分++++++++++++++++++++++++++++++
    private float autoPlayAudioWaitTime = -1f;
    /// <summary>
    /// 自动播放声音的停顿时间
    /// </summary>
    private float AutoPlayAudioWaitTime
    {
        get
        {
            if (autoPlayAudioWaitTime == -1f)
            {
                autoPlayAudioWaitTime = float.Parse(ConfigGlobalValueModel.GetValue("GamePlayAudioWaitTime"));
            }
            return autoPlayAudioWaitTime;
        }
    }

    private float autoAttackDis = 0.2f;//自动攻击的攻击句子
    private bool IsAudoAtack = false;//是否正在自动攻击中
    private bool isCanAtack = false;//是否可以攻击

    /// <summary>
    /// 开启自动攻击方法
    /// </summary>
    public void OpenAutoAtack(Transform target)
    {
        if (!isCanAtack)
            return;

        if (!IsAudoAtack && target != null)
            StartCoroutine(AutoAtack(target));
    }

    /// <summary>
    /// 自动攻击的协程 参数1 目标
    /// </summary>
    private IEnumerator AutoAtack(Transform target)
    {
        if (IsAudoAtack)
            yield break; ;
        IsAudoAtack = true;

        yield return 0;
        while (true)
        {
            float dis = target.position.x - PlayerShip.position.x;
            if (Mathf.Abs(dis) <= autoAttackDis)
            {
                //进入攻击距离
                CtrlShipRun(false);//停船
                OnClickAttack();//攻击
                break;
            }

            if (PlayerShip.localPosition.x > runRightDistance || PlayerShip.localPosition.x < runLeftDistance)
            {
                Debug.LogWarning("船已经到了顶点了！");
                CtrlShipRun(false);//停船
                OnClickAttack();//攻击
                if (dis > 0)
                {
                    PlayerShip.localPosition = new Vector3(runRightDistance, PlayerShip.localPosition.y, 0);
                }
                else
                {
                    PlayerShip.localPosition = new Vector3(runLeftDistance, PlayerShip.localPosition.y, 0);
                }
                break;
            }

            //追击敌舰
            CtrlShipRun(true, dis < 0);

            yield return 0;
        }
        IsAudoAtack = false;
    }

    /// <summary>
    /// 控制自动播放声音的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator CtrlAutoPlayAudio()
    {
        yield return 0;

        var wf = new WaitForSeconds(AutoPlayAudioWaitTime);

        while (true)
        {
            if (StartData.dataType == DataType.Ask)
            {
                if (!CheckIsPlayAskAudio())
                {
                    yield return wf;
                    PlayerAudioMusic();
                }
            }
            else if (audioSource == null || !audioSource.isPlaying)
            {
                yield return wf;
                PlayerAudioMusic();
            }
            yield return 0;
        }
    }

    /// <summary>
    /// 判断是否正在播放对话声音
    /// </summary>
    private bool CheckIsPlayAskAudio()
    {
        if (askAudioSources != null)
        {
            foreach (var v in askAudioSources)
            {
                if (v != null && v.isPlaying)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #endregion
}
