using UnityEditor;
using UnityEngine;

public class TurretTrigger : MonoBehaviour
{
    enum TurretTriggerE
    {
        CylindricalSector,
        Spherical,
        SphericalSector,
    }
    [SerializeField] TurretTriggerE triggerType; 
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
        Vector3 right = Vector3.right;

        Vector3 center = Vector3.zero; // since we are going to draw in local space center is Vector3(0,0,0)
        Vector3 top = up * height;
        
        
        switch (triggerType)
        {
            case TurretTriggerE.CylindricalSector:
                {
                    Quaternion q = Quaternion.Euler(0, angle / 2, 0);
                    Vector3 leftDir = Quaternion.Inverse(q) * forward;
                    Vector3 rightDir = q * forward;

                    Vector3 innerLeftCorner = leftDir * innerRadius;
                    Vector3 innerRightCorner = rightDir * innerRadius;
                    Vector3 outerLeftCorner = innerLeftCorner + leftDir * outerRadius;
                    Vector3 outerRightCorner = innerRightCorner + rightDir * outerRadius;

                    Handles.matrix = Gizmos.matrix = transform.localToWorldMatrix;

                    { // inner trigger drawing
                      // Gizmos.DrawRay(center, innerLeftCorner);
                      // Gizmos.DrawRay(center, innerRightCorner);
                      // Gizmos.DrawRay(top, innerLeftCorner);
                      // Gizmos.DrawRay(top, innerRightCorner);

                        // Gizmos.DrawLine(center, top);
                        // Gizmos.DrawLine(center + innerLeftCorner, top + innerLeftCorner);
                        // Gizmos.DrawLine(center + innerRightCorner, top + innerRightCorner);

                        // Handles.DrawWireArc(center, up, leftDir, angle, innerRadius);
                        // Handles.DrawWireArc(top, up, leftDir, angle, innerRadius);
                    }


                    bool contains = !Contains(forward, innerRadius) && Contains(forward, innerRadius + outerRadius);

                    Gizmos.color = Handles.color = contains ? Color.green : Color.white;

                    Handles.DrawWireArc(center, up, leftDir, angle, innerRadius);
                    Handles.DrawWireArc(top, up, leftDir, angle, innerRadius);

                    Gizmos.DrawLine(innerLeftCorner, outerLeftCorner);
                    Gizmos.DrawLine(innerRightCorner, outerRightCorner);
                    Gizmos.DrawLine(top + innerLeftCorner, top + outerLeftCorner);
                    Gizmos.DrawLine(top + innerRightCorner, top + outerRightCorner);

                    Gizmos.DrawLine(outerLeftCorner, top + outerLeftCorner);
                    Gizmos.DrawLine(outerRightCorner, top + outerRightCorner);

                    Handles.DrawWireArc(center, up, leftDir, angle, outerRadius + innerRadius);
                    Handles.DrawWireArc(top, up, leftDir, angle, outerRadius + innerRadius);
                }                
                break;
            case TurretTriggerE.Spherical:
                Handles.matrix = Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.color = SphereTriggerContains() ? Color.green : Color.white;
                Gizmos.DrawWireSphere(center, innerRadius);
                Gizmos.DrawWireSphere(center, outerRadius);
                Handles.matrix = Gizmos.matrix = transform.localToWorldMatrix;
                break;
            case TurretTriggerE.SphericalSector:
                {
                    Gizmos.color = Handles.color = SphereSectorTriggerContains() ? Color.green : Color.white;

                    Handles.matrix = Gizmos.matrix = transform.localToWorldMatrix;
                    Vector3 verticalArcStartingPoint = Quaternion.Euler(-angle / 2, 0, 0) * forward * outerRadius;
                    Vector3 verticalArcEndPoint = Quaternion.Euler(angle, 0, 0) * verticalArcStartingPoint;
                    Vector3 horizontalArcStartingPoint = Quaternion.Euler(0, -angle / 2, 0) * forward * outerRadius;
                    Vector3 horizontalArcEndPoint = Quaternion.Euler(0, angle, 0) * horizontalArcStartingPoint;
                    Handles.DrawWireArc(center, right, verticalArcStartingPoint, angle, outerRadius);
                    Handles.DrawWireArc(center, up, horizontalArcStartingPoint, angle, outerRadius);

                    Handles.DrawWireArc(center, right, verticalArcStartingPoint, angle, innerRadius);
                    Handles.DrawWireArc(center, up, horizontalArcStartingPoint, angle, innerRadius);

                    Gizmos.DrawLine(center + horizontalArcStartingPoint.normalized * innerRadius, horizontalArcStartingPoint);
                    Gizmos.DrawLine(center + horizontalArcEndPoint.normalized * innerRadius, horizontalArcEndPoint);
                    Gizmos.DrawLine(center + verticalArcStartingPoint.normalized * innerRadius, verticalArcStartingPoint);
                    Gizmos.DrawLine(center + verticalArcEndPoint.normalized * innerRadius, verticalArcEndPoint);

                    Gizmos.DrawWireSphere(verticalArcEndPoint, 0.1f);
                    float circleDist = Vector3.Dot(forward, verticalArcEndPoint);
                    float circleRadi = Mathf.Tan(angle / 2 * Mathf.Deg2Rad) * circleDist;
                    Handles.DrawWireDisc(forward * circleDist, forward, circleRadi);
                }
                break;
        }
    }

    void OnValidate()
    {
        if(innerRadius > outerRadius)
            innerRadius = outerRadius - 0.1f;
    }

    bool SphereTriggerContains()
    {
        float dist = Vector3.Distance(target.position, transform.position);
        return dist > innerRadius && dist < outerRadius;
    }

    bool SphereSectorTriggerContains()
    {
        // distance check
        float dist = Vector3.Distance(target.position, transform.position);
        Debug.DrawLine(target.position, transform.position);
        if(dist < innerRadius || dist > outerRadius) return false;

        // angle check
        Vector3 toTarget = target.position - transform.position;

        Vector3 toTargetProjectedToXZPlane = new(toTarget.x, 0, toTarget.z);
        float dotXZ = Vector3.Dot(transform.forward, toTargetProjectedToXZPlane.normalized);
        float angleXZ = Mathf.Acos(dotXZ) * Mathf.Rad2Deg;

        Vector3 toTargetProjectedToYZPlane = new(0, toTarget.y, toTarget.z);
        float dotYZ = Vector3.Dot(transform.forward, toTargetProjectedToYZPlane.normalized);
        float angleYZ = Mathf.Acos(dotYZ) * Mathf.Rad2Deg;

        return angleXZ < angle / 2 && angleYZ < angle / 2;
    }


    void RotateHeadToTarget()
    {
        Vector3 dirToTarget = (target.position - head.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dirToTarget, Vector3.up);
        Quaternion currentRot = head.rotation;
        head.rotation = Quaternion.Slerp(currentRot, targetRot, Time.deltaTime);
    }

    bool Contains(Vector3 forward, float radius)
    {
        Vector3 tp = transform.InverseTransformPoint(target.position);
        
        // we do calculations in local space of this object

        if(tp.y < 0 || tp.y > height) return false;
        
        Vector3 toTargetProjectedToXZPlane = new Vector3(tp.x, 0.0f, tp.z);

        float dot = Vector3.Dot(forward, toTargetProjectedToXZPlane.normalized);
        float angleTo  = Mathf.Acos(dot) * Mathf.Rad2Deg;
        if(angleTo  > angle / 2) return false;

        if(toTargetProjectedToXZPlane.magnitude > radius) return false;

        return true;;
    }
}
