using System.Collections.Generic;
using UnityEngine;
using Mathf = UnityEngine.Mathf;
using Random = UnityEngine.Random;
using Ex;
using Grid = Ex.Grid;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Burst;

public class Demo : MonoBehaviour
{
    #region fields
    [SerializeField] Vector2 bounds;
    [SerializeField] int circleCount = 10;
    [SerializeField] float speed = 1;

    [SerializeField] float circleRadius = 1;
    float circle2RadiusSqr;

    [SerializeField] Material circleMaterial;
    Mesh circleMesh;

    [SerializeField] bool usePlayerPrefs;

    ComputeBuffer positionBuffer;
    RenderParams rp;

    Grid grid;
    ListPool listPool;
    Dictionary<Vector2Int, List<int>> spacePartitionDict;
    
    NativeArray<float2> velocities;
    NativeArray<float2> positions;
    JobHandle positionUpdateJobHandle;
    #endregion

    void Start()
    {
        if(usePlayerPrefs) circleCount = PlayerPrefs.GetInt("circle");
        if(usePlayerPrefs) circleRadius = PlayerPrefs.GetFloat("radius");

        circleMesh = Utils.CreateCircleMesh(circleRadius);

        circle2RadiusSqr = (circleRadius + circleRadius) * (circleRadius + circleRadius);

        velocities = new NativeArray<float2>(circleCount, Allocator.Persistent);
        positions = new NativeArray<float2>(circleCount, Allocator.Persistent);

        positionBuffer = new ComputeBuffer(circleCount, sizeof(float) * 2);
        rp = new(circleMaterial)
        {
            worldBounds = new Bounds(Vector3.zero, 10000 * Vector3.one), // use tighter bounds
            matProps = new MaterialPropertyBlock()
        };
        

        for (int i = 0; i < circleCount; i++)
        {
            velocities[i] = Utils.RndVec2(speed);
            positions[i] = new Vector2(Random.Range(-bounds.x / 2 + circleRadius, bounds.x / 2 - circleRadius), Random.Range(-bounds.y / 2 + circleRadius, bounds.y / 2 - circleRadius));
        }

        float cellSize = circleRadius * 2;
        grid = new Grid((int)(bounds.x / cellSize), (int)(bounds.y / cellSize), transform.position, cellSize, cellSize);
        
        spacePartitionDict = new(grid.cellCount);
        listPool = new(Mathf.CeilToInt(circleCount / grid.cellCount));
    }

    void Update()
    {
        grid.Draw();
        SpacePartition();

        var positionUpdateJob = new PositionUpdateJob{
            circleVelocities = velocities,
            positions = positions,
            deltaTime = Time.deltaTime,
        };
        positionUpdateJobHandle = positionUpdateJob.Schedule(circleCount, 64);
    }

    void LateUpdate()
    {
        positionUpdateJobHandle.Complete();

        for (int i = 0; i < circleCount; i++)
        {
            ResolveCollisions_LevelBoundries(i);

            var (isCollide, otherIndex) = CheckForCollisions(i);
            if (isCollide) ResolveCollisions_Circles(i, otherIndex);
        }

        positionBuffer.SetData(positions);
        rp.matProps.SetBuffer("_PositionBuffer", positionBuffer);
        Graphics.RenderMeshPrimitives(rp, circleMesh, 0, circleCount);
    }

    void SpacePartition()
    {
        foreach (var list in spacePartitionDict.Values)
        {
            listPool.Return(list);
        }

        spacePartitionDict.Clear();

        for (int i = 0; i < circleCount; i++)
        {
            var (isInRange,coordinates) = grid.MapToGrid(positions[i]);
            if(!isInRange) continue; // circles go out of bounds. due to floating point precision errors. they return eventually so we'll just not put it in any cell.
            FillSpacePartitionDict(coordinates, i);
        }
    }

