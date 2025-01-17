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
        for (int i = 0; i < pointsTr.Length; i++)
        {
            points[i] = pointsTr[i].position;
        }
        mesh.vertices = points;

        int[] triangles = new int[pointsTr.Length];
        for (int i = 0; i < points.Length; i += 4)
        {
            Debug.Log(i);
            triangles[i] = i;
            triangles[i+1] = i+1;
            if (i + 2 > points.Length - 1) triangles[i + 2] = 0;
            else triangles[i + 2] = i+2;
        }

        Debug.Log(triangles.Length);
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDestroy()
    {
        Destroy(mesh);
    }
}
