using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIPropWidget : Graphic
{
    private enum AnimationStatus
    {
        NOT_START,
        ANIMATING,
        FINISH,
    }

    public enum PolygonType
    {
        FivePolygon = 5,
        SixPolygon = 6,
        EightPolygon = 8,
    }


    public PolygonType m_polygonType;
    public List<RectTransform> _maxPropVector;
    public bool _withAnimation = true;
    private int _vertex_size;

    private const float ANIMATION_TIME = 0.8f;
    private const float MAX_PROP_VALUE = 1.0f;

    private List<Vector2> _propList = new List<Vector2>();
    private List<Vector2> _currentList = new List<Vector2>();

    private List<float> m_attributeValue;

    private bool _isStartAnimation = false;
    private bool _isAnimationFinish = false;
    private bool _isSetValue = false;

    private float _startTime = 0;
    //private float _currentTime = 0;

    protected override void Awake()
    {
        _isStartAnimation = false;
        _isAnimationFinish = false;
        _isSetValue = false;
        _vertex_size = (int)m_polygonType * 3;

        for (int i = 0; i < _vertex_size; ++i)
        {
            _propList.Add(Vector2.zero);
            _currentList.Add(Vector2.zero);
        }
    }

    // 设置属性值
    public void SetPropList(List<float> attribute, bool withAnimation = false)
    {
        if (attribute.Count < 5)
        {
            Debug.LogError("目前只支持五个, 六个, 八个属性!!!");
            return;
        }
        m_attributeValue = attribute;
        switch (m_polygonType)
        {
            case PolygonType.FivePolygon:
                FillFivePolygon();
                break;
            case PolygonType.SixPolygon:
                FillSixPolygon();
                break;
            case PolygonType.EightPolygon:
                FillEightPolygon();
                break;
        }

        _isSetValue = true;

        if (withAnimation)
        {
            PlayAnimation();
        }
        else
        {
            for (int i = 0; i < _vertex_size; ++i)
            {
                _currentList[i] = _propList[i];
            }
        }

        SetVerticesDirty();
    }

    private void SD(int prop, int maxV, int attri)
    {
        var vert = (_maxPropVector[maxV].anchoredPosition - Vector2.zero) * m_attributeValue[attri] / MAX_PROP_VALUE;
        _propList[prop] = vert;
    }

    private void SC(int prop)
    {
        _propList[prop] = Vector2.zero;
    }

    /* 
     *  左下角开始依次属性0,1,2,3,4
    */
    private void FillFivePolygon()
    {
        //五边形分为五个三角形,从左下开始
        SC(0);
        SD(1, 0, 0);
        SD(2, 1, 1);

        SC(3);
        SD(4, 1, 1);
        SD(5, 2, 2);

        SC(6);
        SD(7, 2, 2);
        SD(8, 3, 3);

        SC(9);
        SD(10, 3, 3);
        SD(11, 4, 4);

        SC(12);
        SD(13, 4, 4);
        SD(14, 0, 0);
    }


    /*       
     * 左下角开始依次属性0,1,2,3,4,5
    */
    private void FillSixPolygon()
    {
        //六边形分为六个三角形,从左下开始
        SC(0);
        SD(1, 0, 0);
        SD(2, 1, 1);

        SC(3);
        SD(4, 1, 1);
        SD(5, 2, 2);

        SC(6);
        SD(7, 2, 2);
        SD(8, 3, 3);

        SC(9);
        SD(10, 3, 3);
        SD(11, 4, 4);

        SC(12);
        SD(13, 4, 4);
        SD(14, 5, 5);

        SC(15);
        SD(16, 5, 5);
        SD(17, 0, 0);
    }

    /*
     * 左下角开始依次属性0,1,2,3,4,5,6,7
    */
    private void FillEightPolygon()
    {
        //八边形分为八个三角形,从左下开始
        SC(0);
        SD(1, 0, 0);
        SD(2, 1, 1);

        SC(3);
        SD(4, 1, 1);
        SD(5, 2, 2);

        SC(6);
        SD(7, 2, 2);
        SD(8, 3, 3);

        SC(9);
        SD(10, 3, 3);
        SD(11, 4, 4);

        SC(12);
        SD(13, 4, 4);
        SD(14, 5, 5);

        SC(15);
        SD(16, 5, 5);
        SD(17, 6, 6);

        SC(18);
        SD(19, 6, 6);
        SD(20, 7, 7);

        SC(21);
        SD(22, 7, 7);
        SD(23, 0, 0);
    }

    // 开始播放动画
    public void PlayAnimation()
    {
        _isAnimationFinish = false;
        _isStartAnimation = true;
        _startTime = Time.time;
    }

    void Update()
    {
        if (_isAnimationFinish || !_isSetValue || !_isStartAnimation)
        {
            return;
        }

        // 动画播放完毕
        if (Time.time - _startTime >= ANIMATION_TIME)
        {
            for (int i = 0; i < _vertex_size; ++i)
            {
                _currentList[i] = _propList[i];
            }

            _isAnimationFinish = true;
            return;
        }

        // 更新当前动画的数据
        float percent = (Time.time - _startTime) / ANIMATION_TIME;
        for (int i = 0; i < _vertex_size; ++i)
        {
            _currentList[i] = _propList[i] * percent;
        }

        SetVerticesDirty();
    }

    private List<int> UpdateVertex(List<UIVertex> vbo, List<Vector2> current)
    {
        var triangles = new List<int>();
        for (int i = 0; i < current.Count; ++i)
        {
            var vert = UIVertex.simpleVert;
            vert.color = color;
            vert.position = current[i];
            vbo.Add(vert);
            triangles.Add(i);
        }
        return triangles;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        // 尚未赋值，不用绘制
        if (!_isSetValue)
        {
            return;
        }
        var vbo = new List<UIVertex>();
        var triangles = UpdateVertex(vbo, _currentList);
        vh.AddUIVertexStream(vbo, triangles);
    }

    public bool m_draw = false;             //是否一直绘制不要求选择对象
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
    public Vector3[] GetPoints()
    {
        Vector3[] points = null;
        if (_maxPropVector != null)
        {
            points = new Vector3[_maxPropVector.Count + 1];
            for (int i = 0; i < _maxPropVector.Count; i++)
            {
                points[i] = _maxPropVector[i].position;
            }
            points[_maxPropVector.Count] = _maxPropVector[0].position;
        }
        return points;
    }
}