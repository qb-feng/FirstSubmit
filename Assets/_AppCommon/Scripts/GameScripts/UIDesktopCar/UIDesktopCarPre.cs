using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class UIDesktopCarPre : UIBaseLevelGame
{
    /// <summary>
    /// 单词位置
    /// </summary>
    private GameObject ContentLetters { get { return Get("Content"); } }
    public Transform ImageCarPosition { get { return GetT("CarPos"); } }
    private Transform ImgPosition0 { get { return GetT("0"); } }
    private Transform ImgPosition1 { get { return GetT("1"); } }
    private Transform ImgPosition2 { get { return GetT("2"); } }

    /// <summary>
    /// 生成字母合集，所有词根除去重复项
    /// </summary>
    private string letterAmount;
    /// <summary>
    /// 考察单词词根用_代替后的字符串例如单词pan则worContain_=p__
    /// </summary>
    private string wordContain_;
    /// <summary>
    /// _在单词中的位置
    /// </summary>
    private int _InWordIndex;
    private const float m_carSpeed = 10f;
    protected Rigidbody2D m_currentCar;
    public Rigidbody2D Car { get { return Get<Rigidbody2D>("ImageCar"); } }
    private GameObject ShowWord { get { return Get("WordClick"); } }

    private int m_currentCarMatchLetterIndex = -1;

    private bool m_pointDown;

    private List<GameObject> ShowWordList = new List<GameObject>();
    //特效
    public AudioSource m_carMoveAudio;
    /// <summary>
    /// 引导小手
    /// </summary>
    public GameObject goGuideHand;

    public virtual string CarMoveAudioName { get { return "bus_run"; } }

    public static UIDesktopCarPre Instance { get; set; }
    IEnumerator DisabledGruopFunc()
    {
        yield return new WaitForEndOfFrame();
        ContentLetters.GetComponent<GridLayoutGroup>().enabled = false;
    }

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Destroy(m_carMoveAudio.gameObject);
        Instance = null;
    }

    public override void Refresh()
    {
        m_carMoveAudio = AudioManager.Instance.Play(CarMoveAudioName, destroy: false);
        m_carMoveAudio.loop = true;
        m_carMoveAudio.volume = 0;
    }

    public override void PlayGame()
    {
        if (goGuideHand != null)
        {
            Destroy(goGuideHand);
        }
        if (IsGameEnd)
            return;
        foreach (Transform child in ContentLetters.transform)
        {
            child.ClearAllChild();
        }
        ContentLetters.GetComponent<GridLayoutGroup>().enabled = true;
        m_currentCarMatchLetterIndex = 0;
        //TODO传进词根
        letterAmount = "";
        foreach (var item in m_originalWordList)
        {
            for (int i = 0; i < m_originalWordList.Count; i++)
            {
                if (letterAmount.Contains(item.letter))
                    continue;
                else
                {
                    letterAmount += item.letter;
                }
            }               
        }   
        RandomWordPosition(letterAmount);

        InitCurrentWord(CurrentWord.word,CurrentWord.letter);
        SetCurrentWordImg(CurrentWord.sprite);
        m_currentCar = Car;
        SetCurrentCarImg();
        StartCoroutine(DisabledGruopFunc());
        CurrentWord.PlaySound();

        InitShowWord();
    }
    /// <summary>
    /// 设置当前单词图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCurrentWordImg(Sprite sprite)
    {

        GameObject goImg = Get("Picture").gameObject;
        foreach (var item in goImg.GetComponentsInChildren<Image>())
        {
            item.rectTransform.anchoredPosition = Vector2.zero;
        }
        int posIndex = Random.Range(0, 3);
        for (int i = 0; i < goImg.transform.childCount; i++)
        {
            if (posIndex == i)
            {
                goImg.transform.GetChild(i).gameObject.SetActive(true);
                goImg.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = sprite;
            }
            else
            {
                goImg.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 随机单词字母位置
    /// </summary>
    /// <param name="sWord"></param>
    private void RandomWordPosition(string sWord)
    {
        var iletters = sWord.ToCharArray();
        int[] LetterIndex = new int[iletters.Length];
        if (iletters.Length >= 17)
        {
            for (int i = 0; i < iletters.Length; i++)
            {
                LetterIndex[i] = Random.Range(0, 20);
                for (int j = 0; j < LetterIndex.Length; j++)
                {
                    if (LetterIndex[j] == LetterIndex[i] && i > j)
                    {
                        i -= 1;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < iletters.Length; i++)
            {
                LetterIndex[i] = Random.Range(0, 17);
                for (int j = 0; j < LetterIndex.Length; j++)
                {
                    if (LetterIndex[j] == LetterIndex[i] && i > j)
                    {
                        i -= 1;
                    }
                }
            }
        }
        for (int k = 0; k < iletters.Length; k++)
        {
            if (iletters[k] != ' ')
            {
                Transform wordpos = ContentLetters.transform.GetChild(LetterIndex[k]);
                var charWord = CreateUIItem("UIDesktopCarPreCharWordItem", wordpos);
                RectTransform Rt = charWord.GetComponent<RectTransform>();
                Rt.Rotate(new Vector3(0, 0, Random.Range(-50, 50)));
                TextMeshProUGUI wordText = charWord.GetComponent<TextMeshProUGUI>();
                wordText.text = iletters[k].ToString();
                wordText.gameObject.AddComponent<UIDesktopCarItemLetter>();
                BoxCollider2D goBox = wordText.gameObject.GetComponent<BoxCollider2D>();
                goBox.enabled = true;
                goBox.isTrigger = true;
                ///引导
                if (k == 0 && IsFirstPlay)//第一次玩游戏
                {
                    goGuideHand = CreateUIItem("GuideHandAnimationItem", charWord.transform);
                    goGuideHand.transform.SetAsLastSibling();
                    Vector3 vpo = new Vector3(0, -1, 0);
                    goGuideHand.transform.position = charWord.transform.position + vpo;
                    RectTransform RGuider = goGuideHand.GetComponent<RectTransform>();
                    RGuider.Rotate(Vector3.zero);
                }
            }
        }

    }
    /// <summary>
    /// 创建单词
    /// </summary>
    /// <param name="sWrod"></param>
    private void InitCurrentWord(string sWrod,string sLetter)
    {
        Utility.ClearChild(ShowWord.transform);
        //先判断词根的数量，再把单词中词根替换成-
        int letterAmount = sLetter.Length;
        string letterTo_ = "";
        for (int i = 0; i < letterAmount; i++)
        {
            letterTo_ += "_";
        }
        wordContain_ = sWrod.Replace(sLetter, letterTo_);
        for (int i = 0; i < wordContain_.Length; i++)
        {
            if (wordContain_[i].ToString().Contains("_"))
            {
                _InWordIndex = i;
                break;
            }
        }
        var word = wordContain_.ToCharArray();
        //var word = sWrod.ToCharArray();
        for (int i = 0; i < word.Length; i++)
        {
            GameObject charWorditem = CreateUIItem("UIDesktopCarPreCharWord", ShowWord.transform);
            var cw = charWorditem.GetComponent<TextMeshProUGUI>();
            cw.text = word[i].ToString();
            var wordColor = cw.color;
            if (cw.text == "_")
            {
                wordColor.a = 128f / 255f;
            }
            else
            {
                wordColor.a = 255f / 255f;
            }           
            cw.color = wordColor;           
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_pointDown = true;
            if (m_currentCar != null)
                m_currentCar.drag = 0;
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_pointDown = false;
            if (m_currentCar != null)
            {
                m_currentCar.drag = 5;
                Vector2 velocity = m_currentCar.velocity;
                //                m_currentCar.velocity = m_currentCar.transform.TransformDirection(Vector2.up) * velocity.magnitude;
            }
            m_carMoveAudio.volume = 0;
        }
    }


    private string GetCurrentWordLetter(int Index)
    {
        string lt = CurrentWord.word.Substring(m_currentCarMatchLetterIndex, 1);
        return lt;
    }
    void FixedUpdate()
    {
        if (m_currentCar == null)
            return;
        if (!m_pointDown)
            return;

        float value = m_currentCar.velocity.magnitude / m_carSpeed;
        m_carMoveAudio.volume = Mathf.Max(0.4f, value);

        Vector2 target = UIManager.Instance.WorldCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 velocity = m_currentCar.velocity;
        Vector2.SmoothDamp(m_currentCar.position, target, ref velocity, 0.3f, m_carSpeed, Time.deltaTime);
        m_currentCar.velocity = velocity;
        CarDirectionCompute();


    }

    public virtual void CarDirectionCompute()
    {
        Vector3 carScreenPosition = UIManager.Instance.WorldCamera.WorldToScreenPoint(m_currentCar.transform.position);
        carScreenPosition.z = 0;
        Vector3 screenDirection = Input.mousePosition - carScreenPosition;
        Quaternion quatertion = Quaternion.FromToRotation(Vector3.up, screenDirection);
        m_currentCar.transform.localRotation = quatertion;
    }

    /// <summary>
    /// 设置随机汽车
    /// </summary>
    public virtual void SetCurrentCarImg()
    {
        Image carImg = Get("ImageCar").GetComponent<Image>();
        carImg.sprite = GetS("UIDesktopCar" + Random.Range(1, 7));
        Transform tCar = carImg.gameObject.transform;
        tCar.Rotate(new Vector3(0, 0, Random.Range(-50, 50)));
        tCar.position = ImageCarPosition.position;
    }

    public bool MatchCurrentLetter(Collider2D collider, GameObject obj)
    {
        if (m_currentCar.gameObject.Equals(collider.gameObject))
        {
            if (CurrentWord.word != null)
            {
                string temp = CurrentWord.letter.Replace(" ", "");
                if (m_currentCarMatchLetterIndex >= temp.Length)
                    return false;
                string letter = temp.Substring(m_currentCarMatchLetterIndex, 1);
                TextMeshProUGUI txt = obj.GetComponent<TextMeshProUGUI>();
                if (letter == txt.text)
                {
                    MatchSuccEffect(obj, _InWordIndex+m_currentCarMatchLetterIndex, letter);
                    m_currentCarMatchLetterIndex++;
                    if (m_currentCarMatchLetterIndex == temp.Length)
                    {
                        StartCoroutine("MatchWordFinish");
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public float PlayWordAudio()
    {
        foreach (ConfigWordLibraryModel item in m_originalWordList)
        {
            if (item.word.Equals(CurrentWord.word))
            {
                AudioSource audioSource = item.PlaySound();
                return audioSource.clip.length;
            }
        }
        return 0;
    }

    private IEnumerator MatchWordFinish()
    {
        yield return new WaitForSeconds(1f);
        float time = PlayWordAudio();
        TweenBtnWord();
        yield return new WaitForSeconds(time);
        FlyStar(true).OnComplete += PlayGame;
    }

    private void MatchSuccEffect(GameObject obj, int index, string letter)
    {
        AudioManager.Instance.Play(letter);
        obj.transform.DOMove(GetWordLetter(index, letter).transform.position, 0.5f);
        Color temp = Color.white;
        temp.a = 0;
        //TextMeshProUGUI t = GetWordLetter(index, letter).GetComponent<TextMeshProUGUI>();
        var txtcharLetter = obj.GetComponent<TextMeshProUGUI>();
        txtcharLetter.DOColor(temp, 0.5f).OnComplete(() =>
        {
            GetWordLetter(index, letter).GetComponent<TextMeshProUGUI>().color = Color.white;
            GetWordLetter(index, letter).GetComponent<TextMeshProUGUI>().text = letter;
            AudioManager.Instance.Play("bling_02");
            Destroy(obj);
            // txtcharLetter.text = "";
        });
    }
    /// <summary>
    /// 获取下排单词字母位置
    /// </summary>
    /// <param name="index"></param>
    /// <param name="letter"></param>
    /// <returns></returns>
    private GameObject GetWordLetter(int index, string letter)
    {
        var go = ShowWordList[index];
        if (go)
        {
            return go.gameObject;
        }
        return null;
    }

    public void TweenBtnWord()
    {
        ShowWord.transform.DOScale(Vector3.one * 1.2f, 0.4f).SetLoops(2, LoopType.Yoyo);
    }

    /// <summary>
    ///  把下面的目标位置单词先注册进来
    /// </summary>
    private void InitShowWord()
    {
        ShowWordList.Clear();
        var list = ShowWord.transform.GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i].text != "-")
            {
                ShowWordList.Add(list[i].gameObject);
            }
        }
    }
}
