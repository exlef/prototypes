using Unity.Mathematics;
using UnityEngine;
using Ex;

public class Demo : MonoBehaviour
{
    Material mat;
    Mesh mesh;
    ComputeBuffer positionBuffer;
    RenderParams rp;

    void Start()
    {
        mat = Utils.GenerateMaterial();
        mesh = Utils.GenerateCircleMesh(1);

        Vector2[] instanceWorldPositions = {
            new(0, 0),
            new(1.1f, 0),
            new(2.2f, 0),
        };
        positionBuffer = new ComputeBuffer(instanceWorldPositions.Length, sizeof(float) * 2);
        positionBuffer.SetData(instanceWorldPositions);

        rp = new(mat)
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
        positionBuffer.Dispose();
        positionBuffer?.Release();
        positionBuffer = null;
    }



    struct Point
    {
        float2 pos;
        float2 prevPos;

        public Point(float2 _pos)
        {
            pos = _pos;
            prevPos = _pos - new float2(1,1); // for now. later I probably should set this to float2.Zero
        }
    }
}
