using UnityEngine;

namespace NumbersVectorsDot_Product
{
    public class RadialTrigger : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] float triggerRadius = 2;
        void OnDrawGizmos()
        {
            if(target == null) return;
            float distance = Vector3.Distance(transform.position, target.position);
            Gizmos.color = distance < triggerRadius ? Color.red : Color.green; 
            Gizmos.DrawWireSphere(transform.position, triggerRadius);
        }
    }
}
