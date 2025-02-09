using System;
using System.Collections.Generic;
using UnityEngine;

public class ConcavePolygonTriangulation : MonoBehaviour
{
    [SerializeField] Transform shapeTr;
    [SerializeField] [Range(0, 10)] int selectedPointIndex;

    private void OnDrawGizmos()
    {
        if(!shapeTr) return;
        List<Point> points = new List<Point>();
        for (int i = 0; i < shapeTr.childCount; i++)
        {
            Point p = new Point()
            {
                pos = shapeTr.GetChild(i).position,
            };
            points.Add(p);
        }
        
        foreach (var p in points)
        {
            Gizmos.DrawWireSphere(p.pos, 0.05f);
        }

        for (int i = 1; i < points.Count; i++)
        {
            Debug.DrawLine(points[i-1].pos, points[i].pos);
            if(i == points.Count-1) Debug.DrawLine(points[i].pos, points[0].pos);
        }

        for (int i = 0; i < points.Count; i++)
        {
            int adj1index = (i + 1) % points.Count;
            int adj2index = (i - 1 + points.Count) % points.Count;

            points[i].adj1 = points[adj1index];
            points[i].adj2 = points[adj2index];
        }

        selectedPointIndex = Mathf.Clamp(selectedPointIndex, 0, points.Count-1);
        Point selectedPoint = points[selectedPointIndex];
        // Draw points with appropriate colors
        foreach (var p in points)
        {
            if (p == selectedPoint)
                Gizmos.color = Color.red; // Selected point
            else if (p == selectedPoint?.adj1 || p == selectedPoint?.adj2)
                Gizmos.color = Color.green; // Adjacent points
            else
                Gizmos.color = Color.white; // Default color

            Gizmos.DrawWireSphere(p.pos, 0.05f);
        }
    }
}

class Point
{
    public Vector3 pos;
    public Point adj1;
    public Point adj2;
}
