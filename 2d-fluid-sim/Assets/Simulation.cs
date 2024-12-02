using UnityEngine;
using Ex;
using Unity.Mathematics;
using System.Collections.Generic;

public class Simulation : MonoBehaviour
{
    [SerializeField] int count = 1;
    [SerializeField] float radius = 1;
    [SerializeField] Bounds worldBounds;

    Particle[] particles;
    Ex.Grid grid;
    List<List<Particle>> gridData = new();

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
        
        float cellSize = 2 * radius;
        int columnCount = Mathf.CeilToInt(worldBounds.extents.x / cellSize);
        int rowCount = Mathf.CeilToInt(worldBounds.extents.y / cellSize);
        grid = new Ex.Grid(columnCount, rowCount, worldBounds.center, cellSize, cellSize);

        int initialCellListCapacity = count / grid.cellCount;
        for (int c = 0; c < grid.columnCount; c++)
        {
            for (int r = 0; r < grid.rowCount; r++)
            {
                // int index = c * grid.columnCount + r;
                gridData.Add(new List<Particle>(initialCellListCapacity));
            }
        }

        DrawingSetup();
        SpacePartition();
    }

    void Update()
    {   
        for (int i = 0; i < count; i++)
        {
            particles[i].PredictPosition(Time.deltaTime);
            particles[i].ComputeNextVelocity(Time.deltaTime);
            particles[i].KeepWithinBounds(worldBounds);
        }

        Draw();
        if(Input.anyKey) grid.Draw();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(worldBounds.center, worldBounds.extents);
    }

    void OnDestroy()
    {
        posBuf.Dispose(); 
    }

    void SpacePartition()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            var (inRange, coordinates) = grid.MapToGrid(particles[i].pos);
            if(!inRange)
            {
                Debug.Log("there is a particle which is not on grid.");
                continue;
            }
            
            List<Particle> l = gridData[coordinates.y * grid.columnCount + coordinates.x];
            l.Add(particles[i]);
        }
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
                pos.x = pos.x > maxX ? maxX : minX;
                vel.x *= -1;
            }
            if(pos.y > maxY || pos.y < minY)
            {
                pos.y = pos.y > maxY ? maxY : minY;
                vel.y *= -1;
            }  
        }
    }
}
