using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIWraith : UIFlyCar {

    protected override void CreateText(Transform t, float y)
    {
        int pos_x = Random.Range(0, 3);
        var v = CreateUIItem("UIWraithTextItem", t);
        switch (pos_x)
        {
            case 0:
                v.GetComponent<RectTransform>().anchoredPosition = new Vector2(-287, y);
                break;
            case 1:
                v.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
                break;
            case 2:
                v.GetComponent<RectTransform>().anchoredPosition = new Vector2(287, y);
                break;
        }
        v.transform.GetChild(0).gameObject.AddComponent<UIFlyCarCtrl>().Init(this, ImageCar, RefreshWord());
    }

    protected override IEnumerator WaitWrongTwo()
    {
        ImageCarIcon.sprite = GetS("UIWraith6");
        SetSpeed(-2);
        ImageZhizhen.DOLocalRotate(new Vector3(0, 0, 40), 0.5f);
        ImageWarn.gameObject.SetActive(true);
        AudioManager.Instance.Play("Brake");
        yield return new WaitForSeconds(2);
        ImageZhizhen.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
        ImageWarn.gameObject.SetActive(false);
        ImageCarIcon.sprite = GetS("UIWraith7");
        SetSpeed(-4);
    }
}
