using UnityEngine;

public class DrawRectangle : BaseDrawPoints
{
    public Transform m_center;
    public float m_width = 10;
    public float m_height = 10;

    public override Vector3[] GetPoints()
    {
        if (m_center != null)
            return GetRectanglePoints(m_center.position, m_width, m_height);
        return null;
    }

    public Vector3[] GetRectanglePoints(Vector3 center, float width, float height)
    {
        Vector3[] points = new Vector3[9];
        points[0] = new Vector3(center.x, center.y + height / 2, center.z);
        points[1] = new Vector3(center.x - width / 2, center.y + height / 2, center.z);
        points[2] = new Vector3(center.x - width / 2, center.y, center.z);
        points[3] = new Vector3(center.x - width / 2, center.y - height / 2, center.z);
        points[4] = new Vector3(center.x, center.y - height / 2, center.z);
        points[5] = new Vector3(center.x + width / 2, center.y - height / 2, center.z);
        points[6] = new Vector3(center.x + width / 2, center.y, center.z);
        points[7] = new Vector3(center.x + width / 2, center.y + height / 2, center.z);
        points[8] = points[0];
        return points;
    }
}
