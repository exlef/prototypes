using Freya;
using  Fe = Freya.MathfsExtensions;
using UnityEngine;

public class Demo : MonoBehaviour
{
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;

    ResultsMax2<Vector2> intersects;
    float circleOverlap;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(pointA.position, 1);
        Gizmos.DrawWireSphere(pointB.position, 1);
        Gizmos.color = Color.white;
        if(!CirclesOverlap(pointA.position, 1, pointB.position, 1, out float overlap)) return;
        circleOverlap = overlap;
        intersects = IntersectionTest.CirclesIntersectionPoints(pointA.position, 1, pointB.position, 1);
        Gizmos.DrawWireSphere(intersects.a, .1f);
        Gizmos.DrawWireSphere(intersects.b, .1f);
    }

    [ContextMenu("Solve")]
    void Solve()
    {
        Vector3 aTobDir = Fe.To(pointA.position, pointB.position).normalized;
        Vector3 bToaDir = Fe.To(pointB.position, pointA.position).normalized;
        Vector3 displacementA = circleOverlap * aTobDir;
        Vector3 displacementB = circleOverlap * bToaDir;

        pointA.position += displacementA / 2.0f;
        pointB.position += displacementB / 2.0f;
    }

    public static bool CirclesOverlap(Vector2 aPos, float aRadius, Vector2 bPos, float bRadius, out float overlap)
    {
        float dist = Vector2.Distance(aPos, bPos);
        float maxRad = Mathf.Max(aRadius, bRadius);
        float minRad = Mathf.Min(aRadius, bRadius);
        overlap = dist - (aRadius + bRadius);
        return Mathf.Abs(dist - maxRad) < minRad;
    }
}
