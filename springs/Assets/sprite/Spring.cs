using UnityEngine;

namespace SpriteDemo
{
    public class Spring : MonoBehaviour
    {
        [SerializeField] Transform anchor;
        [SerializeField] float springStiffness = 0.2f;
        [SerializeField] float damping = 0.5f;
        [SerializeField] float mass = 1f;
        private Vector3 velocity = Vector3.zero;

        void FixedUpdate()
        {
            // Calculate spring force using Hooke's Law: F = -kx
            Vector3 displacement = transform.position - anchor.position;
            Vector3 springForce = -springStiffness * displacement;

            // Calculate damping force: F = -cv
            Vector3 dampingForce = -damping * velocity;

            // Sum up forces
            Vector3 totalForce = springForce + dampingForce;

            // Calculate acceleration (F = ma)
            Vector3 acceleration = totalForce / mass;

            // Update velocity (integrate acceleration)
            velocity += acceleration;

            // Update position (integrate velocity)

            transform.position += velocity;
        }
    }

}
