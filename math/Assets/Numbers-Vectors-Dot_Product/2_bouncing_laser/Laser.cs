using UnityEngine;

namespace NumbersVectorsDot_Product
{
    public class Laser : MonoBehaviour
    {
        void FixedUpdate()
        {
            
        }

        void OnDrawGizmos()
        {
            Ray ray = new Ray(transform.position, transform.right);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo);
            Gizmos.color = hit ? Color.green : Color.red;
            float distance = Vector3.Distance(ray.origin, hitInfo.point);
            Gizmos.DrawRay(ray.origin, ray.direction * distance);
        }
    }
}
