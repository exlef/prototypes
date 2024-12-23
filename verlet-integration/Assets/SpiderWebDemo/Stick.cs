using UnityEngine;

namespace SpiderWeb
{
    class Stick : MonoBehaviour
    {
        public Point pointA;
        public Point pointB;
        float length;

        public void Init(Point a, Point b)
        {
            pointA = a;
            pointB = b;
            length = Vector2.Distance(pointA.pos, pointB.pos);
        }

        public void Tick()
        {
            float dx = pointB.pos.x - pointA.pos.x;
            float dy = pointB.pos.y - pointA.pos.y;
            float distance = Vector2.Distance(pointA.pos, pointB.pos);
            float difference = length - distance;

            float percent = difference / distance;
            float offsetX = dx * percent / 2;
            float offsetY = dy * percent / 2;

            if (!pointA.pinned)
            {
                pointA.pos.x -= offsetX;
                pointA.pos.y -= offsetY;
            }
            if (!pointB.pinned)
            {
                pointB.pos.x += offsetX;
                pointB.pos.y += offsetY;
            }
        }
    }
}
