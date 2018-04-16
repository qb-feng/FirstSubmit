using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class UIAntSoldierSelectCtrl : UIBaseSelectCtrl
{
    private int canvas_level;
    private Canvas canvas;
    public bool yesOrNo;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        canvas = gameObject.GetComponent<Canvas>();
        canvas_level = canvas.sortingOrder;

        UGUIEventListener.Get(gameObject).onPointerDown += d =>
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 100;

        };

        UGUIEventListener.Get(gameObject).onPointerUp += d =>
        {
            canvas.sortingOrder = canvas_level;
            //修改类型
            m_rigidbody2d.bodyType = RigidbodyType2D.Kinematic;
            m_rigidbody2d.simulated = true;

        };
    }

    public void Init(bool answer)
    {
        yesOrNo = answer;
    }

    protected override void RighMatchAction()
    {
        UIAntSoldier.Instance.Decidestar(true);
        transform.GetComponent<UIAntSoldierSelect>().Text.DOFade(0, 1f);
    }

    protected override void WrongMatchAction()
    {

    }

    protected override bool GetMatch()
    {
        if (yesOrNo && m_cacheCollider.transform.name == "TextShort")
        {
            return true;
        }
        else if (m_cacheCollider.transform.name == "UIAntSoldierSelect(Clone)")
        {
            return false;
        }
        else
        {
            UIAntSoldier.Instance.Decidestar(false);
            return false;
        }
    }
}
