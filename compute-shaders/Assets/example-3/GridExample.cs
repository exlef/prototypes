using System.Collections;
using UnityEngine;

public class GridExample : MonoBehaviour
{
    [SerializeField] bool cpuBased = true;
    [SerializeField] GameObject cubePrefab;
    [SerializeField] int width = 8;
    [SerializeField] int height = 8;
    [SerializeField] ComputeShader computeShader;
    ComputeBuffer dataBuffer;
    GameObject[] cubes;
    const int threadGroupSize = 8;
    static int kernelIndex = -1;

    IEnumerator Start()
    {
        cubes = new GameObject[width * height];
        dataBuffer = new ComputeBuffer(width * height, sizeof(float) * 3);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var go = Instantiate(cubePrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                cubes[MapTo1D(x, y)] = go;
                yield return null;
            }
        }

        float[] data = new float[width * height * 3];
        for (int i = 0, cubesIndex = 0; cubesIndex < width * height; i += 3, cubesIndex++)
        {
            data[i]   = cubes[cubesIndex].transform.position.x;
            data[i+1] = cubes[cubesIndex].transform.position.y;
            data[i+2] = cubes[cubesIndex].transform.position.z;
        }
        dataBuffer.SetData(data);

        kernelIndex = computeShader.FindKernel("CSMain");
        computeShader.SetInt("_Height", height);
        computeShader.SetInt("_Width", width);
         computeShader.SetInt("_IndexOfClickedCube", -1);
        computeShader.SetBuffer(kernelIndex, "_DataBuffer", dataBuffer); 
    }

    public void Clicked(GameObject cube)
    {
        if(cpuBased) ClickedCPU(cube);
        else ClickedGPU(cube);
        
    }

    void ClickedCPU(GameObject cube)
     {
        int clickedIndex = -1;
        for (int i = 0; i < height * width; i++)
        {
            if(cube == cubes[i])
            {
                clickedIndex = i;
            }
        }

        int x = clickedIndex % width;
        int y = (clickedIndex - x) / width;


        cubes[clickedIndex].GetComponent<Renderer>().material.color = Color.red;

        int right = MapTo1D(x + 1, y);
        int left = MapTo1D(x - 1, y);
        int up = MapTo1D(x, y + 1);
        int down = MapTo1D(x, y - 1);


        if(InRange(right)) cubes[right].transform.localPosition += new Vector3(0,1,0);
        if(InRange(left))  cubes[left].transform.localPosition += new Vector3(0,1,0);
        if(InRange(up))    cubes[up].transform.localPosition += new Vector3(0,1,0);
        if(InRange(down))  cubes[down].transform.localPosition += new Vector3(0,1,0);
     }

    void ClickedGPU(GameObject cube)
    {
        int clickedIndex = -1;
        for (int i = 0; i < height * width; i++)
        {
            if(cube == cubes[i])
            {
                clickedIndex = i;
            }
        }

        cubes[clickedIndex].GetComponent<Renderer>().material.color = Color.red;


        computeShader.SetInt("_IndexOfClickedCube", clickedIndex);


        int threadGroupsX = Mathf.CeilToInt((float)width * height / threadGroupSize);
        int threadGroupsY = Mathf.CeilToInt((float)width * height / threadGroupSize);
        int threadGroupsZ = 1;
        computeShader.Dispatch(kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);

        float[] resultData = new float[width * height * 3];
        dataBuffer.GetData(resultData);
        for (int i = 0, cubesIndex = 0; cubesIndex < width * height; i += 3, cubesIndex++)
        {
            cubes[cubesIndex].transform.position = new Vector3(resultData[i], resultData[i + 1], resultData[i + 2]);
        }

    }

    int MapTo1D(int x, int y)
    {
        if(InRange(x, y) == false) return -1;
        return y * width + x;
    }

    bool InRange(int x)
    {
        return x >= 0 && x < width * height;
    }

    bool InRange(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
