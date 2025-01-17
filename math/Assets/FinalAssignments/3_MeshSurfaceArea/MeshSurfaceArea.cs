using System;
using UnityEngine;

public class MeshSurfaceArea : MonoBehaviour
{
    [SerializeField] Mesh mesh;
    void Start()
    {
        // var verts = mesh.vertices;
        // var tris = mesh.triangles;
        // for (int i = 0; i < tris.Length; i += 3)
        // {
        //     var index1 = tris[i];
        //     var index2 = tris[i+1];
        //     var index3 = tris[i+2];
        //
        //     var p1 = verts[index1];
        //     var p2 = verts[index2];
        //     var p3 = verts[index3];
        //     Debug.Log($"{p1} || {p2}  ||  {p3}");
        // }
        
        var verts = mesh.vertices;
        var tris = mesh.triangles;
        float area = 0f;
        for (int i = 0; i < tris.Length; i += 3)
        {
            var index1 = tris[i];
            var index2 = tris[i+1];
            var index3 = tris[i+2];

            var p1 = verts[index1];
            var p2 = verts[index2];
            var p3 = verts[index3];
            
            var p1_p2 = p2 - p1;
            var p1_p3 = p3 - p1;
            var projectedVec = Vector3.Dot(p1_p2, p1_p3) * p1_p3.normalized;

            var baseLength = Vector3.Distance(p1, p3);
            var height = Vector3.Distance(p2, projectedVec);

            area += baseLength * height / 2f;
        }

        Debug.Log(area);
    }

    void OnDrawGizmos()
    {
        // return;
        var verts = mesh.vertices;
        var tris = mesh.triangles;
        float area = 0f;
        for (int i = 0; i < tris.Length; i += 3)
        {
            var index1 = tris[i];
            var index2 = tris[i+1];
            var index3 = tris[i+2];

            var p1 = verts[index1];
            var p2 = verts[index2];
            var p3 = verts[index3];
            
            // debugging
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(p1, 0.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(p2, 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(p3, 0.2f);
            //---------

            var p1_p2 = p2 - p1;
            var p1_p3 = p3 - p1;
            var projectedVec = p1 + Vector3.Dot(p1_p2, p1_p3.normalized) * p1_p3.normalized;

            var baseLength = Vector3.Distance(p1, p3);
            var height = Vector3.Distance(p2, projectedVec);
            
            Debug.DrawLine(p1, p2);
            Debug.DrawLine(p2, p3);
            Debug.DrawLine(p1, p3);
            Debug.DrawLine(p2, projectedVec, Color.yellow);

            area += baseLength * height / 2f;
        }

        Debug.Log(area);
    }

    void Update()
    {
        
    }
}
