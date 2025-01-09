using UnityEngine;

public class Segment : MonoBehaviour
{
    [SerializeField] float speed = 10;
    public bool hasArrived {get; private set;}

    public void Move(Vector2 centerCircle, float radius)
    {
        if(hasArrived) return;
        
        transform.Translate(Vector2.up * (speed * Time.deltaTime));

        if (Vector2.Distance(transform.position, centerCircle) <= radius)
        {
            transform.position = (Vector3)centerCircle + (-transform.up * radius);
            hasArrived = true;
        }
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
}
