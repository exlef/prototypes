using UnityEngine;

namespace Matrix_DotProduct
{
    public class TurretDemo : MonoBehaviour
    {
        [SerializeField] Transform turret;
        [SerializeField] Transform target;
        [SerializeField] float targetDetectionRange = 1;
        [SerializeField] float targetDetectionHeight = 1;
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
            
            float maxHeight = (turret.transform.position + turret.transform.up * targetDetectionHeight).y;
            float minHeight = turret.transform.position.y;

            // target detection
            var distance = Vector3.Distance(turret.position, target.position);
            if(distance > targetDetectionRange) return;
            var dot = Vector3.Dot(turret.forward, (target.position - turret.position).normalized);
            if(dot < .7f) return;
            if(InRange(target.position.y, minHeight, maxHeight) == false) return;
            Debug.Log("in front");
        }

        bool InRange(float x, float min, float max, float padding = 0.0f, bool includeEdges = false)
        {
            if (min > max)
                throw new System.ArgumentException("min cannot be greater than max.");

            // Adjust the range with padding
            float paddedMin = min - padding;
            float paddedMax = max + padding;

            if (includeEdges)
                return x >= paddedMin && x <= paddedMax; // Include edges
            else
                return x > paddedMin && x < paddedMax;   // Exclude edges
        }
    }
}
