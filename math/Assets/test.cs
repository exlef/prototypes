using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] Vector3 childPos;
    [SerializeField] Transform child;
    
    void OnDrawGizmos()
    {
        child.position = LocalToWorld(childPos);
    }

    Vector3 LocalToWorld(Vector3 localPos)
    {
        return transform.localToWorldMatrix.MultiplyPoint(localPos);
    }
}
