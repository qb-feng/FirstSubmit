using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 忍者无敌游戏模型
/// </summary>
public class UINinjiaAicient : UIDataGameBase
{
    //组件
    private Transform ItemPoints { get { return GetT("ItemPoints"); } }//item的位置的总父物体
    private List<Transform> itemPointsList = new List<Transform>();//item的所有的位置点
    private RectTransform Sword { get { return GetR("Sword"); } }//剑
    private RectTransform AttackNum { get { return GetR("AttackNum"); } }//显示攻击次数的物体
    private TMPro.TextMeshProUGUI AttackNumText { get { return Get<TMPro.TextMeshProUGUI>("AttackNumText"); } }//显示当前的连击次数

    //变量
    private List<UINinjiaAicientItemBase> currentItemsList = new List<UINinjiaAicientItemBase>();//当前存在的所有item
    private List<string> currentStringList;//当前的单词容器（保存当前单词的每一个字母）
    private List<string> currentCharList = new List<string>();//保存当前单词出现的所有字母（不可重复）
    private List<int> ranomSortList = new List<int>();//随机序列
    private float ItemFlyMaxSpeed = 1.7f;//子物体最慢飞行速度
    private float ItemFlyMinSpeed = 0.6f;//子物体最快飞行速度
    private System.Text.StringBuilder answerSb = new System.Text.StringBuilder();//当前已填写的单词
    private System.Text.StringBuilder sb = new System.Text.StringBuilder();//操作类型的字符串
    private bool isAnswerOK = false;//当前是否回答完成
    RaycastHit2D hit;//2d射线检测效果器
    private int attackNum = 0;//连击次数
    private float attackNumMaxScale = 2f;//连击效果的最大放大倍数
    private float attackNumMaxMaxScale = 4f;//连击效果的超级放大倍数
    private string SpaceString = " ";//空格

    //剑光参数
    private Vector2 swordStartPoint;//剑光的起点
    private Vector2 swordNowPoint;//剑光此刻的点
    private Vector2 swordNowDir;//剑光此刻的方向
    private Vector2 swordToRightAnchors = new Vector2(0, 0.5f);//剑光往右侧滑时固定的Anchors
    private Vector2 swordToLeftAnchors = new Vector2(1, 0.5f);//剑光往左侧滑时固定的Anchors
    private Vector2 swordSize;//剑光的大小
    private float swordAngle = 0;//剑关需要转动的角度
    private float swordNowTime = 0;//剑光当前出现的时间
    private float swordMaxLifeTime = 0.08f;//剑光最大存活时间
    private bool swordCurrentStatue = false;//当前剑光是开启的吗 - 默认不是

    private float mainCanvalsScale = 0;
    private float MainCanvalsScale//主相机缩小的scale
    {
        get
        {
            if (mainCanvalsScale == 0)
                mainCanvalsScale = UIManager.Instance.UIRootHorizontal.transform.localScale.x;
            return mainCanvalsScale;
        }
    }
    /// <summary>
    /// 游戏启动处
    /// </summary>
    protected override void OnGameEnd()
    {
        base.OnGameEnd();
    }

