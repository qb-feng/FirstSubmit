using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UIConveyorClamp : UIBaseInitItem
{
    private Image Gear
    {
        get { return Get<Image>("ImageGear"); }
    }

    private TextMeshProUGUI Letter
    {
        get { return Get<TextMeshProUGUI>("TextWord"); }
    }

    private Text Index
    {
        get { return Get<Text>("Index"); }
    }

    // Use this for initialization
    private void Start()
    {
        UGUIEventListener.Get(Gear).onPointerDown = d => AudioManager.Instance.Play(Letter.text);
    }

    public void Init(char letter,int index,int difficult)
    {
        Letter.text = letter.ToString();
        Index.text = index.ToString();      
        switch (difficult)
        {
            case 1:
                break;
            case 2:
                Letter.DOFade(1, 3f).onComplete += () =>
                {
                    Letter.DOFade(0, 1f);
                };                                
                break;
        }
    }

    public void SetGearNormal()
    {
        Gear.overrideSprite = GetS("UIConveyorGear1");
        Letter.DOFade(1, 1f);       
    }

    public void SetGearDotted()
    {
        Gear.overrideSprite = null;
    }

    public Tweener TweenGearScale()
    {
        Tweener tween = Gear.transform.DOScale(1.3f, 0.5f).SetLoops(2, LoopType.Yoyo);
        return tween;
    }

}