using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using System.Collections.Generic;
public class UISnowMountainTopPlayer : MonoBehaviour
{
    private UISnowMountainTop Model;
    public void Init(UISnowMountainTop model)
    {
        Model = model;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);

        if (collision.name == "ImageStone1" || collision.name == "ImageStone2")
        {
            Model.TouchStone();
        }
        else
        {
            Model.TouchWord(collision.transform.GetComponentInChildren<TextMeshProUGUI>().text);
        }
        collision.gameObject.SetActive(false);
    }
}