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
    List<Stick> sticks = new();
    
    void Start()
    {
        for (int i = 0; i < pointsParent.childCount; i++)
        {
            Transform tr = pointsParent.GetChild(i);
            if (!tr.gameObject.activeInHierarchy) continue;

            Point p;
            if (i == 0 || i == pointsParent.childCount - 1) 
            {
                p = new(tr, true);
            }
            else
            {
            p = new (tr);
                
            }

            points.Add(p);
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            Stick s = new(points[i], points[i+1]);
            sticks.Add(s);
        }
    }

    void Update()
    {
        AddPointsAtMouseClick();
        RemovePointOnInput();
        RemoveStickOnInput();

        UpdateSim();
    }

    void RemovePointOnInput()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            var p = points[9];

            points.RemoveAt(9);

            Destroy(p.tr.gameObject);
        }
    }

    void RemoveStickOnInput()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            // var s = sticks[3];

            sticks.RemoveAt(3);

            // Destroy(s.tr.gameObject);
        }
    }

    void UpdateSim()
    {
        foreach (var p in points)
        {
            p.Tick();
        }

        
        for (int i = 0; i < 12; i++)
        {
            foreach (var s in sticks)
            {
                s.Tick();
            }

            foreach (var p in points)
            {
                p.ConstrainWorldBounds(bounds);
            }

            foreach (var p1 in points)
            {
                foreach (var p2 in points)
                {
                    float p1Weight = 0;
                    float p2Weight = 0;
                    if(p1.pinned)
                    {
                        p1Weight = 1;
                    }
                    if(p2.pinned)
                    {
                        p2Weight = 1;
                    }
                    if(p1.pinned == false && p2.pinned == false)
                    {
                        p1Weight = 1;
                        p2Weight = 1;
                    }
                    // var result = ExPhysics2d.SolveCircles(p1.pos, p1.radius, p2.pos, p2.radius);
                    var result = ExPhysics2d.SolveCirclesCollisionBasedOnWeight(p1.pos, p1.radius, p1Weight, p2.pos, p2.radius, p2Weight);
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
