using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UILookWindowItem : UIBaseInit
{
    /// <summary>
    /// 用来标记当前的状态
    /// </summary>
    private static bool s_selected = false;
    private GameObject WindowClose { get { return Get("WindowClose"); } }
    private GameObject WindowOpen { get { return Get("WindowOpen"); } }
    private Image ImageContent { get { return Get<Image>("ImageContent"); } }
    private TextMeshProUGUI TextWord { get { return Get<TextMeshProUGUI>("TextWord"); } }
    private GameObject WordBg { get { return Get("WordBg"); } }//背景图
    private ConfigWordLibraryModel m_model;

    /// <summary>
    /// 初始化窗户
    /// </summary>
    /// <param name="model">当前配置的模型对象</param>
    public void Init(ConfigWordLibraryModel model)
    {
        s_selected = false;
        m_model = model;
        ImageContent.sprite = m_model.sprite;
        TextWord.text = m_model.word;
        WindowOpen.gameObject.SetActive(false);

        switch (UILookWindow.Instance.CurrentGameType)
        {
            case UILookWindow.GameType.PromptSpriteWindowWord:
                WordBg.SetActive(false);
                break;
            case UILookWindow.GameType.WindowSpritePromptWord:
                WordBg.SetActive(true);
                TextWord.text = null;
                break;
        }
    }
    /// <summary>
    /// 为该对象添加点击事件
    /// </summary>
    public void SetClick()
    {
        UGUIEventListener.Get(gameObject).onPointerClick = GoClick;
    }
    public Image Imagecon { get { return ImageContent; } }
    public GameObject WindowCloses { get { return WindowClose; } }

    private void GoClick(UnityEngine.EventSystems.PointerEventData arg0)
    {
        if (s_selected)
            return;
        s_selected = true;

        ImageContent.transform.DOScale(Vector3.one * 1.2f, 0.4f).SetLoops(2, LoopType.Yoyo);
        TextWord.gameObject.SetActive(true);
        bool right = UILookWindow.Instance.CheckMatch(ImageContent.sprite.name);
        if (right)
        {
            //UITopBarBlack.Instance.AddOneStar();
            m_model.PlaySound();
            var fly = UILookWindow.Instance.FlyStar(true);
            fly.OnComplete += () =>
            {
                s_selected = false;
                UILookWindow.Instance.PlayGame();
            };
        }
        else
        {
            this.WaitSecond(() =>
            {
                //UITopBarBlack.Instance.WrongStarFly();
                UILookWindow.Instance.FlyStar(false);
            }, 1f);
            m_model.PlaySound();
            TextWord.gameObject.SetActive(true);
        }

        WindowOpen.gameObject.SetActive(true);
        this.WaitSecond(() =>
        {
            WindowOpen.gameObject.SetActive(false);
            AudioManager.Instance.Play("CloseWind");

            TextWord.gameObject.SetActive(false);
            TextWord.gameObject.SetActive(false);
            s_selected = false;
        }, 2f);
    }

    public void SetWindowOpen(bool active, float delay)
    {
        TextWord.gameObject.SetActive(true);
        this.WaitSecond(() =>
        {
            WindowOpen.gameObject.SetActive(active);
            TextWord.gameObject.SetActive(active);
            if (active)
                m_model.PlaySound();
        }, delay);
    }

    /// <summary>
    /// CloseWindow
    /// </summary>
    /// <param name="active"></param>
    /// <param name="delay"></param>
    public void SetWindowClose(bool active, float delay)
    {
        TextWord.gameObject.SetActive(false);
        this.WaitSecond(() =>
        {
            if (active)
            {
                AudioManager.Instance.Play("CloseWind");
            }

        }, delay);
    }
}
