using UnityEngine;
using Ex;

public class Point : MonoBehaviour
{
    Ex.Verlet.Point p;
    void Start()
    {
        p = new(transform);
    }

    void Update()
    {
        p.Tick();
        p.ConstrainWorldBounds(Demo.bounds);
    }
}
