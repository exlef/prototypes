using UnityEngine;

namespace RolyPoly
{

    public class RolyPolyDemo : MonoBehaviour
    {
        [SerializeField] Transform anchor;
        [SerializeField] Transform cube;
        [SerializeField] Transform sphere;
        [SerializeField] Transform capsule;
        [SerializeField] LayerMask cubeLayer;
        [SerializeField] LayerMask planeLayer;
        [SerializeField] float springLength = 0;
        [SerializeField] float springStiffness = 0.5f;  // Controls how "stiff" the spring is
        [SerializeField] float damping = 0.5f;         // Controls how quickly oscillations settle
        [SerializeField] float mass = 1f;              // Mass of the point
        Vector3 velocity = Vector3.zero;               // Current velocity of the point
        bool isCubeGrabbed;

        void Start()
        {
            sphere.position = Vector3.up;
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, cubeLayer))
                {
                    if (hit.transform.gameObject.name == "Cube")
                    {
                        isCubeGrabbed = true;
                    }
                }

                if (Physics.Raycast(ray, out RaycastHit hitPlane, Mathf.Infinity, planeLayer) && isCubeGrabbed)
                {
                    cube.transform.position = hitPlane.point;


                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isCubeGrabbed = false;
            }


            sphere.position = cube.position + Vector3.up * 2;
            Vector3 dir = (sphere.position - capsule.position).normalized;
            capsule.up = dir;
        }
        void FixedUpdate()
        {
            if (isCubeGrabbed) return;
            AnchorPointSpring(anchor, cube);
        }

        void PointPointSpring(Transform pointA, Transform pointB)
        {
            var velocity = SpringCore(pointA.position, pointB.position);

            pointB.position += velocity;
            pointA.position -= velocity;
        }

        void AnchorPointSpring(Transform anchor, Transform point)
        {
            var velocity = SpringCore(anchor.position, point.position);
            point.position += velocity;
        }

        Vector3 SpringCore(Vector3 p1, Vector3 p2)
        {
            // Calculate spring force using Hooke's Law: F = -kx
            Vector3 displacement = p2 - p1;

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

            return velocity;
        }
    }

}