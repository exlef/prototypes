using UnityEngine;
public class Point : MonoBehaviour
{
    public Ex.Verlet.Point point;
    [SerializeField] private bool pinned;
    public void Init()
    {
        point = new Ex.Verlet.Point(transform, pinned);
    }

    public void Tick()
    {
        point.Tick();
    }

    public void ConstrainWorldBounds(Bounds bounds)
    {
        point.ConstrainWorldBounds(bounds);
    }
}
