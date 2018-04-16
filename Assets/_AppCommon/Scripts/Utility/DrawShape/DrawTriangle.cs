using UnityEngine;

public class DrawTriangle : BaseDrawPoints
{
    public Transform m_center;
    public float m_length = 0.5f;

    public override Vector3[] GetPoints()
    {
        if (m_center != null)
            return GetTrianlePoints(m_center.position, m_length);
        return null;
    }

    public Vector3[] GetTrianlePoints(Vector3 center, float length)
    {
        Vector3[] points = new Vector3[4];
        float hypotenuse = m_length / Mathf.Sqrt(3f);
        float rightAngleSide1 = hypotenuse / 2;
        float rightAngleSide2 = m_length / 2;
        points[0] = new Vector3(center.x, center.y + hypotenuse, center.z);
        points[1] = new Vector3(center.x - rightAngleSide2, center.y - rightAngleSide1, center.z);
        points[2] = new Vector3(center.x + rightAngleSide2, center.y - rightAngleSide1, center.z);
        points[3] = points[0];
        return points;
    }
}
