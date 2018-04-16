using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMouseRunStone : UIBaseInit
{
    public string answerStr;
    public TextMeshProUGUI StoneText { get { return Get<TextMeshProUGUI>("StoneText"); } }
    private Image StoneImage { get { return Get<Image>("StoneImage"); } }
    private Image myImage;
    private string[] sImage = new string[] { "UIMouseRun_Stone1", "UIMouseRun_Stone2", "UIMouseRun_Stone3" };
    public bool isWork;
    private string currentImage;
    private Color showColor = new Color(255, 255, 255, 255);
    private Color closeColor = new Color(255, 255, 255, 0);
    public bool isRightShow=false;
    void Awake()
    {
        myImage = this.GetComponent<Image>();
    }

    public void InitWord(string word,bool iswork)
    {
        answerStr = word;
        StoneText.text = word;
        isWork = iswork;
        currentImage = sImage[Random.Range(0, 3)];
        myImage.sprite = (iswork ? GetS(currentImage): GetS("UIMouseRun_HideStone"));
        StoneText.color = (iswork ? showColor : closeColor);
        StoneImage.sprite = GetS(currentImage);

    }
}