    void FillSpacePartitionDict(Vector2Int key, int number)
    {
        if (!spacePartitionDict.ContainsKey(key))
        {
            spacePartitionDict[key] = listPool.Get();
        }
        spacePartitionDict[key].Add(number);
    }

    void ResolveCollisions_LevelBoundries(int currentIndex)
    {
        Vector2 halfBoundsSize = bounds / 2 - Vector2.one * circleRadius;

        if (Mathf.Abs(positions[currentIndex].x) > halfBoundsSize.x)
        {
            positions[currentIndex]= new float2(halfBoundsSize.x * Mathf.Sign(positions[currentIndex].x), positions[currentIndex].y);
            velocities[currentIndex] *= new float2(-1.0f, 1.0f); 
        }
        if (Mathf.Abs(positions[currentIndex].y) > halfBoundsSize.y)
        {
            positions[currentIndex] = new float2 (positions[currentIndex].x, halfBoundsSize.y * Mathf.Sign(positions[currentIndex].y));
            velocities[currentIndex] *= new float2(1.0f, -1.0f);
        }
    }

    void ResolveCollisions_Circles(int currentIndex, int otherIndex)
    {
        Vector2 co = positions[otherIndex] - positions[currentIndex];
        Vector2 normal = co.normalized;

        float overlap = circleRadius + circleRadius - co.magnitude;
        if (overlap > 0)
        {
            float2 correction = co.normalized * (overlap / 2);
            positions[currentIndex] -= correction;
            positions[otherIndex] += correction;
        }

        velocities[currentIndex] = Vector2.Reflect(velocities[currentIndex], normal);
        velocities[otherIndex] = Vector2.Reflect(velocities[otherIndex], normal);
    }

    List<int> BroadPhaseCollisionFilter(Vector2Int cell)
    {
        List<int> neighbors = new();

        var right = cell + Vector2Int.right;
        var left = cell + Vector2Int.left;
        var up = cell + Vector2Int.up;
        var down = cell + Vector2Int.down;

        var topRight = cell + new Vector2Int(1, 1);
        var topLeft = cell + new Vector2Int(-1, 1);
        var bottomRight = cell + new Vector2Int(1, -1);
        var bottomLeft = cell + new Vector2Int(-1, -1);

        if (spacePartitionDict.ContainsKey(cell)) neighbors.AddRange(spacePartitionDict[cell]);
        if (spacePartitionDict.ContainsKey(right)) neighbors.AddRange(spacePartitionDict[right]);
        if (spacePartitionDict.ContainsKey(left)) neighbors.AddRange(spacePartitionDict[left]);
        if (spacePartitionDict.ContainsKey(up)) neighbors.AddRange(spacePartitionDict[up]);
        if (spacePartitionDict.ContainsKey(down)) neighbors.AddRange(spacePartitionDict[down]);

        if (spacePartitionDict.ContainsKey(topRight)) neighbors.AddRange(spacePartitionDict[topRight]);
        if (spacePartitionDict.ContainsKey(topLeft)) neighbors.AddRange(spacePartitionDict[topLeft]);
        if (spacePartitionDict.ContainsKey(bottomRight)) neighbors.AddRange(spacePartitionDict[bottomRight]);
        if (spacePartitionDict.ContainsKey(bottomLeft)) neighbors.AddRange(spacePartitionDict[bottomLeft]);

        return neighbors;
    }

    (bool, int) CheckForCollisions(int cci) // cci = current circle index
    {
        var (_, coordinates) = grid.MapToGrid(positions[cci]);

        List<int> nearCircleIndexes = BroadPhaseCollisionFilter(coordinates);

        foreach (var index in nearCircleIndexes)
        {
            if (index == cci) continue;

            Vector2 distance = positions[index] - positions[cci];
            if (Vector2.SqrMagnitude(distance) > circle2RadiusSqr) continue;
            return (true, index);
        }

        return (false, -1);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, bounds);
    }

    private void OnDestroy()
    {
        velocities.Dispose();
        positions.Dispose();
        positionBuffer.Dispose();
    }
    
    [BurstCompile]
    struct PositionUpdateJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float2> circleVelocities;
        public NativeArray<float2> positions;
        [ReadOnly] public float deltaTime;

        public void Execute(int index)
        {
            var v = circleVelocities[index] * deltaTime;
            positions[index] += new float2(v.x, v.y);
        }
    }
}



























