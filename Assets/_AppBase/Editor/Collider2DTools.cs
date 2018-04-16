using UnityEditor;
using UnityEngine;

public class Collider2DTools : Editor
{
    [MenuItem("Collider2DTools/BoxCollider2D Match Size")]
    public static void BoxCollider2DMatchSize()
    {
        GameObject[] targets = Selection.gameObjects;
        if (targets != null)
        {
            foreach (GameObject item in targets)
            {
                var box = item.GetComponent<BoxCollider2D>();
                if (box == null)
                {
                    box = item.AddComponent<BoxCollider2D>();
                }
                var rt = item.transform as RectTransform;
                var offset = Vector2.one * 0.5f - rt.pivot;
                box.size = rt.rect.size;
                box.offset = new Vector2(offset.x * rt.rect.size.x, offset.y * rt.rect.size.y);
            }
        }
    }

    [MenuItem("Collider2DTools/PolygonCollider2D Match Size")]
    public static void PolygonCollider2DMatchSize()
    {
        GameObject[] targets = Selection.gameObjects;
        if (targets != null)
        {
            foreach (GameObject item in targets)
            {
                var polygon = item.GetComponent<PolygonCollider2D>();
                if (polygon == null)
                {
                    polygon = item.AddComponent<PolygonCollider2D>();
                }
                var rt = item.transform as RectTransform;
                var v3 = new Vector3[4];
                var v2 = new Vector2[4];
                rt.GetLocalCorners(v3);
                for (int i = 0; i < v3.Length; i++)
                {
                    v2[i] = v3[i];
                }
                polygon.points = v2;
                polygon.offset = Vector2.zero;
            }
        }
    }

    [MenuItem("Collider2DTools/EdgeCollider2D Left Match Size")]
    public static void EdgeCollider2DLeft()
    {
        EdgeCollider2DInternal(RectTransform.Edge.Left);
    }

    [MenuItem("Collider2DTools/EdgeCollider2D Right Match Size")]
    public static void EdgeCollider2DRight()
    {
        EdgeCollider2DInternal(RectTransform.Edge.Right);
    }

    [MenuItem("Collider2DTools/EdgeCollider2D Top Match Size")]
    public static void EdgeCollider2DTop()
    {
        EdgeCollider2DInternal(RectTransform.Edge.Top);
    }

    [MenuItem("Collider2DTools/EdgeCollider2D Bottom Match Size")]
    public static void EdgeCollider2DBottom()
    {
        EdgeCollider2DInternal(RectTransform.Edge.Bottom);
    }

    private static void EdgeCollider2DInternal(RectTransform.Edge edge)
    {
        GameObject[] targets = Selection.gameObjects;
        if (targets != null)
        {
            foreach (GameObject item in targets)
            {
                var edge2D = item.AddComponent<EdgeCollider2D>();
                var rt = item.transform as RectTransform;
                var v3 = new Vector3[4];
                var v2 = new Vector2[4];
                rt.GetLocalCorners(v3);
                for (int i = 0; i < v3.Length; i++)
                {
                    v2[i] = v3[i];
                }
                int first = 0;
                int second = 0;
                switch (edge)
                {
                    case RectTransform.Edge.Bottom:
                        first = 3;
                        second = 0;
                        break;
                    case RectTransform.Edge.Left:
                        first = 0;
                        second = 1;
                        break;
                    case RectTransform.Edge.Right:
                        first = 2;
                        second = 3;
                        break;
                    case RectTransform.Edge.Top:
                        first = 1;
                        second = 2;
                        break;
                }
                edge2D.points = new Vector2[] { v2[first], v2[second] };
                edge2D.offset = Vector2.zero;
            }
        }
    }
}