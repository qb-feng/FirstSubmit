using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UICityHunterSelectCtrl : UIBaseSelectCtrl
{
    private int canvas_level;
    private Canvas canvas;
    private Vector3 movePos;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        RectTransform rect = gameObject.GetComponent<RectTransform>();

        UGUIEventListener.Get(gameObject).onPointerUp += d =>
        {
            transform.GetComponent<Rigidbody2D>().simulated = false;
            if (UICityHunter.Instance.Bullet != null)
            {
                UICityHunter.Instance.Bullet.GetComponent<UICityHunterBullet>().endPos = rect.anchoredPosition;
                UICityHunter.Instance.Bullet.GetComponent<UICityHunterBullet>().isShoot = true;
            }
            rect.DOAnchorPosX(0, 0.1f);
            rect.DOAnchorPosY(-400, 0.1f);
        };
        UGUIEventListener.Get(gameObject).onDrag += d =>
        {
            movePos = transform.position;
            if (transform.position.x > 2.7)
            {
                movePos.x = 2.7f;
            }
            if (transform.position.x < -2.7)
            {
                movePos.x = -2.7f;
            }
            if (transform.position.y < -4.7)
            {
                movePos.y = -4.7f;
            }
            if (transform.position.y > -3.7)
            {
                movePos.y = -3.7f;
            }
            transform.position = movePos;
        };
    }

    protected override void RighMatchAction()
    {

    }

    protected override void WrongMatchAction()
    {

    }

    protected override bool GetMatch()
    {
        return false;
    }
}
