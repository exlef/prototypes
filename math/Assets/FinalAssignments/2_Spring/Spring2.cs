using System;
using UnityEngine;

public class Spring2 : MonoBehaviour
{
    [SerializeField] [Min(0f)] int resolution = 256;
    [SerializeField] [Min(0f)] private float turns;
    [SerializeField] [Min(0f)] private float height;
    [SerializeField] [Min(0f)] private float radius;

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        
        Vector3 prevPoint = GetSpringPoint(0f);
        for (int i = 0; i < resolution; i++)
        {
            var t = i / (resolution - 1f);
            Vector3 p = GetSpringPoint(t);
            Gizmos.DrawLine(prevPoint, p);
            prevPoint = p;
        }
    }

    Vector3 GetSpringPoint(float t)
    {
        float angleRad = MathF.PI * 2 * turns * t;
        Vector2 vecXZ = new(MathF.Cos(angleRad) * radius, MathF.Sin(angleRad) * radius);
        return new(vecXZ.x, height * t, vecXZ.y);
    }
    
    void Draw(Func<float, Vector3> f)
    {
        Vector3 prevPoint = f(0f);
        for (int i = 0; i < resolution; i++)
        {
            var t = i / (resolution - 1f);
            Vector3 p = f(t);
            Gizmos.DrawLine(prevPoint, p);
            prevPoint = p;
        }
    }
}
