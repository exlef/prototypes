using UnityEngine;

namespace StickDemo
{
    public class Stick : MonoBehaviour
    {
        [SerializeField] Transform anchor;
        [SerializeField] Transform point;
        [SerializeField] float constrainAngle;
        [SerializeField] bool followMouse = false;
        [SerializeField] float springLength = 0;
        [SerializeField] float springStiffness = 0.5f;  // Controls how "stiff" the spring is
        [SerializeField] float damping = 0.5f;         // Controls how quickly oscillations settle
        [SerializeField] float mass = 1f;              // Mass of the point
        Vector3 velocity = Vector3.zero;               // Current velocity of the point

        [SerializeField] Transform marker;

        void Update()
        {
            if (followMouse)
                anchor.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        void FixedUpdate()
        {
            // Calculate spring force using Hooke's Law: F = -kx
            Vector3 displacement = point.position - anchor.position;
            
            // Calculate the current distance between anchor and point
            float currentDistance = displacement.magnitude;
            // Normalize the displacement vector
            Vector3 direction = displacement.normalized;
            // Calculate the spring force using Hooke's Law: F = -k * (x - springLength)
            Vector3 springForce = -springStiffness * (currentDistance - springLength) * direction;

            // Calculate damping force: F = -cv
            Vector3 dampingForce = -damping * velocity;

            // Sum up forces
            Vector3 totalForce = springForce + dampingForce;

            // Calculate acceleration (F = ma)
            Vector3 acceleration = totalForce / mass;

            // Update velocity (integrate acceleration)
            velocity += acceleration;

            // constraints
            var tempPos = point.position + velocity;
            Vector3 constraintDir = anchor.up;
            Quaternion constrainRotationPozitive = Quaternion.Euler(0, 0, constrainAngle);
            Quaternion constrainRotationNegative = Quaternion.Euler(0, 0, -constrainAngle);
            Vector3 constrain1 = constrainRotationPozitive * constraintDir;
            Vector3 constrain2 = constrainRotationNegative * constraintDir;
            var interecting1 = RayCircleIntersection(new Ray(anchor.position, constrain1), tempPos, 0.1f);
            if(interecting1)
            {
                Debug.DrawRay(anchor.position, constrain1, Color.green);
            }
            else
            {
                Debug.DrawRay(anchor.position, constrain1, Color.red);
            }
            var interecting2 = RayCircleIntersection(new Ray(anchor.position, constrain2), tempPos, 0.1f);
            if (interecting2)
            {
                Debug.DrawRay(anchor.position, constrain2, Color.green);
            }
            else
            {
                Debug.DrawRay(anchor.position, constrain2, Color.red);
            }

            // Update position (integrate velocity)
            point.position += velocity;
        }

        void OnDrawGizmos()
        {
            Vector3 constraintDir = anchor.up;
            Quaternion constrainRotationPozitive = Quaternion.Euler(0, 0, constrainAngle);
            Quaternion constrainRotationNegative = Quaternion.Euler(0, 0, -constrainAngle);
            Vector3 constrain1 =  constrainRotationPozitive * constraintDir;
            Vector3 constrain2 = constrainRotationNegative * constraintDir;

            // Debug.DrawRay(anchor.position, constrain1);
            // Debug.DrawRay(anchor.position, constrain2);
        }

        bool RayCircleIntersection(Ray ray, Vector3 circlePos, float radius)
        {
            var toCircle = circlePos - ray.origin;
            var dot = Vector3.Dot(ray.direction, toCircle);
            if(dot < 0) return false; // I want intersection only happens starting from ray origin 
            var projection = dot * ray.direction + ray.origin;
            marker.position = projection;
            var distance = Vector3.Magnitude(projection - circlePos);
            return distance < radius;
        }        
    }
}
