using UnityEngine;

public class Quad : MonoBehaviour
{
    [SerializeField] Transform[] points;

    Mesh mesh;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        Vector3[] positions = new Vector3[4];

        for (int i = 0; i < points.Length; i++)
        {
            positions[i] = points[i].position;
        }
        mesh.vertices = positions;
    }
}
