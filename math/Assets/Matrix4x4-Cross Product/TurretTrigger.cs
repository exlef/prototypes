using UnityEditor;
using UnityEngine;

public class TurretTrigger : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform head;
    [SerializeField] [Min(0)] float innerRadius = 1;
    [SerializeField][Min(0)] float outerRadius = 2;
    [SerializeField] float height = 1;
    [SerializeField] [Range(0, 360)] float angle;
    

    void OnDrawGizmos()
    {
        Vector3 up = Vector3.up;
        Vector3 forward = Vector3.forward;

        Vector3 center = Vector3.zero; // since we are going to draw in local space center is Vector3(0,0,0)
        Vector3 top = up * height;

        Quaternion q = Quaternion.Euler(0, angle/2, 0);
        Vector3 leftDir = Quaternion.Inverse(q) * forward;
        Vector3 rightDir = q * forward;

        Vector3 innerLeftCorner = leftDir * innerRadius;
        Vector3 innerRightCorner = rightDir * innerRadius;
        Vector3 outerLeftCorner = innerLeftCorner + leftDir * outerRadius;
        Vector3 outerRightCorner = innerRightCorner + rightDir * outerRadius;

        Handles.matrix = Gizmos.matrix = transform.localToWorldMatrix;

        { // inner trigger drawing
            Gizmos.DrawRay(center, innerLeftCorner);
            Gizmos.DrawRay(center, innerRightCorner);
            Gizmos.DrawRay(top, innerLeftCorner);
            Gizmos.DrawRay(top, innerRightCorner);

            Gizmos.DrawLine(center, top);
            Gizmos.DrawLine(center + innerLeftCorner, top + innerLeftCorner);
            Gizmos.DrawLine(center + innerRightCorner, top + innerRightCorner);

            Handles.DrawWireArc(center, up, leftDir, angle, innerRadius);
            Handles.DrawWireArc(top, up, leftDir, angle, innerRadius);
        }

        Gizmos.color = Handles.color = Color.red;

        Gizmos.DrawLine(innerLeftCorner, outerLeftCorner);
        Gizmos.DrawLine(innerRightCorner, outerRightCorner);
        Gizmos.DrawLine(top + innerLeftCorner, top + outerLeftCorner);
        Gizmos.DrawLine(top + innerRightCorner, top + outerRightCorner);

        Gizmos.DrawLine(outerLeftCorner, top  + outerLeftCorner);
        Gizmos.DrawLine(outerRightCorner, top + outerRightCorner);

        Handles.DrawWireArc(center, up, leftDir, angle, outerRadius + innerRadius);
        Handles.DrawWireArc(top, up, leftDir, angle, outerRadius + innerRadius);
    }

    void RotateHeadToTarget()
    {
        Vector3 dirToTarget = (target.position - head.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dirToTarget, Vector3.up);
        Quaternion currentRot = head.rotation;
        head.rotation = Quaternion.Slerp(currentRot, targetRot, Time.deltaTime);
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

        if(toTargetProjectedToXZPlane.magnitude > innerRadius) return false;

        return true;;
    }
}
