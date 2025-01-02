using UnityEngine;

namespace SpriteDemo
{

    public class Quad : MonoBehaviour
    {
        [SerializeField] Transform[] points;
        [SerializeField] Transform anchor;

        Mesh mesh;

        void Start()
        {
            mesh = GetComponent<MeshFilter>().mesh;
            // for (int i = 0; i < points.Length; i++)
            // {
            //     Debug.Log(mesh.vertices[i]);
            // }
        }

        void Update()
        {
            transform.position = anchor.position;

            Vector3[] positions = new Vector3[4];
            for (int i = 0; i < points.Length; i++)
            {
                // positions[i] = points[i].position;
                positions[i] = transform.InverseTransformPoint(points[i].position);
            }
            mesh.vertices = positions;
        }
    }

}