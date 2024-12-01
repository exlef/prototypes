using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CircleMeshGen : MonoBehaviour
{
    [SerializeField] float circleRadius = 1;
    Mesh circleMesh;

    void Start()
    {
        circleMesh = CreateCircleMesh(circleRadius);
        GetComponent<MeshFilter>().mesh = circleMesh;

        // Optionally, set a material for the MeshRenderer
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard")); // Use a standard shader
    }

    // Mesh CreateCircleMesh(float radius)
    // {
    //     Mesh mesh = new Mesh();

    //     // Create vertices
    //     int segments = 32;
    //     Vector3[] vertices = new Vector3[segments + 1];
    //     vertices[0] = Vector3.zero;

    //     for (int i = 0; i <= segments; i++)
    //     {
    //         float angle = i * (2f * Mathf.PI / segments);
    //         vertices[i] = new Vector3(
    //             Mathf.Cos(angle) * radius,
    //             Mathf.Sin(angle) * radius,
    //             0
    //         );
    //     }

    //     // Create triangles
    //     int[] triangles = new int[segments * 3];
    //     for (int i = 0; i < segments; i++)
    //     {
    //         triangles[i * 3] = 0;
    //         triangles[i * 3 + 1] = i + 1;
    //         triangles[i * 3 + 2] = i + 2;
    //     }

    //     mesh.vertices = vertices;
    //     mesh.triangles = triangles;
    //     mesh.RecalculateBounds();

    //     return mesh;
    // }

    // Mesh CreateCircleMesh(float radius)
    // {
    //     Mesh mesh = new Mesh();

    //     // Create vertices
    //     int segments = 32;
    //     Vector3[] vertices = new Vector3[segments + 1];
    //     vertices[0] = Vector3.zero; // Center vertex

    //     for (int i = 0; i < segments; i++)
    //     {
    //         float angle = i * (2f * Mathf.PI / segments);
    //         vertices[i + 1] = new Vector3(
    //             Mathf.Cos(angle) * radius,
    //             Mathf.Sin(angle) * radius,
    //             0
    //         );
    //     }

    //     // Create triangles
    //     int[] triangles = new int[segments * 3];
    //     for (int i = 0; i < segments; i++)
    //     {
    //         triangles[i * 3] = 0; // Center vertex
    //         triangles[i * 3 + 1] = i + 1; // Current vertex
    //         triangles[i * 3 + 2] = (i + 2) % (segments + 1); // Next vertex, wrapping around
    //     }

    //     mesh.vertices = vertices;
    //     mesh.triangles = triangles;
    //     mesh.RecalculateNormals(); // Recalculate normals for proper lighting
    //     mesh.RecalculateBounds();

    //     return mesh;
    // }

    // Mesh CreateCircleMesh(float radius)
    // {
    //     Mesh mesh = new Mesh();

    //     // Create vertices
    //     int segments = 32;
    //     Vector3[] vertices = new Vector3[segments + 1];
    //     vertices[0] = Vector3.zero; // Center vertex

    //     for (int i = 0; i < segments; i++)
    //     {
    //         float angle = i * (2f * Mathf.PI / segments);
    //         vertices[i + 1] = new Vector3(
    //             Mathf.Cos(angle) * radius,
    //             Mathf.Sin(angle) * radius,
    //             0
    //         );
    //     }

    //     // Create triangles
    //     int[] triangles = new int[segments * 3];
    //     for (int i = 0; i < segments; i++)
    //     {
    //         triangles[i * 3] = 0; // Center vertex
    //         triangles[i * 3 + 1] = i + 1; // Current vertex
    //         triangles[i * 3 + 2] = (i + 2) % (segments + 1); // Next vertex, wrapping around
    //     }

    //     // Fix the last triangle
    //     triangles[triangles.Length - 1] = 1; // Last triangle should connect to the first vertex

    //     mesh.vertices = vertices;
    //     mesh.triangles = triangles;
    //     mesh.RecalculateNormals(); // Recalculate normals for proper lighting
    //     mesh.RecalculateBounds();

    //     return mesh;
    // }


    Mesh CreateCircleMesh(float radius)
    {
        Mesh mesh = new Mesh();
        // Create vertices*
        int segments = 32;
        Vector3[] vertices = new Vector3[segments + 1];
        vertices[0] = Vector3.zero; // Center vertex*
        for (int i = 0; i < segments; i++)
        {
            float angle = i * (2f * Mathf.PI / segments);
            vertices[i + 1] = new Vector3(

                 Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0
                );
        }
        // Create triangles - reverse order to face forward*
        int[] triangles = new int[segments * 3];
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0; // Center vertex*
            triangles[i * 3 + 1] = (i + 2) % (segments + 1); // Next vertex, wrapping around*
            triangles[i * 3 + 2] = i + 1; // Current vertex*
        }
        // Fix the last triangle*
        triangles[triangles.Length - 2] = 1; // Last triangle should connect to the first vertex*

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }


}
