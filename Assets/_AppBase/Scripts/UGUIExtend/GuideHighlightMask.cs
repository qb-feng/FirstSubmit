namespace UnityEngine.UI
{
    public class GuideHighlightMask : MaskableGraphic//, ICanvasRaycastFilter
    {
        public RectTransform arrow;
        public Vector2 center = Vector2.zero;
        public Vector2 size = new Vector2(100, 100);

        public void DoUpdate()
        {
            // 当引导箭头位置或者大小改变后更新，注意：未处理拉伸模式
            if (arrow && center != arrow.anchoredPosition || size != arrow.sizeDelta)
            {
                this.center = arrow.anchoredPosition;
                this.size = arrow.sizeDelta;
                SetAllDirty();
            }
        }

        //public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        //{
        //    // 点击在箭头框内部则无效，否则生效
        //    return !RectTransformUtility.RectangleContainsScreenPoint(arrow, sp, eventCamera);
        //}

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            var r = GetPixelAdjustedRect();
            var outer = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);

            Vector4 inner = new Vector4(center.x - size.x / 2,
                                        center.y - size.y / 2,
                                        center.x + size.x * 0.5f,
                                        center.y + size.y * 0.5f);

            vh.Clear();
            var vert = UIVertex.simpleVert;
            vert.color = color;

            // left
            var verts = new UIVertex[4];
            vert.position = new Vector2(outer.x, outer.y);
            //vh.AddVert(vert);
            verts[0] = vert;

            vert.position = new Vector2(outer.x, outer.w);
            //vh.AddVert(vert);
            verts[1] = vert;

            vert.position = new Vector2(inner.x, outer.w);
            //vh.AddVert(vert);
            verts[2] = vert;

            vert.position = new Vector2(inner.x, outer.y);
            //vh.AddVert(vert);
            verts[3] = vert;

            vh.AddUIVertexQuad(verts);

            // top
            verts = new UIVertex[4];
            vert.position = new Vector2(inner.x, inner.w);
            //vh.AddVert(vert);
            verts[0] = vert;

            vert.position = new Vector2(inner.x, outer.w);
            //vh.AddVert(vert);
            verts[1] = vert;

            vert.position = new Vector2(inner.z, outer.w);
            //vh.AddVert(vert);
            verts[2] = vert;

            vert.position = new Vector2(inner.z, inner.w);
            //vh.AddVert(vert);
            verts[3] = vert;

            vh.AddUIVertexQuad(verts);

            // right
            verts = new UIVertex[4];
            vert.position = new Vector2(inner.z, outer.y);
            //vh.AddVert(vert);
            verts[0] = vert;

            vert.position = new Vector2(inner.z, outer.w);
            //vh.AddVert(vert);
            verts[1] = vert;

            vert.position = new Vector2(outer.z, outer.w);
            //vh.AddVert(vert);
            verts[2] = vert;

            vert.position = new Vector2(outer.z, outer.y);
            //vh.AddVert(vert);
            verts[3] = vert;

            vh.AddUIVertexQuad(verts);

            // bottom
            verts = new UIVertex[4];
            vert.position = new Vector2(inner.x, outer.y);
            //vh.AddVert(vert);
            verts[0] = vert;

            vert.position = new Vector2(inner.x, inner.y);
            //vh.AddVert(vert);
            verts[1] = vert;

            vert.position = new Vector2(inner.z, inner.y);
            //vh.AddVert(vert);
            verts[2] = vert;

            vert.position = new Vector2(inner.z, outer.y);
            //vh.AddVert(vert);
            verts[3] = vert;

            vh.AddUIVertexQuad(verts);

            //vh.AddTriangle(0, 1, 2);
            //vh.AddTriangle(2, 3, 0);
            //vh.AddTriangle(4, 5, 6);
            //vh.AddTriangle(6, 7, 4);
            //vh.AddTriangle(8, 9, 10);
            //vh.AddTriangle(10, 11, 8);
            //vh.AddTriangle(12, 13, 14);
            //vh.AddTriangle(14, 15, 12);
        }

        void Update()
        {
            //DoUpdate();
        }

        void LateUpdate()
        {
            DoUpdate();
        }
    }
}