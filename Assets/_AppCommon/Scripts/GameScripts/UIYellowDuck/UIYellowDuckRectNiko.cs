using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A*中每一个小矩形包含的信息类
public class UIYellowDuckRectNiko
{
    //该矩形区域是否可以行走
    public bool _canWalk;
    //该矩形在世界坐标系中坐标
    public Vector3 _worldPos;
    //自身矩形大小
    public Rect _mRect;
    //网格索引——在整个网格中的位置
    public int _gridX, _gridY;
    //与起始节点长度
    public int gCost;
    //与目标节点长度
    public int hCost;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    //父节点
    public UIYellowDuckRectNiko parent;
    //构造函数
    public UIYellowDuckRectNiko(bool CanWalk, Vector3 Postion, int x, int y,Rect m)
    {
        _canWalk = CanWalk;
        _worldPos = Postion;
        _gridX = x;
        _gridY = y;
        _mRect = m;
    }
}
