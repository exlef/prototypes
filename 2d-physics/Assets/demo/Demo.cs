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
        if (!CirclesCheck(pointA.position, radiusA, out float overlapA, pointB.position, radiusB, out float overlapB)) return;
        Vector3 AtoBdir = (pointB.position - pointA.position).normalized;
        Vector3 BtoAdir = (pointA.position - pointB.position).normalized;
        Vector3 displacementA = overlapA * BtoAdir;
        Vector3 displacementB = overlapB * AtoBdir;

        pointA.position += displacementA;
        pointB.position += displacementB;
    }

    /// <summary>
    /// checks if two circles are colliding.
    /// </summary>
    public static bool CirclesCheck(Vector2 aPos, float aRadius, out float overlapA, Vector2 bPos, float bRadius, out float overlapB)
    {
        float dist = Vector2.Distance(aPos, bPos);
        float totalRadius = aRadius + bRadius;
        overlapA = (totalRadius - dist) / 2;
        overlapB = (totalRadius - dist) / 2;
        // overlap = totalRadius - dist;
        return dist <= totalRadius;
    }
}
