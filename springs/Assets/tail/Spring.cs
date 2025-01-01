using System.Collections.Generic;
using UnityEngine;

namespace TailDemo
{
    public class Spring : MonoBehaviour
    {
        [SerializeField] Transform pointRed;
        [SerializeField] Transform pointBlue;
        [SerializeField] List<Point> points;
        [SerializeField] bool followMouse = false;
        [SerializeField] float springLength = 0;
        [SerializeField] float springStiffness = 0.5f;  // Controls how "stiff" the spring is
        [SerializeField] float damping = 0.5f;         // Controls how quickly oscillations settle
        [SerializeField] float mass = 1f;              // Mass of the point
        Vector3 velocity = Vector3.zero;               // Current velocity of the point

        void Start()
        {
            if(points[0].isAnchor == false) Debug.Log("first point in list should be anchor");
            for (int i = 1; i < points.Count; i++)
            {
                if(points[i].isAnchor)
                {
                    Debug.Log("no other point can be anchor besides first poin in the list");
                    break;
                }
            }
            if(points.Count %2 != 0) Debug.Log("number of points should be even");
        }

        void FixedUpdate()
        {
            // for (int i = 0; i < points.Count; i += 2)
            // {
            //     if(i == 0)
            //     {
            //         AnchorPointSpring(points[i].tr, points[i + 1].tr);
            //     }
            //     else
            //     {
            //         PointPointSpring(points[i].tr, points[i + 1].tr);
            //     }
            // }

            for (int i = 0; i < points.Count; i++)
            {
                // if((i + 1) >= points.Count) continue;
                if (i == 0)
                {
                    continue;
                }
                else if(i == 1)
                {
                    AnchorPointSpring(points[i - 1].tr, points[i].tr);
                }
                else
                {
                    PointPointSpring(points[i].tr, points[i - 1].tr);
                }
            }
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

        [System.Serializable]
        struct Point
        {
            public bool isAnchor;
            public Transform tr;
            public readonly Vector3 pos => tr.position;
        }
    }
}

// void Update()
// {
// if (followMouse)
// pointRed.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
// }