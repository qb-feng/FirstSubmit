using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 航海日记 - 攻击弹药Item
/// </summary>
public class UIJournalWampItem : UIBaseInitItem
{

    /// <summary>
    /// 供外部访问的初始化数据 
    /// 参数1：创建出来时的世界坐标位置  
    /// 参数2：需要轰炸的目标单词
    /// 参数3：轰炸结果的回调方法
    /// </summary>
    public void Init(Vector3 createWordPositon, string targetWord, Action<bool> resultBackFunction)
    {
        initWordPosition = createWordPositon;
        transform.position = createWordPositon;
        wampTargetWord = targetWord;
        resultBackFunc = resultBackFunction;

        isRun = true;
        thisRigidbody.AddForce(Vector2.down * wampRunSpeed, ForceMode2D.Force);
    }
    //组件
    private Rigidbody2D thisRigidbody { get { return Get<Rigidbody2D>("UIJournalWampItem(Clone)"); } }//当前刚体

    //数据
    private Vector3 initWordPosition;//出生点坐标
    private float wampRunSpeed = 500f;//弹药飞行速度
    private string wampTargetWord;//弹药需要轰炸的目标单词
    private bool isRun = false;//弹药是否可以开始飞行了
    private Action<bool> resultBackFunc;//轰炸结果回调方法
    private bool isOnAttack = false;//是否已经攻击过（保证当前弹药只能够使用一次）

    /// <summary>
    /// 轰炸敌人
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log(collision.transform.name);
        if (collision.gameObject.tag.Equals(base.InitTag))
        {
            var enumy = collision.gameObject.GetComponent<UIJournalEnemyItem>();
            if (enumy != null && !enumy.IsOnAttack && !isOnAttack)
            {
                //使用当前炮弹
                isOnAttack = true;
                //确定是敌军了
                resultBackFunc(enumy.CurrentWord.Equals(wampTargetWord));//返回轰炸结果
                enumy.OpenOnAttackState();//击中敌军
                GameObject.Destroy(gameObject);//销毁自己
            }
        }
    }
    void Start()
    {
        Destroy(gameObject, 5f);
    }

}
