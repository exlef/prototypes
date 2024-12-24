using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpiderWeb
{
    public class SpiderWebDemo : MonoBehaviour
    {
        [SerializeField] Bounds bounds;
        List<Point> points;
        List<Stick> sticks;

        void Start()
        {
            points = FindObjectsByType<Point>(FindObjectsSortMode.None).ToList();
            sticks = FindObjectsByType<Stick>(FindObjectsSortMode.None).ToList();

            foreach (var point in points)
            {
                point.Init();
            }

            foreach (var stick in sticks)
            {
                stick.Init();
            }
        }

        void FixedUpdate()
        {
            UpdatePoints();
            for (int i = 0; i < 8; i++)
            {
                UpdateSticks();
                ConstrainPointsToWorldBounds();
            }
        }

        void UpdatePoints()
        {
            foreach (var point in points)
            {
                point.Tick();
            }
        }

        void UpdateSticks()
        {
            foreach (var stick in sticks)
            {
                stick.Tick();
            }
        }

        void ConstrainPointsToWorldBounds()
        {
            foreach (var point in points)
            {
                point.ConstrainWorldBounds(bounds);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(bounds.center, bounds.extents);
        }
    }
}