using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;

public class UIHitMoleMole : UIBaseInit
{

    private Transform Hammer { get { return GetT("Hammer"); } }
    private TextMeshProUGUI Text { get { return Get<TextMeshProUGUI>("Text"); } }
    private Transform Wood { get { return GetT("Wood"); } }
    private Image Idel { get { return Get<Image>("Idel"); } }
    private Transform Hit { get { return GetT("Hit"); } }
    private Tween Up;
    private Tween Down;
    private bool isClick = false;
    private RectTransform Star { get { return GetR("Star"); } }


    void Start()
    {
        Hammer.gameObject.SetActive(false);
        Hit.gameObject.SetActive(false);
        UGUIEventListener.Get(Wood).onPointerClick = OnClickMole;
        UGUIEventListener.Get(Idel).onPointerClick = OnClickMole;
        StartCoroutine(ShowTime());
    }

    public void KillAnimation()
    {
        StopAllCoroutines();
        //StopCoroutine(ShowTime());
        DOTween.Kill("Up");
        DOTween.Kill("Down");
    }
    public void OnClickMole(PointerEventData data)
    {
        //UIHitMole.Instance.ShowHammer();
        if (!isClick)
        {
            UIHitMole.Instance.KillAllAnimation();
            if (Text.text == UIHitMole.Instance.CurrentWord.word)
            {
                StartCoroutine(RightHit());

            }
            else
            {
                StartCoroutine(WrongHit());
            }
        }

    }
    public void InitMole(string word)
    {
        Text.text = word;
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
        yield return new WaitForSeconds(0.2f);
        Wood.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        Destroy(this);

        UIHitMole.Instance.CreateMole();
    }
    IEnumerator WrongHit()
    {
        isClick = true;
        Hammer.gameObject.SetActive(true);
        Hammer.DORotate(new Vector3(0, 0, 70), 0.1f).From();
        yield return new WaitForSeconds(0.1f);
        transform.DOLocalMoveY(-70, 0.05f);
        yield return new WaitForSeconds(0.05f);
        Wood.gameObject.SetActive(false);

        Hammer.gameObject.SetActive(false);
        UITopBarStarFly fly = UIHitMole.Instance.FlyStar(false, true);
        fly.OnComplete += () =>
        {
            Destroy(this);
            UIHitMole.Instance.CreateMole();
        };
    }
    IEnumerator RightHit()
    {

        isClick = true;
        Hammer.gameObject.SetActive(true);
        Hammer.DORotate(new Vector3(0, 0, 70), 0.1f).From();
        yield return new WaitForSeconds(0.1f);
        Hammer.gameObject.SetActive(false);
        Hit.gameObject.SetActive(true);
        Idel.gameObject.SetActive(false);
        Star.DORotate(new Vector3(90, 90, 180), 1.4f);


        UITopBarStarFly fly = UIHitMole.Instance.FlyStar(true, true);
        fly.OnComplete += () =>
        {
            UIHitMole.Instance.PlayGame();
        };
    }


}
