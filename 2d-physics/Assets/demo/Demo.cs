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
        if(!result.isColliding) return;
        pointA.position += (Vector3)result.displacementADir * result.overlap / 2;
        pointB.position += (Vector3)result.displacementBDir * result.overlap / 2;
    }

    [ContextMenu("Solve Based on Size")]
    void SolveBasedOnSize()
    {
        float totalRadius = radiusA + radiusB;
        float aWeight = radiusB / totalRadius; // Smaller radius moves more if it's lighter
        float bWeight = radiusA / totalRadius;

        var result = ExPhysics2d.CirclesSolve(pointA.position, radiusA, pointB.position, radiusB);
        pointA.position += (Vector3)result.displacementADir * (result.overlap * aWeight);
        pointB.position += (Vector3)result.displacementBDir * (result.overlap * bWeight);
    }

}
