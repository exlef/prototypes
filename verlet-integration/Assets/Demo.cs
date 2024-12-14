using Unity.Mathematics;
using UnityEngine;
using Ex;

public class Demo : MonoBehaviour
{
    ExGraphics gfx;

    void Start()
    {
        gfx = new();
    }

    void Update()
    {
        gfx.DrawCircle(Vector2.zero, 1);

        gfx.DrawCircle(Vector2.one, 1);

    }

    void OnDestroy()
    {
        gfx?.Dispose();
    }

    struct Point
    {
        float2 pos;
        float2 prevPos;

        public Point(float2 _pos)
        {
            pos = _pos;
            prevPos = _pos - new float2(1,1); // for now. later I probably should set this to float2.Zero
        }
    }
}
