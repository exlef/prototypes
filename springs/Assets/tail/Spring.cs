using UnityEngine;

namespace TailDemo
{
    public class Spring : MonoBehaviour
    {
        [SerializeField] Transform pointRed;
        [SerializeField] Transform pointBlue;
        [SerializeField] bool followMouse = false;
        [SerializeField] float springLength = 0;
        [SerializeField] float springStiffness = 0.5f;  // Controls how "stiff" the spring is
        [SerializeField] float damping = 0.5f;         // Controls how quickly oscillations settle
        [SerializeField] float mass = 1f;              // Mass of the point
        Vector3 velocity = Vector3.zero;               // Current velocity of the point

        void Update()
        {
            if (followMouse)
                pointRed.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        void FixedUpdate()
        {
            AnchorPointSpring(pointRed, pointBlue);
        }

        void AnchorPointSpring(Transform anchor, Transform point)
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

            // Update position (integrate velocity)
            point.position += velocity;
        }
    }
}

