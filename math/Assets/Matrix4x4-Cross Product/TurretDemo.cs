using UnityEngine;

namespace Matrix_DotProduct
{
    public class TurretDemo : MonoBehaviour
    {
        [SerializeField] Transform turret;
        Camera cam;

        void Start()
        {
            cam = Camera.main;            
        }

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
        }
    }
}
