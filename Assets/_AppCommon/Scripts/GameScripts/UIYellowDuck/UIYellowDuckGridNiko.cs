using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIYellowDuckGridNiko : MonoBehaviour {

    private UIYellowDuckRectNiko[,] grid;
    //网格大小/在该游戏中设置为（18，10）
    public Vector2 gridSize=new Vector2 (20,10);
    //矩形边长的一半
    public float rectHalfLength=0.1f;
    //矩形边长
    private float rectLength;
    //标志节点是否可行走
    public LayerMask WhatLayer = 1 << 17;
    //游戏玩家对象引用
    public Transform player;
    //横纵轴矩形数量
    public int gridCntX, gridCntY;

    //保存路径的列表
    public List<UIYellowDuckRectNiko> path = new List<UIYellowDuckRectNiko>();



	void Start () {        
        rectLength = rectHalfLength * 2;
        gridCntX = Mathf.RoundToInt(gridSize.x / rectLength);
        gridCntY = Mathf.RoundToInt(gridSize.y / rectLength);
        grid = new UIYellowDuckRectNiko[gridCntX, gridCntY];
        CreateGrid();
	}

    //创建网格
    private void CreateGrid()
    {
        //起始点，脚本所挂物体对象在屏幕中心，所以需要进行运算将起始点设置为左下角
        Vector3 startPoint = transform.position - gridSize.x / 2 * Vector3.right - Vector3.up * gridSize.y / 2;
        for (int i = 0; i < gridCntX; i++)
        {
            for (int j = 0; j < gridCntY; j++)
            {
                Vector3 worldPoint = startPoint + Vector3.right * (i * rectLength + rectHalfLength) +
                    Vector3.up * (j * rectLength + rectHalfLength);
                bool walkable = !Physics2D.CircleCast(worldPoint, rectHalfLength/2, Vector2.zero, 0, WhatLayer);
                Rect rect = new Rect();
                rect.width = rectLength-0.05f;
                rect.height = rectLength - 0.05f;
                rect.x = worldPoint.x;
                rect.y = worldPoint.y;
                grid[i, j] = new UIYellowDuckRectNiko(walkable, worldPoint, i, j, rect);
            }
        }
       
    }
	// Update is called once per frame
	void Update () {
		
	}
    public static void DrawRect(Rect rect, Color color)
    {
        Vector3[] line = new Vector3[5];
        line[0] = new Vector3(rect.x, rect.y, 0);
        line[1] = new Vector3(rect.x + rect.width, rect.y, 0);
        line[2] = new Vector3(rect.x + rect.width, rect.y + rect.height, 0);
        line[3] = new Vector3(rect.x, rect.y + rect.height, 0);
        line[4] = new Vector3(rect.x, rect.y, 0);
        if (line != null && line.Length > 0)
        {
            DrawLineHelper(line, color);
        }
    }
    private static void DrawLineHelper(Vector3[] line, Color color)
    {
        Gizmos.color = color;
        for (int i = 0; i < line.Length - 1; i++)
        {         
            Gizmos.DrawLine(line[i], line[i + 1]);            
        }
    }
    //画矩阵
   /*
    void OnDrawGizmos()
    { 
        //整个大的矩形窗口  
        Rect maxRect = new Rect ();
        maxRect.width = gridSize.x;
        maxRect.height= gridSize.y;
        maxRect.x=transform.position.x-gridSize.x/2;
        maxRect.y=transform.position.y-gridSize.y/2;
        DrawRect(maxRect, Color.white);

        if (grid == null) return;

        //绘制小矩形
        foreach (var rect in grid)
        {
            Color co = rect._canWalk ? Color.white : Color.red;
            DrawRect(rect._mRect, co);
        }      

        //获取游戏对象所在的矩形
        UIYellowDuckRectNiko playerRect = GetFromPostion(player.position);

        //绘制游戏对象矩形位置
        if (playerRect != null && playerRect._canWalk)
        {
            DrawRect(playerRect._mRect, Color.blue);
        }

        //绘制路径
        if (path != null)
        {
            foreach (var rect in path)
            {
                DrawRect(rect._mRect, Color.black);
            }
        }
    }
    */
    
     

    public UIYellowDuckRectNiko GetFromPostion(Vector3 postion)
    {
        float percentX = (postion.x + gridSize.x / 2) / gridSize.x;
        float percentY = (postion.y + gridSize.y / 2) / gridSize.y;

        //确保数值在0-1之间
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridCntX - 1) * percentX);
        int y = Mathf.RoundToInt((gridCntY - 1) * percentY);

        return grid[x, y];
    }

    public List<UIYellowDuckRectNiko> GetNeighbours(UIYellowDuckRectNiko rect)
    {
        List<UIYellowDuckRectNiko> neighbour = new List<UIYellowDuckRectNiko>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                //该邻居节点实际在表中的位置
                int tempX = rect._gridX + i;
                int tempY = rect._gridY + j;
                //判断是否越界
                if (tempX < gridCntX && tempX > 0 && tempY > 0 && tempY < gridCntY)
                {
                    neighbour.Add(grid[tempX, tempY]);
                }
            }
        }
        return neighbour;
    }
}
