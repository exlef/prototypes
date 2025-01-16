using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Donut2 : MonoBehaviour
{
    [SerializeField] [Min(0f)] int resolution = 256;
    [SerializeField] [Min(0f)] private float turns;
    [SerializeField] [Min(0f)] private float height; 
    [SerializeField] [Min(0f)] private float radiusMinor;
    [SerializeField] [Min(0f)] private float radiusMajor;
    const float TAU = Mathf.PI * 2;


    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        
        Vector3 prevPoint = GetSpringPoint(0f);
        for (int i = 0; i < resolution; i++)
        {
            var t = i / (resolution - 1f);
            Vector3 p = GetSpringPoint(t);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(prevPoint, p);
            prevPoint = p;
        }
    }

    Vector3 GetSpringPoint(float t)
    {
        float angleRad = TAU  * t;

        // this is the center of coil
        Vector3 localCenter = new Vector3( MathF.Cos(angleRad), 0f, MathF.Sin(angleRad) ) * radiusMajor;
        Vector3 localAxisX = localCenter.normalized;
        Vector3 localAxisY = Vector3.up;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(localCenter, localAxisX);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(localCenter, localAxisY);
        
        float coilAngleRad = TAU * turns * t;
        Vector3 v = new Vector3(MathF.Cos(coilAngleRad), MathF.Sin(coilAngleRad), 0);
        Vector3 coilRight = Vector3.Dot(Vector3.right, v) * localAxisX;
        Vector3 coilLeft = Vector3.Dot(Vector3.up, v) * localAxisY;
        // Vector3 coilDir = new Vector3(MathF.Cos(coilAngleRad) * localAxisX.x, MathF.Sin(coilAngleRad) * localAxisY.y, 0);
        // Vector3 coilDir = new Vector3(MathF.Cos(coilAngleRad), MathF.Sin(coilAngleRad), 0);
        Gizmos.color = Color.yellow;
        Vector3 coilDir = coilRight + coilLeft;
        Gizmos.DrawRay(localCenter, coilDir.normalized);
        
        return localCenter;
        // return localCenter + coilDir;
    }
}
