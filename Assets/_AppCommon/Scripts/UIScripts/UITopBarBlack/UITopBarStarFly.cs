using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class UITopBarStarFly
{
    private Vector3 m_startP;
    private Vector3 m_startT;
    private Vector3 m_endT;
    private Vector3 m_endP;
    private float m_duration;
    private Vector3[] m_path;
    private GameObject m_flyStar;

    public bool m_trigger = false;
    public Tweener TweenFly { get; set; }

    public event Action OnComplete;

    public static List<UITopBarStarFly> s_allFly = new List<UITopBarStarFly>();

    public UITopBarStarFly(Vector3 startP, Vector3 startT, Vector3 endT, Vector3 endP, float duration, bool trigger = true)
    {
        TweenStar(startP, startT, endT, endP, duration, trigger);
    }

    public UITopBarStarFly(DrawBezier draw, float duration, bool trigger = true)
    {
        TweenStar(draw.startP.position, draw.startT.position, draw.endT.position, draw.endP.position, duration, trigger);
    }

    private void TweenStar(Vector3 startP, Vector3 startT, Vector3 endT, Vector3 endP, float duration, bool trigger)
    {
        m_startP = startP;
        m_startT = startT;
        m_endT = endT;
        m_endP = endP;
        m_duration = duration;
        m_trigger = trigger;
        m_path = DrawBezier.ComputeBezierPoints(startP, startT, endT, endP);
        if (m_flyStar == null)
            m_flyStar = UIBaseInit.CreateFX("CFX_FlyStar_Right");
        m_flyStar.transform.position = m_path[0];
        TweenFly = m_flyStar.transform.DOPath(m_path, duration, PathType.Linear).SetEase(Ease.InOutCubic).OnComplete(TweenOnComplete);
        s_allFly.Add(this);
    }

    private void TweenOnComplete()
    {
        if (m_trigger)
        {
            if (OnComplete != null)
            {
                OnComplete();
                OnComplete = null;
            }
        }
        s_allFly.Remove(this);
        UnityEngine.Object.Destroy(m_flyStar);
    }

    public void RandomTangent(bool startTangent, bool endTangent)
    {
        if (startTangent)
        {
            int x1 = UnityEngine.Random.Range(-9, 9);
            int y1 = UnityEngine.Random.Range(-3, 3);
            m_startT = new Vector3(x1, y1, -1.5f);
        }
        if (endTangent)
        {
            int x2 = UnityEngine.Random.Range(-9, 9);
            int y2 = UnityEngine.Random.Range(-3, 3);
            m_endT = new Vector3(x2, y2, -1.5f);
        }
        TweenStar(m_startP, m_startT, m_endT, m_endP, m_duration, true);
    }

    public void ClearOnComplete()
    {
        UnityEngine.Object.Destroy(m_flyStar);
        OnComplete = null;
    }

    public static void ClearAllOnComplete()
    {
        foreach (var item in s_allFly)
        {
            item.ClearOnComplete();
        }
    }
}
