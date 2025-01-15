using UnityEngine;
public class Point : MonoBehaviour
{
    public Vector2 pos
    {
        get => transform.position;
        set => transform.position = value;
    }
    [HideInInspector] public Vector2 oldPos;
    public bool pinned;
    public float radius { get; private set; }
    
    readonly float maxSpeed = 0.2f;
    readonly float friction = 0.7f;
    readonly Vector2 gravity = new(0, -0.1f);
    public void Init()
    {
        oldPos = pos;
        radius = transform.localScale.x / 2;
    }

    public void Tick()
    {
        if (pinned)
        {
            oldPos = pos; // otherwise the oldPos will be the its value when we pinned the point and when it got unpinned this will cause to point move unexpectedly since its oldPos is not updated correctly.
            return;
        }
        var v = pos - oldPos;
        v += gravity;
        oldPos = pos;
        v = Vector2.ClampMagnitude(v, maxSpeed);
        v *= friction;
        pos += v;
        // pos += gravity;
    }

    public void ConstrainWorldBounds(Bounds bounds)
    {
        if (pinned) return;

        float maxX = bounds.center.x + bounds.extents.x / 2 - radius;
        float minX = bounds.center.x - bounds.extents.x / 2 + radius;
        float maxY = bounds.center.y + bounds.extents.y / 2 - radius;
        float minY = bounds.center.y - bounds.extents.y / 2 + radius;

        var x = Mathf.Clamp(pos.x, minX, maxX);
        var y = Mathf.Clamp(pos.y, minY, maxY);

        pos = new Vector2(x,y);
    }
}
