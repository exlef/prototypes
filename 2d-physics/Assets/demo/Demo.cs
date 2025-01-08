using UnityEngine;
using Ex;

public class Demo : MonoBehaviour
{
    [SerializeField] Transform pointA;
    [SerializeField] float radiusA = 1;
    [SerializeField] Transform pointB;
    [SerializeField] float radiusB = 1;

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
        var result = ExPhysics2d.CirclesSolve(pointA.position, radiusA, pointB.position, radiusB);
        pointA.position = result.Item1;
        pointB.position = result.Item2;
    }

    [ContextMenu("Solve Based on Size")]
    void SolveBasedOnSize()
    {
        float totalRadius = radiusA + radiusB;
        float aWeight = radiusB / totalRadius; // Smaller radius moves more if it's lighter
        float bWeight = radiusA / totalRadius;

        var result = ExPhysics2d.CirclesSolve(pointA.position, radiusA, pointB.position, radiusB);
        pointA.position += (Vector3)result.Item6 * result.Item5 * aWeight;
        pointB.position += (Vector3)result.Item7 * result.Item5 * bWeight;
    }

}
