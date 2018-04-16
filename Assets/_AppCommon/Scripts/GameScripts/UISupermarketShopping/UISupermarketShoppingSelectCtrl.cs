using UnityEngine;
using UnityEngine.UI;

public class UISupermarketShoppingSelectCtrl : UIBaseSelectCtrl
{
    protected override void Start()
    {
        base.Start();
        m_isKinematics = false;
        UGUIEventListener.Get(gameObject).onPointerDown += d =>
       {
           this.GetComponent<BoxCollider2D>().isTrigger = false;
//           UISupermarketShopping.Instance.GuiderTween.Kill();
       };
        UGUIEventListener.Get(gameObject).onPointerUp += d =>
        {
            this.GetComponent<BoxCollider2D>().isTrigger = true;
        };
    }
    /// <summary>
    /// 匹配正确
    /// </summary>
    protected override void RighMatchAction()
    {
        UISupermarketShopping.Instance.CheckColorMatch(this);
    }

    protected override void WrongMatchAction()
    {
        UISupermarketShopping.Instance.FlyStar(false);
    }
    /// <summary>
    /// 检测移动的瓶子是否是当前单词的匹配颜色
    /// </summary>
    /// <returns></returns>
    protected override bool GetMatch()
    {
        string currentWord = UISupermarketShopping.Instance.CurrentWord.word;
        var image = GetComponent<Image>();
        string currentName = image.sprite.name.Split('!')[0];
        //currentName = currentName.ToLower();

        #region qiubin 修改 - 目标图片是小写，当前单词是大小（会出现没有正确选项）
        currentWord = currentWord.ToLower();
        currentName = currentName.ToLower();
        #endregion

        return currentName.Equals(currentWord);
    }
}
