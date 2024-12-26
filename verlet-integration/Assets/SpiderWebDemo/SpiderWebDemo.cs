using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace SpiderWeb
{
    public class SpiderWebDemo : MonoBehaviour
    {
        [SerializeField] Spider spider;
        [SerializeField] GameObject fly;
        [SerializeField] Bounds bounds;
        [HideInInspector] public List<Point> points;
        [HideInInspector] public List<Stick> sticks;
        List<Point> originalPinnedPoints = new();

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

            foreach (var point in points)
            {
                if(point.pinned)
                    originalPinnedPoints.Add(point);
            }

            SetConnectedPoints();

            spider.Init(points[0], this);
        }

        void SetConnectedPoints()
        {
            foreach (var stick in sticks)
            {
                stick.pointA.connectedPoints.Add(stick.pointB);
                stick.pointB.connectedPoints.Add(stick.pointA);
            }
        }

        void Update()
        {
            Render();
            DragPinnedPoints();
            MoveSpider();
        }

        void MoveSpider()
        {
            if(spider.hasRoute) {spider.Tick(); return;}

            var unpinnedPoints = points.Where(point => !point.pinned).ToList();
            if(unpinnedPoints.Count == 0) return;

            var target = unpinnedPoints[Random.Range(0, unpinnedPoints.Count)];
            spider.SetRoute(target);
            JiggleWeb();
            fly.transform.position = (Vector2)target.tr.position;
            fly.transform.parent = target.tr;
            spider.Tick();
        }

        void JiggleWeb()
        {
            var unpinnedPoints = points.Where(point => !point.pinned).ToList();
            foreach (var point in unpinnedPoints)
            {
                point.tr.position += new Vector3(Random.Range(0.0f, 0.2f), Random.Range(0.0f, 0.2f), 0);
            }
        }

        void Render()
        {
            foreach (var stick in sticks)
            {
                stick.lineRnd.SetPosition(0, stick.pointA.tr.position);
                stick.lineRnd.SetPosition(1, stick.pointB.tr.position);
            }
        }

        Point draggedPoint;

        void DragPinnedPoints()
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                foreach (var point in points)
                {
                    // if (!point.pinned) continue;
                    if (Vector2.Distance(mousePos, point.tr.position) < 0.2f)
                    {
                        draggedPoint = point;
                        draggedPoint.pinned = true;
                    }

                }
            }

            if(Input.GetMouseButtonUp(0))
            {
                if(originalPinnedPoints.Contains(draggedPoint) == false)
                    draggedPoint.pinned = false;
                draggedPoint = null;
            }

            if(draggedPoint == null) return;

            draggedPoint.tr.position = (Vector2)mousePos;
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