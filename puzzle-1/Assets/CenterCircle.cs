using UnityEngine;

public class CenterCircle : MonoBehaviour
{
    public float radius = 1;
    [SerializeField] float turningSpeed = 10;
    public Vector2 sectorDir => transform.up;

    public void Rotate()
    {
        transform.Rotate(0,0, -turningSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, sectorDir * 5);
    }

    void OnValidate()
    {
        float r = radius * 2;
        transform.localScale = new(r, r, 1);
    }
}
