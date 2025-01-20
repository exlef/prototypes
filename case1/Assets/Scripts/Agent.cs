using UnityEngine;

public class Agent : MonoBehaviour
{
    public bool hasReached { get; private set; }
    public float radius = 1;
    public bool isStopped { get; private set; }
    Vector2 destination;
    float t;

    public Vector2 pos
    {
        get => new Vector2(transform.position.x, transform.position.z);
        set => transform.position = new Vector3(value.x, transform.position.y, value.y);
    }

    public void SetDestination(Vector3 dest)
    {
        hasReached = false;
        isStopped = false;
        
        // destination = new Vector3(dest.x, transform.position.y, dest.y);
        destination = new Vector2(dest.x, dest.z);
    }

    public void Stop()
    {
        isStopped = true;
    }


    public void Tick()
    {
        if (isStopped || hasReached) return;
        Vector3 displacementVec = new Vector3(destination.x, transform.position.y, destination.y) - transform.position;
        Vector3 dir = displacementVec.normalized;
        transform.position += dir * (Time.deltaTime * 3);
        if (Vector3.SqrMagnitude(displacementVec) < radius * radius)
        {
            hasReached = true;
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
