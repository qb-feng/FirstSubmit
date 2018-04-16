using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollingLayer : MonoBehaviour
{
    public RawImage mainRenderer;           //The main renderer
    public float startingSpeed;             //The starting scrolling speed

    //private float speedMultiplier;          //The current speed multiplier
    //private bool paused;                    //True, if the level is paused

    private Vector2 offset;
    private Tweener tween;

    // Use this for initialization
    void Start()
    {
        //speedMultiplier = 1;
        //paused = true;
        TweenMove();
    }

    private void TweenMove()
    {
        Vector2 original = mainRenderer.rectTransform.anchoredPosition;
        tween = mainRenderer.rectTransform.DOAnchorPosX(original.x - UIManager.Instance.UIRoot.GetComponent<CanvasScaler>().referenceResolution.x, 10 / startingSpeed);
        tween.OnComplete(() =>
        {
            Vector2 current = mainRenderer.rectTransform.anchoredPosition;
            current.x += UIManager.Instance.UIRoot.GetComponent<CanvasScaler>().referenceResolution.x;
            mainRenderer.rectTransform.anchoredPosition = current;
            TweenMove();
        });
    }
    // Update is called once per frame
    //void Update()
    //{
    //    if (!paused)
    //    {
    //        offset = mainRenderer.uvRect.position;
    //        offset.x += startingSpeed * speedMultiplier * Time.deltaTime;

    //        if (offset.x > 1)
    //            offset.x -= 1;
    //        Rect rect = mainRenderer.uvRect;
    //        rect.x = offset.x;
    //        mainRenderer.uvRect = rect;
    //    }
    //}

    //Sets scrolling state
    public void SetPauseState(bool state)
    {
        //paused = state;
        if (state)
            tween.Pause();
        else
            tween.Play();
    }
    //Updates speed multiplier
    public void UpdateSpeedMultiplier(float n)
    {
        //speedMultiplier = n;
    }
}
