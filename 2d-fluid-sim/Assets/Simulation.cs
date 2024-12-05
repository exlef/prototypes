using UnityEngine;
using Ex;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Simulation : MonoBehaviour
{
    [SerializeField] int count = 1;
    [SerializeField] float radius = 1;
    [SerializeField] Bounds worldBounds;
    [SerializeField]Vector2 gravity = new(0, 0);
    [SerializeField] float restDensity = 10;
    [SerializeField] float kNear = 3;
    [SerializeField] float k = 0.5f;
    [SerializeField] float interactionRadius = 25;


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

        float pX = -10;
        for (int x = 0, index = 0; x < xCount; x++, pX += radius * 2)
        {
            float pY = 0;
            float xoff = 1;
            for (int y = 0; y < yCount; y++, pY -= radius * 2) 
            {
                xoff += 0.2f;
                var p = new Particle(new Vector2(pX + 0, pY + xoff), radius);
                particles[index] = p;
                index++;
            }
        }
        
        float cellSize = 2 * radius;
        int columnCount = Mathf.CeilToInt(worldBounds.extents.x / cellSize);
        int rowCount = Mathf.CeilToInt(worldBounds.extents.y / cellSize);
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

    // void Update()
    // {   
    //     var dt = Time.deltaTime;
    //     SpacePartition();
    //     for (int i = 0; i < count; i++)
    //     {
    //         particles[i].ApplyGravity(gravity, dt);
    //         particles[i].PredictPosition(dt);
    //         DoubleDensityRelaxation(i);
    //         particles[i].KeepWithinBounds(worldBounds);
    //         particles[i].ComputeNextVelocity(dt);
    //     }

    //     DebugMode();
    // }

    void Update()
    {
        // Phase 1: Predict Positions
        for (int i = 0; i < count; i++)
        {
            particles[i].ApplyGravity(gravity, Time.deltaTime);
            particles[i].PredictPosition(Time.deltaTime);
        }

        for (int i = 0; i < count; i++)
        {
            particles[i].KeepWithinBounds(worldBounds);
        }

        // Phase 2: Spatial Partitioning
        SpacePartition();

        // Phase 3: Compute Density
        float[] densities = new float[count];
        float[] nearDensities = new float[count];
        ComputeDensities(ref densities, ref nearDensities);

        // Phase 4: Pressure Solve (Multiple Iterations)
        for (int iteration = 0; iteration < 2; iteration++)  // Multiple relaxation passes
        {
            ApplyPressure(densities, nearDensities, Time.deltaTime);
        }

        // Phase 5: Update Velocities and Bounds
        for (int i = 0; i < count; i++)
        {
            particles[i].ComputeNextVelocity(Time.deltaTime);
            particles[i].KeepWithinBounds(worldBounds);
        }

        Draw();
    }

    void DebugMode()
    {
        if (Input.anyKey) grid.Draw();
    }

    void ComputeDensities(ref float[] densities, ref float[] nearDensities)
    {
        for (int i = 0; i < count; i++)
        {
            float density = 0;
            float nearDensity = 0;
            var neighborIndexes = GetNeighbourParticleIndexes(i);

            foreach (var neighborIndex in neighborIndexes)
            {
                if (i == neighborIndex) continue;

                Vector2 rij = particles[neighborIndex].pos - particles[i].pos;
                float q = Mathf.Clamp(rij.magnitude / interactionRadius, 0, 1);

                if (q < 1)
                {
                    density += (1 - q) * (1 - q);
                    nearDensity += (1 - q) * (1 - q) * (1 - q);
                }
            }

            densities[i] = density;
            nearDensities[i] = nearDensity;
        }
    }

    void ApplyPressure(float[] densities, float[] nearDensities, float dt)
    {
        for (int i = 0; i < count; i++)
        {
            float pressure = k * (densities[i] - restDensity);
            float pressureNear = kNear * nearDensities[i];
            Vector2 displacement = Vector2.zero;

            var neighborIndexes = GetNeighbourParticleIndexes(i);
            foreach (var neighborIndex in neighborIndexes)
            {
                if (i == neighborIndex) continue;

                Vector2 rij = particles[neighborIndex].pos - particles[i].pos;
                float q = Mathf.Clamp(rij.magnitude / interactionRadius, 0, 1);

                if (q < 1)
                {
                    rij.Normalize();
                    float displacementMagnitude = Mathf.Pow(dt, 2) *
                        (pressure * (1 - q) + pressureNear * Mathf.Pow(1 - q, 2));

                    Vector2 neighborDisplacement = rij * displacementMagnitude * 0.5f;

                    particles[neighborIndex].AddPos(neighborDisplacement);
                    displacement -= neighborDisplacement;
                }
            }

            particles[i].AddPos(displacement);
        }
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

    List<uint> GetNeighbourParticleIndexes(int index)
    {
        List<uint> neighbourParticleIndexes = new();
        var (inRange, coordinates) = grid.MapToGrid(particles[index].pos);
        
        if (!inRange) 
        {
            Debug.Log("there is a particle outside of the grid. the function that this error is occuring is " + nameof(GetNeighbourParticleIndexes));
            return neighbourParticleIndexes;
        }

        int x = coordinates.x, y = coordinates.y;

        for (int dx = -1; dx < 2; dx++)
        {
            for (int dy = -1; dy < 2; dy++)
            {
                var nX = x + dx;
                var nY = y + dy;

                if (nX < 0 || nX >= grid.columnCount) continue;
                if (nY < 0 || nY >= grid.rowCount) continue;

                var cellParticleIndexs = GetCellData(nX, nY);

                neighbourParticleIndexes.AddRange(cellParticleIndexs);
            }
        }

        return neighbourParticleIndexes;
    }

    // void DoubleDensityRelaxation(int cci) // cci = current particle index || opi = other particle index
    // {
    //     float density = 0;
    //     float densityNear = 0;
    //     var (inRange, coordinates) = grid.MapToGrid(particles[cci].pos);
    //     if(!inRange) return;

    //     var neighborIndexes = GetNeighbourParticleIndexes(cci);

    //     for (int i = 0; i < neighborIndexes.Count; i++)
    //     {
    //         uint opi = neighborIndexes[i];
    //         if (cci == opi) continue;

    //         Vector2 rij = particles[opi].pos - particles[cci].pos;
    //         // float q = rij.magnitude / interactionRadius;
    //         float q = Mathf.Clamp(rij.magnitude / interactionRadius, 0, 1);

    //         if (q < 1)
    //         {
    //             density += (1 - q) * (1 - q);
    //             densityNear += (1 - q) * (1 - q) * (1 - q);
    //         }
    //     }

    //     float pressure = k * (density - restDensity);
    //     float pressureNear = kNear * densityNear;
    //     Vector2 crntPrtDisplacement = Vector2.zero;

    //     for (int i = 0; i < neighborIndexes.Count; i++)
    //     {
    //         uint opi = neighborIndexes[i];
    //         if (cci == opi) continue;

    //         Vector2 rij = particles[opi].pos - particles[cci].pos;
    //         // float q = rij.magnitude / interactionRadius;
    //         float q = Mathf.Clamp(rij.magnitude / interactionRadius, 0, 1);

    //         if (q < 1)
    //         {
    //             rij.Normalize();
    //             float displacementTerm = Mathf.Pow(Time.deltaTime, 2) * (pressure * (1 - q) + pressureNear * Mathf.Pow(1 - q, 2));
    //             Vector2 D = rij * displacementTerm;
                
    //             particles[opi].AddPos(D * 0.5f);

    //             crntPrtDisplacement -= D * 0.5f;


    //         }
    //     }
    //     particles[cci].AddPos(crntPrtDisplacement);
    // }

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
            if(!inRange) continue;

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
            vel = Vector2.zero;
            radius = _radius;
            color = 1;
        }

        public void ChangeColor(float _color)
        {
            color = _color;
        }

        public void SetPos(Vector2 _pos)
        {
            pos = _pos;
        }

        public void AddPos(Vector2 _pos)
        {
            pos += _pos;
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
                prevPos.x = pos.x = pos.x > maxX ? maxX : minX;
            }
            if(pos.y > maxY || pos.y < minY)
            {
                prevPos.y = pos.y = pos.y > maxY ? maxY : minY;
            } 
        }
    }
}
