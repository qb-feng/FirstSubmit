using UnityEngine;

public class DrawCircle : BaseDrawPoints
{
    public float m_radius = 1;
    public float m_startAngle = 90;
    public Transform m_center;

    public override Vector3[] GetPoints()
    {
        if (m_center != null)
            return GetEquilateralPolygonPoints(m_center.position, m_radius, m_division);
        return null;
    }

    public Vector3[] GetEquilateralPolygonPoints(Vector3 center, float radius, int division)
    {
        Vector3[] points = new Vector3[division + 1];
        float perAngle = 360f / division;
        for (int i = 0; i < division; i++)
        {
            float radian = (m_startAngle + i * perAngle) * Mathf.PI / 180f;
            Vector3 point = new Vector3(center.x + radius * Mathf.Cos(radian), center.y + radius * Mathf.Sin(radian));
            points[i] = point;
        }
        if (division > 0)
            points[division] = points[0];
        return points;
    }
}
