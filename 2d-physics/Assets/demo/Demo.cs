using UnityEngine;
using Ex;

public class Demo : MonoBehaviour
{
    [SerializeField] Transform pointA;
    [SerializeField] float radiusA = 1;
    [SerializeField] float weightA = 1;
    [SerializeField] Transform pointB;
    [SerializeField] float radiusB = 1;
    [SerializeField] float weightB = 1;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(pointA.position, radiusA);
        Gizmos.DrawWireSphere(pointB.position, radiusB);
        Gizmos.color = Color.white;
        
    }

    [ContextMenu("Solve")]
    void Solve()
    {
        var result = ExPhysics2d.SolveCircles(pointA.position, radiusA, pointB.position, radiusB);
        pointA.position = (Vector3)result.Item1;
        pointB.position = (Vector3)result.Item2;
    }

    [ContextMenu("Solve Based on Size")]
    void SolveBasedOnSize()
    {
        var result = ExPhysics2d.SolveCirclesCollisionBasedOnSize(pointA.position, radiusA, pointB.position, radiusB);
        pointA.position = (Vector3)result.Item1;
        pointB.position = (Vector3)result.Item2;
    }

    [ContextMenu("Solve Based on Weight")]
    void SolveBasedOnWeight()
    {
        var result = ExPhysics2d.SolveCirclesCollisionBasedOnWeight(pointA.position, radiusA, weightA, pointB.position, radiusB, weightB);
        pointA.position = (Vector3)result.Item1;
        pointB.position = (Vector3)result.Item2;
    }
}
