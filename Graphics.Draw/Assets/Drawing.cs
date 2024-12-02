using UnityEngine;

public class Drawing : MonoBehaviour
{
    public Material material;
    public Mesh mesh;

    GraphicsBuffer commandBuf;
    GraphicsBuffer.IndirectDrawIndexedArgs[] commandData;
    const int commandCount = 3;
    RenderParams rp;

    void Start()
    {
        commandBuf = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, commandCount, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[commandCount];

        commandData[0].indexCountPerInstance = mesh.GetIndexCount(0);
        commandData[0].instanceCount = 100;
        commandData[1].indexCountPerInstance = mesh.GetIndexCount(0);
        commandData[1].instanceCount = 10;
        commandData[2].indexCountPerInstance = mesh.GetIndexCount(0);
        commandData[2].instanceCount = 1;

        commandBuf.SetData(commandData);

        rp = new RenderParams(material)
        {
            worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one) // use tighter bounds for better FOV culling
        };
    }

    void Update()
    {
        Graphics.RenderMeshIndirect(rp, mesh, commandBuf, commandCount);
    }

    void OnDestroy()
    {
        commandBuf?.Release();
        commandBuf = null;
    }


}
