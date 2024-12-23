using System.Collections.Generic;
using UnityEngine;

namespace SpiderWeb
{
    public class SpiderWebDemo : MonoBehaviour
    {
        [SerializeField] List<Point> points;
        [SerializeField] List<Stick> sticks;
        [SerializeField] Bounds bounds;

        void Start()
        {
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
            for (int i = 0; i < 2; i++)
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