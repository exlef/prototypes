using System.Linq;
using UnityEngine;

namespace SpiderWeb
{
    public class Spider : MonoBehaviour
    {
        [SerializeField] float speed = 5;
        [SerializeField] float rotationSpeed = 1;
        [SerializeField] float stopDistance = 0.2f;
        [SerializeField] float stopDistanceForDestination = 0.6f;

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
                // RotateTowardsTarget();
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

        void RotateTowardsTarget2()
        {
            // Calculate the direction to the target
            Vector3 directionToTarget = target.tr.position - transform.position;

            // Ensure the direction vector is normalized
            directionToTarget.Normalize();

            // Calculate the target rotation
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.Cross(directionToTarget, Vector3.right), directionToTarget);

            // Smoothly interpolate between the current rotation and the target rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        void RotateTowardsTarget3()
        {
            if (target == null) return;

            // Calculate the direction to the target
            Vector3 directionToTarget = (target.tr.position - transform.position).normalized;

            // Calculate the angle between the current up vector and the target direction
            float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90;

            // Smoothly rotate towards the target angle
            float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);

            // Apply the rotation
            transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
        }


        public void Tick()
        {
            if(target == null) return;
            if(HasReachedTarget()) target = FindNextClosestPointToDestination();
            if(HasReachedDestination()) { target = null; destination = null; hasRoute = false; return; }
            transform.position = Vector3.MoveTowards(transform.position, target.tr.position, speed * Time.deltaTime);
            RotateTowardsTarget3();
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
            return Vector3.Distance(transform.position, destination.tr.position) < stopDistanceForDestination;
        }
    }
}
