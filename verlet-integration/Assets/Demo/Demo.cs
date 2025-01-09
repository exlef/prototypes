using UnityEngine;

public class Demo : MonoBehaviour
{
    public static Bounds bounds = new(Vector3.zero, new(12,12,0));

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.extents);
    }
}
