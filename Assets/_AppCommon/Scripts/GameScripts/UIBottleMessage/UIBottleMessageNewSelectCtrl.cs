using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIBottleMessageNewSelectCtrl : UIBaseSelectCtrl
{
    private UIBottleMessageNew Model;
    private int Index;

    private int canvas_level;
    private Canvas canvas;
    protected override void Start()
    {
        base.Start();
        m_isKinematics = false;

        canvas = gameObject.GetComponent<Canvas>();
        canvas_level = canvas.sortingOrder;

        UGUIEventListener.Get(gameObject).onPointerDown += d =>
        {
            if (Index == 1)
                Model.ImageTextPos1.SetActive(true);
            else
                Model.ImageTextPos2.SetActive(true);

            canvas.overrideSorting = true;
            canvas.sortingOrder = 100;
        };
        UGUIEventListener.Get(gameObject).onPointerUp += d =>
        {
            if (Index == 1)
                Model.ImageTextPos1.SetActive(false);
            else
                Model.ImageTextPos2.SetActive(false);

            canvas.sortingOrder = canvas_level;
        };
    }

    public void Init(UIBottleMessageNew model, int index)
    {
        Model = model;
        Index = index;
    }
    protected override void RighMatchAction()
    {

    }

    protected override void WrongMatchAction()
    {
        WrongRigibodyTween();
    }
    /// <summary>
    /// 检测碰撞区域
    /// </summary>
    /// <returns></returns>
    protected override bool GetMatch()
    {
        if (m_cacheCollider.name == "ImageAnswer")
        {
            transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            transform.localScale = new Vector2(0.5f, 0.5f);
            Model.CheckAnswer(Index);
            return true;
        }

        transform.localScale = new Vector2(1, 1);
        return false;
    }


}
