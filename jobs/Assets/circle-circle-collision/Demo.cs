using System.Collections.Generic;
using UnityEngine;
using Mathf = UnityEngine.Mathf;

public class Demo : MonoBehaviour
{
    #region fields
    [SerializeField] Vector2 bounds;
    [SerializeField] Transform circlePrefab;    
    [SerializeField] int circleCount = 10;
    Circle[] circles;
    Grid grid;
    Dictionary<Vector2Int, List<int>> spacePartitionDict = new Dictionary<Vector2Int, List<int>>();
    #endregion

    void Start()
    {
        circles = new Circle[circleCount];
        for (int i = 0; i < circleCount; i++)
        {
            Transform go = Instantiate(circlePrefab, transform);
            circles[i] = new Circle(go);
            go.position = new Vector3(Random.Range(-bounds.x/2 + circles[i].radius, bounds.x/2 - circles[i].radius), Random.Range(-bounds.y/2 + circles[i].radius, bounds.y/2 - circles[i].radius));
        }

        grid = new Grid((int)bounds.x, (int)bounds.y, transform.position);
    }

    void Update()
    {
        grid.Draw();
        SpacePartition();
        for (int i = 0; i < circles.Length; i++)
        {
            Circle circle = circles[i];
            circle.tr.position += (Vector3)circle.velocity * Time.deltaTime;
            ResolveCollisions_LevelBoundries(i);

            var (isCollide, otherIndex) = CheckForCollisions(i);
            if(isCollide) ResolveCollisions_Circles(i, otherIndex);
        }
        if(Input.GetMouseButtonDown(0))
        {
            DebugMode();
        }
    }

    void DebugMode()
    {
        var (isInRange, coordinates) = grid.MapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (!isInRange) throw new System.NotSupportedException();

        Debug.Log(coordinates);
        List<int> nearCircleIndexes = BroadPhaseCollisionFilter(coordinates);
        for (int i = 0; i < nearCircleIndexes.Count; i++)
        {
            // circles[i].Color(Color.red);
            // circles[i].tr.GetComponent<SpriteRenderer>().color = Color.red;
            // Debug.Log(i, circles[i].tr.gameObject);
        }
        foreach (var index in nearCircleIndexes)
        {
            circles[index].tr.GetComponent<SpriteRenderer>().color = Color.red;
        }
        for (int i = 0; i < circles.Length; i++)
        {
            circles[i].velocity = Vector2.zero;
        }
    }

    void SpacePartition()
    {
        spacePartitionDict.Clear();

        for (int i = 0; i < circles.Length; i++)
        {
            var (isInRange,coordinates) = grid.MapToGrid(circles[i].tr.position);
            if(!isInRange) throw new System.NotSupportedException();
            FillSpacePartitionDict(coordinates, i);
        }
    }

    void FillSpacePartitionDict(Vector2Int key, int number)
    {
        if (!spacePartitionDict.ContainsKey(key))
        {
            spacePartitionDict[key] = new List<int>();
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
            c.velocity.x *= -1;
        }
        if (Mathf.Abs(c.tr.position.y) > halfBoundsSize.y)
        {
            c.tr.SetPosY(halfBoundsSize.y * Mathf.Sign(c.tr.position.y));
            c.velocity.y *= -1;
        }

        // Clamp the circle's position to ensure it's within bounds. add some error margin to overcome floating point precision Issues
        Vector2 clampedPosition = new Vector2(
            Mathf.Clamp(c.tr.position.x, -halfBoundsSize.x + .1f, halfBoundsSize.x - .1f),
            Mathf.Clamp(c.tr.position.y, -halfBoundsSize.y + .1f, halfBoundsSize.y - .1f)
        );

        c.tr.position = clampedPosition;

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

        current.velocity = Vector2.Reflect(current.velocity, normal);
        other.velocity = Vector2.Reflect(other.velocity, normal);

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
        var (isInRange, coordinates) = grid.MapToGrid(circles[cci].tr.position);
        if (!isInRange) throw new System.NotSupportedException();


        List<int> nearCircleIndexes = BroadPhaseCollisionFilter(coordinates);
        // for (int i = 0; i < nearCircles.Count; i++)
        // {
        //     if (i == cci) continue;

        //     var other = circles[i];
        //     var current = circles[cci];

        //     Vector2 distance = other.tr.position - current.tr.position;
        //     if (Vector2.SqrMagnitude(distance) > (other.radius + current.radius) * (other.radius + current.radius)) continue;
        //     return (true, i);
        // }

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

    struct Circle
    {
        public Transform tr;
        public float radius;
        public Vector2 velocity;

        public Circle(Transform i_tr)
        {
            tr = i_tr;
            radius = tr.GetComponent<Renderer>().bounds.extents.x;
            velocity = ExUtils.RndVec2(3);
        }

        public void Color(Color color)
        {
            
        }
    }

    struct Grid
    {
        public int columnCount;
        public int rowCount;
        public float cellWidth;
        public float cellHeight;
        public Vector2 centerPos;
        public readonly int cellCount => rowCount * columnCount;
        public readonly float width => columnCount * cellWidth;
        public readonly float height => rowCount * cellHeight;
        public readonly Vector2 bottomLeftPos  => new(centerPos.x - width / 2, centerPos.y - height / 2);
        public readonly Vector2 topLeftPos     => new(centerPos.x - width / 2, centerPos.y + height / 2);
        public readonly Vector2 bottomRightPos => new(centerPos.x + width / 2, centerPos.y - height / 2);
        public readonly Vector2 topRightPos    => new(centerPos.x + width / 2, centerPos.y + height / 2);


        public Grid(int _columnCount, int _rowCount, Vector2 _centerPos)
        {
            columnCount = _columnCount;
            rowCount = _rowCount;
            cellWidth = 1;
            cellHeight = 1;
            centerPos = _centerPos;
        }

        public readonly (bool, Vector2Int) MapToGrid(Vector2 pos)
        {
            if(pos.x < bottomLeftPos.x || pos.x > bottomRightPos.x || pos.y < bottomLeftPos.y || pos.y > topLeftPos.y) return (false, new Vector2Int(-1, -1)); // out of bounds.
            
            int gridX = Mathf.FloorToInt((pos.x - bottomLeftPos.x) / cellWidth);
            int gridY = Mathf.FloorToInt((pos.y - bottomLeftPos.y) / cellHeight);

            return (true, new Vector2Int(gridX, gridY));
        }

        public readonly void Draw()
        {
            Debug.DrawRay(bottomLeftPos, Vector2.left, Color.magenta);
            Debug.DrawRay(topLeftPos, Vector2.left, Color.magenta);
            Debug.DrawRay(bottomRightPos, Vector2.right, Color.magenta);
            Debug.DrawRay(topRightPos, Vector2.right, Color.magenta);

            for (int y = 0; y <= rowCount; y++)
            {
                Debug.DrawLine(bottomLeftPos + new Vector2(0, cellHeight) * y, bottomRightPos + new Vector2(0, cellHeight) * y, Color.red);
            }

            for (int x = 0; x <= columnCount; x++)
            {
                Debug.DrawLine(bottomLeftPos + new Vector2(cellWidth, 0) * x, topLeftPos + new Vector2(cellWidth, 0) * x, Color.green);
            }
        }
    }
}
