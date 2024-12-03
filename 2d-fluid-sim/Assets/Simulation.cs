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

        int xCount = (int)Mathf.Sqrt(count), yCount = (int)Mathf.Sqrt(count);

        float pX = 0;
        for (int x = 0, index = 0; x < xCount; x++, pX += radius * 2)
        {
            float pY = 0;
            for (int y = 0; y < yCount; y++, pY -= radius * 2) 
            {
                var p = new Particle(new Vector2(pX, pY), radius);
                particles[index] = p;
                index++;
            }
        }

        DrawingSetup();
    }

    void Update()
    {   
        for (int i = 0; i < count; i++)
        {
            particles[i].PredictPosition(Time.deltaTime);
        }
        
        for (int i = 0; i < count; i++)
        {
            particles[i].ComputeNextVelocity(Time.deltaTime);
        }

        for (int i = 0; i < count; i++)
        {
            particles[i].KeepWithinBounds(worldBounds);
        }

        Draw();
    }

    void OnDestroy()
    {
        posBuf.Dispose();
        posBuf = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(worldBounds.center, worldBounds.extents);

        Gizmos.color = Color.red;

        var bounds = worldBounds;

        // bounds.extents -= Vector3.right * radius;
        // bounds.extents -= Vector3.up * radius;

        float maxX = bounds.center.x + bounds.extents.x / 2 - radius;
        float minX = bounds.center.x - bounds.extents.x / 2 + radius;
        float maxY = bounds.center.y + bounds.extents.y / 2 - radius;
        float minY = bounds.center.y - bounds.extents.y / 2 + radius;

        Gizmos.DrawWireCube(bounds.center, new Vector3( maxX - minX , maxY - minY, 0));

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
            vel = Utils.RndVec2(1);
            radius = _radius;
        }

        public void PredictPosition(float dt)
        {
            prevPos = pos;
            pos += vel * dt;
        }

        public void ComputeNextVelocity(float dt)
        {
            vel = (pos - prevPos) / dt;
        }

        public void KeepWithinBounds(Bounds bounds)
        {
            float maxX = bounds.center.x + bounds.extents.x/2 - radius;
            float minX = bounds.center.x - bounds.extents.x/2 + radius;
            float maxY = bounds.center.y + bounds.extents.y/2 - radius;
            float minY = bounds.center.y - bounds.extents.y/2 + radius;

            if (pos.x > maxX || pos.x < minX)
            {
                // pos.x = pos.x > maxX ? maxX - radius : minX + radius;
                pos.x = pos.x > maxX ? maxX : minX;
                vel.x *= -1;
            }
            if(pos.y > maxY || pos.y < minY)
            {
                // pos.y = pos.y > maxY ? maxY - radius : minY + radius;
                pos.y = pos.y > maxY ? maxY : minY;
                vel.y *= -1;
            }    
        }
    }
}
