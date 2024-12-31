using UnityEngine;

namespace Matrix_DotProduct
{
    public class TurretDemo : MonoBehaviour
    {
        [SerializeField] Transform turret;
        [SerializeField] float targetDetectionRange = 1;
        [SerializeField] float targetDetectionHeight = 1;
        [SerializeField] float targetDetectionAngleInDeg = 30;
        Camera cam;

        void Start()
        {
            cam = Camera.main;            
        }

        Vector3 upLeftCornerG;
        Vector3 bottomLeftCornerG;

        void Update()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            var result = Physics.Raycast(ray, out RaycastHit hit);
            if(!result) return;
            turret.position = hit.point;
            

            Debug.DrawLine(cam.transform.position, hit.point);

            var xAxis = Vector3.Cross(hit.normal, ray.direction).normalized;
            var yAxis = hit.normal.normalized;
            var zAxis = Vector3.Cross(xAxis, yAxis).normalized;

            Debug.DrawRay(turret.position, xAxis, Color.red);
            Debug.DrawRay(turret.position, yAxis, Color.green);
            Debug.DrawRay(turret.position, zAxis, Color.blue);

            turret.rotation = Quaternion.LookRotation(zAxis, yAxis);

            // target detection cone
            var upLeftCorner = turret.position +  (yAxis * targetDetectionHeight) + (zAxis * targetDetectionRange);
            upLeftCornerG = upLeftCorner;
            var bottomLeftCorner = turret.position - (yAxis * targetDetectionHeight) + (zAxis * targetDetectionRange);
            bottomLeftCornerG = bottomLeftCorner;
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(upLeftCornerG, .1f);
            Gizmos.DrawWireSphere(bottomLeftCornerG, .1f);
        }
    }
}
