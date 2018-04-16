using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITestShowLevelBookOptionItem : UIBaseInitItem
{
    private TMP_Text Volume { get { return Get<TMP_Text>("Volume"); } }
    private Toggle Select { get { return Get<Toggle>(); } }

    private UITestShowLevel m_parent;
    private ConfigBookModel m_model;

    public void Init(UITestShowLevel parent, ConfigBookModel model)
    {
        m_parent = parent;
        m_model = model;
        Volume.text = model.volume;
        UGUIEventListener.Get(gameObject).onPointerClick = OnClick;
        Select.group = GetComponentInParent<ToggleGroup>();
        if (model.volume.Equals("2A"))
            Select.isOn = true;
    }

    private void OnClick(PointerEventData data)
    {
        m_parent.InitItemList(m_model);
        Select.isOn = true;
    }
}
