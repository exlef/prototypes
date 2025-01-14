using UnityEngine;

public class VerletPhysics : MonoBehaviour
{
    public Bounds bounds;
    [SerializeField] Point[] points;
    [SerializeField] Stick[] sticks;

    private void Start()
    {
        foreach (var p in points)
        {
            p.Init();
        }
        
        foreach (var s in sticks)
        {
            s.Init();
        } 
    }

    void Update()
    {
        foreach (var p in points)
        {
            p.Tick();
        }

        for (int i = 0; i < 12; i++)
        {
            for (int sIndex = 0; sIndex < 4; sIndex++)
            {
                foreach (var s in sticks)
                {
                    s.Tick();
                }    
            }
            
            foreach (var p in points)
            {
                p.ConstrainWorldBounds(bounds);
            }
            
            for (int j = 0; j < points.Length; j++)
            {
                for (int k = j + 1; k < points.Length; k++)
                {
                    var point1 = points[j];
                    var point2 = points[k];

                    var p1 = point1.point;
                    var p2 = point2.point;
                    
                    
                    if(p1.pinned && p2.pinned) continue;

                    if(p1.pinned && !p2.pinned)
                    {
                        var dynCirclePos = Ex.ExPhysics2d.SolveCirclesStaticDynamic(p1.pos, p1.radius, p2.pos, p2.radius);
                        p2.tr.position = dynCirclePos;
                        continue;
                    }
                    if(p2.pinned && !p1.pinned)
                    {
                        var dynCirclePos = Ex.ExPhysics2d.SolveCirclesStaticDynamic(p2.pos, p2.radius, p1.pos, p1.radius);
                        p1.tr.position = dynCirclePos;
                        continue;
                    }

                    var result = Ex.ExPhysics2d.SolveCirclesCollisionBasedOnWeight(p1.pos, p1.radius, 0.5f, p2.pos, p2.radius, 0.5f);
                    p1.tr.position = result.Item1;
                    p2.tr.position = result.Item2;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        bounds.center = transform.position;
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }
}
