using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIHitMoleMine : UIBaseInit
{

    private Transform Hammer { get { return GetT("Hammer"); } }
    private GameObject Mine { get { return Get("Mine"); } }//炸弹
    private Tween Up;
    private Tween Down;
    private bool isClick = false;

    // Use this for initialization
    void Start()
    {
        Hammer.gameObject.SetActive(false);
        UGUIEventListener.Get(Mine).onPointerClick = OnClickMole;
        StartCoroutine(ShowTime());
    }
    /// <summary>
    /// 增加打地鼠的停留时间的控制
    /// </summary>
    private float hitMoleWaitTime = 0f;
    private float HitMoleWaitTime
    {
        get
        {
            if (hitMoleWaitTime == 0)
            {
                hitMoleWaitTime = float.Parse(ConfigGlobalValueModel.GetValue("HitMoleWaitTime"));
            }
            return hitMoleWaitTime;
        }
    }
    IEnumerator ShowTime()
    {
        var pos = transform.localPosition;
        pos.y = -70;
        transform.localPosition = pos;
        Up = transform.DOLocalMoveY(24f, 0.3f);
        yield return new WaitForSeconds(HitMoleWaitTime);
        Down = transform.DOLocalMoveY(-70, 0.2f);

        yield return new WaitForSeconds(1f);
        Destroy(this);
        UIHitMole.Instance.CreateMole();
    }
    public void OnClickMole(PointerEventData data)
    {
        if (!isClick)
        {
            UIHitMole.Instance.KillAllAnimation();
            StartCoroutine(WrongHit());
        }

    }
    public void KillAnimation()
    {
        StopAllCoroutines();
        //StopCoroutine(ShowTime());
        DOTween.Kill("Up");
        DOTween.Kill("Down");
    }
    IEnumerator WrongHit()
    {
        isClick = true;
        Hammer.gameObject.SetActive(true);
        Hammer.DORotate(new Vector3(0, 0, 70), 0.1f).From();
        yield return new WaitForSeconds(0.1f);
        Hammer.gameObject.SetActive(false);
        CreateUIItem("UIHitMoleReek", UIHitMole.Instance.transform).AddComponent<UIHitMoleReek>();
        // yield return new WaitForSeconds(0.7f);  
        UITopBarStarFly fly = UIHitMole.Instance.FlyStar(false, true);
        fly.OnComplete += () =>
        {
            Destroy(this);
            UIHitMole.Instance.CreateMole();
        };
    }
}
