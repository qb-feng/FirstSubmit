using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UIThreeCupGuessAnswerCtrl : UIBaseSelectCtrl
{
    void OnDisable()
    {
        if (m_originalPosition != Vector3.zero)
            transform.position = m_originalPosition;
    }

    protected override bool GetMatch()
    {
        TextMeshProUGUI content = GetComponentInChildren<TextMeshProUGUI>();
        return UIThreeCupGuess.Instance.CheckMatch(content.text);
    }

    protected override void RighMatchAction()
    {
        UIThreeCupGuess.Instance.AnswerRight(ResetPosition, (RectTransform)transform);
        UIThreeCupGuess.Instance.Next(true);
    }

    protected override void WrongMatchAction()
    {
        m_rigidbody2dTween.Kill();
        m_rigidbody2dTween = m_rigidbody2d.DOMove(m_cacheCollider.transform.position, 0.2f);
        m_rigidbody2dTween.OnComplete(() =>
        {
            GetComponent<Collider2D>().enabled = true;
        });
        UIThreeCupGuess.Instance.AnswerWrong(ResetPosition, (RectTransform)transform);
        UIThreeCupGuess.Instance.Next(false);
    }

    private void ResetPosition()
    {
        m_rigidbody2d.velocity = Vector2.zero;
        m_rigidbody2d.transform.position = m_originalPosition;
    }
}
