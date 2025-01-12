using UnityEngine;
using Ex;
using Ex.Verlet;
using System.Collections.Generic;

public class Demo : MonoBehaviour
{
    [SerializeField] Transform pointsParent;
    [SerializeField] GameObject pointVizPrefab;
    [SerializeField] Bounds bounds = new(Vector3.zero, new(12,12,0));

    List<Point> points;
    List<Stick> sticks;
    
    void Start()
    {
        points = new();
        sticks = new();
        
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
    }

    void FixedUpdate()
    {
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
           sticks.RemoveAt(3);
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
            for (int j = 0; j < points.Count; j++)
            {
                for (int k = j + 1; k < points.Count; k++)
                {
                    var p1 = points[j];
                    var p2 = points[k];
                    
                    float p1Weight = 1;
                    float p2Weight = 1;
                    
                    if(p1.pinned && p2.pinned) continue;

                    if(p1.pinned && !p2.pinned)
                    {
                        var dynCirclePos = ExPhysics2d.SolveCirclesStaticDynamic(p1.pos, p1.radius, p2.pos, p2.radius);
                        p2.tr.position = dynCirclePos;
                        continue;
                    }
                    if(p2.pinned && !p1.pinned)
                    {
                        var dynCirclePos = ExPhysics2d.SolveCirclesStaticDynamic(p2.pos, p2.radius, p1.pos, p1.radius);
                        p1.tr.position = dynCirclePos;
                        continue;
                    }

                    var result = ExPhysics2d.SolveCirclesCollisionBasedOnWeight(p1.pos, p1.radius, p1Weight, p2.pos, p2.radius, p2Weight);
                    p1.tr.position = result.Item1;
                    p2.tr.position = result.Item2;
                }
            }
            // foreach (var p1 in points)
            // {
            //     foreach (var p2 in points)
            //     {
            //         float p1Weight = 1;
            //         float p2Weight = 1;
            //         
            //         if(p1.pinned && p2.pinned) continue;
            //
            //         if(p1.pinned && !p2.pinned)
            //         {
            //             var dynCirclePos = ExPhysics2d.SolveCirclesStaticDynamic(p1.pos, p1.radius, p2.pos, p2.radius);
            //             p2.tr.position = dynCirclePos;
            //             continue;
            //         }
            //         if(p2.pinned && !p1.pinned)
            //         {
            //             var dynCirclePos = ExPhysics2d.SolveCirclesStaticDynamic(p2.pos, p2.radius, p1.pos, p1.radius);
            //             p1.tr.position = dynCirclePos;
            //             continue;
            //         }
            //         UnityEngine.Debug.Log("here");
            //         var result = ExPhysics2d.SolveCirclesCollisionBasedOnWeight(p1.pos, p1.radius, p1Weight, p2.pos, p2.radius, p2Weight);
            //         p1.tr.position = result.Item1;
            //         p2.tr.position = result.Item2;
            //     }
            // }
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
            // Debug.Break();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }
}
