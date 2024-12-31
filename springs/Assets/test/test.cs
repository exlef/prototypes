using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] Vector3 rayOrigin;
    [SerializeField] Vector3 rayDir;
    [SerializeField] Circle circleA;
    Vector3 vel;

    void Start()
    {
        vel = Vector3.down;
    }

    void Update()
    {
        Ray ray = new(rayOrigin, rayDir);
        Color color = Color.red;

        circleA.pos += vel * Time.deltaTime * 10;

        Vector3 normal = GetPerpendicularVector(ray.direction);
        Debug.DrawRay(rayOrigin, normal, Color.magenta);

        if(RayCircleIntersection(ray, circleA.pos, circleA.radius, out float distance))
        {
            color = Color.green;

            vel = Vector3.Reflect(vel, normal);
            circleA.pos += -normal * distance;
        }

        DrawCircle(circleA.pos, circleA.radius, 12, color);
        Debug.DrawRay(rayOrigin, rayDir);
    }

    Vector3 GetPerpendicularVector(Vector3 direction)
    {
        // Pick an arbitrary vector that is not parallel to the ray's direction
        Vector3 arbitrary = Mathf.Abs(direction.x) > Mathf.Abs(direction.z)
                            ? Vector3.forward
                            : Vector3.up;

        // Use cross product to find a perpendicular vector
        Vector3 perpendicular = Vector3.Cross(direction, arbitrary);

        // Normalize the result to get a unit vector
        return perpendicular.normalized;
    }

    bool RayCircleIntersection(Ray ray, Vector3 circlePos, float radius, out float dist)
    {
        var toCircle = circlePos - ray.origin;
        var dot = Vector3.Dot(ray.direction, toCircle);
        if (dot < 0) // I want intersection only happens starting from ray origin 
        {
            dist = 0; 
            return false; 
        }
        var projection = dot * ray.direction + ray.origin;
        var distance = Vector3.Magnitude(projection - circlePos);
        dist = distance;
        return distance < radius;
    }

    public static void DrawCircle(Vector3 position, float radius, int segments, Color color)
    {
        // If either radius or number of segments are less or equal to 0, skip drawing
        if (radius <= 0.0f || segments <= 0)
        {
            return;
        }

        // Single segment of the circle covers (360 / number of segments) degrees
        float angleStep = (360.0f / segments);

        // Result is multiplied by Mathf.Deg2Rad constant which transforms degrees to radians
        // which are required by Unity's Mathf class trigonometry methods

        angleStep *= Mathf.Deg2Rad;

        // lineStart and lineEnd variables are declared outside of the following for loop
        Vector3 lineStart = Vector3.zero;
        Vector3 lineEnd = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            // Line start is defined as starting angle of the current segment (i)
            lineStart.x = Mathf.Cos(angleStep * i);
            lineStart.y = Mathf.Sin(angleStep * i);

            // Line end is defined by the angle of the next segment (i+1)
            lineEnd.x = Mathf.Cos(angleStep * (i + 1));
            lineEnd.y = Mathf.Sin(angleStep * (i + 1));

            // Results are multiplied so they match the desired radius
            lineStart *= radius;
            lineEnd *= radius;

            // Results are offset by the desired position/origin 
            lineStart += position;
            lineEnd += position;

            // Points are connected using DrawLine method and using the passed color
            Debug.DrawLine(lineStart, lineEnd, color);
        }
    }

    [System.Serializable]
    struct Circle
    {
        public Vector3 pos;
        public float radius;        
    }
}
