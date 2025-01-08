using UnityEngine;

public class Segment : MonoBehaviour
{
    [SerializeField] float speed = 10;
    public bool hasArrived {get; private set;}

    public void Move(Vector2 centerCircle, float radius)
    {
        if(Vector2.Distance(transform.position, centerCircle) <= radius)
        {
            hasArrived = true;
            return;
        }
        // if (transform.position.y > 0)
        // {
        //     hasArrived = true;
        //     transform.position = new(transform.position.x, 0);
        //     return;
        // }
        transform.Translate(Vector2.up * (speed * Time.deltaTime));
    }

    public void SetParent(Transform tr)
    {
        transform.parent = tr;
    }

    public void SetSector(Vector2 sectorDir, Vector2 centerPos, Transform centerCirclTr)
    {
        var dist = Vector2.Distance(transform.position, centerPos);
        transform.position = centerPos + (sectorDir * dist);
        transform.up = -sectorDir;
        transform.parent = centerCirclTr;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.up);
    }
}
