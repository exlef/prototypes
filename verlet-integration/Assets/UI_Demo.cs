using UnityEngine;
using Ex;
using System.Collections.Generic;

public class UI_Demo : MonoBehaviour
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

        points.Add(new Point(0,0));
        points.Add(new Point(0, 1));
        sticks.Add(new Stick(points[0], points[1]));
    }

    void CreateRectangle()
    {
        points.Add(new Point(-1, 1)); // top     left  0
        points.Add(new Point(1, 1));  // top     right 1
        points.Add(new Point(-1, -1));// bottom  left  2
        points.Add(new Point(1, -1));  // bottom right 3
        points.Add(new Point(0, 0));  // center       4

        sticks.Add(new Stick(points[0], points[1]));
        sticks.Add(new Stick(points[0], points[2]));
        sticks.Add(new Stick(points[1], points[3]));
        sticks.Add(new Stick(points[2], points[3]));

        sticks.Add(new Stick(points[4], points[0]));
        sticks.Add(new Stick(points[4], points[1]));
        sticks.Add(new Stick(points[4], points[2]));
        sticks.Add(new Stick(points[4], points[3]));
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
        HandleMouseInput();
        DragPinnedPoints();
        Render();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Utils.MousePos2D();

            foreach (var point in points)
            {
                if (Vector2.Distance(mousePos, point.pos) < 0.2f)
                {
                    point.pinned = true;
                    break;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            foreach (var point in points)
            {
                point.pinned = false;
            }
        }
    }

    void DragPinnedPoints()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Utils.MousePos2D();

            foreach (var point in points)
            {
                if (point.pinned)
                {
                    point.pos = mousePos;
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
        public Vector2 pos;
        public Vector2 oldPos;
        public bool pinned;
        float radius;

        public Point(float x, float y, bool isPinned = false)
        {
            pos = new Vector2(x, y);
            pinned = isPinned;
            oldPos = pos;
        }

        public void Update()
        {
            if (pinned)
            {
                oldPos = pos; // otherwise the oldPos will be the its value when we pinned the point and when it got unpinned this will cause to point move unexpectedly since its oldPos is not updated correctly.
                return;
            }
            Vector2 v = pos - oldPos;
            oldPos = pos;
            pos += v;
        }

        public void ConstrainWorldBounds(Bounds bounds)
        {
            if (pinned) return;

            float maxX = bounds.center.x + bounds.extents.x / 2 - radius;
            float minX = bounds.center.x - bounds.extents.x / 2 + radius;
            float maxY = bounds.center.y + bounds.extents.y / 2 - radius;
            float minY = bounds.center.y - bounds.extents.y / 2 + radius;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }
    }

    class Stick
    {
        public Point pointA;
        public Point pointB;
        readonly float length;

        public Stick(Point a, Point b)
        {
            pointA = a;
            pointB = b;
            length = Vector2.Distance(pointA.pos, pointB.pos);
        }

        public void Update()
        {
            float dx = pointB.pos.x - pointA.pos.x;
            float dy = pointB.pos.y - pointA.pos.y;
            float distance = Vector2.Distance(pointA.pos, pointB.pos);
            float difference = length - distance;

            float percent = difference / distance;
            float offsetX = dx * percent / 2;
            float offsetY = dy * percent / 2;

            if (!pointA.pinned)
            {
                pointA.pos.x -= offsetX;
                pointA.pos.y -= offsetY;
            }
            if (!pointB.pinned)
            {
                pointB.pos.x += offsetX;
                pointB.pos.y += offsetY;
            }
        }

        // Helper method to rotate a vector by a given angle
        private Vector2 RotateVector2(Vector2 v, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float ca = Mathf.Cos(radians);
            float sa = Mathf.Sin(radians);
            return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
        }

        public void Update2()
        {
            // Length constraint
            float dx = pointB.pos.x - pointA.pos.x;
            float dy = pointB.pos.y - pointA.pos.y;
            float distance = Vector2.Distance(pointA.pos, pointB.pos);
            float difference = length - distance;

            float percent = difference / distance;
            float offsetX = dx * percent / 2;
            float offsetY = dy * percent / 2;

            // Angular constraint
            Vector2 originalDirectionA = (pointA.oldPos - pointB.oldPos).normalized;
            Vector2 currentDirectionA = (pointA.pos - pointB.pos).normalized;

            // Calculate the actual angle difference
            float angleDiff = Mathf.Abs(Vector2.Angle(originalDirectionA, currentDirectionA));
            float maxAngle = 15;

            // If rotation exceeds max angle, limit the rotation
            if (angleDiff > maxAngle)
            {
                // Determine rotation direction
                float cross = originalDirectionA.x * currentDirectionA.y - originalDirectionA.y * currentDirectionA.x;
                int rotationDirection = cross > 0 ? 1 : -1;

                // Rotate the original direction to the maximum allowed angle
                Vector2 limitedDirection = RotateVector2(originalDirectionA, rotationDirection * maxAngle);

                // Adjust position to maintain max angle and original length
                if (!pointA.pinned)
                {
                    pointA.pos = pointB.pos + limitedDirection * length;
                }
                else if (!pointB.pinned)
                {
                    pointB.pos = pointA.pos - limitedDirection * length;
                }

                // Recalculate offsets after angle constraint
                dx = pointB.pos.x - pointA.pos.x;
                dy = pointB.pos.y - pointA.pos.y;
                distance = Vector2.Distance(pointA.pos, pointB.pos);
                difference = length - distance;
                percent = difference / distance;
                offsetX = dx * percent / 2;
                offsetY = dy * percent / 2;
            }

            // Apply length constraint
            if (!pointA.pinned && !pointB.pinned)
            {
                pointA.pos.x -= offsetX;
                pointA.pos.y -= offsetY;
                pointB.pos.x += offsetX;
                pointB.pos.y += offsetY;
            }
            else if (!pointA.pinned)
            {
                pointA.pos.x -= offsetX * 2;
                pointA.pos.y -= offsetY * 2;
            }
            else if (!pointB.pinned)
            {
                pointB.pos.x += offsetX * 2;
                pointB.pos.y += offsetY * 2;
            }
        }
    }
}
