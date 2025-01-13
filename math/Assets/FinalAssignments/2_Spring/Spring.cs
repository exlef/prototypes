using System;
using UnityEditor;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] float radius = 1;
    [SerializeField] int resolution = 32;
    [SerializeField] int layerCount = 2;
    [SerializeField] float heightMultiplier = 0.5f;
    [SerializeField] Color bottom = Color.black;    
    [SerializeField] Color top = Color.white;
    
    Vector3[] points;
    Color[] colors;

    private void OnDrawGizmos()
    {
        Handles.matrix = transform.localToWorldMatrix;
        
        points = new Vector3[resolution * layerCount];
        colors = new Color[resolution * layerCount];
        var angle = 360.0f / resolution;

        for (int j = 0; j < layerCount; ++j)
        {
            for (int i = 0; i < resolution; i++)
            {
                var rotation = Quaternion.Euler(0, angle * i, 0);
                var dir = Vector3.right;
                dir = (rotation * dir).normalized;
                var pos = dir * radius;
                pos.y = Mathf.Lerp(j * heightMultiplier, (j+1) * heightMultiplier, i / (float)resolution);

                points[j * resolution +  i] = pos;
            }    
        }

        for (var i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.Lerp(bottom, top, i / (float)colors.Length);
        }

        Handles.DrawAAPolyLine(5, colors,points);
    }
}