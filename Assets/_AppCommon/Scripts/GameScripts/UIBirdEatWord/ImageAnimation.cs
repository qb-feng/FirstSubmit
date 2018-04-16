using UnityEngine;

public class ImageAnimation : MonoBehaviour
{
    public bool isStatic;
    private bool startingIsStatic;
    public float speed;
    private float startingSpeed;
    private RectTransform mainRenderer;
    //private Vector2 original;
    private float endPosX;
    private bool isReset;
    //int index;

    void Awake()
    {
        startingSpeed = speed;
        startingIsStatic = isStatic;
        mainRenderer = GetComponent<RectTransform>();
        //original = mainRenderer.anchoredPosition;
        //index = transform.GetSiblingIndex() + 1;
        //endPosX = original.x - Mathf.Sign(speed) * Screen.currentResolution.width * index;
        endPosX = Mathf.Sign(speed) * (Screen.currentResolution.width + mainRenderer.rect.width) * 0.5f;
    }

    void Update()
    {
        if (isStatic)
            return;

        float step = Time.deltaTime * speed;
        Vector2 current = mainRenderer.anchoredPosition;
        current.x += step;
        if (Mathf.Sign(endPosX) != Mathf.Sign(speed))
        {
            endPosX *= -1;
        }
        if (Mathf.Sign(endPosX - current.x) != Mathf.Sign(speed))
        {
            current.x -= Mathf.Sign(speed) * Screen.currentResolution.width * transform.parent.childCount;
        }
        mainRenderer.anchoredPosition = current;

        if (isReset)
        {
            speed = Mathf.Lerp(speed, startingSpeed, 0.05f);
            if (Mathf.Approximately(speed, startingSpeed))
            {
                speed = startingSpeed;
                isReset = false;
            }
        }
    }

    public void SetSpeed(Vector2 delta)
    {
        isStatic = false;
        speed = Mathf.Sign(-delta.x) * (startingSpeed - delta.x / startingSpeed);
    }

    public void ResetSpeed()
    {
        isReset = true;
        isStatic = startingIsStatic;
    }
}
