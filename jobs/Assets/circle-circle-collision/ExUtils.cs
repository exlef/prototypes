using UnityEngine;
using System.Runtime.CompilerServices;

public static class ExUtils
{
    public static void SetPosX(this Transform transform, float x)
    {
        Vector3 newPosition = transform.position;
        newPosition.x = x;
        transform.position = newPosition;
    }

    public static void SetPosY(this Transform transform, float y)
    {
        Vector3 newPosition = transform.position;
        newPosition.y = y;
        transform.position = newPosition;
    }

    public static void SetPosZ(this Transform transform, float z)
    {
        Vector3 newPosition = transform.position;
        newPosition.z = z;
        transform.position = newPosition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 RndVec2(float magnitude = 1) => new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * magnitude;
}
