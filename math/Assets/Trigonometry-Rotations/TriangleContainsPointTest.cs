using UnityEngine;

namespace TrigonometryRotations
{
    public class TriangleContainsPointTest : MonoBehaviour
    {
        [SerializeField] Transform a, b, c, p;

        void OnDrawGizmos()
        {
            Gizmos.color = TriangleContainsPoint(a.position, b.position, c.position, p.position) ? Color.white : Color.red;

            Gizmos.DrawLine(a.position, b.position);
            Gizmos.DrawLine(b.position, c.position);
            Gizmos.DrawLine(c.position, a.position);

            Gizmos.DrawWireSphere(p.position, 0.02f);
        }

        bool TriangleContainsPoint(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
        {
            var ab = b - a;
            var bc = c - b;
            var ca = a - c;

            var ap = p - a;
            var bp = p - b;
            var cp = p - c;

            var adet = Determinant(ab, ap);
            var bdet = Determinant(bc, bp);
            var cdet = Determinant(ca, cp);
            
            return Mathf.Sign(adet) == Mathf.Sign(bdet) && Mathf.Sign(bdet) == Mathf.Sign(cdet);
        }

        float Determinant(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
    }
}
