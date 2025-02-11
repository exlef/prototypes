using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class test : MonoBehaviour
{
    public float angle;
    private void Update()
    {
        Vector2 center = Vector2.zero;
        Vector2 pos = GetMousePosInWorld2D();
        Debug.DrawLine(center, pos);
        angle = Vector2.SignedAngle(Vector2.up, pos - center);
        angle *= Mathf.Sign(angle);
    }
    
    private List<Vector2> points;
    
    private void Start()
    {
        points = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            Vector2 pos = transform.GetChild(i).position;
            points.Add(pos);
        }

        // points.Reverse();

        foreach (var point in points)
        {
            Debug.Log(point);
        }

        points = SortPointsClockWise(points);

        Debug.Log("-------------");
        
        foreach (var point in points)
        {
            Debug.Log(point);
        }
    }

    static List<Vector2> SortPointsClockWise(List<Vector2> points)
    {
        Vector2 center = CalculateCentroid(points);
        List<(Vector2, float)> PointsWithAngles = new();
        foreach (var point in points)
        {
            var angle = Vector2.SignedAngle(Vector2.up, point - center);
            PointsWithAngles.Add((point, angle));
        }
        
        // sort PointsWithAngles list based on angle. from smallest to biggest. 
        PointsWithAngles = PointsWithAngles.OrderBy(p => p.Item2).ToList();
    
        // Extract the sorted points back into a list
        List<Vector2> sortedPoints = PointsWithAngles.Select(p => p.Item1).ToList();
    
        return sortedPoints;
    }

    static Vector2 CalculateCentroid(List<Vector2> points)
    {
        float x = 0;
        float y = 0;

        for (int i = 0; i < points.Count; i++)
        {
            x += points[i].x;
            y += points[i].y;
        }

        x = x / points.Count;
        y = y / points.Count;

        return new(x, y);
    }
    
    static Vector2 GetMousePosInWorld2D()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
