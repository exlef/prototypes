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
                tempRay.direction = RefletctRay(hitInfo.point - ray.origin, hitInfo.normal);
                // tempRay.direction = ReflectOriginal(hitInfo.point - ray.origin, hitInfo.normal);

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

        Vector3 RefletctRay(Vector3 inDirection, Vector3 inNormal)
        {
            inNormal = inNormal.normalized;
            float p = Vector3.Dot(inNormal, inDirection);
            Vector3 outDirection = inDirection + (inNormal * 2 * -p); // p is the negative. since the dot between inDir and normal will be negative. since we want to add to the inDir to find ReflectedDir we will negate the p.
            return outDirection;
        }

        Vector3 ReflectOriginal(Vector3 inDirection, Vector3 inNormal)
        {
            return Vector3.Reflect(inDirection, inNormal);
        }
    }
}
