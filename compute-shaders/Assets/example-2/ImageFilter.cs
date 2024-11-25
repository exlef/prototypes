using UnityEngine;

public class ImageFilter : MonoBehaviour
{
    [SerializeField] Texture2D texture;
    [Space]
    [SerializeField] ComputeShader computeShader;
    ComputeBuffer dataBuffer;
    int bufferSize;
    const int threadGroupSize = 64;

    static readonly int 
        bufferSizeId = Shader.PropertyToID("_BufferSize"),
        dataBufferId = Shader.PropertyToID("_DataBuffer"),
        widthId = Shader.PropertyToID("_Width"),
        heightId = Shader.PropertyToID("_Height");
    static readonly string kernelName = "CSMain";

    void Start()
    {
        bufferSize = texture.width * texture.height;
        dataBuffer = new ComputeBuffer(bufferSize, sizeof(float) * 4);

        Color[] data = new Color[bufferSize];
        data = texture.GetPixels();
        
        dataBuffer.SetData(data);

        int kernelIndex = computeShader.FindKernel(kernelName);
        computeShader.SetInt(bufferSizeId, bufferSize);
        computeShader.SetInt(heightId, texture.height);
        computeShader.SetInt(widthId, texture.width);
        computeShader.SetBuffer(kernelIndex, dataBufferId, dataBuffer);

        int threadGroupsX = Mathf.CeilToInt((float)bufferSize / threadGroupSize);
        int threadGroupsY = 1;
        int threadGroupsZ = 1;

        computeShader.Dispatch(kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);

        Color[] resultData = new Color[bufferSize];
        dataBuffer.GetData(resultData);
        
        texture.SetPixels(resultData);

        // foreach (var item in resultData)
        // {
            // Debug.Log($"{item.r}, {item.g}, {item.b}");
        // }

        texture.Apply();
    }

    void OnDestroy()
    {
        if (dataBuffer != null)
        {
            dataBuffer.Release();
        }
    }
}
