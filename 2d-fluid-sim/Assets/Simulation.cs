using UnityEngine;
using Ex;
using Unity.Mathematics;
using System.Collections.Generic;

public class Simulation : MonoBehaviour
{
    [SerializeField] int count = 1;
    [SerializeField] float radius = 1;
    [SerializeField] Bounds worldBounds;
    [SerializeField]Vector2 gravity = new(0, 0);

    Particle[] particles;
    Ex.Grid grid;
    List<uint>[] gridData;

    Mesh mesh;
    [SerializeField] Material material;
    ComputeBuffer posBuf;
    ComputeBuffer colBuf;
    RenderParams rp;

     

    void Start()
    {
        posBuf = new ComputeBuffer(count, sizeof(float) * 2);
        colBuf = new ComputeBuffer(count, sizeof(float) * 1);
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
        // for debugging
        columnCount /= 10;
        rowCount /= 10;
        cellSize *= 10; 
        //
        grid = new Ex.Grid(columnCount, rowCount, worldBounds.center, cellSize, cellSize);

        gridData = new List<uint>[grid.cellCount];

        int initialCellListCapacity = count / grid.cellCount;
        for (int r = 0; r < grid.rowCount; r++) 
        {
            for (int c = 0; c < grid.columnCount; c++)
            {
                int index = r * grid.columnCount + c;
                gridData[index] = new List<uint>(initialCellListCapacity);
            }
        }

        DrawingSetup();
    }

    void Update()
    {   
        var dt = Time.deltaTime;
        for (int i = 0; i < count; i++)
        {
            particles[i].ApplyGravity(gravity, dt);
            particles[i].PredictPosition(dt);
            particles[i].ComputeNextVelocity(dt);
            particles[i].KeepWithinBounds(worldBounds);
        }

        SpacePartition();

        foreach (var cellList in gridData)
        {
            foreach (var index in cellList)
            {
                particles[index].ChangeColor(1);
            }
        }

        if (Input.GetMouseButton(0))
        {
            var (inRange, coordinates) = grid.MapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if(inRange) ColorParticlesInNeighborCell(coordinates.x, coordinates.y, 0.5f);
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
        colBuf.Dispose();
    }

    void ColorParticlesInCell(int x, int y, float color)
    {
        var l = GetCellData(x, y);
        
        foreach (var i in l)
        {
            particles[i].ChangeColor(color);
        }
    }

    void ColorParticlesInNeighborCell(int x, int y, float color)
    {
        for (int dx = -1; dx < 2; dx++)
        {
            for (int dy = -1; dy < 2; dy++)
            {
                var nX = x + dx;
                var nY = y + dy;

                if(nX < 0 || nX >= grid.columnCount) continue;
                if(nY < 0 || nY >= grid.rowCount) continue;
                
                var l = GetCellData(nX, nY);

                foreach (var i in l)
                {
                    particles[i].ChangeColor(color);
                }
            }
        }
        
    }

    void SpacePartition()
    {
        foreach (var cellList in gridData)
        {
            cellList.Clear();
        }

        for (uint i = 0; i < particles.Length; i++)
        {
            var (inRange, coordinates) = grid.MapToGrid(particles[i].pos);
            if(!inRange)
            {
                Debug.Log("there is a particle which is not on grid.");
                continue;
            }
            
            List<uint> l = gridData[coordinates.y * grid.columnCount + coordinates.x];
            l.Add(i);
        }
    }

    List<uint> GetCellData(int x, int y)
    {
        int index = y * grid.columnCount + x;
        if(index < 0 || index > gridData.Length) throw new System.NotSupportedException();
        return gridData[index];
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
        float[] colors = new float[count];
        for (int i = 0; i < count; i++)
        {
            positions[i] = new float2(particles[i].pos.x, particles[i].pos.y);
            colors[i] = particles[i].color;
        }
        posBuf.SetData(positions);
        colBuf.SetData(colors);

        rp.matProps.SetBuffer("_PositionBuffer", posBuf);
        rp.matProps.SetBuffer("_ColorBuffer", colBuf);
        Graphics.RenderMeshPrimitives(rp, mesh, 0, count);
    }

    struct Particle
    {
        public Vector2 pos;
        public Vector2 prevPos;
        public Vector2 vel;
        public float radius;
        public float color;

        public Particle(Vector2 _pos, float _radius)
        {
            pos = _pos;
            prevPos = _pos;
            vel = Utils.RndVec2(1);
            radius = _radius;
            color = 1;
        }

        public void ChangeColor(float _color)
        {
            color = _color;
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

        public void ApplyGravity(Vector2 gravity, float dt)
        {
            vel += gravity * dt;
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
