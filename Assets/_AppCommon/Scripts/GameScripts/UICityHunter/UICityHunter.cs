using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class UICityHunter : UIBaseLevelGame {

    public static UICityHunter Instance { get; set; }
    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        Instance = null;
    }
    public Action onRefreshBullet;
    void OnEnable()
    {
        onRefreshBullet = OnRefreshBulletHandler;      
    }
    void OnDisable()
    {
        onRefreshBullet = null;
        CancelInvoke("CreateBird");        
    }
    void OnApplicationQuit()
    {
        onRefreshBullet = null;        
        CancelInvoke("CreateBird");
    }
    //Bird出生地
    private Transform BirdPos0 { get { return GetT("BirdPos0"); } }
    private Transform BirdPos1 { get { return GetT("BirdPos1"); } }
    private Transform BirdPos2 { get { return GetT("BirdPos2"); } }
    private List<Transform> BirdPosList;

    //子弹生成位置
    private Transform BulletPos { get { return GetT("BulletPos"); } }
    public GameObject Bullet;
    private Vector3 BullerVec;

    //弹弓底座
    private RectTransform SlingshotBase { get { return GetR("SlingshotBase"); } }
    private Vector3 RopeLeft;
    private Vector3 RopeRight;
    

    //画线的左右基础
    private LineRenderer SlingshotLeft { get { return Get<LineRenderer>("SlingshotLeft"); } }
    private LineRenderer SlingshotRight { get { return Get<LineRenderer>("SlingshotRight"); } }

    //单词
    private Image WordText { get { return Get<Image>("WordText"); } }

    //播音
    private Transform Audio { get { return GetT("Audio"); } }

    public override void Refresh()
    {       
        SlingshotBase.gameObject.AddComponent<UICityHunterSelectCtrl>();
        Bullet = CreateUIItem("UICityHunterBullet", BulletPos);
        Bullet.AddComponent<UICityHunterBullet>();
        UGUIEventListener.Get(Audio).onPointerClick = OnAudioPlay;
    }
    private void OnAudioPlay(PointerEventData data)
    {
        if (CurrentWord.PlaySound().isPlaying)
        {
            return;
        }
        CurrentWord.PlaySound();
    }

    public override void PlayGame()
    {
        CancelInvoke("CreateBird");   
        if (IsGameEnd)                
            return;
        CreateBird();
        InvokeRepeating("CreateBird", 5, 5);       
    }
    public void CreateBird()
    {
        CurrentWord.PlaySound();
        BirdPos0.ClearAllChild();
        BirdPos1.ClearAllChild();
        BirdPos2.ClearAllChild();
        List<ConfigWordLibraryModel> WordList = new List<ConfigWordLibraryModel>(m_originalWordList);
        WordList.Remove(CurrentWord);
        WordList = Utility.RandomSortList(WordList);
        int indexPos = UnityEngine.Random.Range(0, 3);
        WordList.Insert(indexPos, CurrentWord);
        List<int> numList = new List<int>();
        numList.Add(0);
        numList.Add(1);
        numList.Add(2);
        numList = Utility.RandomSortList(numList);
        CreateUIItem("UICityHunterBird", BirdPos0).AddComponent<UICityHunterBird>().Init(WordList[0].word, "UICityHunter_Feather"+numList[0], "UICityHunter_Bird"+numList[0]);
        CreateUIItem("UICityHunterBird", BirdPos1).AddComponent<UICityHunterBird>().Init(WordList[1].word, "UICityHunter_Feather" + numList[1], "UICityHunter_Bird" + numList[1]);
        CreateUIItem("UICityHunterBird", BirdPos2).AddComponent<UICityHunterBird>().Init(WordList[2].word, "UICityHunter_Feather" + numList[2], "UICityHunter_Bird" + numList[2]);
        WordText.sprite= CurrentWord.sprite;
        WordText.SetNativeSize();
        WordText.transform.DOScale(2f, 1f).From();

    }

    void Update()
    {
        RopeLeft.x = SlingshotBase.anchoredPosition.x + 130;
        RopeLeft.y = SlingshotBase.anchoredPosition.y + 270;
        RopeLeft.z = -0.1f;
        SlingshotLeft.SetPosition(1, RopeLeft);
        RopeRight.x = SlingshotBase.anchoredPosition.x - 130;
        RopeRight.y = SlingshotBase.anchoredPosition.y + 270;
        RopeRight.z = -0.1f;       
        SlingshotRight.SetPosition(1, RopeRight);
        if (BulletPos.childCount==1)
        {
            if (!Bullet.GetComponent<UICityHunterBullet>().isShoot)
            {
                RectTransform child = (RectTransform)Bullet.transform;
                BullerVec = SlingshotBase.anchoredPosition;
                BullerVec.y = SlingshotBase.anchoredPosition.y +400;
                child.anchoredPosition = BullerVec;
                
            }
        }
      
    }
    public void CreateBullet()
    {       
        Bullet = CreateUIItem("UICityHunterBullet", BulletPos);
        Bullet.AddComponent<UICityHunterBullet>();   
    }
    private void OnRefreshBulletHandler()
    {
        BulletPos.ClearAllChild();
        CreateBullet();
    }
    public void CloseInvoke()
    {
        CancelInvoke("CreateBird");   
    }
}
