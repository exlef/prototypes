using UnityEngine;

namespace SpiderWeb
{
    class Point : MonoBehaviour
    {
        public Vector2 pos;
        public Vector2 oldPos;
        public bool pinned;
        readonly float radius;

        public void Init(float x, float y, bool isPinned = false)
        {
            pos = new Vector2(x, y);
            pinned = isPinned;
            oldPos = pos;
        }

        public void Tick()
        {
            if (pinned)
            {
                oldPos = pos; // otherwise the oldPos will be the its value when we pinned the point and when it got unpinned this will cause to point move unexpectedly since its oldPos is not updated correctly.
                return;
            }
            Vector2 v = pos - oldPos;
            oldPos = pos;
            pos += v;
        }

        public void ConstrainWorldBounds(Bounds bounds)
        {
            if (pinned) return;

            float maxX = bounds.center.x + bounds.extents.x / 2 - radius;
            float minX = bounds.center.x - bounds.extents.x / 2 + radius;
            float maxY = bounds.center.y + bounds.extents.y / 2 - radius;
            float minY = bounds.center.y - bounds.extents.y / 2 + radius;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }
    }
}