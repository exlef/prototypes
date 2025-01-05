using UnityEngine;

public class WorldSpaceGradient : MonoBehaviour
{
    [SerializeField] Material mat;
    [SerializeField] Transform a;
    [SerializeField] Transform b;
    void Update()
    {
        mat.SetVector("_PointA", new Vector4(a.position.x, a.position.y, a.position.z, 0));
        mat.SetVector("_PointB", b.position);
    }
}
