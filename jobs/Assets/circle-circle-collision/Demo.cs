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
using System.Linq;

public class Demo : MonoBehaviour
{
    #region fields
    [SerializeField] Vector2 bounds;
    [SerializeField] Transform circlePrefab;    
    [SerializeField] int circleCount = 10;
    [SerializeField] float speed = 1;

    Circle[] circles;
    
    Grid grid;
    ListPool listPool;
    Dictionary<Vector2Int, List<int>> spacePartitionDict;
    
    private NativeArray<float2> velocities;
    private TransformAccessArray transformAccessArray;
    JobHandle positionUpdateJobHandle;
    #endregion

    void Start()
    {
        velocities = new NativeArray<float2>(circleCount, Allocator.Persistent);
        transformAccessArray = new TransformAccessArray(circleCount);

        circles = new Circle[circleCount];
        for (int i = 0; i < circleCount; i++)
        {
            Transform go = Instantiate(circlePrefab, transform);
            go.gameObject.name = $"Circle {i + 1}";
            circles[i] = new Circle(go, speed);
            go.position = new Vector3(Random.Range(-bounds.x/2 + circles[i].radius, bounds.x/2 - circles[i].radius), Random.Range(-bounds.y/2 + circles[i].radius, bounds.y/2 - circles[i].radius));

            velocities[i] = Utils.RndVec2(speed);
            transformAccessArray.Add(go);
        }

        float cellSize = circles[0].radius * 2;
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

        for (int i = 0; i < circles.Length; i++)
        {
            // Circle circle = circles[i];
            // circle.tr.position += (Vector3)circle.velocity * Time.deltaTime;
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
            circles[index].tr.GetComponent<SpriteRenderer>().color = Color.red;
        }
        // for (int i = 0; i < circles.Length; i++)
        // {
        //     circles[i].velocity = Vector2.zero;
        // }
    }

    void SpacePartition()
    {
        foreach (var list in spacePartitionDict.Values)
        {
            listPool.Return(list);
        }

        spacePartitionDict.Clear();

        for (int i = 0; i < circles.Length; i++)
        {
            var (isInRange,coordinates) = grid.MapToGrid(circles[i].tr.position);
            // if(!isInRange) throw new System.NotSupportedException(); // circles go out of bounds. due to floating point precision errors. they return eventually so we'll just ignore it.
            if(!isInRange) continue; // so we'll just not put it in any cell.
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
        var c = circles[currentIndex];

        Vector2 halfBoundsSize = bounds / 2 - Vector2.one * c.radius;

        if (Mathf.Abs(c.tr.position.x) > halfBoundsSize.x)
        {
            c.tr.SetPosX(halfBoundsSize.x * Mathf.Sign(c.tr.position.x));
            // c.velocity.x *= -1;
            velocities[currentIndex] *= new float2(-1.0f, 1.0f); 
        }
        if (Mathf.Abs(c.tr.position.y) > halfBoundsSize.y)
        {
            c.tr.SetPosY(halfBoundsSize.y * Mathf.Sign(c.tr.position.y));
            // c.velocity.y *= -1;
            velocities[currentIndex] *= new float2(1.0f, -1.0f);
        }

        circles[currentIndex] = c;
    }

    void ResolveCollisions_Circles(int currentIndex, int otherIndex)
    {
        var current = circles[currentIndex];
        var other = circles[otherIndex];

        Vector2 co = other.tr.position - current.tr.position;
        Vector2 normal = co.normalized;

        float overlap = current.radius + other.radius - co.magnitude;
        if (overlap > 0)
        {
            Vector2 correction = co.normalized * (overlap / 2);
            current.tr.position -= (Vector3)correction;
            other.tr.position += (Vector3)correction;
        }

        // current.velocity = Vector2.Reflect(current.velocity, normal);
        // other.velocity = Vector2.Reflect(other.velocity, normal);

        velocities[currentIndex] = Vector2.Reflect(velocities[currentIndex], normal);
        velocities[otherIndex] = Vector2.Reflect(velocities[otherIndex], normal);

        circles[currentIndex] = current;
        circles[otherIndex] = other;
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
        var (_, coordinates) = grid.MapToGrid(circles[cci].tr.position);

        List<int> nearCircleIndexes = BroadPhaseCollisionFilter(coordinates);

        foreach (var index in nearCircleIndexes)
        {
            if (index == cci) continue;

            var other = circles[index];
            var current = circles[cci];

            Vector2 distance = other.tr.position - current.tr.position;
            if (Vector2.SqrMagnitude(distance) > (other.radius + current.radius) * (other.radius + current.radius)) continue;
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
    
    struct Circle
    {
        public Transform tr;
        public float radius;
        // public Vector2 velocity;

        public Circle(Transform i_tr, float speed)
        {
            tr = i_tr;
            radius = tr.GetComponent<Renderer>().bounds.extents.x;
            // velocity = Utils.RndVec2(speed);
        }

        public void Color(Color color)
        {
            
        }
    }

    struct PositionUpdateJob : IJobParallelForTransform
    {
        public NativeArray<float2> circleVelocities;
        public float deltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            var v = circleVelocities[index] * deltaTime;
            transform.position += new Vector3(v.x, v.y, 0);
        }
    }
}
