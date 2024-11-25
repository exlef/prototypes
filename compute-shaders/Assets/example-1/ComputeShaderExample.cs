using UnityEngine;

public class ComputeShaderExample : MonoBehaviour
{
    [SerializeField] ComputeShader computeShader;
    ComputeBuffer dataBuffer;
    const int bufferSize = 1024;
    const int threadGroupSize = 64;

    static readonly int 
        bufferSizeId = Shader.PropertyToID("_BufferSize"),
        dataBufferId = Shader.PropertyToID("_DataBuffer");
    static readonly string kernelName = "CSMain";

    void Start()
    {
        dataBuffer = new ComputeBuffer(bufferSize, sizeof(float));

        float[] data = new float[bufferSize];
        for (int i = 0; i < bufferSize; i++)
        {
            data[i] = 1;
        }
        dataBuffer.SetData(data);

        int kernelIndex = computeShader.FindKernel(kernelName);
        computeShader.SetInt(bufferSizeId, bufferSize);
        computeShader.SetBuffer(kernelIndex, dataBufferId, dataBuffer);

        int threadGroupsX = Mathf.CeilToInt((float)bufferSize / threadGroupSize);
        int threadGroupsY = 1;
        int threadGroupsZ = 1;

        computeShader.Dispatch(kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);

        float[] resultData = new float[bufferSize];
        dataBuffer.GetData(resultData);
        for (int i = 0; i < bufferSize; i++)
        {
            Debug.Log($"Result {i}: {resultData[i]}");
        }
    }

    void OnDestroy()
    {
        if (dataBuffer != null)
        {
            dataBuffer.Release();
        }
    }
}
