using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Jobs;

namespace Ex
{
    public static class Utils
    {
        public static void SetPosX(this Transform transform, float x)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = x;
            transform.position = newPosition;
        }

        public static void SetPosX(this TransformAccess transform, float x)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = x;
            transform.position = newPosition;
        }

        public static void SetPosY(this Transform transform, float y)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = y;
            transform.position = newPosition;
        }

        public static void SetPosY(this TransformAccess transform, float y)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = y;
            transform.position = newPosition;
        }

        public static void SetPosZ(this Transform transform, float z)
        {
            Vector3 newPosition = transform.position;
            newPosition.z = z;
            transform.position = newPosition;
        }

        public static void SetPosZ(this TransformAccess transform, float z)
        {
            Vector3 newPosition = transform.position;
            newPosition.z = z;
            transform.position = newPosition;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RndVec2(float magnitude = 1) => new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * magnitude;

        public static float MapNum(float x, float originalMin, float originalMax, float newMin, float newMax)
        {
            // Apply the linear mapping formula
            return newMin + (x - originalMin) * (newMax - newMin) / (originalMax - originalMin);
        }

        public static Mesh CreateCircleMesh(float radius, int segmentCount = 32)
        {
            Mesh mesh = new();
            // Create vertices*
            int segments = segmentCount;
            Vector3[] vertices = new Vector3[segments + 1];
            vertices[0] = Vector3.zero; // Center vertex*
            for (int i = 0; i < segments; i++)
            {
                float angle = i * (2f * Mathf.PI / segments);
                vertices[i + 1] = new Vector3(

                     Mathf.Cos(angle) * radius,
                        Mathf.Sin(angle) * radius,
                        0
                    );
            }
            // Create triangles - reverse order to face forward*
            int[] triangles = new int[segments * 3];
            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3] = 0; // Center vertex*
                triangles[i * 3 + 1] = (i + 2) % (segments + 1); // Next vertex, wrapping around*
                triangles[i * 3 + 2] = i + 1; // Current vertex*
            }
            // Fix the last triangle*
            triangles[^2] = 1; // Last triangle should connect to the first vertex*

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public static (Vector2, Vector2) KeepWithinBounds2D(Bounds bounds, float radius, Vector2 pos, Vector2 vel)
        {
            float maxX = bounds.center.x + bounds.extents.x / 2 - radius;
            float minX = bounds.center.x - bounds.extents.x / 2 + radius;
            float maxY = bounds.center.y + bounds.extents.y / 2 - radius;
            float minY = bounds.center.y - bounds.extents.y / 2 + radius;

            if (pos.x > maxX || pos.x < minX)
            {
                pos.x = pos.x > maxX ? maxX : minX;
                vel.x *= -1;
            }
            if (pos.y > maxY || pos.y < minY)
            {
                pos.y = pos.y > maxY ? maxY : minY;
                vel.y *= -1;
            }

            return (pos, vel);
        }
    }

    class ListPool
    {
        private readonly Stack<List<int>> pool = new();
        readonly int listInitialCapacity;

        public ListPool(int _listInitialCapacity = 0)
        {
            listInitialCapacity = _listInitialCapacity;
        }

        public List<int> Get()
        {
            return pool.Count > 0 ? pool.Pop() : new List<int>(listInitialCapacity);
        }

        public void Return(List<int> list)
        {
            list.Clear();
            pool.Push(list);
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
        public readonly Vector2 bottomLeftPos => new(centerPos.x - width / 2, centerPos.y - height / 2);
        public readonly Vector2 topLeftPos => new(centerPos.x - width / 2, centerPos.y + height / 2);
        public readonly Vector2 bottomRightPos => new(centerPos.x + width / 2, centerPos.y - height / 2);
        public readonly Vector2 topRightPos => new(centerPos.x + width / 2, centerPos.y + height / 2);


        public Grid(int _columnCount, int _rowCount, Vector2 _centerPos, float _cellWidth, float _cellHeight)
        {
            columnCount = _columnCount;
            rowCount = _rowCount;
            cellWidth = _cellWidth;
            cellHeight = _cellHeight;
            centerPos = _centerPos;
        }

        public readonly (bool, Vector2Int) MapToGrid(Vector2 pos)
        {
            if (pos.x < bottomLeftPos.x || pos.x > bottomRightPos.x || pos.y < bottomLeftPos.y || pos.y > topLeftPos.y) return (false, new Vector2Int(-1, -1)); // out of bounds.

            int gridX = Mathf.FloorToInt((pos.x - bottomLeftPos.x) / cellWidth);
            int gridY = Mathf.FloorToInt((pos.y - bottomLeftPos.y) / cellHeight);

            // Clamp values to avoid off-by-one errors. (The calculation of gridX and gridY in MapToGrid uses Mathf.FloorToInt, which could cause an off-by-one error for edge cases near the grid’s boundaries. If pos.x is exactly on the grid’s right edge (bottomRightPos.x), the calculated gridX might exceed columnCount - 1. Similarly, if pos.y is exactly on the top edge, gridY might exceed rowCount -1.
            gridX = Mathf.Clamp(gridX, 0, columnCount - 1);
            gridY = Mathf.Clamp(gridY, 0, rowCount - 1);

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