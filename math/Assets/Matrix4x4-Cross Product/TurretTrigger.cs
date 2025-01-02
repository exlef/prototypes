using UnityEditor;
using UnityEngine;

public class TurretTrigger : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float radius = 1;
    [SerializeField] float height = 1;
    [SerializeField] float angle;

    void OnDrawGizmos()
    {
        Vector3 center = Vector3.zero; // since we are going to draw in local space center is Vector3(0,0,0)
        Vector3 top = transform.up * height;

        Vector3 up = Vector3.up;
        Vector3 forward = Vector3.forward;

        Quaternion q = Quaternion.Euler(0, angle, 0);
        Quaternion qN = Quaternion.Euler(0, -angle, 0);
        Vector3 left = qN * forward;
        Vector3 right = q * forward;

        Handles.matrix = Gizmos.matrix = transform.localToWorldMatrix;
        Handles.color = Gizmos.color = Contains(forward) ? Color.green : Color.white;

        Gizmos.DrawRay(center, left);
        Gizmos.DrawRay(center, right);
        Gizmos.DrawRay(top, left);
        Gizmos.DrawRay(top, right);

        Gizmos.DrawLine(center, top);
        Gizmos.DrawLine(center + left, top + left);
        Gizmos.DrawLine(center + right, top + right);

        Handles.DrawWireArc(center, up, left, angle * 2, radius);
        Handles.DrawWireArc(top, up, left, angle * 2, radius);
    }

    bool Contains(Vector3 forward)
    {
        Vector3 tp = transform.InverseTransformPoint(target.position);
        
        // we do calculations in local space of this object

        if(tp.y < 0 || tp.y > height) return false;
        
        Vector3 toTargetProjectedToXZPlane = new Vector3(tp.x, 0.0f, tp.z);

        float dot = Vector3.Dot(forward, toTargetProjectedToXZPlane.normalized);
        float angleTo  = Mathf.Acos(dot) * Mathf.Rad2Deg;
        if(angleTo  > angle) return false;

        if(toTargetProjectedToXZPlane.magnitude > radius) return false;

        return true;;
    }
}
