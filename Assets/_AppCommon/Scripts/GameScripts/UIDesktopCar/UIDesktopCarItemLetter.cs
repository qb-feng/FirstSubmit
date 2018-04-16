using UnityEngine;

public class UIDesktopCarItemLetter : MonoBehaviour
{
    private BoxCollider2D m_boxCollider2D;

    private void Start()
    {
        var rt = transform as RectTransform;
        m_boxCollider2D = GetComponent<BoxCollider2D>();
        m_boxCollider2D.size = rt.rect.size;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // UIDesktopCar.Instance.MatchCurrentLetter(collision, gameObject);
        bool matched = UIDesktopCarPre.Instance.MatchCurrentLetter(collision, gameObject);
        m_boxCollider2D.enabled = !matched;
    }
}
