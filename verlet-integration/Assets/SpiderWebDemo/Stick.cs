using UnityEngine;

namespace SpiderWeb
{
    [RequireComponent(typeof(LineRenderer))]
    public class Stick : MonoBehaviour
    {
        public Point pointA;
        public Point pointB;
        float length;

        public void Init()
        {
            length = Vector2.Distance(pointA.tr.position, pointB.tr.position);
        }

        public void Tick()
        {
            float distance = Vector3.Distance(pointA.tr.position, pointB.tr.position);
            float difference = length - distance;
            float percent = difference / distance;

            float dx = pointB.tr.position.x - pointA.tr.position.x;
            float dy = pointB.tr.position.y - pointA.tr.position.y;
            Vector3 offset = Vector3.zero;
            offset.x = dx * percent / 2;
            offset.y = dy * percent / 2;

            if (!pointA.pinned)
            {
                pointA.tr.position -= offset;
            }
            if (!pointB.pinned)
            {
                pointB.tr.position += offset;
            }

            var line = GetComponent<LineRenderer>();
            line.positionCount = 2;
            line.SetPosition(0, pointA.tr.position);
            line.SetPosition(1, pointB.tr.position);
        }

        void OnValidate()
        {
            if(pointA == null || pointB == null) return;
            
            var posA = pointA.transform.position;
            var posB = pointB.transform.position;
            var line = GetComponent<LineRenderer>();
            line.positionCount = 2;
            line.SetPosition(0, posA);
            line.SetPosition(1, posB);
        }

        public void ForceToRefresh()
        {
            OnValidate();
        }
    }
}
