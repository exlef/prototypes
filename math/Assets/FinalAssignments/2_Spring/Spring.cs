using System;
using UnityEditor;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] Vector3[] points;
    [SerializeField] float radius = 1;
    [SerializeField] int resolution = 32;
    [SerializeField] int layerCount = 2;
    [SerializeField] float heightMultiplier = 0.5f;
    [SerializeField] Color[] colors;

    private void OnDrawGizmos()
    {
        Handles.matrix = transform.localToWorldMatrix;
        
        points = new Vector3[resolution * layerCount];
        var angle = 360.0f / resolution;

        for (int j = 0; j < layerCount; ++j)
        {
            for (int i = 0; i < resolution; i++)
            {
                var rotation = Quaternion.Euler(0, angle * i, 0);
                var dir = Vector3.right;
                dir.y = Mathf.Lerp(j * heightMultiplier, (j+1) * heightMultiplier, i / (float)resolution);

                dir = rotation * dir;
                points[j * resolution +  i] = dir * radius;
            }    
        }
        
        Handles.DrawAAPolyLine(5, points);
    }
}
