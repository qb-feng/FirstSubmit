using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UICutThingSlice : UIBaseInit
{
    public Image MelonSlice { get { return Get<Image>("MelonSlice"); } }
    private TextMeshProUGUI MelonSliceLetter { get { return Get<TextMeshProUGUI>("MelonSliceLetter"); } }

    private Text Index { get { return Get<Text>("Index"); } }


    public Image MelonSliceDotted { get { return Get<Image>("MelonSliceDotted"); } }

    public bool m_random = false;

    void Start()
    {
        if (m_random)
        {
            this.WaitSecond(() =>
            {
                MelonSlice.gameObject.AddComponent<UICutThingSelectSliceMoveCtrl>();
            }, 0.2f);
        }
        MelonSliceDotted.overrideSprite = GetS(MelonSlice.sprite.name + "Dotted");
    }

    public void Init(char letter, bool random, int index)
    {
        MelonSliceLetter.text = letter.ToString();
        if (!random)
        {
            Index.text = index.ToString();
        }
        m_random = random;       
    }

    public Tweener TweenMelonAlpha(float alpha)
    {
        Color color = Color.white;
        color.a = alpha;       
        return MelonSlice.DOColor(color, 0.3f);
    }

    public Tweener TweenLetterAlpha(float alpha)
    {
        Color color = Color.white;
        color.a = alpha;
        return MelonSliceLetter.DOColor(color, 0.3f);
    }

    public void SetMelonAlpha(float alpha)
    {
        GetComponent<CanvasGroup>().alpha = alpha;
    }

    public Tweener TweenCanvasGroupAlpha(float alpha)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        return DOTween.To(() => canvasGroup.alpha, (v) => canvasGroup.alpha = v, alpha, 0.3f);
    }
}
