using UnityEngine;

public abstract class BaseDrawPoints : MonoBehaviour
{
    public bool m_draw = false;             //是否一直绘制不要求选择对象
    [Range(3, 100)]
    public int m_division = 20;             //分割曲线获取对应数量的轨迹点
    public Color m_color = Color.white;     //画线颜色

    void OnDrawGizmos()
    {
        if (!m_draw)
            return;
        Draw(GetPoints());
    }

    void OnDrawGizmosSelected()
    {
        if (m_draw)
            return;
        Draw(GetPoints());
    }

    private void Draw(Vector3[] points)
    {
        Gizmos.color = m_color;
        if (points != null)
            for (int i = 0; i < points.Length; i++)
                if (i > 0)
                    Gizmos.DrawLine(points[i - 1], points[i]);
    }

    public abstract Vector3[] GetPoints();

    public Vector3[] GetReversePoints()
    {
        Vector3[] points = GetPoints();
        if (points != null)
        {
            System.Array.Reverse(points);
            return points;
        }
        return null;
    }
}