/*
using System.Collections.Generic;
using UnityEngine;
using Mathf = UnityEngine.Mathf;
using Random = UnityEngine.Random;
using Ex;
using Grid = Ex.Grid;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Burst;

public class Demo : MonoBehaviour
{
    #region fields
    [SerializeField] Vector2 bounds;
    [SerializeField] Transform circlePrefab;    
    [SerializeField] int circleCount = 10;
    [SerializeField] float circleRadius = 1;
    float circle2RadiusSqr;
    [SerializeField] float speed = 1;

    Grid grid;
    ListPool listPool;
    Dictionary<Vector2Int, List<int>> spacePartitionDict;
    
    NativeArray<float2> velocities;
    NativeArray<float2> positions;
    JobHandle positionUpdateJobHandle;
    #endregion

    void Start()
    {
        circlePrefab.localScale = circleRadius * Vector3.one;
        circleRadius = circlePrefab.GetComponent<SpriteRenderer>().bounds.extents.x;
        circle2RadiusSqr = (circleRadius + circleRadius) * (circleRadius + circleRadius);

        velocities = new NativeArray<float2>(circleCount, Allocator.Persistent);
        positions = new NativeArray<float2>(circleCount, Allocator.Persistent);

        for (int i = 0; i < circleCount; i++)
        {

            velocities[i] = Utils.RndVec2(speed);
            positions[i] = new Vector2(Random.Range(-bounds.x / 2 + circleRadius, bounds.x / 2 - circleRadius), Random.Range(-bounds.y / 2 + circleRadius, bounds.y / 2 - circleRadius));
        }

        float cellSize = circleRadius * 2;
        grid = new Grid((int)(bounds.x / cellSize), (int)(bounds.y / cellSize), transform.position, cellSize, cellSize);
        
        spacePartitionDict = new(grid.cellCount);
        listPool = new(Mathf.CeilToInt(circleCount / grid.cellCount));
    }

    void Update()
    {
        grid.Draw();
        SpacePartition();

        var positionUpdateJob = new PositionUpdateJob{
            circleVelocities = velocities,
            deltaTime = Time.deltaTime,
        };
        positionUpdateJobHandle = positionUpdateJob.Schedule(circleCount, 64);
    }

    void LateUpdate()
    {
        positionUpdateJobHandle.Complete();

        for (int i = 0; i < circleCount; i++)
        {
            ResolveCollisions_LevelBoundries(i);

            var (isCollide, otherIndex) = CheckForCollisions(i);
            if (isCollide) ResolveCollisions_Circles(i, otherIndex);
        }
    }

    void SpacePartition()
    {
        foreach (var list in spacePartitionDict.Values)
        {
            listPool.Return(list);
        }

        spacePartitionDict.Clear();

        for (int i = 0; i < circleCount; i++)
        {
            var (isInRange,coordinates) = grid.MapToGrid(positions[i]);
            if(!isInRange) continue; // circles go out of bounds. due to floating point precision errors. they return eventually so we'll just not put it in any cell.
            FillSpacePartitionDict(coordinates, i);
        }
    }

    void FillSpacePartitionDict(Vector2Int key, int number)
    {
        if (!spacePartitionDict.ContainsKey(key))
        {
            spacePartitionDict[key] = listPool.Get();
        }
        spacePartitionDict[key].Add(number);
    }

    void ResolveCollisions_LevelBoundries(int currentIndex)
    {
        Vector2 halfBoundsSize = bounds / 2 - Vector2.one * circleRadius;

        if (Mathf.Abs(positions[currentIndex].x) > halfBoundsSize.x)
        {
            positions[currentIndex]= new float2(halfBoundsSize.x * Mathf.Sign(positions[currentIndex].x), positions[currentIndex].y);
            velocities[currentIndex] *= new float2(-1.0f, 1.0f); 
        }
        if (Mathf.Abs(positions[currentIndex].y) > halfBoundsSize.y)
        {
            positions[currentIndex] = new float2 (positions[currentIndex].x, halfBoundsSize.y * Mathf.Sign(positions[currentIndex].y));
            velocities[currentIndex] *= new float2(1.0f, -1.0f);
        }
    }

    void ResolveCollisions_Circles(int currentIndex, int otherIndex)
    {
        Vector2 co = positions[otherIndex] - positions[currentIndex];
        Vector2 normal = co.normalized;

        float overlap = circleRadius + circleRadius - co.magnitude;
        if (overlap > 0)
        {
            float2 correction = co.normalized * (overlap / 2);
            positions[currentIndex] -= correction;
            positions[otherIndex] += correction;
        }

        velocities[currentIndex] = Vector2.Reflect(velocities[currentIndex], normal);
        velocities[otherIndex] = Vector2.Reflect(velocities[otherIndex], normal);
    }

    List<int> BroadPhaseCollisionFilter(Vector2Int cell)
    {
        List<int> neighbors = new();

        var right = cell + Vector2Int.right;
        var left = cell + Vector2Int.left;
        var up = cell + Vector2Int.up;
        var down = cell + Vector2Int.down;

        var topRight = cell + new Vector2Int(1, 1);
        var topLeft = cell + new Vector2Int(-1, 1);
        var bottomRight = cell + new Vector2Int(1, -1);
        var bottomLeft = cell + new Vector2Int(-1, -1);

        if (spacePartitionDict.ContainsKey(cell)) neighbors.AddRange(spacePartitionDict[cell]);
        if (spacePartitionDict.ContainsKey(right)) neighbors.AddRange(spacePartitionDict[right]);
        if (spacePartitionDict.ContainsKey(left)) neighbors.AddRange(spacePartitionDict[left]);
        if (spacePartitionDict.ContainsKey(up)) neighbors.AddRange(spacePartitionDict[up]);
        if (spacePartitionDict.ContainsKey(down)) neighbors.AddRange(spacePartitionDict[down]);

        if (spacePartitionDict.ContainsKey(topRight)) neighbors.AddRange(spacePartitionDict[topRight]);
        if (spacePartitionDict.ContainsKey(topLeft)) neighbors.AddRange(spacePartitionDict[topLeft]);
        if (spacePartitionDict.ContainsKey(bottomRight)) neighbors.AddRange(spacePartitionDict[bottomRight]);
        if (spacePartitionDict.ContainsKey(bottomLeft)) neighbors.AddRange(spacePartitionDict[bottomLeft]);

        return neighbors;
    }

    (bool, int) CheckForCollisions(int cci) // cci = current circle index
    {
        var (_, coordinates) = grid.MapToGrid(positions[cci]);

        List<int> nearCircleIndexes = BroadPhaseCollisionFilter(coordinates);

        foreach (var index in nearCircleIndexes)
        {
            if (index == cci) continue;

            Vector2 distance = positions[index] - positions[cci];
            if (Vector2.SqrMagnitude(distance) > circle2RadiusSqr) continue;
            return (true, index);
        }

        return (false, -1);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, bounds);
    }

    private void OnDestroy()
    {
        velocities.Dispose();
        positions.Dispose();
    }
    
    [BurstCompile]
    struct PositionUpdateJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float2> circleVelocities;
        public NativeArray<float2> positions;
        [ReadOnly] public float deltaTime;

        public void Execute(int index)
        {
            var v = circleVelocities[index] * deltaTime;
            positions[index] += new float2(v.x, v.y);
        }
    }
}

*/