using UnityEngine;

public class VerletPhysics : MonoBehaviour
{
    public Bounds bounds;
    [SerializeField] Rope rope;
    [SerializeField] Point[] points;
    [SerializeField] Stick[] sticks;

    private void Start()
    {
        rope.Init();
        points = FindObjectsByType<Point>(FindObjectsSortMode.None);
        sticks = FindObjectsByType<Stick>(FindObjectsSortMode.None);
        
        foreach (var p in points)
        {
            p.Init();
        }
        
        foreach (var s in sticks)
        {
            s.Init();
        } 
    }

    void FixedUpdate()
    {
        foreach (var p in points)
        {
            p.Tick();
        }

        for (int i = 0; i < 12; i++)
        {
            for (int sIndex = 0; sIndex < 1; sIndex++)
            {
                foreach (var s in sticks)
                {
                    s.Tick();
                }    
            }
            
            foreach (var p in points)
            {
                // p.ConstrainWorldBounds(bounds);
            }
            
            for (int j = 0; j < points.Length; j++)
            {
                for (int k = j + 1; k < points.Length; k++)
                {
                    var p1 = points[j];
                    var p2 = points[k];

                    if(p1.pinned && p2.pinned) continue;

                    if(p1.pinned && !p2.pinned)
                    {
                        var dynCirclePos = Ex.ExPhysics2d.SolveCirclesStaticDynamic(p1.pos, p1.radius, p2.pos, p2.radius);
                        p2.pos = dynCirclePos;
                        continue;
                    }
                    if(p2.pinned && !p1.pinned)
                    {
                        var dynCirclePos = Ex.ExPhysics2d.SolveCirclesStaticDynamic(p2.pos, p2.radius, p1.pos, p1.radius);
                        p1.pos = dynCirclePos;
                        continue;
                    }

                    var result = Ex.ExPhysics2d.SolveCirclesCollisionBasedOnWeight(p1.pos, p1.radius, 0.5f, p2.pos, p2.radius, 0.5f);
                    p1.pos = result.Item1;
                    p2.pos = result.Item2;
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
