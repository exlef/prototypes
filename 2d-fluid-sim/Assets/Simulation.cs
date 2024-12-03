using UnityEngine;
using Ex;
using Unity.Collections;
using Unity.Mathematics;

public class Simulation : MonoBehaviour
{
    [SerializeField] int count = 1;
    [SerializeField] float radius = 1;
    [SerializeField] Bounds worldBounds;

    Particle[] particles;

    Mesh mesh;
    [SerializeField] Material material;
    ComputeBuffer posBuf;
    RenderParams rp;

    void Start()
    {
        posBuf = new ComputeBuffer(count, sizeof(float) * 2);

        particles = new Particle[count];

        int xCount = Mathf.FloorToInt(worldBounds.extents.x / radius);
        int yCount = Mathf.FloorToInt(worldBounds.extents.y / radius);

        for (int x = 0, index = 0, pX = (int)radius; x < xCount; x++, pX += (int)radius * 2)
        {
            for (int y = 0, pY = (int)radius; y < yCount; y++, pY += (int)radius * 2)
            {
                var p = new Particle(new Vector2(pX, pY), radius);
                particles[index] = p;
                index++;
            }
        }
    }

    void Update()
    {
        Draw();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(worldBounds.center, worldBounds.extents);
    }

    void DrawingSetup()
    {
        mesh = Utils.CreateCircleMesh(radius);
        posBuf = new ComputeBuffer(count, sizeof(float) * 2);
        rp = new(material)
        {
            worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one), // use tighter bounds
            matProps = new MaterialPropertyBlock()
        };
    }

    void Draw()
    {
        float2[] positions = new float2[count];
        for (int i = 0; i < count; i++)
        {
            positions[i] = new float2(particles[i].pos.x, particles[i].pos.y);
        }
        posBuf.SetData(positions);

        rp.matProps.SetBuffer("_PositionBuffer", posBuf);
        Graphics.RenderMeshPrimitives(rp, mesh, 0, count);
    }

    struct Particle
    {
        public Vector2 pos;
        public Vector2 prevPos;
        public Vector2 vel;
        public float radius;

        public Particle(Vector2 _pos, float _radius)
        {
            pos = _pos;
            prevPos = _pos;
            vel = Vector2.zero;
            radius = _radius;
        }

        public void PredictPosition(float dt)
        {
            prevPos = pos;
            pos += vel * dt;
        }

        public void ComputeNextVelocity(float dtDividedByOne)
        {
            vel = (pos - prevPos) * dtDividedByOne;
        }

        public void KeepWithinBounds(Bounds bounds)
        {
            bounds.extents -= Vector3.right * radius;
            bounds.extents -= Vector3.up * radius;

            float maxX = bounds.center.x + bounds.extents.x;
            float minX = bounds.center.x - bounds.extents.x;
            float maxY = bounds.center.y + bounds.extents.y;
            float minY = bounds.center.y - bounds.extents.y;

            if (pos.x > maxX || pos.x < minX) vel.x *= -1;
            if(pos.y > maxY || pos.y < minY) vel.y *= -1;    
        }
    }
}
