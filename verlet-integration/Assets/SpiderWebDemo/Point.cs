using System.Collections.Generic;
using UnityEngine;

namespace SpiderWeb
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Point : MonoBehaviour
    {
        public Transform tr;
        [ExReadOnly] public Vector3 oldPos;
        public bool pinned;
        public HashSet<Point> connectedPoints = new ();
        float radius;

        public void Init()
        {
            tr = transform;
            oldPos = transform.position;
            radius = transform.localScale.x / 2;
        }

        // debugging
        [ContextMenu("PrintNeighbours")]
        void PrintNeighbours()
        {
            foreach (var neighbour in connectedPoints)
            {
                Debug.Log(neighbour, neighbour.gameObject);
            }
        }

        public void Tick()
        {
            if (pinned)
            {
                oldPos = tr.position; // otherwise the oldPos will be the its value when we pinned the point and when it got unpinned this will cause to point move unexpectedly since its oldPos is not updated correctly.
                return;
            }
            Vector3 v = tr.position - oldPos;
            oldPos = tr.position;
            tr.position += v;

            tr.position += new Vector3(0, -0.1f, 0);
        }

        public void ConstrainWorldBounds(Bounds bounds)
        {
            if (pinned) return;

            float maxX = bounds.center.x + bounds.extents.x / 2 - radius;
            float minX = bounds.center.x - bounds.extents.x / 2 + radius;
            float maxY = bounds.center.y + bounds.extents.y / 2 - radius;
            float minY = bounds.center.y - bounds.extents.y / 2 + radius;

            Vector3 pos = tr.position;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            tr.position = pos;
        }
    }
}