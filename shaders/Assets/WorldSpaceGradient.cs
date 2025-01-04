using UnityEngine;

public class WorldSpaceGradient : MonoBehaviour
{

    Bounds bounds;
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;
            Bounds bounds = mesh.bounds;

            // Pass the bounding box to the shader
            // material.SetVector("_MinBounds", bounds.min);
            // material.SetVector("_MaxBounds", bounds.max);
        }
    }

    void OnDrawGizmos()
    {
        // MeshFilter meshFilter = GetComponent<MeshFilter>();
        // Bounds bounds = default;
        // if (meshFilter != null)
        // {
            // Mesh mesh = meshFilter.mesh;
            // bounds = mesh.bounds;

            // Pass the bounding box to the shader
            // material.SetVector("_MinBounds", bounds.min);
            // material.SetVector("_MaxBounds", bounds.max);
        // }
        Gizmos.DrawCube(bounds.center, bounds.size);
    }
}
