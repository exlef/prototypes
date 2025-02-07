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
        if(points.Length < 3) {Debug.Log("not supported"); return;}
        for (int i = 0; i < pointsTr.Length; i++) points[i] = pointsTr[i].position;
        mesh.vertices = points;

        int numberOfTriangles = mesh.vertices.Length - 2;
        int[] triangleIndexes = new int[numberOfTriangles * 3];
        triangleIndexes[0] = 0;
        triangleIndexes[1] = 1;
        triangleIndexes[2] = 2;
        for (int vi = 2, ti = 1; ti < numberOfTriangles; vi++, ti++) // ti : triangle index || vi : vertex index
        {
            triangleIndexes[ti * 3 + 0] = 0;
            triangleIndexes[ti * 3 + 1] = vi;
            triangleIndexes[ti * 3 + 2] = vi + 1 < points.Length ? vi + 1 : points.Length - 1;
        }
        
        mesh.triangles = triangleIndexes;
        
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDestroy()
    {
        Destroy(mesh);
    }
}
