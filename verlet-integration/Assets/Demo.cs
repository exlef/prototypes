using Unity.Mathematics;
using UnityEngine;
using Ex;
using System.Collections.Generic;

public class Demo : MonoBehaviour
{
    [SerializeField] Bounds bounds;
    [SerializeField] Material circleMat;
    [SerializeField] Material lineMat;

    ExGraphics gfx;

    List<Point> points = new();
    List<Stick> sticks = new();

    void Start()
    {
        gfx = new(circleMat, lineMat);
        
        points.Add(new Point(-1, 1)); // top left 0
        points.Add(new Point(1, 1)); // top right 1
        points.Add(new Point(-1, -1)); // bottom left 2
        points.Add(new Point(1, -1)); // bottom right 3
        points.Add(new Point(-0.5f, 1)); // antena base left 4
        points.Add(new Point(0.5f, 1)); // antena base right 5
        points.Add(new Point(-0.5f, 1.5f, true)); // antena top left 6
        points.Add(new Point(0.5f, 1.5f)); // antena top right 7

        sticks.Add(new Stick(points[0], points[1]));
        sticks.Add(new Stick(points[2], points[3]));
        sticks.Add(new Stick(points[0], points[2]));
        sticks.Add(new Stick(points[1], points[3]));
        sticks.Add(new Stick(points[0], points[3]));
        sticks.Add(new Stick(points[4], points[6]));
        sticks.Add(new Stick(points[5], points[7]));
        sticks.Add(new Stick(points[4], points[0]));
        sticks.Add(new Stick(points[4], points[1]));



        // points[0].pos += new float2(1.0f, 0);
    }

    void FixedUpdate()
    {
        UpdatePoints();
        for (int i = 0; i < 2; i++)
        {
            UpdateSticks();
            ConstrainPointsToWorldBounds();
        }
    }

    void Update()
    {
        Render();
    }

    void OnDestroy()
    {
        gfx?.Dispose(); 
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }

    void UpdatePoints()
    {
        foreach (var point in points)
        {
            point.Update();
        }
    }

    void UpdateSticks()
    {
        foreach (var stick in sticks)
        {
            stick.Update();
        }
    }

    void ConstrainPointsToWorldBounds()
    {
        foreach (var point in points)
        {
            point.ConstrainWorldBounds(bounds);
        }
    }

    void Render()
    {
        List<Vector2> pointPos = new();
        List<Vector4> lines = new();
        pointPos.Clear();
        for (int i = 0; i < points.Count; i++)
        {
            pointPos.Add(points[i].pos);
        }
        gfx.DrawCircles(pointPos);

        lines.Clear();
        for (int i = 0; i < sticks.Count; i++)
        {
            Vector2 p0 = sticks[i].pointA.pos;
            Vector2 p1 = sticks[i].pointB.pos;

            lines.Add(new Vector4(p0.x, p0.y, p1.x, p1.y));
        }
        gfx.DrawLines(lines);
    }

    class Point
    {
        public float2 pos;
        public bool pinned;
        float2 oldPos;
        float radius;
        float bounce;
        float friction;
        float2 gravity;
        
        public Point(float x, float y, bool isPinned = false)
        {
            pos = new float2(x,y);
            pinned = isPinned;
            oldPos = pos;
            radius = 0.1f;
            bounce = 0.9f;
            friction = 0.999f;
            gravity = new float2(0, -0.1f);
        }

        public void Update()
        {
            if(pinned) return;
            float2 v = (pos - oldPos) * friction;
            oldPos = pos;
            pos += v;
            pos += gravity;
        }

        public void ConstrainWorldBounds(Bounds bounds)
        {
            if(pinned) return;
            float2 v = (pos - oldPos) * friction;

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

    class Stick
    {
        public Point pointA;
        public Point pointB;
        float length;

        public Stick(Point a, Point b)
        {
            pointA = a;
            pointB = b;
            length = Vector2.Distance(pointA.pos, pointB.pos);
        }

        public void Update()
        {
            Vector2 p0 = pointA.pos;
            Vector2 p1 = pointB.pos;

            float dx = p1.x - p0.x;
            float dy = p1.y - p0.y;
            float distance = math.sqrt(dx * dx + dy * dy);
            float difference = length - distance;
            float percent = difference / distance / 2; // divide by two because each point will move
            float offsetX = dx * percent;
            float offsetY = dy * percent;

            if(!pointA.pinned)
            {
                p0.x -= offsetX;
                p0.y -= offsetY;
            }
            if(!pointB.pinned)
            {
                p1.x += offsetX;
                p1.y += offsetY;
            }

            pointA.pos = p0;
            pointB.pos = p1;
        }
    }
}
