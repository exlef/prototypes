using Unity.Mathematics;
using UnityEngine;
using Ex;
using System.Collections.Generic;

public class Demo : MonoBehaviour
{
    [SerializeField] Material circleMat;
    [SerializeField] Material lineMat;

    ExGraphics gfx;
    ResizableArray<Point> points = new();
    List<Vector2> pointPos = new();
    List<Vector4> lines;

    [SerializeField] Bounds bounds;


    void Start()
    {
        gfx = new(circleMat, lineMat);
        
        points.Add(new Point(0, 0));


        lines = new List<Vector4>
        {
            // Vector2.right * 2,
            // Vector2.left * 2,
        };
    }

    void Update()
    {

        for (int i = 0; i < points.Count; i++)
        {
            points.array[i].Move(bounds);
        }

        // render
        pointPos.Clear();
        for (int i = 0; i < points.Count; i++)
        {
            pointPos.Add(points.array[i].pos);
        }
        gfx.DrawCircles(pointPos);
        // gfx.DrawLines(lines);
    }

    void OnDestroy()
    {
        gfx?.Dispose(); 
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }

    struct Point
    {
        public float2 pos;
        float2 oldPos;
        float radius;

        public Point(float x, float y)
        {
            pos = new float2(x,y);
            oldPos = pos;
            oldPos = pos - new float2(0.2f, 0.2f); // for debugging. delete this.
            radius = 1;
        }

        public void Move(Bounds bounds)
        {
            float2 v = pos - oldPos;
            oldPos = pos;
            pos += v;

            float maxX = bounds.center.x + bounds.extents.x / 2 - radius;
            float minX = bounds.center.x - bounds.extents.x / 2 + radius;
            float maxY = bounds.center.y + bounds.extents.y / 2 - radius;
            float minY = bounds.center.y - bounds.extents.y / 2 + radius;

            if (pos.x > maxX)
            {
                pos.x = maxX;
                oldPos.x = pos.x + v.x;
            }
            else if (pos.x < minX)
            {
                pos.x = minX;
                oldPos.x = pos.x + v.x;
            }

            if(pos.y > maxY)
            {
                pos.y = maxY;
                oldPos.y = pos.y + v.y;
            }
            else if (pos.y < minY)
            {
                pos.y = minY;
                oldPos.y = pos.y + v.y;
            }
        }
    }
}
