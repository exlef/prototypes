using System;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float radius = 1;

    public Vector2 pos
    {
        get => new Vector2(transform.position.x, transform.position.z);
        set => transform.position = new Vector3(value.x, transform.position.y, value.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
