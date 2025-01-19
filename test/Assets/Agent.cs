using System;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public bool hasReached { get; private set; }
    public float radius = 1;
    bool stop;
    Vector3 initialPos;
    Vector3 destination;
    float t;

    public Vector2 pos
    {
        get => new Vector2(transform.position.x, transform.position.z);
        set => transform.position = new Vector3(value.x, transform.position.y, value.y);
    }

    public void SetDestination(Vector2 dest)
    {
        hasReached = false;
        stop = false;
        initialPos = transform.position;
        
        destination = new Vector3(dest.x, transform.position.y, dest.y);
    }

    public void Stop()
    {
        stop = true;
    }


    public void Tick()
    {
        if (stop) return;
        Vector3 dir = (destination - transform.position).normalized;
        transform.position += dir * Time.deltaTime * 3;
        if (t >= 1)
        {
            hasReached = true;
            stop = true;
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
