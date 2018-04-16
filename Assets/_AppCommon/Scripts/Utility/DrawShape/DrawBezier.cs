using UnityEngine;

public class DrawBezier : BaseDrawPoints
{
    public Transform startP;        //第一个点
    public Transform endP;          //第四个点
    public Transform startT;        //第二个点  
    public Transform endT;          //第三个点

    public override Vector3[] GetPoints()
    {
        if (startP != null && endP != null && startT != null && endT != null)
            return ComputeBezierPoints(startP.position, startT.position, endT.position, endP.position, m_division);
        return null;
    }

    public static Vector3[] ComputeBezierPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int division = 20)
    {
        division = Mathf.Max(1, division);
        Vector3[] array = new Vector3[division + 1];
        // B(t) = (1-t)^3 * startPosition + 3 * (1-t)^2 * t * startTangent + 3 * (1-t) * t^2 * endTangent + t^3 * endPosition, t=[0,1]
        for (int i = 0; i <= division; i++)
        {
            float t = i / (float)division;
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            Vector3 p;
            p = uuu * p0;           //first point
            p += 3 * uu * t * p1;   //second point
            p += 3 * u * tt * p2;     //third point
            p += ttt * p3;            //fourth point 
            array[i] = p;
        }
        return array;
    }
}