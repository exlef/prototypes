using Unity.Mathematics;
using UnityEngine;
using Ex;
using System.Collections.Generic;

public class Demo : MonoBehaviour
{
    [SerializeField] Material mat;
    ExGraphics gfx;
    List<Vector2> positions = new();

    void Start()
    {
        gfx = new(mat);

        positions.Add(Vector2.zero);
        positions.Add(Vector2.one *  2 );
        positions.Add(Vector2.right * 2);
        positions.Add(Vector2.left * 2);
        positions.Add(Vector2.up * 2);
        positions.Add(Vector2.down * 2);
    }

    void Update()
    {
        gfx.DrawCircles(positions);
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
