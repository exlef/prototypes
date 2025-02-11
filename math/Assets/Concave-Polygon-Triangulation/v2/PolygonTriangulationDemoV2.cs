using System;
using System.Collections.Generic;
using UnityEngine;

public class PolygonTriangulationDemoV2 : MonoBehaviour
{
    private Vector2[] points;
    private Line line;
    private GameObject go;
    void Start()
    {
        points = new Vector2[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            Vector2 pos = transform.GetChild(i).position;
            points[i] = pos;
        }
        
        // Get triangulation indices
        List<int> triangles = EarClipping2D.Triangulate(points);

        // Create a mesh from the results
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vertices[i] = points[i];
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        
        go = new GameObject();
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        
        var renderer2 = go.GetComponent<MeshRenderer>();
        renderer2.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        go.GetComponent<MeshFilter>().mesh = mesh;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            line.start = GetMousePosInWorld2D();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            line.finish = GetMousePosInWorld2D();
            CutMesh(line.start, line.finish);
        }
    }

    void CutMesh(Vector2 start, Vector2 finish)
    {
        Debug.DrawLine(start, finish, Color.red, 100);
        var vertices = go.GetComponent<MeshFilter>().mesh.vertices;
        var triangles = go.GetComponent<MeshFilter>().mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int vertexIndexA = triangles[i];
            int vertexIndexB = triangles[i + 1];
            int vertexIndexC = triangles[i + 2];

            var a = vertices[vertexIndexA];
            var b = vertices[vertexIndexB];
            var c = vertices[vertexIndexC];

            Vector2?[] results = new Vector2?[3];
            
            results[0] = GetIntersectionPoint(a, b, start, finish);
            results[1] =  GetIntersectionPoint(a, c, start, finish);
            results[2] =  GetIntersectionPoint(b, c, start, finish);

            foreach (var result in results)
            {
                if (result.HasValue)
                {
                    Debug.DrawRay((Vector3)result, Vector3.up, Color.red, 100);
                }
            }
           
            
        }
    }

    struct Line
    {
        public Vector2 start;
        public Vector2 finish;
    }
    
    static Vector2 GetMousePosInWorld2D()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    
    private static Vector2? GetIntersectionPoint(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        // Line p1p2 represented as a1x + b1y = c1
        float a1 = p2.y - p1.y; // Coefficient for x
        float b1 = p1.x - p2.x; // Coefficient for y
        float c1 = a1 * p1.x + b1 * p1.y; // Constant term

        // Line p3p4 represented as a2x + b2y = c2
        float a2 = p4.y - p3.y;
        float b2 = p3.x - p4.x;
        float c2 = a2 * p3.x + b2 * p3.y;

        // Calculate the determinant
        float determinant = a1 * b2 - a2 * b1;

        if (determinant == 0)
        {
            // Lines are parallel or coincident
            return null; // No intersection
        }
        else
        {
            // Lines intersect at a single point
            float x = (b2 * c1 - b1 * c2) / determinant;
            float y = (a1 * c2 - a2 * c1) / determinant;

            Vector2 intersectionPoint = new Vector2(x, y);

            // Check if the intersection point is within the bounds of both line segments
            if (IsPointOnSegment(intersectionPoint, p1, p2) && IsPointOnSegment(intersectionPoint, p3, p4))
            {
                return intersectionPoint; // Return the intersection point
            }
            else
            {
                return null; // Intersection point is outside the segments
            }
        }
    }
    
    private static bool IsPointOnSegment(Vector2 point, Vector2 start, Vector2 end)
    {
        // Check if the point is within the bounding box of the segment
        return (point.x >= Mathf.Min(start.x, end.x) && point.x <= Mathf.Max(start.x, end.x) &&
                point.y >= Mathf.Min(start.y, end.y) && point.y <= Mathf.Max(start.y, end.y));
    }
}
