using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
public class UIDuckHunter : UIBaseLevelGame
{
    private string Current_Empty;
    private RectTransform ImageOn1 { get { return Get<RectTransform>("ImageOn1"); } }
    private RectTransform ImageOn2 { get { return Get<RectTransform>("ImageOn2"); } }
    private RectTransform ImageDuckParent { get { return Get<RectTransform>("ImageDuckParent"); } }
    private TextMeshProUGUI TextWord1 { get { return Get<TextMeshProUGUI>("TextWord1"); } }
    private TextMeshProUGUI TextDuck1 { get { return Get<TextMeshProUGUI>("TextDuck1"); } }
    private TextMeshProUGUI TextDuck2 { get { return Get<TextMeshProUGUI>("TextDuck2"); } }
    private TextMeshProUGUI TextDuck3 { get { return Get<TextMeshProUGUI>("TextDuck3"); } }
    private TextMeshProUGUI TextDuck4 { get { return Get<TextMeshProUGUI>("TextDuck4"); } }
    private TextMeshProUGUI TextDuck5 { get { return Get<TextMeshProUGUI>("TextDuck5"); } }
    private TextMeshProUGUI TextDuck6 { get { return Get<TextMeshProUGUI>("TextDuck6"); } }
    private TextMeshProUGUI TextDuck7 { get { return Get<TextMeshProUGUI>("TextDuck7"); } }
    private TextMeshProUGUI TextDuckAudio1 { get { return Get<TextMeshProUGUI>("TextDuckAudio1"); } }
    private TextMeshProUGUI TextDuckAudio2 { get { return Get<TextMeshProUGUI>("TextDuckAudio2"); } }
    private TextMeshProUGUI TextDuckAudio3 { get { return Get<TextMeshProUGUI>("TextDuckAudio3"); } }
    private TextMeshProUGUI TextDuckAudio4 { get { return Get<TextMeshProUGUI>("TextDuckAudio4"); } }
    private TextMeshProUGUI TextDuckAudio5 { get { return Get<TextMeshProUGUI>("TextDuckAudio5"); } }
    private TextMeshProUGUI TextDuckAudio6 { get { return Get<TextMeshProUGUI>("TextDuckAudio6"); } }
    private TextMeshProUGUI TextDuckAudio7 { get { return Get<TextMeshProUGUI>("TextDuckAudio7"); } }
    private Image ImageWord1 { get { return Get<Image>("ImageWord1"); } }
    private Image ImageLaba { get { return Get<Image>("ImageLaba"); } }
    private List<TextMeshProUGUI> ducklist;
    private List<TextMeshProUGUI> ducklist_wrong;
    public override void Refresh()
    {

        //ImageOn1.DOLocalRotate(new Vector3(0, 0, -180), 10f).SetLoops(-1, LoopType.Incremental);
        //ImageOn2.DOLocalRotate(new Vector3(0, 0, -180), 7f).SetLoops(-1, LoopType.Incremental);
        UGUIEventListener.Get(TextWord1.transform.parent).onPointerClick = BtnAudio;

        ducklist = new List<TextMeshProUGUI>();
        ducklist_wrong = new List<TextMeshProUGUI>();
        ducklist.Add(TextDuckAudio1);
        ducklist.Add(TextDuckAudio2);
        ducklist.Add(TextDuckAudio3);
        ducklist.Add(TextDuckAudio4);
        ducklist.Add(TextDuckAudio5);
        ducklist.Add(TextDuckAudio6);
        ducklist.Add(TextDuckAudio7);
        ducklist_wrong.Add(TextDuck1);
        ducklist_wrong.Add(TextDuck2);
        ducklist_wrong.Add(TextDuck3);
        ducklist_wrong.Add(TextDuck4);
        ducklist_wrong.Add(TextDuck5);
        ducklist_wrong.Add(TextDuck6);
        ducklist_wrong.Add(TextDuck7);
        StartCoroutine(UpdateFrame());

        for (int i = 0; i < ducklist.Count; i++)
        {
            UGUIEventListener.Get(ducklist[i].transform.parent).onPointerClick = BtnClick;
        }
    }

    private bool isRight = true;
    private void BtnClick(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (isRight)
        {
            isRight = false;

            CheckMatch(arg0.pointerEnter.GetComponentsInChildren<TextMeshProUGUI>(true));

            ImageDuckParent.DOPause();
        }
    }

