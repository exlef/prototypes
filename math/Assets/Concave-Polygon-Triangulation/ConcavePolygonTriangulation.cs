using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConcavePolygonTriangulation : MonoBehaviour
{
    [SerializeField] Transform shapeTr;
    [SerializeField] [Range(0, 4)] int selectedPointIndex;
    static readonly List<Point> points = new List<Point>();
    
    [InitializeOnLoadMethod]
    static void Init()
    {
        Transform tr = FindAnyObjectByType<ConcavePolygonTriangulation>().transform;
        points.Clear();
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
            Debug.DrawLine(points[i-1].pos, points[i].pos);
            if(i == points.Count-1) Debug.DrawLine(points[i].pos, points[0].pos);
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

        
    }
    
    
    [Button]
    void Triangulate()
    {
        Point p = GetSelectedPoint();
        if (p.IsReflex()) {Debug.Log("the Vertex is reflex"); return;}
        Debug.DrawLine(p.adj1.pos, p.adj2.pos, Color.yellow, 5f);
        p.adj1.SwapAdjacent(p, p.adj2);
        p.adj2.SwapAdjacent(p, p.adj1);
    }

    [Button]
    void Refresh()
    {
        Debug.Log("refresh");
        Init();
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
        if(adj1 == null || adj2 == null) {throw new System.NotSupportedException("there is no adjacent vertex");}
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
}
