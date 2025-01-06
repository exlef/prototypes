using Freya;
using  Fe = Freya.MathfsExtensions;
using UnityEngine;

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
        if (!CirclesCheck(pointA.position, radiusA, pointB.position, radiusB, out float overlap)) return;
        Vector3 AtoBdir = (pointB.position - pointA.position).normalized;
        Vector3 BtoAdir = (pointA.position - pointB.position).normalized;
        Vector3 displacementA = overlap / 2 * BtoAdir;
        Vector3 displacementB = overlap / 2 * AtoBdir;

        pointA.position += displacementA;
        pointB.position += displacementB;
    }

    /// <summary>
    /// checks if two circles are colliding.
    /// </summary>
    public static bool CirclesCheck(Vector2 aPos, float aRadius, Vector2 bPos, float bRadius, out float overlap)
    {
        float dist = Vector2.Distance(aPos, bPos);
        float totalRadius = aRadius + bRadius;
        overlap = totalRadius - dist;
        return dist <= totalRadius;
    }
}
