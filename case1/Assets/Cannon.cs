using UnityEngine;

class Cannon : MonoBehaviour
{
    public void Move(float x, float limit)
    {
        Vector3 pos = transform.position + new Vector3(x, 0, 0);
        // this assumes cannon is at Vector3.zero at start
        pos.x = Mathf.Clamp(pos.x, -limit, limit);
        transform.position = pos;
    }
}