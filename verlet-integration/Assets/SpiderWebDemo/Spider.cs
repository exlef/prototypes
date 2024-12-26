using System.Linq;
using UnityEngine;

namespace SpiderWeb
{
    public class Spider : MonoBehaviour
    {
        [SerializeField] float speed = 5;
        [SerializeField] float stopDistance = 0.2f;
        public bool hasRoute{get; private set;}
        Point destination;
        SpiderWebDemo demo;
        Point _target;
        public Point target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
                if(value == null) return;
                transform.parent = _target.tr;
                RotateTowardsTarget();
            }
        }

        public void Init(Point spiderTarget, SpiderWebDemo demo)
        {
            target = spiderTarget;
            this.demo = demo;
        }

        public void SetRoute(Point myDestination)
        {
            hasRoute = true;
            destination = myDestination;
            target = FindNextClosestPointToSpider();
        }

        void RotateTowardsTarget()
        {
            // Calculate the direction from the object to the target
            Vector3 directionToTarget = target.tr.position - transform.position;

            // Create a rotation that aligns the up vector with the direction to the target
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.Cross(directionToTarget, Vector3.right), directionToTarget);

            // Apply the rotation to the object
            transform.rotation = targetRotation;
        }

        public void Tick()
        {
            if(target == null) return;
            if(HasReachedTarget()) target = FindNextClosestPointToDestination();
            if(HasReachedDestination()) { target = null; destination = null; hasRoute = false; return; }
            transform.position = Vector3.MoveTowards(transform.position, target.tr.position, speed * Time.deltaTime);
        }

        Point FindNextClosestPointToDestination()
        {
            return target.connectedPoints.OrderBy(point => Vector3.Distance(point.tr.position, destination.tr.position)).FirstOrDefault();
        }

        Point FindNextClosestPointToSpider()
        {
            return demo.points.OrderBy(point => Vector3.Distance(point.tr.position, transform.position)).FirstOrDefault();
        }

        bool HasReachedTarget()
        {
            return Vector3.Distance(transform.position, target.tr.position) < stopDistance;
        }

        bool HasReachedDestination()
        {
            return Vector3.Distance(transform.position, destination.tr.position) < stopDistance;
        }
    }
}
