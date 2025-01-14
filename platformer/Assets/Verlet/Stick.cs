using UnityEngine;

public class Stick : MonoBehaviour
{
    [SerializeField] Point a;
    [SerializeField] Point b;
    Ex.Verlet.Stick stick;
    
    public void Init()
    {
        stick = new Ex.Verlet.Stick(a.point, b.point);
    }

    public void Tick()
    {
        stick.Tick();
    }
}
