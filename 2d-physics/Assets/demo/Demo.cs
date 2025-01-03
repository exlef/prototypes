using Freya;
using UnityEngine;

public class Demo : MonoBehaviour
{
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] bool solveForA = false;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(pointA.position, 1);
        Gizmos.DrawWireSphere(pointB.position, 1);
        Gizmos.color = Color.white;
        if(!IntersectionTest.CirclesOverlap(pointA.position, 1, pointB.position, 1)) return;
        ResultsMax2<Vector2> intersects = IntersectionTest.CirclesIntersectionPoints(pointA.position, 1, pointB.position, 1);
        Gizmos.DrawWireSphere(intersects.a, .1f);
        Gizmos.DrawWireSphere(intersects.b, .1f);
        if(solveForA)
        {
            Vector3 aTobDir = MathfsExtensions.To(pointA.position, pointB.position).normalized;
            Vector3 aToIntersect = MathfsExtensions.To(pointA.position, (Vector3)intersects.a);
            Vector3 displacementA = -Vector3.Dot(aTobDir, aToIntersect) * aTobDir;
            pointA.position += displacementA;
            Gizmos.DrawWireSphere(pointA.position + Vector3.Dot(aTobDir, aToIntersect) * aTobDir, 0.05f);
        }
        
    }
}
