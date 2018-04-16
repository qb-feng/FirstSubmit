using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class TurnCardGameControllerUGUI : MonoBehaviour
{
    public bool bAgain = false;

    public Sprite[] spriteCardsBack;
    private Sprite[] spriteCardsFront;
    public Sprite[] spriteCardsFrontBg;
    public Sprite spriteCardShadow;
    private string[] audioCardsA;
    private string[] audioCardsB;
    private List<ConfigWordLibraryModel> wordList;
    private List<CardProperties> CardList;
    private UITurnCardGame tempParent;

    /// <summary>
    /// How fast to uncover a card, higher values = faster
    /// </summary>
    [Range(1f, 16f)]
    public float uncoverTime = 4f;

    /// <summary>
    /// how fast to deal one card, higher values = faster
    /// </summary>
    [Range(1f, 16f)]
    public float dealTime = 4f;

    [Range(0.1f, 10f)]
    public float checkPairTime = 0.5f;

    /// <summary>
    /// The Padding between 2 Cards
    /// </summary>
    [Range(2, 32)]
    public int cardsPadding = 4;

    [Range(2, 8)]
    public int pairCount = 4;

    /// <summary>
    /// Create a fake shadow?
    /// </summary>
    public bool shadow = true;
    [Range(-32, 32)]
    public float shadowOffsetX = 4;
    [Range(-32, 32)]
    public float shadowOffsetY = -4;

    float shadOffsetX;
    float shadOffsetY;

    int chosenCardsBack = 0;
    int[] chosenCardsFront;

    Vector3 dealingStartPosition = new Vector3(-12800, -12800, -8);

    // move counters
    int totalMoves = 0;
    int bestMoves = 0;
    int uncoveredCards = 0;
    Transform[] selectedCards = new Transform[2];

    //bool memorySolved = false;

    int oldPairCount;

    // Input check
    bool isTouching = false;
    bool isUncovering = false;
    bool isDealing = false;

    // Soundeffects
    public AudioClip soundDealCard;
    //public AudioClip soundButtonClick;
    public AudioClip soundUncoverCard;
    public AudioClip soundFoundPair;
    public AudioClip soundNoPair;

    public delegate void PairFinishAction();
    public event PairFinishAction PairFinishActionEvent;

    private bool m_click = false;

    // Use this for initialization
    void Start()
    {
        //CreateDeck();
        tempParent = GetComponent<UITurnCardGame>();        
    } 

    public void SetCardFront(List<ConfigWordLibraryModel> word)
    {
        spriteCardsFront = new Sprite[word.Count];
        audioCardsA = new string[word.Count];
        audioCardsB = new string[word.Count];
        wordList = new List<ConfigWordLibraryModel>();
        for (int i = 0; i < word.Count; i++)
        {
            spriteCardsFront[i] = word[i].sprite;
            audioCardsA[i] = word[i].word;
            audioCardsB[i] = word[i].word;
            wordList.Add(word[i]);
        }
    }

    void OnGUI()
    {
        GUI.BeginGroup(new Rect(0, 0, Screen.width, 50));
        {
            if (pairCount != oldPairCount)
            {
                oldPairCount = pairCount;

                bestMoves = PlayerPrefs.GetInt("Memory_" + pairCount + "_Pairs", 0);
            }
            //GUIStyle style = new GUIStyle();
            //style.fontSize = 20;
            //GUI.Label(new Rect(100, 0, 128, 32), "移动次数: " + totalMoves, style);
            //GUI.Label(new Rect(300, 0, 128, 32), "最佳次数: " + bestMoves, style);
        }
        GUI.EndGroup();
    }

    public void Clear()
    {
        StopAllCoroutines();
        DestroyImmediate(GameObject.Find("DeckParent"));
        DestroyImmediate(GameObject.Find("Temp"));
    }

    void CreateDeck()
    {
        CardList = new List<CardProperties>();

        isDealing = true;

        // clear the game field and reset variables
        Clear();
        selectedCards = new Transform[2];
        totalMoves = 0;
        uncoveredCards = 0;
        //memorySolved = false;

        // get personal best for this board size
        bestMoves = PlayerPrefs.GetInt("Memory_" + pairCount + "_Pairs", 0);

        // randomly chose the back graphic of the cards
        chosenCardsBack = Random.Range(0, spriteCardsBack.Length);

        // randomly chose the card motives to play with
        List<int> tmp = new List<int>();
        for (int i = 0; i < spriteCardsFront.Length; i++)
        {
            tmp.Add(i);
        }
        tmp.Shuffle();
        chosenCardsFront = tmp.GetRange(0, pairCount).ToArray();

        GameObject deckParent = new GameObject("DeckParent"); // this holds all the cards
        deckParent.layer = 8;
        //deckParent.transform.SetParent(UITurnCardGame.Instance.GoContainerCard.transform);
        deckParent.transform.SetParent(tempParent.GoContainerCard.transform);
        deckParent.transform.ResetLocal();
        GameObject temp = new GameObject("Temp");
        //temp.transform.SetParent(UITurnCardGame.Instance.GoContainerCard.transform);
        temp.transform.SetParent(tempParent.GoContainerCard.transform);
        temp.transform.ResetLocal();

        int cur = 0;

        float minX = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float minY = Mathf.Infinity;
        float maxY = Mathf.NegativeInfinity;

        // calculate columns and rows needed for the selected pair count
        int cCards = pairCount * 2;
        //int cols = (int)Mathf.Sqrt(cCards);
        int cols = 2;
        //int rows = (int)Mathf.Ceil(cCards / (float)cols);
        int rows = pairCount;

        List<int> deck = new List<int>();
        for (int i = 0; i < pairCount; i++)
        {
            deck.AddRange(new int[] { i, i });
        }
        deck.Shuffle();

        int cardWidth = 0;
        int cardHeight = 0;

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                if (cur > cCards - 1)
                    break;

                // Create the Card
                GameObject card = new GameObject("Card"); // parent object
                card.layer = 8;
                GameObject cardFront = new GameObject("CardFront");

                cardFront.layer = 8;
                GameObject cardMiddle = new GameObject("CardMiddle");
                cardMiddle.layer = 8;
                GameObject cardBack = new GameObject("CardBack");
                cardBack.layer = 8;
                GameObject destination = new GameObject("Destination");

                cardFront.transform.SetParent(card.transform); // make front child of card
                cardMiddle.transform.SetParent(card.transform);
                cardBack.transform.SetParent(card.transform); // make back child of card

                // front (motive)
                InitCardImage(cardFront, spriteCardsFront[chosenCardsFront[deck[cur]]], 200);

                // middle
                InitCardImage(cardMiddle, spriteCardsFrontBg[0]);

                // back
                InitCardImage(cardBack, spriteCardsBack[chosenCardsBack]);

                cardWidth = (int)spriteCardsBack[chosenCardsBack].rect.width;
                cardHeight = (int)spriteCardsBack[chosenCardsBack].rect.height;

                // now add the Card GameObject to the Deck GameObject "deckParent"
                card.tag = "Card";
                card.transform.SetParent(deckParent.transform);
                card.transform.localScale = Vector3.one;
                card.transform.localPosition = dealingStartPosition;
                card.transform.SetAsLastSibling();
                card.AddComponent<BoxCollider>();
                card.GetComponent<BoxCollider>().size = new Vector2(cardWidth, cardHeight);
                card.AddComponent<CardProperties>().Pair = deck[cur];
                card.GetComponent<CardProperties>().SoundA = audioCardsA[chosenCardsFront[deck[cur]]];
                card.GetComponent<CardProperties>().SoundB = audioCardsB[chosenCardsFront[deck[cur]]];
                card.GetComponent<CardProperties>().Word = wordList[chosenCardsFront[deck[cur]]];
                CardList.Add(card.GetComponent<CardProperties>());

                destination.transform.SetParent(temp.transform);
                destination.transform.localScale = Vector3.one;
                destination.tag = "Destination";
                float tempWidth = ((pairCount - 1) * (cardWidth + cardsPadding)) / 2;
                float tempHeight = (cardHeight + cardsPadding) / 2;
                Vector3 startV3 = new Vector3(-tempWidth, tempHeight, 0);
                Vector3 offestCardV3 = new Vector3(x * (cardWidth + cardsPadding), -y * (cardHeight + cardsPadding));
                destination.transform.localPosition = startV3 + offestCardV3;
                //destination.transform.localPosition = new Vector3(x * (cardWidth + cardsPadding), y * (cardHeight + cardsPadding));

                if (shadow)
                {
                    GameObject cardShadow = new GameObject("CardShadow");
                    cardShadow.layer = 8;
                    cardShadow.tag = "CardShadow";
                    cardShadow.transform.SetParent(deckParent.transform);
                    cardShadow.transform.localScale = Vector3.one;
                    cardShadow.transform.localPosition = dealingStartPosition;
                    Image cardShadowImage = cardShadow.AddComponent<Image>();
                    cardShadowImage.sprite = spriteCardShadow;
                    cardShadowImage.transform.SetAsFirstSibling();
                    cardShadowImage.SetNativeSize();
                }
                cur++;

                // determine positions for the camera helper objects
                Vector3 pos = destination.transform.localPosition;
                minX = Mathf.Min(minX, pos.x - cardWidth);
                minY = Mathf.Min(minY, pos.y - cardHeight);
                maxX = Mathf.Max(maxX, pos.x + cardWidth + shadowOffsetX);
                maxY = Mathf.Max(maxY, pos.y + cardHeight + shadowOffsetY);
            }
        }

        //scale to fit onto the "table"
        //float tableScale = (GameObject.Find("Table") == null) ? 1f : GameObject.Find("Table").transform.localScale.x;
        //float scale = (pairCount < 3) ? GameObject.Find("Table").transform.localScale.y / (maxX + cardsPadding) : tableScale / (maxX + cardsPadding);

        //Vector2 point = LineIntersectionPoint(
        //    new Vector2(minX, maxY),
        //    new Vector2(maxX, minY),
        //    new Vector2(minX, minY),
        //    new Vector2(maxX, maxY)
        //    );

        //temp.transform.localPosition -= new Vector3(point.x * scale, point.y * scale);

        //shadOffsetX = shadowOffsetX * scale;
        //shadOffsetY = shadowOffsetY * scale;

        //deckParent.transform.localScale = new Vector3(scale, scale, scale);
        //temp.transform.localScale = new Vector3(scale, scale, scale);

        shadOffsetX = shadowOffsetX;
        shadOffsetY = shadowOffsetY;

        DealCards();
    }

    private void InitCardImage(GameObject cardPart, Sprite sprite, int length = 256)
    {
        Image cardImage = cardPart.AddComponent<Image>();
        //增加一行旋转代码,是的正面展示的的sprite与设计时保持一致
        cardPart.transform.rotation = new Quaternion(0, 180, 0, 0);

        cardImage.sprite = sprite;
        cardImage.transform.SetAsLastSibling();
        cardImage.preserveAspect = true;
        RectTransform rtBack = cardImage.transform as RectTransform;
        rtBack.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, length);
        rtBack.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, length);
    }

    void DealCards()
    {
        StartCoroutine(dealCards());
    }

    IEnumerator dealCards()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
        GameObject[] cardsShadow = GameObject.FindGameObjectsWithTag("CardShadow");
        GameObject[] destinations = GameObject.FindGameObjectsWithTag("Destination");

        for (int i = 0; i < cards.Length; i++)
        {
            float t = 0;

            if (soundDealCard != null)
                AudioManager.Instance.Play(soundDealCard);

            while (t < 1f)
            {
                t += Time.deltaTime * dealTime;

                cards[i].transform.localPosition = Vector3.Lerp(
                    dealingStartPosition, destinations[i].transform.localPosition, t);

                if (cardsShadow.Length > 0)
                {
                    cardsShadow[i].transform.localPosition = Vector3.Lerp(
                        dealingStartPosition,
                        destinations[i].transform.localPosition + new Vector3(shadOffsetX, shadOffsetY), t);
                }

                yield return null;
            }
            yield return null;
        }

        isDealing = false;

        foreach (var item in cards)
        {
            StartCoroutine(uncoverCard(item.transform, true));
            item.GetComponent<BoxCollider>().enabled = false;
        }

        #region qiubin修改 - 增加对当前单词的读音和放大效果~~~~~~~~~~~~~~~~~~~~~~~~
        yield return new WaitForSeconds(1f);
        List<ConfigWordLibraryModel> allWord = new List<ConfigWordLibraryModel>();//所有单词
        Dictionary<int, ConfigWordLibraryModel> allCards = new Dictionary<int, ConfigWordLibraryModel>();
        for (int i = 0; i < cards.Length; ++i) 
        {
            var v = cards[i].GetComponent<CardProperties>().Word;
            if(!allWord.Contains(v))
                allWord.Add(v);
            allCards.Add(i, v);
        }
        //读当前所有单词
        AudioSource asource = null;
        float wordTime = 0;
        for (int i = 0; i < allWord.Count; ++i) 
        {
            asource = allWord[i].PlaySound();
            wordTime = asource.clip.length + 1.5f;
            //放大所有该单词图片
            foreach (var v in allCards)
            {
                if (v.Value == allWord[i]) 
                {
                    cards[v.Key].transform.DOScale(1.1f, wordTime * 0.5f).SetLoops(2, LoopType.Yoyo);
                }
            }
            yield return new WaitForSeconds(wordTime);
        }      
        #endregion
        yield return new WaitForSeconds(1f);

        foreach (var item in cards)
        {
            StartCoroutine(uncoverCard(item.transform, false));
            item.GetComponent<BoxCollider>().enabled = true;
        }
        yield return new WaitForSeconds(0.2f);
