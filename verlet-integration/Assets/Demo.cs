using Unity.Mathematics;
using UnityEngine;
using Ex;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Demo : MonoBehaviour
{
    [SerializeField] Bounds bounds;
    [SerializeField] Material circleMat;
    [SerializeField] Material lineMat;
    Mesh mesh;

    ExGraphics gfx;

    List<Point> points = new();
    List<Stick> sticks = new();

    void Start()
    {
        gfx = new(circleMat, lineMat);

        mesh = GetComponent<MeshFilter>().mesh;

        foreach (var vert in mesh.vertices)
        {
            points.Add(new Point(vert.x, vert.y));
        }

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int index1 = mesh.triangles[i];
            int index2 = mesh.triangles[i + 1];
            int index3 = mesh.triangles[i + 2];

            sticks.Add(new Stick(points[index1], points[index2]));
            sticks.Add(new Stick(points[index2], points[index3]));
            sticks.Add(new Stick(points[index1], points[index3]));
        }
    }

    void FixedUpdate()
    {
        UpdatePoints();
        for (int i = 0; i < 4; i++)
        {
            UpdateSticks();
            ConstrainPointsToWorldBounds();
        }

        UpdateMeshVertices();
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

    void UpdateMeshVertices()
    {
        Vector3[] verts = mesh.vertices;

        for (int i = 0; i < verts.Length; i++)
        {
            var pos = points[i].pos;
            verts[i] = new Vector3(pos.x, pos.y, 0);
        }

        mesh.vertices = verts;
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
