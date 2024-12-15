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
    ResizableArray<Stick> sticks = new();
    List<Vector2> pointPos = new();
    List<Vector4> lines = new();

    [SerializeField] Bounds bounds;

    void Start()
    {
        gfx = new(circleMat, lineMat);
        
        points.Add(new Point(0, -1));
        points.Add(new Point(2, 0));

        sticks.Add(new Stick(0, 1, points));
    }

    void FixedUpdate()
    {
        for (int i = 0; i < points.Count; i++)
        {
            points.array[i].Update(bounds, Time.deltaTime);
        }

        for (int i = 0; i < sticks.Count; i++)
        {
            sticks.array[i].Update();
        }
    }

    void Update()
    {
        // render
        pointPos.Clear();
        for (int i = 0; i < points.Count; i++)
        {
            pointPos.Add(points.array[i].pos);
        }
        gfx.DrawCircles(pointPos);
        lines.Clear();
        for (int i = 0; i < sticks.Count; i++)
        {
            Vector2 p0 = points.array[sticks.array[i].pAindex].pos;
            Vector2 p1 = points.array[sticks.array[i].pBindex].pos;

            lines.Add(new Vector4(p0.x, p0.y, p1.x, p1.y));
        }
        gfx.DrawLines(lines);
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
            radius = 0.1f;
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

    struct Stick
    {
        ResizableArray<Point> points;
        public int pAindex;
        public int pBindex;
        float length;

        public Stick(int pointAindex, int pointBindex, ResizableArray<Point> pointsArray)
        {
            points = pointsArray;
            pAindex = pointAindex;
            pBindex  = pointBindex;
            length = Vector2.Distance(pointsArray.array[pAindex].pos, pointsArray.array[pBindex].pos);
        }

        public void Update()
        {
            Vector2 p0 = points.array[pAindex].pos;
            Vector2 p1 = points.array[pBindex].pos;

            float dx = p1.x - p0.x;
            float dy = p1.y - p0.y;
            float distance = math.sqrt(dx * dx + dy * dy);
            float difference = length - distance;
            float percent = difference / distance / 2; // divide by two because each point will move
            float offsetX = dx * percent;
            float offsetY = dy * percent;

            p0.x -= offsetX;
            p0.y -= offsetY;
            p1.x += offsetX;
            p1.y += offsetY;

            points.array[pAindex].pos = p0;
            points.array[pBindex].pos = p1;
        }
    }
}
