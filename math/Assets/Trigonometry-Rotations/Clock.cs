using System.Collections.Generic;
using UnityEngine;
using System;

namespace TrigonometryRotations
{
    public class Clock : MonoBehaviour
    {
        [SerializeField] float size = 1;

        void Update()
        {
            DateTime currentTime = DateTime.Now;
            Vector3 center = default;

            { // hour
                int hour = currentTime.Hour;
                hour = hour % 12;
                float angleBetweenDeg = 360 / 12;
                float angleDeg = hour * angleBetweenDeg;
                Vector3 dir = Quaternion.Euler(0, 0, angleDeg) * Vector3.up;
                Debug.DrawRay(center, dir * 0.4f * size);
            }

            { // minutes
                int minutes = currentTime.Minute;
                float angleBetweenDeg = 360 / 60;
                float angleDeg = minutes * angleBetweenDeg;
                Vector3 dir = Quaternion.Euler(0, 0, angleDeg) * Vector3.up;
                Debug.DrawRay(center, dir * 0.8f * size);
            }

            { // seconds
                int seconds = currentTime.Second;
                float angleBetweenDeg = 360 / 60;
                float angleDeg = seconds * angleBetweenDeg;
                Vector3 dir = Quaternion.Euler(0, 0, angleDeg) * Vector3.up;
                Debug.DrawRay(center, dir * 0.8f * size, Color.red);
            }

            DrawCircle(center, size, 32);
            DrawTickmarks(center, size);
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
                Debug.DrawLine(positions[i], positions[i + 1]);
            }
            Debug.DrawLine(positions[^1], positions[0]);


        }

        void DrawTickmarks(Vector3 center, float radius)
        {
            int tickmarkCount = 60;
            float angleBetween = 360.0f / tickmarkCount;
            for (int i = 0; i < tickmarkCount; i++)
            {
                var currentDir = Quaternion.Euler(0, 0, angleBetween * i) * Vector3.up;
                var pos = center + currentDir * radius;
                float size = Mathf.Lerp(0.2f, 0.1f, i % 5);
                Debug.DrawRay(pos, currentDir.normalized * size, Color.red);
            }
        }
    }
}
