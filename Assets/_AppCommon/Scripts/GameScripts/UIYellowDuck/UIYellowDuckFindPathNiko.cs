using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIYellowDuckFindPathNiko : MonoBehaviour {


    public Transform player;
    public Vector3 Endpoint;

    //获取整个网络
    private UIYellowDuckGridNiko _grid;

	void Start () {
        _grid = GetComponent<UIYellowDuckGridNiko>();
	}
	

	void Update () {
       // FindingPath(player.position, Endpoint);
	}

    public void FindingPath(Vector3 StartPos, Vector3 EndPos)
    {
        UIYellowDuckRectNiko startRect = _grid.GetFromPostion(StartPos);
        UIYellowDuckRectNiko EndRect = _grid.GetFromPostion(EndPos);
        List<UIYellowDuckRectNiko> openSet = new List<UIYellowDuckRectNiko>();
        HashSet<UIYellowDuckRectNiko> closeSet = new HashSet<UIYellowDuckRectNiko>();

        openSet.Add(startRect);

        while (openSet.Count > 0)
        {
            UIYellowDuckRectNiko currentRect = openSet[0];

            for (int i = 0; i < openSet.Count; i++)
            {
                if(openSet[i].fCost<currentRect.fCost||
                    openSet[i].fCost == currentRect.fCost && openSet[i].hCost < currentRect.hCost)
                {
                    currentRect = openSet[i];
                }
            }

            openSet.Remove(currentRect);
            closeSet.Add(currentRect);

            if (currentRect == EndRect)
            {
                //生成路径
                GeneratePath(startRect, EndRect);
                return;
            }

            //A*后续部分
            foreach (var rect in _grid.GetNeighbours(currentRect))
            {
                if (!rect._canWalk || closeSet.Contains(rect)) continue;
                //当前格子的相邻格子 与 开始格子之间的距离
                int newCost = currentRect.gCost + GetDistanceRects(currentRect,rect);
                //重新更新距离值
                if (newCost < rect.gCost || !openSet.Contains(rect))
                {
                    rect.gCost = newCost;
                    rect.hCost = GetDistanceRects(rect, EndRect);
                    rect.parent = currentRect;
                    if(!openSet.Contains(rect))
                    {
                        openSet.Add(rect);
                    }
                }

            }
        }
    }

    private void GeneratePath(UIYellowDuckRectNiko startRect,UIYellowDuckRectNiko endRect)
    {
        List<UIYellowDuckRectNiko> path = new List<UIYellowDuckRectNiko>();
        UIYellowDuckRectNiko temp = endRect;
        while (temp != startRect)
        {
            path.Add(temp);
            temp = temp.parent;
        }
        path.Reverse();
        _grid.path = path;
        
    }

    private int GetDistanceRects(UIYellowDuckRectNiko a, UIYellowDuckRectNiko b)
    {
        //判断两个节点在X轴上相差更多还是Y轴上相差更多
        int cntX = Mathf.Abs(a._gridX - b._gridX);
        int cntY = Mathf.Abs(a._gridY - b._gridY);
        if (cntX > cntY)
        {
            return 14 * cntY + 10 * (cntX - cntY);
        }
        else
        {
            return 14 * cntX + 10 * (cntY - cntX);
        }
    }
}
