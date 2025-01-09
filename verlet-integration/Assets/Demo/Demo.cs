using UnityEngine;
using Ex;
using Ex.Verlet;
using System.Collections.Generic;

public class Demo : MonoBehaviour
{
    [SerializeField] Transform pointsParent;
    [SerializeField] Bounds bounds = new(Vector3.zero, new(12,12,0));

    List<Point> points = new();
    
    void Start()
    {
        for (int i = 0; i < pointsParent.childCount; i++)
        {
            var tr = pointsParent.GetChild(i);

            var p = new Point(tr);

            points.Add(p);
        }
    }

    void Update()
    {
        foreach (var p in points)
        {
            p.Tick();
        }

        foreach (var p1 in points)
        {
            foreach (var p2 in points)
            {
                var result = ExPhysics2d.SolveCircles(p1.pos, p1.radius, p2.pos, p2.radius);
                p1.tr.position = result.Item1;
                p2.tr.position = result.Item2;
            }
        }

        foreach (var p in points)
        {
            p.ConstrainWorldBounds(bounds);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }
}
