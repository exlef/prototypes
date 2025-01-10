using UnityEngine;
using Ex;
using Ex.Verlet;
using System.Collections.Generic;

public class Demo : MonoBehaviour
{
    [SerializeField] Transform pointsParent;
    [SerializeField] GameObject pointVizPrefab;
    [SerializeField] Bounds bounds = new(Vector3.zero, new(12,12,0));

    List<Point> points = new();
    
    void Start()
    {
        for (int i = 0; i < pointsParent.childCount; i++)
        {
            Transform tr = pointsParent.GetChild(i);
            if (!tr.gameObject.activeInHierarchy) continue;

            Point p = new(tr);
            points.Add(p);
        }
    }

    void Update()
    {
        AddPointsAtMouseClick();

        UpdateSim();
    }

    void UpdateSim()
    {
        foreach (var p in points)
        {
            p.Tick();
        }

        
        for (int i = 0; i < 12; i++)
        {
            foreach (var p in points)
            {
                p.ConstrainWorldBounds(bounds);
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
        }
    }

    void AddPointsAtMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Utils.MousePos2D();
            var tr = Instantiate(pointVizPrefab, mousePos, Quaternion.identity).GetComponent<Transform>();
            Point p = new(tr);
            points.Add(p);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }
}
