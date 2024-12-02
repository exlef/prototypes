using UnityEngine;

public class Drawing : MonoBehaviour
{
    public Material material;
    public Mesh mesh;

    ComputeBuffer positionBuffer;
    RenderParams rp;

    void Start()
    {
        Vector4[] instanceWorldPositions = {
            new(2, 2, 2, 1),
            new(3, 3, 3, 1),
            new(50, 50, 50, 1),
        };
        positionBuffer = new ComputeBuffer(instanceWorldPositions.Length, sizeof(float) * 4);
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
