using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class UIShootingWords : UIBaseLevelGame
{

    /// <summary>
    /// 单词出现的位置
    /// </summary>
    public RectTransform WordPos { get { return GetR("WordPos"); } }

    private Image ImgWord { get { return Get<Image>("ImgWord"); } }
    /// <summary>
    /// 炮管
    /// </summary>
    private GameObject ImgBarrel { get { return Get("ImgBarrel"); } }

    public Rigidbody2D RbBarrel { get { return Get<Rigidbody2D>("ImgBarrel"); } }
    /// <summary>
    /// 炮弹的挂点位置
    /// </summary>
    private Transform ImgBulletPos { get { return GetR("ImgBulletPos"); } }
    /// <summary>
    /// 炮弹
    /// </summary>
    GameObject goBullet;
    /// <summary>
    /// 鼠标点下
    /// </summary>
    private bool m_pointDown;
    /// <summary>
    /// 当前创建的词组
    /// </summary>
    List<GameObject> listWords = new List<GameObject>();
    /// <summary>
    /// 单词牌正确
    /// </summary>
    GameObject goRight;
    /// <summary>
    /// 错误单词牌
    /// </summary>
    GameObject goWrong;
    /// <summary>
    /// 错误单词文字
    /// </summary>
   public static string WrongWord;
    /// <summary>
    /// 错误单词的ID
    /// </summary>
    public static string WrongWordId;
    /// <summary>
    /// 当前创建的单词数量
    /// </summary>
    int WordCount = 0;
    public static UIShootingWords Instance { get; set; }
    private bool isShootRight ;
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
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_pointDown = true;
            if (RbBarrel != null)
                RbBarrel.drag = 0;
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_pointDown = false;
            if (RbBarrel != null)
            {
                RbBarrel.drag = 5;
                Vector2 velocity = RbBarrel.velocity;
                RbBarrel.velocity = RbBarrel.transform.TransformDirection(Vector2.up) * velocity.magnitude;
            }
        }
    }

    void FixedUpdate()
    {

        if (RbBarrel == null)
            return;
        if (!m_pointDown)
            return;
        CarDirectionCompute();
    }

    private void CarDirectionCompute()
    {
        Vector3 carScreenPosition = UIManager.Instance.WorldCamera.WorldToScreenPoint(RbBarrel.transform.position);
        carScreenPosition.z = 0;
        Vector3 screenDirection = Input.mousePosition - carScreenPosition;
        Quaternion quatertion = Quaternion.FromToRotation(Vector3.up, screenDirection);
        RbBarrel.transform.localRotation = quatertion;

    }
    public override void PlayGame()
    {
        if (IsGameEnd)
            return;
        isShootRight = false;
        WordPos.ClearAllChild();
        WordCount = 0;
        listWords.Clear();
        ImgWord.sprite = CurrentWord.sprite;//找到对应的图片
        WrongWord = GetRandomOtherWord(CurrentWord.letter);
        CurrentWord.PlaySound();
        //创建单词
        CreateWords();
    }

    /// <summary>
    /// 创建目标单词对象
    /// </summary>
    private void CreateWords()
    {
        if (WordCount == 0)
        {
            int random = Random.Range(0, 2);
            if (random == 0)
            {
                goRight = CreatItemWord();
                if (IsFirstPlay && goRight != null)
                {
                    CreateUIItem("GuideHandAnimationItem", goRight.transform.GetChild(0).transform);
                }
                this.WaitSecond(() =>
                {
                    goWrong = CreatItemWord();
                    goWrong.GetComponentInChildren<TextMeshProUGUI>().text = WrongWord;

                }, 1f);
            }
            else
            {
                goWrong = CreatItemWord();
                goWrong.GetComponentInChildren<TextMeshProUGUI>().text = WrongWord;
                //创建正确单词
                this.WaitSecond(() =>
                {
                    goRight = CreatItemWord();
                    if (IsFirstPlay && goRight != null)
                    {
                        CreateUIItem("GuideHandAnimationItem", goRight.transform.GetChild(0).transform);
                    }
                }, 1f);
            }
        }
    }

    /// <summary>
    /// 创建单词
    /// </summary>
    /// <returns></returns>
    private GameObject CreatItemWord()
    {
        GameObject go = CreateUIItem("UIShootingWordsItem", WordPos);
        WordCount++;
        RandomWordPosition(go.transform as RectTransform);
        go.GetComponentInChildren<TextMeshProUGUI>().text = CurrentWord.letter;
        listWords.Add(go);
        UGUIEventListener.Get(go).onPointerDown = (d) =>
        {
            CannonFire();
        };
        RectTransform Rt = go.transform as RectTransform;
        TweenWordCome(1.5f, Rt).OnComplete(() =>
        {
            Destroy(go);
            listWords.Remove(go);
            WordCount--;
            if (listWords.Count == 0 && WordCount == 0)
            {
                CreateWords();
            }
        });
        return go;
    }

    /// <summary>
    /// 随机出现位置
    /// </summary>
    /// <param name="rt"></param>
    private RectTransform RandomWordPosition(RectTransform rt)
    {
        Vector2 temp = rt.anchoredPosition;
        int random = 0;
        random = Random.Range(-210, 250);
        temp.y = random;
        rt.anchoredPosition = temp;
        return rt;
    }
    /// <summary>
    /// 随机出现与当前单词错误的单词
    /// </summary>
    /// <param name="currentWord"></param>
    /// <returns></returns>
    private string GetRandomOtherWord(string currentWord)
    {
        int random = Random.Range(0, m_randomWordList.Count);
        ConfigWordLibraryModel otherWord = m_randomWordList[random];
        if (otherWord.letter == currentWord)
        {
            return GetRandomOtherWord(currentWord);
        }
        WrongWordId = otherWord.id;
        return otherWord.letter;
    }

    /// <summary>
    /// 单词板移动
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    public Tweener TweenWordCome(float delay, RectTransform go)
    {
        float y = go.anchoredPosition.y;
        go.anchoredPosition = new Vector2(100, y);
        Tweener tween = go.DOAnchorPosX(-UIManager.Instance.UIRoot.GetComponent<CanvasScaler>().referenceResolution.x - 700f, 6f);
        tween.SetEase(Ease.Linear);
        tween.SetDelay(delay);
        return tween;
    }

    /// <summary>
    /// 大炮发射子弹
    /// </summary>
    private void CannonFire()
    {
        goBullet = CreateUIItem("UIShootingWordsBulletItem", ImgBulletPos);
        UIBulletMove bullet = goBullet.AddComponent<UIBulletMove>();
        AudioManager.Instance.Play("FireSound");//开炮的声音
        bullet.transform.position = ImgBulletPos.position;
        bullet.direction = (Input.mousePosition - UIManager.Instance.WorldCamera.WorldToScreenPoint(ImgBarrel.transform.position)).normalized;
    }

    /// <summary>
    /// 射击到了真确的单词
    /// </summary>
    public void ShootingRightWord()
    {
        isShootRight = true;
        StartCoroutine(shootRightIE());
    }
    private IEnumerator shootRightIE()
    {
        yield return new WaitForSeconds(AudioManager.Instance.PlayThreeAudio(CurrentWord.letter, CurrentWord.letter, CurrentWord.word));
        listWords.Remove(goRight);
        WordCount--;
        var fly = FlyStar(true);
        fly.OnComplete += () => PlayGame();
        //CurrentWord.PlayLetter();
    }
    /// <summary>
    /// 射击了错误单词
    /// </summary>
    internal void ShootingWrongWord()
    {
        if (isShootRight)
            return;
        if (!isFlayStar) 
        {
            AudioManager.Instance.Play(UIShootingWords.WrongWord);
            isFlayStar = true;
            FlyStar(false).OnComplete += ()=>isFlayStar = false;
        }
    }

    /// <summary>
    ///qiubin 添加，解决连续炮击错误单词飞星出错问题 添加是否正在飞星的判断 - 
    /// </summary>
    private bool isFlayStar = false;
}
