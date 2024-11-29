using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

        public static void SetPosY(this Transform transform, float y)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RndVec2(float magnitude = 1) => new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * magnitude;

        public static float MapNum(float x, float originalMin, float originalMax, float newMin, float newMax)
        {
            // Apply the linear mapping formula
            return newMin + (x - originalMin) * (newMax - newMin) / (originalMax - originalMin);
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