    protected override void RefreshData()
    {
        base.RefreshData();
        ItemPoints.ClearAllChild();
        currentItemsList.Clear();
        currentCharList.Clear();
        ranomSortList.Clear();
        answerSb.Remove(0, sb.Length);
        sb.Remove(0, sb.Length);
        AttackNum.gameObject.SetActive(false);
        attackNum = 0;
        isAnswerOK = false;
    }
    public override void Refresh()
    {
        base.IsHaveAuioPlayer = true;//有播放器
        base.Refresh();

        //数据初始化 
        //初始化item的初始化点
        for (int i = 0; i < ItemPoints.childCount; ++i)
        {
            itemPointsList.Add(ItemPoints.GetChild(i));
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
        //获取数据
        currentStringList = base.GetCurrntTextListString();//保存当前存在的所有单词
        //得到当前需要出现的所有单词
        foreach (var v in currentStringList)
        {
            if (!currentCharList.Contains(v))
            {
                currentCharList.Add(v);
            }
        }
        //播放读音 - 强制开启
        PlayAudio(true);

        //显示当前单词
        for (int i = 0; i < currentStringList.Count; ++i)
        {
            sb.Append("_");
        }
        ShowText.SetText(sb.ToString());

        //创建炸弹和竹子item
        GetRandomSeries(ranomSortList, 0, currentCharList.Count);
        int bombIndex = Random.Range(0, ranomSortList.Count);//得到炸弹的随机位置
        float flySpeed = 0;//飞行速度
        for (int i = 0; i < ranomSortList.Count; ++i)
        {
            flySpeed = Random.Range(ItemFlyMinSpeed, ItemFlyMaxSpeed) + i * 0.1f;
            if (i == bombIndex)
            {
                //是炸弹
                currentItemsList.Add(CreateUIItem("UINinjiaAicientBombItem", ItemPoints).AddComponent<UINinjiaAicientItemBase>().Init(true, null, flySpeed));
                flySpeed = Random.Range(ItemFlyMinSpeed, ItemFlyMaxSpeed) + i * 0.3f;
            }
            //判断单词是否是空格 - 空格的话跳过去！！！
            if (currentCharList[ranomSortList[i]].Equals(" "))
                continue;
            //是竹子
            if (i % 3 == 0)
            {
                currentItemsList.Add(CreateUIItem("UINinjiaAicientFoodItem1", ItemPoints).AddComponent<UINinjiaAicientItemBase>().Init(false, currentCharList[ranomSortList[i]], flySpeed));
            }
            else if (i % 3 == 1)
            {
                currentItemsList.Add(CreateUIItem("UINinjiaAicientFoodItem2", ItemPoints).AddComponent<UINinjiaAicientItemBase>().Init(false, currentCharList[ranomSortList[i]], flySpeed));
            }
            else if (i % 3 == 2)
            {
                currentItemsList.Add(CreateUIItem("UINinjiaAicientFoodItem3", ItemPoints).AddComponent<UINinjiaAicientItemBase>().Init(false, currentCharList[ranomSortList[i]], flySpeed));
            }
        }
    }


    //剑光的控制 - 可怕~~update
    private void Update()
    {
        if (isGameEnd)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            //鼠标左键按下
            swordCurrentStatue = true;//开启剑光
            swordStartPoint = UIManager.Instance.WorldCamera.ScreenToWorldPoint(Input.mousePosition);//初始化起点
            Sword.position = swordStartPoint;

        }
        if (Input.GetMouseButton(0))
        {
            if (swordNowTime == 0)
            {
                swordCurrentStatue = true;//开启剑光
                swordStartPoint = UIManager.Instance.WorldCamera.ScreenToWorldPoint(Input.mousePosition);//初始化起点
                Sword.position = swordStartPoint;
            }

            if (!swordCurrentStatue)
                return;

            if (swordNowTime >= swordMaxLifeTime)
            {
                swordCurrentStatue = false;//关闭剑光
                //取消光
                swordSize.x = 0;
                swordSize.y = Sword.sizeDelta.y;
                //赋值光的大小
                Sword.sizeDelta = swordSize;
                swordNowTime = 0;
                return;
            }

            swordNowTime += Time.deltaTime;

            //鼠标左键按着
            swordNowPoint = UIManager.Instance.WorldCamera.ScreenToWorldPoint(Input.mousePosition);//得到当前点
            //计算玩家鼠标的移动方向
            swordNowDir = swordNowPoint - swordStartPoint;
            //根据鼠标的移动方向计算玩家的移动方向与x轴的角度
            if (swordNowDir.x > 0)
            {
                //鼠标往右拉
                Sword.pivot = swordToRightAnchors;
                swordAngle = Vector2.Angle(swordNowDir, Vector2.right);
                swordAngle = swordAngle * (swordNowDir.y > 0 ? 1 : -1);
            }
            else
            {
                //鼠标往左拉
                Sword.pivot = swordToLeftAnchors;
                swordAngle = Vector2.Angle(swordNowDir, Vector2.left);
                swordAngle = swordAngle * (swordNowDir.y > 0 ? -1 : 1);
            }
            //计算光的长度
            swordSize.x = swordNowDir.magnitude / MainCanvalsScale;
            swordSize.y = Sword.sizeDelta.y;
            //赋值光的大小
            Sword.sizeDelta = swordSize;
            //赋值光的角度
            Sword.rotation = Quaternion.Euler(0, 0, swordAngle);

            //射线检测是否切到item
            hit = Physics2D.Raycast(UIManager.Instance.WorldCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit != null && hit.collider != null && hit.transform != null)
            {
                var trans = hit.transform.parent;
                if (trans != null)
                {
                    var item = trans.GetComponent<UINinjiaAicientItemBase>();
                    if (item != null)
                        item.OnAttack(ResultBackFunc, swordNowDir.x, swordNowDir.y);//切掉该物体
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            //鼠标左键松手
            //取消光
            swordSize.x = 0;
            swordSize.y = Sword.sizeDelta.y;
            //赋值光的大小
            Sword.sizeDelta = swordSize;
        }
    }
    /// <summary>
    /// 切竹子item的回调函数
    /// </summary>
    /// <param name="attackStirng"></param>
    private void ResultBackFunc(string attackStirng)
    {
        if (isAnswerOK)
            return;

        if (attackStirng == null) 
        {
            //此时表明切到了炸弹
            attackNum = 0;
            ShowAttackNumText();
            return;
        }

        //判断是否切到了当前需要的单词
        if (attackStirng.Equals(currentStringList[answerSb.Length]))
        {
            //切对了 - 将该单词添加到回答字母序列中
            answerSb.Append(attackStirng);

            //判断当前是否全部选择完成
            if (answerSb.Length == currentStringList.Count)
            {
                //选择完成
                isAnswerOK = true;//回答完成
                //飞星 - 并到下一关
                FlyStar(true, true).OnComplete += PlayGame;
            }

            //显示单词
            int index = answerSb.Length - 1;
            sb.Insert(index, answerSb[index]);
            sb.Remove(sb.Length - 1, 1);//移除最后一位
            ShowText.SetText(sb.ToString());

            //判断下一个格子是否是空格处 - 处理下一个是空格的情况
            if (answerSb.Length < currentStringList.Count && currentStringList[answerSb.Length].Equals(SpaceString))
            {
                answerSb.Append(SpaceString);
                //显示单词
                index = answerSb.Length - 1;
                sb.Insert(index, answerSb[index]);
                sb.Remove(sb.Length - 1, 1);//移除最后一位
                ShowText.SetText(sb.ToString());
                return;
            }              
        }
        else
        {
            //切错了
            FlyStar(false, false);
        }

        //切中了，连击次数加1
        attackNum += 1;
        ShowAttackNumText();

    }

    /// <summary>
    /// 显示连击次数
    /// </summary>
    private void ShowAttackNumText()
    {
        if (attackNum == 1)
            AttackNum.gameObject.SetActive(true);
        AttackNumText.SetText(attackNum.ToString());
        if (attackNum % 1 == 0)
        {
            AttackNum.DOScale(attackNumMaxScale, attackNuMdoSclaeTime).SetLoops(2, LoopType.Yoyo).onComplete = () =>
            {
                AttackNum.localScale = Vector3.one;
            };
        }
        if (attackNum % 20 == 0)
        {
            AttackNum.DOScale(attackNumMaxMaxScale, attackNuMdoSclaeTime * 2).SetLoops(2, LoopType.Yoyo).onComplete = () =>
            {
                AttackNum.localScale = Vector3.one;
            };
        }
    }
    private float attackNuMdoSclaeTime = 0.08f;
}
