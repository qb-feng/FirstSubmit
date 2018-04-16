using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIRevolvingRestaurant : UIBaseLevelGame
{
    private Image WordPicture { get { return Get<Image>("WordPicture"); } }
    private Transform Plate { get { return GetT("Plate"); } }
    private Transform PlateList { get { return GetT("PlateList"); } }
    private Transform Hole { get { return GetT("Hole"); } }
    private DrawPath Path { get { return Get<DrawPath>("Path"); } }
    private GameObject Voice { get { return Get("Voice"); } }

    private int m_firstPlatePosIndex = 3;
    private int m_secondPlatePosIndex = 4;
    private int m_thirdPlatePosIndex = 5;
    private Vector2 m_platePutPosition = new Vector2(0, 20);
    private UIRevolvingRestaurantItem m_firstPlate;
    private UIRevolvingRestaurantItem m_secondPlate;
    private UIRevolvingRestaurantItem m_thirdPlate;

    public override void Refresh()
    {
        base.Refresh();
        UGUIEventListener.Get(Voice).onPointerClick = OnClickVoice;
    }

    public override void PlayGame()
    {
        if (IsGameEnd)
            return;
        WordPicture.sprite = CurrentWord.sprite;
        var audio = CurrentWord.PlaySound();
        WordPicture.transform.DOScale(1.2f, audio.clip.length).SetLoops(2, LoopType.Yoyo);
        var randomPlateList = Utility.RandomCombine(CurrentWord, m_randomWordList, 3);
        PlateList.transform.ClearAllChild();
        m_firstPlate = MovePlateAppear(randomPlateList[0], 5, 5, 0);
        m_secondPlate = MovePlateAppear(randomPlateList[1], 4, 4, 1);
        m_thirdPlate = MovePlateAppear(randomPlateList[2], 3, 3, 2);
        Hole.transform.SetAsLastSibling();
        WordPicture.transform.SetAsLastSibling();
    }

    private UIRevolvingRestaurantItem MovePlateAppear(ConfigWordLibraryModel model, float time, int index, float delay)
    {
        var points = Path.GetPoints();
        var tempList = new List<Vector3>();
        for (int i = 0; i < index; i++)
        {
            tempList.Add(points[i]);
        }
        var itemCp = CreateUIItem<UIRevolvingRestaurantItem>(PlateList);
        itemCp.Init(this, model);
        var tween = itemCp.transform.DOPath(tempList.ToArray(), time);
        tween.SetDelay(delay);
        tween.onComplete = delegate
        {
            itemCp.SetClick();
            itemCp.Brand.SetActive(true);
            PlateList.transform.SetAsLastSibling();
        };
        return itemCp;
    }

    private void OnClickVoice(PointerEventData data)
    {
        CurrentWord.PlaySound();
    }

    public void AnswerRight(UIRevolvingRestaurantItem item)
    {
        var audio = CurrentWord.PlaySound();
        item.WordText.transform.DOScale(1.2f, audio.clip.length).SetLoops(2, LoopType.Yoyo);
        WordPicture.transform.SetAsLastSibling();
        var tween = WordPicture.transform.DOScale(1.2f, audio.clip.length).SetLoops(2, LoopType.Yoyo);
        tween.onComplete = delegate
        {
            item.Brand.SetActive(false);
            item.transform.SetParent(Plate, true);
            var rt = item.transform as RectTransform;
            tween = rt.DOAnchorPos(m_platePutPosition, 2);
            tween.onComplete = delegate
            {
                item.CoverActive(true);
                var flyStar = FlyStar(true);
                flyStar.OnComplete += delegate
                {
                    if (m_firstPlate)
                        Destroy(m_firstPlate.gameObject);
                    if (m_secondPlate)
                        Destroy(m_secondPlate.gameObject);
                    if (m_thirdPlate)
                        Destroy(m_thirdPlate.gameObject);
                    PlayGame();
                };
            };
        };
    }

    public void AnswerWrong(UIRevolvingRestaurantItem item)
    {
        FlyStar(false);
    }
}