//         if (UITurnCardGame.Instance.GetIsFirstPlay())
//         {
//             UITurnCardGame.Instance.CreatUI(CardList[0].transform);
//         }
        if (tempParent.GetIsFirstPlay())
        {
            tempParent.CreatUI(CardList[0].transform);
        }
        yield return 0;
    }

    void UncoverCard(Transform card)
    {
        m_click = true;
        StartCoroutine(uncoverCard(card, true));
    }

    IEnumerator uncoverCard(Transform card, bool uncover)
    {
        isUncovering = true;

        float minAngle = uncover ? 0 : 180;
        float maxAngle = uncover ? 180 : 0;

        float t = 0;
        bool uncovered = false;

        if (soundUncoverCard != null)
            AudioManager.Instance.Play(soundUncoverCard);

        // find the shadow for the selected card
        var shadows = GameObject.FindGameObjectsWithTag("CardShadow");
        GameObject shadow = null;
        foreach (var item in shadows)
        {
            if (item.transform.localPosition == card.localPosition + new Vector3(shadOffsetX, shadOffsetY))
            {
                shadow = item;
            }
        }

        while (t < 1f)
        {
            t += Time.deltaTime * uncoverTime;

            float angle = Mathf.LerpAngle(minAngle, maxAngle, t);
            card.eulerAngles = new Vector3(0, angle, 0);

            if (shadow != null)
                shadow.transform.eulerAngles = new Vector3(0, angle, 0);

            if (((angle >= 90 && angle < 180) ||
                  (angle >= 270 && angle < 360)) && !uncovered)
            {
                uncovered = true;
                for (int i = 0; i < card.childCount; i++)
                {
                    // reverse sorting order to show the otherside of the card
                    // otherwise you would still see the same sprite because they are sorted 
                    // by order not distance (by default)
                    Transform c = card.GetChild(i);
                    c.SetAsFirstSibling();
                    yield return null;
                }
            }

            yield return null;
        }

        // check if we uncovered 2 cards
        if (uncoveredCards == 2)
        {
            // if so compare the cards
            if (selectedCards[0].GetComponent<CardProperties>().Pair !=
                selectedCards[1].GetComponent<CardProperties>().Pair)
            {
                if (m_click)
                {
                    m_click = false;
                    List<string> ids = new List<string>();
                    ids.Add(selectedCards[0].GetComponent<CardProperties>().Word.id);
                    ids.Add(selectedCards[1].GetComponent<CardProperties>().Word.id);
                    //UITurnCardGame.Instance.FlyStar(ids, false);
                    tempParent.FlyStar(ids, false);
                }
                if (soundNoPair != null)
                    AudioManager.Instance.Play(soundNoPair);
                // if they are not equal cover back
                yield return new WaitForSeconds(checkPairTime);
                StartCoroutine(uncoverCard(selectedCards[0], false));
                StartCoroutine(uncoverCard(selectedCards[1], false));

                if (selectedCards[0].transform.localRotation.y < 3 && selectedCards[1].transform.localRotation.y < 3)
                {
                    StartCoroutine(FiveSeconds());
                    selectedCards[0].GetComponent<BoxCollider>().enabled = true;
                    selectedCards[1].GetComponent<BoxCollider>().enabled = true;
                }
            }
            else
            {
                if (soundFoundPair != null)
                    AudioManager.Instance.Play(soundFoundPair);

                // set as solved
                selectedCards[0].GetComponent<CardProperties>().Solved = true;
                selectedCards[1].GetComponent<CardProperties>().Solved = true;
                //CreateTextWord(selectedCards[0]);
                //CreateTextWord(selectedCards[1]);
                isfiveseconds = true;
            }
            selectedCards[0].GetComponent<CardProperties>().Selected = false;
            selectedCards[1].GetComponent<CardProperties>().Selected = false;
            totalMoves++;
            uncoveredCards = 0;
            yield return new WaitForSeconds(0.1f);
        }

        // check if the memory is solved
        if (IsSolved())
        {
            int score = PlayerPrefs.GetInt("Memory_" + pairCount + "_Pairs", 0);

            if (score > totalMoves || score == 0)
            {
                bestMoves = totalMoves;
            }
            PlayerPrefs.SetInt("Memory_" + pairCount + "_Pairs", bestMoves);

            //memorySolved = true;
        }

        isUncovering = false;
        yield return 0;
    }

    bool IsSolved()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Card"))
        {
            if (!g.GetComponent<CardProperties>().Solved)
            {
                return false;
            }
        }
        Debug.Log("complete!");
        if (PairFinishActionEvent != null)
        {
            PairFinishActionEvent();
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
//         UITurnCardGame.Instance.LabelTotalMoves.text = string.Format(Localization.Get("turn_card_game_key1"), totalMoves.ToString());
//         UITurnCardGame.Instance.LabelBestMoves.text = string.Format(Localization.Get("turn_card_game_key2"), bestMoves.ToString());

        if (bAgain)
        {
            bAgain = false;
            CreateDeck();
        }

        if (isDealing)
            return;

        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !isTouching && !isUncovering && uncoveredCards < 2)
        {
            if (!isfiveseconds) return;
            //            isTouching = true;
            Ray ray = UIManager.Instance.WorldCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            // we hit a card
            if (hit.collider != null)
            {
                if (!hit.collider.GetComponent<CardProperties>().Selected)
                {
                    // and its not one of the already solved ones
                    if (!hit.collider.GetComponent<CardProperties>().Solved)
                    {
                        // uncover it
                        UncoverCard(hit.collider.transform);
                        selectedCards[uncoveredCards] = hit.collider.transform;
                        selectedCards[uncoveredCards].GetComponent<CardProperties>().Selected = true;
                        selectedCards[uncoveredCards].GetComponent<BoxCollider>().enabled = false;
                        //if(soundUncoverCard != null)
                        //GetComponent<AudioSource>().PlayOneShot(soundUncoverCard);
                        uncoveredCards += 1;
                        if (uncoveredCards == 1)
                        {
                            if (hit.collider.GetComponent<CardProperties>().SoundA != null)
                                AudioManager.Instance.Play(hit.collider.GetComponent<CardProperties>().SoundA);
                            //if (UITurnCardGame.Instance.GetIsFirstPlay())
                            if (tempParent.GetIsFirstPlay())
                            {
                                //if (UITurnCardGame.Instance.goGuideHand != null)
                                if (tempParent.goGuideHand != null)
                                {
                                    for (int i = 1; i < CardList.Count; i++)
                                    {
                                        if (CardList[i].Word == CardList[0].Word)
                                        {
//                                             Destroy(UITurnCardGame.Instance.goGuideHand);
//                                             UITurnCardGame.Instance.CreatUI(CardList[i].transform);
                                            Destroy(tempParent.goGuideHand);
                                            tempParent.CreatUI(CardList[i].transform);

                                            CardList.Remove(CardList[i]);
                                            CardList.Remove(CardList[0]);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //if (UITurnCardGame.Instance.GetIsFirstPlay())
                            if (tempParent.GetIsFirstPlay())
                            {
                                //if (UITurnCardGame.Instance.goGuideHand != null)
                                if (tempParent.goGuideHand != null)
                                {
                                    //Destroy(UITurnCardGame.Instance.goGuideHand);
                                    Destroy(tempParent.goGuideHand);
                                    if (CardList.Count > 0)
                                        //UITurnCardGame.Instance.CreatUI(CardList[0].transform);
                                        tempParent.CreatUI(CardList[0].transform);
                                }
                            }
                            isfiveseconds = false;
                            if (hit.collider.GetComponent<CardProperties>().SoundB != null)
                                AudioManager.Instance.Play(hit.collider.GetComponent<CardProperties>().SoundB);
                        }
                    }
                }
            }
        }
        else
        {
            //            isTouching = false;
        }

        //2017年2月7日17:43:11 白建新
        //强制引导,其他卡片不让点击
        if (tempParent.goGuideHand)
        {
            //GameObject goCard = UITurnCardGame.Instance.goGuideHand.transform.parent.gameObject;
            GameObject goCard = tempParent.goGuideHand.transform.parent.gameObject;
            if (goCard)
            {
                GameObject[] cards = GameObject.FindGameObjectsWithTag("Card");
                foreach (var item in cards)
                {
                    item.GetComponent<BoxCollider>().enabled = false;
                }
                goCard.GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
    {
        // Get A,B,C of first line - points : ps1 to pe1
        float A1 = pe1.y - ps1.y;
        float B1 = ps1.x - pe1.x;
        float C1 = A1 * ps1.x + B1 * ps1.y;

        // Get A,B,C of second line - points : ps2 to pe2
        float A2 = pe2.y - ps2.y;
        float B2 = ps2.x - pe2.x;
        float C2 = A2 * ps2.x + B2 * ps2.y;

        // Get delta and check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
            return new Vector2();

        // now return the Vector2 intersection point
        return new Vector2(
            (B2 * C1 - B1 * C2) / delta,
            (A1 * C2 - A2 * C1) / delta
            );
    }

    private bool isfiveseconds = true;

    IEnumerator FiveSeconds()
    {
        isfiveseconds = false;
        yield return new WaitForSeconds(0.4f);
        isfiveseconds = true;
    }
}
