using UnityEngine;

namespace SpiderWeb
{
    public class Spider : MonoBehaviour
    {
        [SerializeField] float speed = 5;
        Vector3 _target;
        public Vector3 target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
                RotateTowardsTarget();
            }
        }

        public void Init(Vector3 spiderTarget)
        {
            target = spiderTarget;
        }

        void RotateTowardsTarget()
        {
            // Calculate the direction from the object to the target
            Vector3 directionToTarget = target - transform.position;

            // Create a rotation that aligns the up vector with the direction to the target
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.Cross(directionToTarget, Vector3.right), directionToTarget);

            // Apply the rotation to the object
            transform.rotation = targetRotation;
        }

        public void Tick()
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
    }
}
