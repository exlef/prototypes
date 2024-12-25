using UnityEngine;

namespace NumbersVectorsDot_Product
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] int reflectionCount = 2;

        void OnDrawGizmos()
        {
            var ray = new Ray(transform.position, transform.right);

            for (int i = 0; i < reflectionCount; i++)
            {
                var hitInfo = CastRay(ray);

                Ray tempRay = new();

                tempRay.origin = hitInfo.point;
                tempRay.direction = RefletctRay(ray.origin - hitInfo.point, hitInfo.point, hitInfo.normal);

                ray = tempRay;
            }
        }


        RaycastHit CastRay(Ray ray)
        {
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo);
            Gizmos.color = hit ? Color.green : Color.red;
            float distance = Vector3.Distance(ray.origin, hitInfo.point);
            Gizmos.DrawRay(ray.origin, ray.direction * distance);
            return hitInfo;
        }

        Vector3 RefletctRay(Vector3 dir, Vector3 hitPoint, Vector3 normal)
        {
            normal = normal.normalized;
            Vector3 dirProjectedOnNormal = Vector3.Dot(normal, dir) * normal;
            Vector3 differenceVec = dirProjectedOnNormal - dir;
            Vector3 targetPoint = dirProjectedOnNormal + differenceVec;
            Vector3 reflectedVec = targetPoint - hitPoint;
            return reflectedVec;
        }
    }
}
