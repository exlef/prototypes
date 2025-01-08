using UnityEngine;

public class Angles : MonoBehaviour
{
    [SerializeField] Transform pointTr;

    void OnDrawGizmos()
    {
        Vector3 toPointDir = pointTr.position - transform.position;

        // this fucntion gives angle relative to x axis. 
        // the angle is signed. 
        float angleDeg = Mathf.Atan2(toPointDir.y, toPointDir.x) * Mathf.Rad2Deg;
        angleDeg = (angleDeg + 360) % 360;
        // this gives the angle between two vectors.
        // float angleDeg = Vector2.SignedAngle(transform.right, toPointDir);
        Debug.Log(angleDeg);
    }
}
