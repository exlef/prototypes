using UnityEngine;

public class Agent 
{
    public bool hasReached { get; private set; }
    public readonly float radius = 1;
    readonly float stoppingDistance = 1;
    public bool isStopped { get; private set; }
    Vector2 destination;
    float t;
    private readonly float speed;

    private readonly Transform tr;

    public Agent(Transform tr, float radius, float stoppingDistance, float speed)
    {
        this.tr = tr;
        this.radius = radius;
        this.stoppingDistance = stoppingDistance;
        this.speed = speed;
    }

    public Vector2 pos
    {
        get => new Vector2(tr.position.x, tr.position.z);
        set => tr.position = new Vector3(value.x, tr.position.y, value.y);
    }
    
    public void SetDestination(Vector3 dest, float targetWidth = 0)
    {
        hasReached = false;
        isStopped = false;

        // destination = new Vector3(dest.x, transform.position.y, dest.y);
        float targetDeviation = Random.Range(-targetWidth, targetWidth);
        destination = new Vector2(dest.x + targetDeviation, dest.z);
    }

    public void Stop()
    {
        isStopped = true;
    }


    public void Tick()
    {
        if (isStopped || hasReached) return;
        Vector3 displacementVec = new Vector3(destination.x, tr.position.y, destination.y) - tr.position;
        Vector3 dir = displacementVec.normalized;
        //----------check if target is behind
        float dot = Vector3.Dot(dir, tr.forward);
        if (dot < 0f)
        {
            // if so then move to next point
            hasReached = true;
        }
        //
        // tr.forward = dir; // TODO: if we do this then the dot product check becomes meaningless. Better solution will be rotating the model towards to target instead of the root object. 
        tr.position += dir * (Time.deltaTime * speed);
        if (Vector3.SqrMagnitude(displacementVec) < stoppingDistance * stoppingDistance)
        {
            hasReached = true;
        }
        
    }
}
