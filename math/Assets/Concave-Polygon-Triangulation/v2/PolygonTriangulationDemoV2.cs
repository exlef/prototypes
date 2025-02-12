using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolygonTriangulationDemoV2 : MonoBehaviour
{
    private List<Vector2> points;
    private Line line;
    private GameObject go;
    void Start()
    {
        points = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            Vector2 pos = transform.GetChild(i).position;
            points.Add(pos);
        }
        
        GenerateMesh();
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
                    points.Add(result.Value);
                    Debug.DrawRay((Vector3)result, Vector3.up, Color.red, 100);
                }
            }
        }
        
        GenerateMesh();
    }

    void GenerateMesh()
    {
        points = SortPointsClockWise(points);

        foreach (var point in points)
        {
            Debug.Log(point);
        }
        
        // Get triangulation indices
        List<int> triangles = EarClipping2D.Triangulate(points.ToArray());

        // Create a mesh from the results
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
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
    
    static List<Vector2> SortPointsClockWise(List<Vector2> points)
    {
        if (points.Count < 3)
            return points;

        // Calculate true centroid using polygon area
        Vector2 centroid = CalculatePolygonCentroid(points);
    
        // Convert points to angles and preserve original indices
        var pointAngles = new List<(Vector2 point, float angle)>();
    
        for (int i = 0; i < points.Count; i++)
        {
            // Calculate angle from centroid to point
            float angle = Mathf.Atan2(points[i].y - centroid.y, points[i].x - centroid.x);
            // Convert to degrees and normalize to 0-360 range
            angle = angle * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;
        
            pointAngles.Add((points[i], angle));
        }
    
        // Sort by angle
        return pointAngles.OrderBy(p => p.angle).Select(p => p.point).ToList();
    }

    static Vector2 CalculatePolygonCentroid(List<Vector2> points)
    {
        float area = 0;
        float cx = 0;
        float cy = 0;
    
        for (int i = 0; i < points.Count; i++)
        {
            int j = (i + 1) % points.Count;
            float factor = (points[i].x * points[j].y - points[j].x * points[i].y);
        
            area += factor;
            cx += (points[i].x + points[j].x) * factor;
            cy += (points[i].y + points[j].y) * factor;
        }
    
        area *= 0.5f;
        cx /= (6 * area);
        cy /= (6 * area);
    
        return new Vector2(cx, cy);
    }
    
    // static List<Vector2> SortPointsClockWise(List<Vector2> points)
    // {
    //     Vector2 center = CalculateCentroid(points);
    //     List<(Vector2, float)> PointsWithAngles = new();
    //     foreach (var point in points)
    //     {
    //         var angle = Vector2.SignedAngle(Vector2.up, point - center);
    //         angle -= Mathf.Sign(angle) > 0 ? 360 : 0;
    //         angle *= Mathf.Sign(angle);
    //         PointsWithAngles.Add((point, angle));
    //     }
    //     
    //     // sort PointsWithAngles list based on angle. from smallest to biggest. 
    //     PointsWithAngles = PointsWithAngles.OrderBy(p => p.Item2).ToList();
    //
    //     // Extract the sorted points back into a list
    //     List<Vector2> sortedPoints = PointsWithAngles.Select(p => p.Item1).ToList();
    //
    //     return sortedPoints;
    // }
    
    // static List<Vector2> SortPointsClockWise(List<Vector2> points)
    // {
    //     Vector2 center = CalculateCentroid(points);
    //
    //     // return points.OrderBy(p => Mathf.Atan2(p.y - center.y, p.x - center.x)).ToList();
    //     // return points.OrderByDescending(p => Mathf.Atan2(p.y - center.y, p.x - center.x)).ToList();
    //     return points.OrderBy(p => Mathf.Atan2(p.y - center.y, p.x - center.x)).Reverse().ToList();
    // }
    //
    //
    // static Vector2 CalculateCentroid(List<Vector2> points)
    // {
    //     float x = 0;
    //     float y = 0;
    //
    //     for (int i = 0; i < points.Count; i++)
    //     {
    //         x += points[i].x;
    //         y += points[i].y;
    //     }
    //
    //     x = x / points.Count;
    //     y = y / points.Count;
    //
    //     return new(x, y);
    // }
    
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

struct Line
{
    public Vector2 start;
    public Vector2 finish;
}
