using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class UIStarThroughFallIngStoneItem : UIBaseInitItem
{
    private Image ImageStone { get { return Get<Image>("ImageStone"); } }
    private TextMeshProUGUI TextWord { get { return Get<TextMeshProUGUI>("TextWord"); } }
    private Transform ImageBg { get { return GetT("ImageBg"); } }
    private GameObject ImageClick { get { return Get("ImageClick"); } }
    private RectTransform ImageBomb { get { return Get<RectTransform>("ImageBomb"); } }

    private float speed = 200;

    private RectTransform rect;

    private UIStarThrough Model;
    public void Init(UIStarThrough model,string word)
    {
        Model = model;

        TextWord.text = word;

        ImageStone.sprite = GetS("UIStarThroughyunshi" + Random.Range(1, 4));

        rect = GetComponent<RectTransform>();

        transform.SetAsFirstSibling();

        StartCoroutine(Move());

    }

    private void ButtonClick(UnityEngine.EventSystems.PointerEventData arg0)
    {
        ImageBomb.DOScale(1, 0.3f).OnComplete(delegate
        {
            ImageBomb.gameObject.SetActive(false);
            Destroy(gameObject);
        });
        Model.Check(TextWord.text);
    }
    IEnumerator Move()
    {
        int i = Random.Range(1, 5);
        Vector2 v = rect.anchoredPosition;

        switch (i)
        {
            case 1:
                v = new Vector2(Random.Range(-Model.ImageBg.rect.width / 2, Model.ImageBg.rect.width / 2), Model.ImageBg.rect.height / 2 + 200);
                break;
            case 2:
                v = new Vector2(Random.Range(-Model.ImageBg.rect.width / 2, Model.ImageBg.rect.width / 2), -Model.ImageBg.rect.height / 2 - 200);
                break;
            case 3:
                v = new Vector2(-Model.ImageBg.rect.width / 2 - 200,Random.Range(-Model.ImageBg.rect.height / 2, Model.ImageBg.rect.height / 2));
                break;
            case 4:
                v = new Vector2(Model.ImageBg.rect.width / 2 + 200, Random.Range(-Model.ImageBg.rect.height / 2, Model.ImageBg.rect.height / 2));
                break;
        }
//        if (i == 0)
//            v = new Vector2(Random.Range(-2000, 2000), 800);
//        else
//            v = new Vector2(Random.Range(-2000, 2000), -800);

        float f = Vector2.Distance(v, rect.anchoredPosition);
        float time = f / speed;
        rect.DOAnchorPos(v, time).SetEase(Ease.InExpo).OnComplete(delegate
        {
            Destroy(gameObject);
        });
        
//        ImageStone.transform.DOScale(2, time).SetEase(Ease.InExpo);
        var size = ImageStone.rectTransform.sizeDelta*2;
        ImageStone.rectTransform.sizeDelta = new Vector2(0, 0);
        ImageStone.gameObject.SetActive(true);
        ImageStone.GetComponent<RectTransform>().DOSizeDelta(size, time).SetEase(Ease.InExpo);

        yield return new WaitForSeconds(3.5f);

        if (i > 2)
        {
            UGUIEventListener.Get(ImageClick).onPointerClick = ButtonClick;
            ImageBg.gameObject.SetActive(true);
        }

//        rect.DOKill();
//        float f1 = Vector2.Distance(v, rect.anchoredPosition);
//        speed = 300;
//        float time1 = f / speed;
//        rect.DOAnchorPos(v, time1).OnComplete(delegate
//        {
//            Destroy(gameObject);
//        });
    }
}