using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPath : BaseDrawPoints
{
    public List<Transform> m_pointsList;
    private List<Vector3> m_pointsPosList = new List<Vector3>();

    public override Vector3[] GetPoints()
    {
        m_pointsPosList.Clear();
        if (m_pointsList != null)
        {
            foreach (var item in m_pointsList)
            {
                if (item)
                    m_pointsPosList.Add(item.position);
            }
        }
        return m_pointsPosList.ToArray();
    }
}