    private bool isplay = true;
    private bool isRotate = false;
    IEnumerator UpdateFrame()
    {

        while (true)
        {
            if (isRotate)
            {
                if (TextDuck1.transform.position.y > 1.0f && TextDuck1.transform.position.y < 1.1f && TextDuck1.transform.position.x > 0f)
                {
                    if (isplay)
                    {
                        yield return PlayTextAudio(TextDuck1, TextDuckAudio1.text);
                        // StartCoroutine(PlayTextAudio(TextDuck1, TextDuckAudio1.text));
                    }
                }
                if (TextDuck2.transform.position.y > 1.0f && TextDuck2.transform.position.y < 1.1f && TextDuck2.transform.position.x > 0f)
                {
                    if (isplay)
                    {
                        yield return PlayTextAudio(TextDuck2, TextDuckAudio2.text);
                        //StartCoroutine(PlayTextAudio(TextDuck2, TextDuckAudio2.text));
                    }
                }
                if (TextDuck3.transform.position.y > 1.0f && TextDuck3.transform.position.y < 1.1f && TextDuck3.transform.position.x > 0f)
                {
                    if (isplay)
                        yield return PlayTextAudio(TextDuck3, TextDuckAudio3.text);
                }
                if (TextDuck4.transform.position.y > 1.0f && TextDuck4.transform.position.y < 1.1f && TextDuck4.transform.position.x > 0f)
                {
                    if (isplay)
                        yield return PlayTextAudio(TextDuck4, TextDuckAudio4.text);
                }
                if (TextDuck5.transform.position.y > 1.0f && TextDuck5.transform.position.y < 1.1f && TextDuck5.transform.position.x > 0f)
                {
                    if (isplay)
                        yield return PlayTextAudio(TextDuck5, TextDuckAudio5.text);
                }
                if (TextDuck6.transform.position.y > 1.0f && TextDuck6.transform.position.y < 1.1f && TextDuck6.transform.position.x > 0f)
                {
                    if (isplay)
                        yield return PlayTextAudio(TextDuck6, TextDuckAudio6.text);
                }
                if (TextDuck7.transform.position.y > 1.0f && TextDuck7.transform.position.y < 1.1f && TextDuck7.transform.position.x > 0f)
                {
                    if (isplay)
                        yield return PlayTextAudio(TextDuck7, TextDuckAudio7.text);
                }
            }
            yield return 0;
            //yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator PlayTextAudio(TextMeshProUGUI text, string audio)
    {
        isplay = false;
        var asource = AudioManager.Instance.Play(audio);
        text.transform.DOScale(1.5f, 0.3f).SetLoops(2, LoopType.Yoyo);
        yield return new WaitForSeconds(asource.clip.length);
        yield return 0;
        isplay = true;
    }
    private void BtnAudio(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (isPlaying)
        {
            isPlaying = false;
            StartCoroutine(Playing());
        }
    }

    private bool isPlaying = true;
    IEnumerator Playing()
    {
        ImageLaba.sprite = GetS("UIDuckHunterlaba2");
        ImageLaba.SetNativeSize();
        var length = AudioManager.Instance.Play(CurrentPronunciationRealWord).clip.length;
        yield return new WaitForSeconds(length);
        ImageLaba.sprite = GetS("UIDuckHunterlaba1");
        ImageLaba.SetNativeSize();
        isPlaying = true;
    }

    private bool isFirst = true;
    public override void PlayGame()
    {
        if (IsGameEnd)
            return;

        switch (StartData.dataType)
        {
            case DataType.Word:
                //单词模式
                ImageWord1.sprite = CurrentWord.sprite;
                break;
            case DataType.Pronunciation:
                //音标模式
                Sprite tempSp = null;
                tempSp = ConfigWordLibraryModel.GetSpriteByWord(CurrentPronunciationRealWord);
                if (tempSp == null)
                {
                    tempSp = CurrentPronunciation.sprite;
                }
                ImageWord1.sprite = tempSp;
                break;
        }



        ImageWord1.transform.DOScale(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo).OnComplete(delegate
        {
            if (isFirst)
            {
                isFirst = false;
                isRotate = true;
                ImageDuckParent.DOLocalRotate(new Vector3(0, 0, 180), 8.5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental);
            }
            else
            {
                ImageDuckParent.DOPlay();
            }
        });

        for (int i = 0; i < m_originalPronunciationList.Count; i++)
        {
            if (CurrentPronunciationIndex == null)
            {
                if (CurrentPronunciationRealWord.Contains(m_originalPronunciationList[i].Split('-')[0]))
                    Current_Empty = m_originalPronunciationList[i].Split('-')[0];//用到的是哪个读音  空缺部分
            }
            else
            {
                Current_Empty = CurrentPronunciationIndex;
            }
        }

        switch (Current_Empty.Length)
        {
            case 1:
                TextWord1.text = CurrentPronunciationRealWord.Replace(Current_Empty, "_");
                break;
            case 2:
                TextWord1.text = CurrentPronunciationRealWord.Replace(Current_Empty, "_ _");
                break;
            case 3:
                TextWord1.text = CurrentPronunciationRealWord.Replace(Current_Empty, "_ _ _");
                break;
            case 4:
                TextWord1.text = CurrentPronunciationRealWord.Replace(Current_Empty, "_ _ _ _");
                break;
        }

        if (CurrentPronunciationWord.Contains("-"))
        {
            Current_Empty = Current_Empty + "-" + CurrentPronunciationWord.Split('-')[1];
        }

        isPlaying = false;
        StartCoroutine(Playing());

        int index = 0;
        for (int i = 0; i < ducklist.Count; i++)
        {
            ducklist[i].text = m_originalPronunciationList[index];
            string s = m_originalPronunciationList[index];
            ducklist_wrong[i].text = s.Split('-')[0];
            index++;
            if (index >= m_originalPronunciationList.Count)
                index = 0;
        }
    }

    public void CheckMatch(TextMeshProUGUI[] textWord)
    {
        textWord[0].transform.parent.GetComponent<Image>().sprite = GetS("UIDuckHunterDuck1");
        textWord[0].rectTransform.anchoredPosition = new Vector2(0, -80);

        var oldLocalEuler = textWord[0].rectTransform.localEulerAngles;
        oldLocalEuler.x = 65;
        textWord[0].rectTransform.localEulerAngles = oldLocalEuler;

        Last_word = textWord[0].transform;

        if (textWord[1].text == Current_Empty)
        {
            Last_parent = textWord[0].transform.parent;

            //            transform.SetParent(textWord[0].transform.parent);
            //            transform.SetParent(textWord[0].transform);
            textWord[0].transform.SetParent(gameObject.transform);
            textWord[0].transform.SetAsLastSibling();

            textWord[0].transform.DOMove(TextWord1.transform.position, 1).OnComplete(delegate
            {
                textWord[0].gameObject.SetActive(false);
                StartCoroutine(FlyTrue());
            });
        }
        else
        {
            StartCoroutine(FlyFlase());
        }
    }

    private Transform Last_parent;
    private Transform Last_word;
    IEnumerator FlyTrue()
    {
        TextWord1.text = CurrentPronunciationRealWord;

        var length = AudioManager.Instance.Play(CurrentPronunciationRealWord).clip.length;
        yield return new WaitForSeconds(length);

        UITopBarStarFly fly = FlyStar(true, false);
        fly.OnComplete += () =>
        {
            isRight = true;
            //            Last_parent.SetParent(Last_word.parent);
            Last_word.SetParent(Last_parent);
            Last_word.SetAsFirstSibling();

            var lastRectTransform = Last_word.GetComponent<RectTransform>();
            lastRectTransform.anchoredPosition = new Vector2(10, -70);
            lastRectTransform.parent.GetComponent<Image>().sprite = GetS("UIDuckHunterDuck");
            var oldLocalEuler = lastRectTransform.localEulerAngles;
            oldLocalEuler.x = 0;
            lastRectTransform.localEulerAngles = oldLocalEuler;

            Last_word.gameObject.SetActive(true);
            PlayGame();
        };
    }

    IEnumerator FlyFlase()
    {
        FlyStar(false, false);
        yield return new WaitForSeconds(0.5f);
        isRight = true;
        var lastRectTransform = Last_word.GetComponent<RectTransform>();

        lastRectTransform.anchoredPosition = new Vector2(10, -70);
        lastRectTransform.parent.GetComponent<Image>().sprite = GetS("UIDuckHunterDuck");
        var oldLocalEuler = lastRectTransform.localEulerAngles;
        oldLocalEuler.x = 0;
        lastRectTransform.localEulerAngles = oldLocalEuler;
        ImageDuckParent.DOPlay();
    }

    #region 2018年4月16日10:38:52 qiubin修改当前游戏以适应单词，音标两种模式

    #endregion

}