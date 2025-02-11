using UnityEngine;
using System.Collections.Generic;

public class EarClipping
{
    private class Vertex
    {
        public Vector2 Position;
        public int Index;
        public Vertex Prev;
        public Vertex Next;

        public Vertex(Vector2 position, int index)
        {
            Position = position;
            Index = index;
        }
    }
    
    public static List<int> Triangulate(Vector2[] points)
    {
        List<int> triangles = new List<int>();
        if (points.Length < 3) return triangles;

        // Create vertex list and link them
        List<Vertex> vertices = new List<Vertex>();
        for (int i = 0; i < points.Length; i++)
        {
            vertices.Add(new Vertex(points[i], i));
        }

        // Create doubly linked list
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i].Prev = vertices[(i + vertices.Count - 1) % vertices.Count];
            vertices[i].Next = vertices[(i + 1) % vertices.Count];
        }

        // Check if polygon is clockwise
        if (!IsClockwise(vertices))
        {
            vertices.Reverse();
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i].Prev = vertices[(i + vertices.Count - 1) % vertices.Count];
                vertices[i].Next = vertices[(i + 1) % vertices.Count];
            }
        }
        
        int maxIterations = vertices.Count * vertices.Count;
        int remainingTriangles = vertices.Count - 2;
        Vertex ear = vertices[0];

        while (remainingTriangles > 0 && maxIterations > 0)
        {
            if (IsEar(ear))
            {
                // Cut the ear
                triangles.Add(ear.Prev.Index);
                triangles.Add(ear.Index);
                triangles.Add(ear.Next.Index);

                // Update links
                ear.Prev.Next = ear.Next;
                ear.Next.Prev = ear.Prev;

                remainingTriangles--;
            }
            ear = ear.Next;
            maxIterations--;
        }

        return triangles;
    }

    private static bool IsClockwise(List<Vertex> vertices)
    {
        float signedArea = 0;
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 current = vertices[i].Position;
            Vector2 next = vertices[(i + 1) % vertices.Count].Position;
            signedArea += (next.x - current.x) * (next.y + current.y);
        }
        return signedArea > 0;
    }

    private static bool IsEar(Vertex vertex)
    {
        Vector2 a = vertex.Prev.Position;
        Vector2 b = vertex.Position;
        Vector2 c = vertex.Next.Position;

        // Check if triangle is valid (not degenerate)
        if (IsTriangleDegenerate(a, b, c)) return false;

        // Check if triangle is counter-clockwise
        if (!IsTriangleClockwise(a, b, c)) return false;

        // Check if any other vertex is inside this triangle
        Vertex current = vertex.Next.Next;
        while (current != vertex.Prev)
        {
            if (IsPointInTriangle(current.Position, a, b, c))
                return false;
            current = current.Next;
        }

        return true;
    }

    private static bool IsTriangleDegenerate(Vector2 a, Vector2 b, Vector2 c)
    {
        float area = ((b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y)) * 0.5f;
        return Mathf.Abs(area) < Mathf.Epsilon;
    }

    private static bool IsTriangleClockwise(Vector2 a, Vector2 b, Vector2 c)
    {
        float crossProduct = (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
        return crossProduct < 0;
    }

    private static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float area = 0.5f * (-b.y * c.x + a.y * (-b.x + c.x) + a.x * (b.y - c.y) + b.x * c.y);
        float s = 1 / (2 * area) * (a.y * c.x - a.x * c.y + (c.y - a.y) * p.x + (a.x - c.x) * p.y);
        float t = 1 / (2 * area) * (a.x * b.y - a.y * b.x + (a.y - b.y) * p.x + (b.x - a.x) * p.y);
        
        return s >= 0 && t >= 0 && (1 - s - t) >= 0;
    }
}