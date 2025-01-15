using UnityEngine;

public class Stick : MonoBehaviour
{
    public Point a;
    public Point b;
    float length;    
    public void Init()
    {
        length = Vector2.Distance(a.pos, b.pos);
    }

    public void Tick()
    {
        float distance = Vector2.Distance(a.pos, b.pos);
        if (distance < Mathf.Epsilon) // Prevent division by zero
            return;
        
        float difference = length - distance;
        float percent = difference / distance;

        float dx = b.pos.x - a.pos.x;
        float dy = b.pos.y - a.pos.y;
        Vector2 offset = Vector2.zero;
        offset.x = dx * percent / 2;
        offset.y = dy * percent / 2;

        if (a.pinned && !b.pinned)
        {
            b.pos += 2 * offset;
        }
        else if (!a.pinned && b.pinned)
        {
            a.pos -= 2 * offset;
        }
        else if (!a.pinned && !b.pinned)
        {
            a.pos -= offset;
            b.pos += offset;
        }
    }
}
