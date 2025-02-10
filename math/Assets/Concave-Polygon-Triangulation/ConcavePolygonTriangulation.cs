using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConcavePolygonTriangulation : MonoBehaviour
{
    [SerializeField] Transform shapeTr;
    [SerializeField] int selectedPointIndex;
    static readonly List<Point> points = new List<Point>();
    static readonly List<Line> lines = new List<Line>();
    
    [InitializeOnLoadMethod]
    static void Init()
    {
        Transform tr = FindAnyObjectByType<ConcavePolygonTriangulation>().transform;
        points.Clear();
        lines.Clear();
        for (int i = 0; i < tr.childCount; i++)
        {
            Point p = new Point()
            {
                pos = tr.GetChild(i).position,
            };
            points.Add(p);
        }
        
        for (int i = 0; i < points.Count; i++)
        {
            int adj1index = (i + 1) % points.Count;
            int adj2index = (i - 1 + points.Count) % points.Count;

            points[i].adj1 = points[adj1index];
            points[i].adj2 = points[adj2index];
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var p in points)
        {
            Gizmos.DrawWireSphere(p.pos, 0.05f);
        }
        
        for (int i = 1; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i-1].pos, points[i].pos);
            if(i == points.Count-1) Gizmos.DrawLine(points[i].pos, points[0].pos);
        }

        Point selectedPoint = GetSelectedPoint();
        // Draw points with appropriate colors
        foreach (var p in points)
        {
            if (p == selectedPoint)
                Gizmos.color = selectedPoint.IsReflex() ? Color.black : Color.red; // Selected point
            else if (p == selectedPoint?.adj1 || p == selectedPoint?.adj2)
                Gizmos.color = Color.green; // Adjacent points
            else
                Gizmos.color = Color.white; // Default color

            Gizmos.DrawWireSphere(p.pos, 0.05f);
        }

        foreach (var line in lines)
        {
            Gizmos.DrawLine(line.a, line.b);
        }
    }

    bool DoesTriangleContainsAnyPoint(Point p)
    {
        Vector3 a = p.adj1.pos;
        Vector3 b = p.adj2.pos;
        Vector3 c = p.pos;

        // Check all points except the selected point and its adjacent points
        foreach (var point in points)
        {
            if (point == p || point == p.adj1 || point == p.adj2)
                continue;    

            if (IsPointInTriangle(point.pos, a, b, c))
                return true;
        }
        return false;
    }
    
    bool IsPointInTriangle(Vector3 pt, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        // Convert to 2D for simplicity
        Vector2 p = new Vector2(pt.x, pt.z); 
        Vector2 a = new Vector2(v1.x, v1.z);
        Vector2 b = new Vector2(v2.x, v2.z);
        Vector2 c = new Vector2(v3.x, v3.z);

        float area = 0.5f * (-b.y * c.x + a.y * (-b.x + c.x) + a.x * (b.y - c.y) + b.x * c.y);
        float sign = area < 0 ? -1f : 1f;

        float s = (a.y * c.x - a.x * c.y + (c.y - a.y) * p.x + (a.x - c.x) * p.y) * sign;
        float t = (a.x * b.y - a.y * b.x + (a.y - b.y) * p.x + (b.x - a.x) * p.y) * sign;

        return s > 0 && t > 0 && (s + t) < 2 * area * sign;
    }
    
    
    [Button]
    void Triangulate()
    {
        Debug.Log("triangulate");
        Point p = GetSelectedPoint();
        if (p.IsReflex()) {Debug.Log("the Vertex is reflex"); return;}
        if (DoesTriangleContainsAnyPoint(p))
        {
            Debug.Log(DoesTriangleContainsAnyPoint(p));
            return;
        }
        lines.Add(new Line{a = p.adj1.pos, b = p.adj2.pos});
        
        p.adj1?.RemoveAdjacent(p);
        p.adj2?.RemoveAdjacent(p);
        p.adj1?.AddAdjacent(p.adj2);
        p.adj2?.AddAdjacent(p.adj1);
        
        // to make it draw newly added lines
        EditorApplication.RepaintHierarchyWindow();
        SceneView.RepaintAll();
    }
    
    [Button]
    void TriangulateAndNext()
    {
        Triangulate();
        
        selectedPointIndex++;
        selectedPointIndex = Mathf.Clamp(selectedPointIndex, 0, points.Count-1);
        
        EditorApplication.RepaintHierarchyWindow();
        SceneView.RepaintAll();
    }

    [Button]
    void TriangulateAll()
    {
        Debug.Log("Triangulate All");
        selectedPointIndex = 0;
        foreach (var point in points)
        {
            if (point.adj1 == null || point.adj2 == null) break;
            Triangulate();
            selectedPointIndex++;
        }
    }

    [Button]
    void Refresh()
    {
        Debug.Log("refresh");
        selectedPointIndex = 0;
        Init();
        EditorApplication.RepaintHierarchyWindow();
        SceneView.RepaintAll();
    }
    
    Point GetSelectedPoint()
    {
        selectedPointIndex = Mathf.Clamp(selectedPointIndex, 0, points.Count-1);
        return points[selectedPointIndex];
    }
}

class Point
{
    public Vector3 pos;
    public Point adj1;
    public Point adj2;
    
    public bool IsReflex()
    {
        if(adj1 == null || adj2 == null) {Debug.Log("there is no adjacent vertex"); return false;}
        Vector3 pToa1 = adj1.pos - pos;
        Vector3 pToa2 = adj2.pos - pos;
        float angle = Vector3.SignedAngle(pToa1, pToa2, Vector3.up);
        return angle < 0;
    }
    
    public void SwapAdjacent(Point oldAdj, Point newAdj)
    {
        if (adj1 == oldAdj)
        {
            adj1 = newAdj;
        }
        else if (adj2 == oldAdj)
        {
            adj2 = newAdj;
        }
    }

    public void RemoveAdjacent(Point adjToRemove)
    {
        if (adj1 == adjToRemove)
            adj1 = null;
        if (adj2 == adjToRemove)
            adj2 = null;
    }

    public void AddAdjacent(Point adjToAdd)
    {
        if (adj1 == null)
            adj1 = adjToAdd;
        if (adj2 == null)
            adj2 = adjToAdd;
    }
}

struct Line
{
    public Vector3 a;
    public Vector3 b;
}
