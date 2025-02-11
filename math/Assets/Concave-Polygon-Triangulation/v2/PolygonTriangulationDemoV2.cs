using System.Collections.Generic;
using UnityEngine;

public class PolygonTriangulationDemoV2 : MonoBehaviour
{
    private Vector2[] points;
    void Start()
    {
        points = new Vector2[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            Vector2 pos = transform.GetChild(i).position;
            points[i] = pos;
        }
        
        // Get triangulation indices
        List<int> triangles = EarClipping.Triangulate(points);

        // Create a mesh from the results
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vertices[i] = points[i];
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        
        var go = new GameObject();
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        
        var renderer2 = go.GetComponent<MeshRenderer>();
        renderer2.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        go.GetComponent<MeshFilter>().mesh = mesh;
    }
}
