using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UISpinningPlate : UIBaseLevelGame
{
    #region 字段 属性 变量
    private GameObject goGuideHand;
    private Transform Plate { get { return GetT("ImagePlate"); } }
    private GameObject RedLight { get { return Get("ImageRedLight"); } }
    private TextMeshProUGUI TextBlue { get { return Get<TextMeshProUGUI>("TextBlue"); } }
    private TextMeshProUGUI TextYellow { get { return Get<TextMeshProUGUI>("TextYellow"); } }
    private TextMeshProUGUI TextGreen { get { return Get<TextMeshProUGUI>("TextGreen"); } }
    private Image ImageWord { get { return Get<Image>("ImageWord"); } }
    //private TextMeshProUGUI TextWord { get { return Get<TextMeshProUGUI>("TextWord"); } }
    private Button ButtonStop { get { return Get<Button>("ButtonStop"); } }
    private Tweener m_plateTween;
    private bool m_blueSoundPlay = false;
    private bool m_yellowSoundPlay = false;
    private bool m_greenSoundPlay = false;
    private bool isCanPlayRoatoAudio = false;//是否可以播放当前转圈的单词
    private int oldTrueWordIndex = 0;//上一个正确项所在的index
    private TextMeshProUGUI[] wordTexts = null;
    private TextMeshProUGUI[] WordTexts
    {
        get
        {
            if (wordTexts == null)
            {
                wordTexts = Plate.GetComponentsInChildren<TextMeshProUGUI>();
            }
            return wordTexts;

        }
    }
    #endregion

    public static UISpinningPlate Instance { get; set; }

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Instance = null;
        m_plateTween.Kill();
    }

    public override void Refresh()
    {
        ButtonStop.onClick.AddListener(() =>
        {
            m_plateTween.Pause();
            RedLight.SetActive(true);
            bool match = CheckMatch();
            if (match)
            {
                //TextWord.gameObject.SetActive(true);
                ImageWordTween().OnComplete((() =>
                {
                    //UITopBarBlack.Instance.AddOneStar();
                    var fly = FlyStar(true);
                    fly.OnComplete += () => PlayGame();
                }));
            }
            else
            {
                //UITopBarBlack.Instance.WrongStarFly();
                var fly = FlyStar(false);
                fly.m_trigger = true;
                fly.OnComplete += () =>
                {
                    m_plateTween.Play();
                    //ButtonStop.gameObject.SetActive(true);
                    RedLight.SetActive(false);
                };
            }
            //ButtonStop.gameObject.SetActive(false);
        });
        PlateRotate();

        UGUIEventListener.Get(ImageWord).onPointerClick += (c) =>
        {
            PlayCurrentAudio();
        };
    }

    public override void PlayGame()
    {
        if (goGuideHand != null)
        {
            Destroy(goGuideHand);
        }
        if (IsGameEnd)
            return;

        //ButtonStop.gameObject.SetActive(false);
        RedLight.SetActive(false);
        m_blueSoundPlay = false;
        m_greenSoundPlay = false;
        m_yellowSoundPlay = false;
        ImageWord.sprite = CurrentWord.sprite;
        //TextWord.gameObject.SetActive(true);
        //TextWord.text = CurrentWord.word;
        List<string> randomWords = GetThreeRandomWord();
        int randomWordsIndex = 0;
        int trueIndex = Random.Range(0, WordTexts.Length);
        while (trueIndex == oldTrueWordIndex)
        {
            trueIndex = Random.Range(0, WordTexts.Length);
        }
        for (int i = 0; i < WordTexts.Length; ++i)
        {
            if (trueIndex == i)
            {
                WordTexts[i].text = randomWords[0];
                oldTrueWordIndex = i;
            }
            else
                WordTexts[i].text = randomWords[++randomWordsIndex];
        }
        ImageWordTween().OnComplete(() =>
        {
            //TextWord.gameObject.SetActive(false);
            //开始转圈
            m_plateTween.Play();
            StartCoroutine(OpenPlayRoatoAudio(3f));//3s后开启右边的旋转发声

        });
        if (IsFirstPlay)
        {
            goGuideHand = CreateUIItem("GuideHandAnimationItem", ButtonStop.transform);
        }
    }

    private Tweener ImageWordTween()
    {
        asource = CurrentWord.PlaySound();
        Tweener tweenWord = ImageWord.transform.DOScale(1.1f, 0.5f);
        tweenWord.SetLoops(2, LoopType.Yoyo);
        return tweenWord;
    }

    private void PlateRotate()
    {
        Vector3 to = Vector3.zero;
        m_plateTween = DOTween.To(() => Vector3.zero, (v) => to = v, new Vector3(0, 0, 360f), 5f);
        m_plateTween.OnUpdate(() =>
        {
            Plate.transform.localEulerAngles = to;
            PlayOnceWordSound();
        });
        m_plateTween.onStepComplete = () =>
        {
            Debug.LogWarning("12323213213");
            //每次转完一圈之后重新刷新
            m_blueSoundPlay = false;
            m_greenSoundPlay = false;
            m_yellowSoundPlay = false;
        };
        m_plateTween.SetLoops(-1);
        m_plateTween.Pause();
    }

    private string GetSelectWord()
    {
        float z = Plate.localEulerAngles.z;
        if (z >= 0 && z <= 77 || z >= 317 && z <= 360)
        {
            return TextBlue.text;
        }
        else if (z >= 77 && z <= 197)
        {
            return TextGreen.text;
        }
        else
        {
            return TextYellow.text;
        }
    }

    private bool CheckMatch()
    {
        string word = GetSelectWord();
        if (CurrentWord.word.Equals(word))
        {
            if (goGuideHand != null)
            {
                Destroy(goGuideHand);
            }
        }
        return CurrentWord.word.Equals(word);
    }

    /// <summary>
    /// 第一个为正确答案
    /// </summary>
    private List<string> GetThreeRandomWord()
    {
        string word1 = CurrentWord.word;
        string word2 = "";
        string word3 = "";
        while (word1 == word2 || word1 == word3 || word2 == word3)
        {
            int randomIndex1 = Random.Range(0, m_randomWordList.Count);
            word2 = m_randomWordList[randomIndex1].word;
            int randomIndex2 = Random.Range(0, m_randomWordList.Count);
            word3 = m_randomWordList[randomIndex2].word;
        }
        List<string> words = new List<string>();
        words.Add(word1);
        words.Add(word2);
        words.Add(word3);
        return words;
        //return Utility.RandomSortList(words);
    }

    private void PlayOnceWordSound()
    {
        string word = GetSelectWord();

        PlayOnceWordSoundInternal(TextBlue, word, ref m_blueSoundPlay);
        PlayOnceWordSoundInternal(TextGreen, word, ref m_greenSoundPlay);
        PlayOnceWordSoundInternal(TextYellow, word, ref m_yellowSoundPlay);
    }

    private void PlayOnceWordSoundInternal(TextMeshProUGUI text, string word, ref bool bPlay)
    {
        if (text.text.Equals(word))
        {
            if (!bPlay)
            {
                bPlay = true;
                Tweener tween = text.transform.DOScale(1.4f, 0.5f).SetLoops(2, LoopType.Yoyo);
                tween.OnComplete(() =>
                {
                    if (m_blueSoundPlay && m_greenSoundPlay && m_yellowSoundPlay)
                    {
                        //ButtonStop.gameObject.SetActive(true);
                    }
                });
                if (isCanPlayRoatoAudio)
                    AudioManager.Instance.Play(word);
            }
        }
    }

    private AudioSource asource = null;
    private bool isClick = false;
    /// <summary>
    /// 播放当前语音
    /// </summary>
    private void PlayCurrentAudio()
    {
        if (!isClick)
        {
            isClick = true;
            StopAllCoroutines();
            isCanPlayRoatoAudio = false;
            ImageWord.transform.localScale = Vector3.one;

            asource = CurrentWord.PlaySound();
            ImageWord.transform.DOScale(1.1f, 0.5f).SetLoops(2, LoopType.Yoyo).onComplete = () =>
            {
                isClick = false;
                StartCoroutine(OpenPlayRoatoAudio(3f));//3s后开启右边的旋转发声
            };
        }
    }

    /// <summary>
    /// 几秒后开启右边的旋转可以发声的方法
    /// </summary>
    private IEnumerator OpenPlayRoatoAudio(float time)
    {
        yield return new WaitForSeconds(time);
        isCanPlayRoatoAudio = true;
    }
}
