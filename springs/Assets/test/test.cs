using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] Vector3 rayOrigin;
    [SerializeField] Vector3 rayDir;
    [SerializeField] Vector3 circlePos;
    [SerializeField] float circleRadius;
    void OnDrawGizmos()
    {
        Ray ray = new(rayOrigin, rayDir);
        Gizmos.DrawRay(ray.origin, ray.direction);
        Gizmos.DrawWireSphere(circlePos, circleRadius);
        Gizmos.DrawWireSphere(RayCircleIntersection(ray, circlePos, circleRadius), .1f);
    }

    Vector3 RayCircleIntersection(Ray ray, Vector3 circlePos, float radius)
    {
        var x = ray.direction;
        Vector3 toCircle = circlePos - ray.origin;
        Vector3 projectedVec = Vector3.Dot(ray.direction, toCircle) * ray.direction + ray.origin;
        return projectedVec;
    }
}
