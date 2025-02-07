using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ConvexPolygonTriangulation : MonoBehaviour
{
    [SerializeField] Transform[] pointsTr;
    Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        
        var points = new Vector3[pointsTr.Length];
        for (int i = 0; i < pointsTr.Length; i++) points[i] = pointsTr[i].position;
        mesh.vertices = points;

        int[] triangles = new int[6]; // how can I calculate the triangles count for given vertex count.
        for (int i= 0, k = 0 ; i < points.Length; i += 2, k += 3)
        {
            triangles[k + 0] = i;
            triangles[k + 1] = i + 1 <= points.Length - 1 ? i + 1 : 0;
            triangles[k + 2] = i + 2 <= points.Length - 1 ? i + 2 : 0;
        }
        mesh.triangles = triangles;
        
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDestroy()
    {
        Destroy(mesh);
    }
}
