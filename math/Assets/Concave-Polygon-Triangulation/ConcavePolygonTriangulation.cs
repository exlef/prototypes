using System;
using System.Collections.Generic;
using UnityEngine;

public class ConcavePolygonTriangulation : MonoBehaviour
{
    [SerializeField] Transform shapeTr;
    private void OnDrawGizmos()
    {
        if(!shapeTr) return;
        List<Transform> points = new List<Transform>();
        for (int i = 0; i < shapeTr.childCount; i++)
        {
            points.Add(shapeTr.GetChild(i));
        }

        for (int i = 1; i < points.Count; i++)
        {
            Debug.DrawLine(points[i-1].position, points[i].position);
            if(i == points.Count-1) Debug.DrawLine(points[i].position, points[0].position);
        }
    }
}
