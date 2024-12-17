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
    [SerializeField] int numCols = 20;
    [SerializeField] int numRows = 20;
    [SerializeField] float spacing = 0.2f;
    [SerializeField] float clothStrength = 2f;
    [SerializeField] Vector2 offset = new Vector2(1,1);

    Mesh mesh;

    ExGraphics gfx;

    List<Point> points = new();
    List<Stick> sticks = new();

    void Start()
    {
        gfx = new(circleMat, lineMat);

        mesh = GetComponent<MeshFilter>().mesh;

        CreateCloth(numCols, numRows, spacing, offset);
    }

    void CreateCloth(int numCols, int numRows, float spacing, Vector2 offset)
    {
        points.Clear();

        // Generate points in a grid layout
        for (int y = 0; y < numRows; y++)
        {
            for (int x = 0; x < numCols; x++)
            {
                // Create a point at position (x * spacing, y * spacing)
                var p = new Point(x * spacing + offset.x, y * spacing + offset.y);
                points.Add(p);
                if(y == numRows - 1)
                {
                    if (x == 0 || x == numCols - 1 || x % 2 == 0)
                    {
                        p.pinned = true;
                        p.anchored = true;
                    }
                }
            }
        }

        sticks.Clear();

        // Connect points horizontally (left to right)
        for (int y = 0; y < numRows; y++)
        {
            for (int x = 0; x < numCols - 1; x++)
            {
                Point pointA = points[y * numCols + x];
                Point pointB = points[y * numCols + x + 1];
                sticks.Add(new Stick(pointA, pointB));
            }
        }

        // Connect points vertically (top to bottom)
        for (int y = 0; y < numRows - 1; y++)
        {
            for (int x = 0; x < numCols; x++)
            {
                Point pointA = points[y * numCols + x];
                Point pointB = points[(y + 1) * numCols + x];
                sticks.Add(new Stick(pointA, pointB));
            }
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
        HandleMouseInput();
        DragPinnedPoints();
        BreakSticks();
        Render();
    }

    void BreakSticks()
    {
        List<int> brokeStickIndexes = new();
        for (int i = 0; i < sticks.Count; i++)
        {
            if(sticks[i].Length() > clothStrength)
            {
                brokeStickIndexes.Add(i);
            }
        }

        foreach (var index in brokeStickIndexes)
        {
            sticks.Remove(sticks[index]);
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Vector2 mousePos = Utils.MousePos2D();

            // Check if the mouse is near any point and pin it
            foreach (var point in points)
            {
                if (Vector2.Distance(mousePos, point.pos) < 0.2f) // Adjust the distance threshold as needed
                {
                    point.pinned = true; // Pin the point
                    break;
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) // Release the mouse button
        {
            foreach (var point in points)
            {
                if(point.anchored == false)
                    point.pinned = false; // Unpin all points
            }
        }
    }

    void DragPinnedPoints()
    {
        if (Input.GetMouseButton(0)) // While holding the left-click button
        {
            Vector2 mousePos = Utils.MousePos2D();

            // Move any pinned point with the mouse
            foreach (var point in points)
            {
                if (point.pinned && point.anchored == false)
                {
                    point.pos = mousePos; // Move the point to the mouse position
                }
            }
        }
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
        // pointPos.Clear();
        // for (int i = 0; i < points.Count; i++)
        // {
        //     pointPos.Add(points[i].pos);
        // }
        // gfx.DrawCircles(pointPos);

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
        public bool anchored;
        float2 oldPos;
        float radius; 
        float bounce;
        float friction;
        float2 gravity;
        
        public Point(float x, float y, bool isPinned = false)
        {
            pos = new float2(x,y);
            pinned = isPinned;
            anchored = false;
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
                // oldPos.x = pos.x + v.x * bounce;
            }
            else if (pos.x < minX)
            {
                pos.x = minX;
                // oldPos.x = pos.x + v.x * bounce;
            }

            if (pos.y > maxY)
            {
                pos.y = maxY;
                // oldPos.y = pos.y + v.y * bounce;
            }
            else if (pos.y < minY)
            {
                pos.y = minY;
                // oldPos.y = pos.y + v.y * bounce;
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

        public float Length()
        {
            Vector2 p0 = pointA.pos;
            Vector2 p1 = pointB.pos;
            float dx = p1.x - p0.x;
            float dy = p1.y - p0.y;
            float distance = math.sqrt(dx * dx + dy * dy);
            return distance;
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
