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
    TransformAccessArray transformAccessArray;
    JobHandle positionUpdateJobHandle;
    #endregion

    void Start()
    {
        circlePrefab.localScale = circleRadius * Vector3.one;
        circleRadius = circlePrefab.GetComponent<SpriteRenderer>().bounds.extents.x;
        circle2RadiusSqr = (circleRadius + circleRadius) * (circleRadius + circleRadius);

        velocities = new NativeArray<float2>(circleCount, Allocator.Persistent);
        transformAccessArray = new TransformAccessArray(circleCount);

        for (int i = 0; i < circleCount; i++)
        {
            Transform go = Instantiate(circlePrefab, transform);
            go.gameObject.name = $"Circle {i + 1}";
            go.position = new Vector3(Random.Range(-bounds.x/2 + circleRadius, bounds.x/2 - circleRadius), Random.Range(-bounds.y/2 + circleRadius, bounds.y/2 - circleRadius));

            velocities[i] = Utils.RndVec2(speed);
            transformAccessArray.Add(go);
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
        positionUpdateJobHandle = positionUpdateJob.Schedule(transformAccessArray);
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

    void DebugMode()
    {
        var (isInRange, coordinates) = grid.MapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (!isInRange) throw new System.NotSupportedException();

        Debug.Log(coordinates);
        List<int> nearCircleIndexes = BroadPhaseCollisionFilter(coordinates);
        
        foreach (var index in nearCircleIndexes)
        {
            transformAccessArray[index].GetComponent<SpriteRenderer>().color = Color.red;
        }
        for (int i = 0; i < circleCount; i++)
        {
            velocities[i] = Vector2.zero;
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
            var (isInRange,coordinates) = grid.MapToGrid(transformAccessArray[i].position);
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

        if (Mathf.Abs(transformAccessArray[currentIndex].position.x) > halfBoundsSize.x)
        {
            transformAccessArray[currentIndex].SetPosX(halfBoundsSize.x * Mathf.Sign(transformAccessArray[currentIndex].position.x));
            velocities[currentIndex] *= new float2(-1.0f, 1.0f); 
        }
        if (Mathf.Abs(transformAccessArray[currentIndex].position.y) > halfBoundsSize.y)
        {
            transformAccessArray[currentIndex].SetPosY(halfBoundsSize.y * Mathf.Sign(transformAccessArray[currentIndex].position.y));
            velocities[currentIndex] *= new float2(1.0f, -1.0f);
        }
    }

    void ResolveCollisions_Circles(int currentIndex, int otherIndex)
    {
        Vector2 co = transformAccessArray[otherIndex].position - transformAccessArray[currentIndex].position;
        Vector2 normal = co.normalized;

        float overlap = circleRadius + circleRadius - co.magnitude;
        if (overlap > 0)
        {
            Vector2 correction = co.normalized * (overlap / 2);
            transformAccessArray[currentIndex].position -= (Vector3)correction;
            transformAccessArray[otherIndex].position += (Vector3)correction;
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

        // Diagonal neighbors
        var topRight = cell + new Vector2Int(1, 1);
        var topLeft = cell + new Vector2Int(-1, 1);
        var bottomRight = cell + new Vector2Int(1, -1);
        var bottomLeft = cell + new Vector2Int(-1, -1);

        if (spacePartitionDict.ContainsKey(cell)) neighbors.AddRange(spacePartitionDict[cell]);
        if (spacePartitionDict.ContainsKey(right)) neighbors.AddRange(spacePartitionDict[right]);
        if (spacePartitionDict.ContainsKey(left)) neighbors.AddRange(spacePartitionDict[left]);
        if (spacePartitionDict.ContainsKey(up)) neighbors.AddRange(spacePartitionDict[up]);
        if (spacePartitionDict.ContainsKey(down)) neighbors.AddRange(spacePartitionDict[down]);

        // Check for diagonal neighbors
        if (spacePartitionDict.ContainsKey(topRight)) neighbors.AddRange(spacePartitionDict[topRight]);
        if (spacePartitionDict.ContainsKey(topLeft)) neighbors.AddRange(spacePartitionDict[topLeft]);
        if (spacePartitionDict.ContainsKey(bottomRight)) neighbors.AddRange(spacePartitionDict[bottomRight]);
        if (spacePartitionDict.ContainsKey(bottomLeft)) neighbors.AddRange(spacePartitionDict[bottomLeft]);

        return neighbors;
    }

    (bool, int) CheckForCollisions(int cci) // cci = current circle index
    {
        var (_, coordinates) = grid.MapToGrid(transformAccessArray[cci].position);

        List<int> nearCircleIndexes = BroadPhaseCollisionFilter(coordinates);

        foreach (var index in nearCircleIndexes)
        {
            if (index == cci) continue;

            Vector2 distance = transformAccessArray[index].position - transformAccessArray[cci].position;
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
        transformAccessArray.Dispose();
        velocities.Dispose();
    }
    
    [BurstCompile]
    struct PositionUpdateJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<float2> circleVelocities;
        [ReadOnly] public float deltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            var v = circleVelocities[index] * deltaTime;
            transform.position += new Vector3(v.x, v.y, 0);
        }
    }
}
