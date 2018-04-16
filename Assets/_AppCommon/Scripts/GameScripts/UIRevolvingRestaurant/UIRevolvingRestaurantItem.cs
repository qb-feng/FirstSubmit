using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRevolvingRestaurantItem : UIBaseInit
{
    private GameObject CoverClose { get { return Get("CoverClose"); } }
    private GameObject CoverOpen { get { return Get("CoverOpen"); } }
    public GameObject Brand { get { return Get("Brand"); } }
    public TMP_Text WordText { get { return Get<TMP_Text>("WordText"); } }
    public Image Food { get { return Get<Image>("Food"); } }

    private UIRevolvingRestaurant m_parent;
    private ConfigWordLibraryModel m_model;

    public void Init(UIRevolvingRestaurant parent, ConfigWordLibraryModel model)
    {
        m_parent = parent;
        m_model = model;
        WordText.text = model.word;
        Brand.SetActive(false);
        int randomFood = Random.Range(1, 4);
        Food.sprite = GetS("UIRevolvingRestaurantFood" + randomFood);
        Food.SetNativeSize();
    }

    public void SetClick()
    {
        UGUIEventListener.Get(gameObject).onPointerClick = OnClick;
    }

    private void OnClick(PointerEventData data)
    {
        //掀开盖子
        CoverActive(true);
        bool right = m_parent.CurrentWord == m_model;
        Food.gameObject.SetActive(right);
        if (right)
        {
            this.WaitSecond(delegate
            {
                CoverActive(false);
                m_parent.AnswerRight(this);
            }, 1.5f);
        }
        else
        {
            m_parent.AnswerWrong(this);
            this.WaitSecond(delegate
            {
                //Destroy(gameObject);
                CoverActive(false);
            }, 1.5f);
        }
    }

    public void CoverActive(bool active)
    {
        CoverClose.SetActive(!active);
        CoverOpen.SetActive(active);
    }
}
