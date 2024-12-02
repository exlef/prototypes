using UnityEngine;

public class Drawing : MonoBehaviour
{
    public Material material;
    public Mesh mesh;

    ComputeBuffer positionBuffer;
    RenderParams rp;

    void Start()
    {
        Vector2[] instanceWorldPositions = {
            new(0, 0),
            new(1.1f, 0),
            new(2.2f, 0),
        };
        positionBuffer = new ComputeBuffer(instanceWorldPositions.Length, sizeof(float) * 2);
        positionBuffer.SetData(instanceWorldPositions);

        rp = new(material)
        {
            worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one), // use tighter bounds
            matProps = new MaterialPropertyBlock()
        };
        rp.matProps.SetBuffer("_PositionBuffer", positionBuffer);
    }

    void Update()
    {
        Graphics.RenderMeshPrimitives(rp, mesh, 0, 3);
    }

    void OnDestroy()
    {
        positionBuffer?.Release();
        positionBuffer = null;
    }


}
