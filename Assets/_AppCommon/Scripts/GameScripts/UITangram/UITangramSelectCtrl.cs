using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UITangramSelectCtrl : UIBaseSelectCtrl
{
    private Transform parent;
    private Vector2 pos;
    private RectTransform rect;

    private bool count_down = true;
    protected override void Start()
    {
        base.Start();
        m_isKinematics = false;

        rect = GetComponent<RectTransform>();

        StartCoroutine(WaitTenSeconds());

        UGUIEventListener.Get(gameObject).onPointerDown += d =>
        {
            parent = transform.parent;
            pos = rect.anchoredPosition;
            transform.SetParent(parent.parent);
        };

        UGUIEventListener.Get(gameObject).onPointerUp += d =>
        {

        };

    }

    IEnumerator WaitTenSeconds()
    {
        yield return new WaitForSeconds(10);
        count_down = false;
    }

    protected override void RighMatchAction()
    {
        Destroy(gameObject);
    }

    protected override void WrongMatchAction()
    {
    }
    /// <summary>
    /// 检测碰撞区域
    /// </summary>
    /// <returns></returns>
    protected override bool GetMatch()
    {
        if (m_cacheCollider.name == "UITangramImageItem(Clone)")
        {
            var item = m_cacheCollider.GetComponent<UITangramImageItem>();
            var self = GetComponent<UITangramItem>();
            item.SetText(self.TextWord.text, self.sp.sprite);
            return true;
        }

        return false;
    }


    protected override void WrongRigibodyTween()
    {
        if (count_down)
        {
            transform.SetParent(parent);
            rect.anchoredPosition = pos;
        }
        else
            Destroy(gameObject);
    }
}
