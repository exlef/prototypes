using UnityEngine;

public class Quad2 : MonoBehaviour
{
    [SerializeField] Mesh mesh;
    [SerializeField] bool followMouse = false;
    [SerializeField] float springStiffness = 0.2f;
    [SerializeField] float damping = 0.5f;
    [SerializeField] float mass = 1f;
    Vector3[] cornerPositions = new Vector3[4];
    Vector3[] originalVertexPositionsInObjectSpace = new Vector3[4];
    Point[] points = new Point[4];

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            originalVertexPositionsInObjectSpace[i] = mesh.vertices[i];
        }

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 pos = mesh.vertices[i];
            cornerPositions[i] = transform.TransformPoint(pos);
            points[i] = new Point(cornerPositions[i]);
        }
    }

    void Update()
    {
        if (followMouse)
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 pos = originalVertexPositionsInObjectSpace[i];
            cornerPositions[i] = transform.TransformPoint(pos);
            Debug.DrawRay(cornerPositions[i], Vector3.up);
        }

        UpdatePoints();

        for (int i = 0; i < points.Length; i++)
        {
            Debug.DrawRay(points[i].position, Vector3.right, Color.red);
        }

        Vector3[] newVetexPositions = new Vector3[4];
        for (int i = 0; i < points.Length; i++)
        {
            newVetexPositions[i] = transform.InverseTransformPoint(points[i].position);
        }
        mesh.vertices = newVetexPositions;
    }

    void UpdatePoints()
    {
        for (int i = 0; i < points.Length; i++)
        {
            // Calculate spring force using Hooke's Law: F = -kx
            Vector3 displacement = points[i].position - cornerPositions[i];
            Vector3 springForce = -springStiffness * displacement;

            // Calculate damping force: F = -cv
            Vector3 dampingForce = -damping * points[i].velocity;

            // Sum up forces
            Vector3 totalForce = springForce + dampingForce;

            // Calculate acceleration (F = ma)
            Vector3 acceleration = totalForce / mass;

            // Update velocity (integrate acceleration)
            points[i].velocity += acceleration;

            // Update position (integrate velocity)

            points[i].position += points[i].velocity;
        }
    }

    [System.Serializable]
    public class Point
    {
        public Vector3 position;
        public Vector3 velocity;

        public Point(Vector3 pos)
        {
            position = pos;
            velocity = Vector3.zero;
        }
    }
}
