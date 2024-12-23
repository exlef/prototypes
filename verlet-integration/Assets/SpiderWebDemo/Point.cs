using UnityEngine;

namespace SpiderWeb
{
    class Point : MonoBehaviour
    {
        public Transform tr
        {
            get
            {
                if (tr == null)
                {
                    tr = GetComponent<Transform>();
                }
                return tr;
            }
            set{}
        }
        public Vector3 oldPos;
        public bool pinned;
        readonly float radius;

        public void Tick()
        {
            if (pinned)
            {
                oldPos = (Vector2)tr.position; // otherwise the oldPos will be the its value when we pinned the point and when it got unpinned this will cause to point move unexpectedly since its oldPos is not updated correctly.
                return;
            }
            Vector3 v = tr.position - oldPos;
            oldPos = tr.position;
            tr.position += v;
        }

        public void ConstrainWorldBounds(Bounds bounds)
        {
            if (pinned) return;

            float maxX = bounds.center.x + bounds.extents.x / 2 - radius;
            float minX = bounds.center.x - bounds.extents.x / 2 + radius;
            float maxY = bounds.center.y + bounds.extents.y / 2 - radius;
            float minY = bounds.center.y - bounds.extents.y / 2 + radius;

            Vector2 pos = Vector2.zero;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            tr.position = (Vector3)pos;
        }
    }
}