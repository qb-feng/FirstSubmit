using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICatJumpingFish : UIBaseInit {

    private TextMeshProUGUI FishText { get { return Get<TextMeshProUGUI>("Text"); } }
    public Image FishImage;
    public string currentText;
    void Awake()
    {
        FishImage = GetComponent<Image>();
    }
    public void Init(string text)
    {
        FishText.text = text;
        currentText = text;
        FishImage.sprite = GetS("UICatJumping_Cyan");
    }

}
