using UnityEngine;
using DG.Tweening;

public class DOTweenPathTool : MonoBehaviour
{
    public AnimationClip m_clip;
    public DOTweenPath m_path;
    public int division = 1;
    public bool log = false;

    void Update()
    {
        if (log)
        {
            log = false;
            var points = GetPoints();
            foreach (var item in points)
                Debug.Log("point : " + item);
        }
    }

    public Vector3[] GetPoints()
    {
        var tween = m_path.GetTween();
        var points = new Vector3[division + 1];
        for (int i = 0; i < division + 1; i++)
        {
            float percent = i / (float)division;
            points[i] = tween.PathGetPoint(percent);
        }
        return points;
    }
}
