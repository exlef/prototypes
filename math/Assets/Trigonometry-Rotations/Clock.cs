using System.Collections.Generic;
using UnityEngine;

namespace TrigonometryRotations
{
    public class Clock : MonoBehaviour
    {
        void OnDrawGizmos()
        {
            DrawCircle(default, 1, 32);
        }

        void DrawCircle(Vector3 center, float radius, int segments)
        {
            Vector3 dir = Vector3.up;
            List<Vector3> positions = new();

            float angleBetween = 360.0f / segments;
            for (int i = 0; i < segments; i++)
            {
                var currentDir = Quaternion.Euler(0, 0, angleBetween * i) * dir;
                var pos = center + currentDir * radius;
                positions.Add(pos);
            }

            for (int i = 0; i < positions.Count - 1; i++)
            {
                Color c = Color.white;
                if(i == 0)
                {
                    c = Color.red;
                }
                Debug.DrawLine(positions[i], positions[i + 1], c);
            }
            Debug.DrawLine(positions[^1], positions[0]);


        }
    }
}
