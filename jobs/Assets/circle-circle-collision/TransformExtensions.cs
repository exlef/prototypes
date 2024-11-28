using UnityEngine;

public static class TransformExtensions
{
    // Set the x component of the position
    public static void SetPosX(this Transform transform, float x)
    {
        Vector3 newPosition = transform.position;
        newPosition.x = x;
        transform.position = newPosition;
    }

    // Set the y component of the position
    public static void SetPosY(this Transform transform, float y)
    {
        Vector3 newPosition = transform.position;
        newPosition.y = y;
        transform.position = newPosition;
    }

    // Set the z component of the position
    public static void SetPosZ(this Transform transform, float z)
    {
        Vector3 newPosition = transform.position;
        newPosition.z = z;
        transform.position = newPosition;
    }
}
