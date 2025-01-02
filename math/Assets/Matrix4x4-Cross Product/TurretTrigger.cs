using UnityEditor;
using UnityEngine;

public class TurretTrigger : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform head;
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

        bool contains = Contains(forward);

        Handles.matrix = Gizmos.matrix = transform.localToWorldMatrix;
        Handles.color = Gizmos.color = contains ? Color.green : Color.white;

        if(contains) RotateHeadToTarget();


        Gizmos.DrawRay(center, left * radius);
        Gizmos.DrawRay(center, right * radius);
        Gizmos.DrawRay(top, left * radius);
        Gizmos.DrawRay(top, right * radius);

        Gizmos.DrawLine(center, top);
        Gizmos.DrawLine(center + left * radius, top + left * radius);
        Gizmos.DrawLine(center + right * radius, top + right * radius);

        Handles.DrawWireArc(center, up, left, angle * 2, radius);
        Handles.DrawWireArc(top, up, left, angle * 2, radius);
    }

    void RotateHeadToTarget()
    {
        Vector3 dirToTarget = (target.position - head.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dirToTarget, Vector3.up);
        head.rotation = rot;
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
