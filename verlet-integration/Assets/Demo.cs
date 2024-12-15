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

    [SerializeField] Bounds bounds;

    void Start()
    {
        gfx = new(circleMat, lineMat);
        
        points.Add(new Point(0, 0));
    }

    void Update()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points.array[i].Update(bounds, Time.deltaTime);
        }

        // render
        pointPos.Clear();
        for (int i = 0; i < points.Count; i++)
        {
            pointPos.Add(points.array[i].pos);
        }
        gfx.DrawCircles(pointPos);
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
        float bounce;
        float friction;
        float2 gravity;

        public Point(float x, float y)
        {
            pos = new float2(x,y);
            oldPos = pos;
            oldPos = pos - new float2(1, 1);
            radius = 1;
            bounce = 0.9f;
            friction = 0.999f;
            gravity = new float2(0, -2.8f);
        }

        public void Update(Bounds bounds, float dt)
        {
            pos += gravity * dt;

            float2 v = (pos - oldPos) * friction;
            oldPos = pos;
            pos += v;


            float maxX = bounds.center.x + bounds.extents.x / 2 - radius;
            float minX = bounds.center.x - bounds.extents.x / 2 + radius;
            float maxY = bounds.center.y + bounds.extents.y / 2 - radius;
            float minY = bounds.center.y - bounds.extents.y / 2 + radius;

            if (pos.x > maxX)
            {
                pos.x = maxX;
                oldPos.x = pos.x + v.x * bounce;
            }
            else if (pos.x < minX)
            {
                pos.x = minX;
                oldPos.x = pos.x + v.x * bounce;
            }

            if (pos.y > maxY)
            {
                pos.y = maxY;
                oldPos.y = pos.y + v.y * bounce;
            }
            else if (pos.y < minY)
            {
                pos.y = minY;
                oldPos.y = pos.y + v.y * bounce;
            }
        }
    }
}
