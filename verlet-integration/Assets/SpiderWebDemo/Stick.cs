using UnityEngine;

namespace SpiderWeb
{
    [RequireComponent(typeof(LineRenderer))]
    class Stick : MonoBehaviour
    {
        public Point pointA;
        public Point pointB;
        [ExReadOnly] [SerializeField] float length;

        public void Tick()
        {
            float dx = pointB.tr.position.x - pointA.tr.position.x;
            float dy = pointB.tr.position.y - pointA.tr.position.y;
            float distance = Vector2.Distance(pointA.tr.position, pointB.tr.position);
            float difference = length - distance;

            float percent = difference / distance;
            float offsetX = dx * percent / 2;
            float offsetY = dy * percent / 2;

            Vector3 posA = Vector3.zero;
            Vector3 posB = Vector3.zero;

            if (!pointA.pinned)
            {
                posA.x -= offsetX;
                posA.y -= offsetY;
            }
            if (!pointB.pinned)
            {
                posB.x += offsetX;
                posB.y += offsetY;
            }
        }

        void OnValidate()
        {
            if(pointA == null || pointB == null) return;
            var line = GetComponent<LineRenderer>();
            var posA = pointA.transform.position;
            var posB = pointB.transform.position;
            length = Vector2.Distance(posA, posB);
            line.positionCount = 2;
            line.SetPosition(0, posA);
            line.SetPosition(1, posB);
        }
    }
}
