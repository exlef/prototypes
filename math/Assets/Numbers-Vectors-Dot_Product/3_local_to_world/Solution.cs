using System;
using UnityEngine;

namespace NumbersVectorsDot_Product
{
    public class Solution : MonoBehaviour
    {   
        [SerializeField] Vector3 localPos;

        void OnDrawGizmos()
        {
            Vector3 worldPos = LocalToWorld(localPos);
            Gizmos.DrawWireSphere(worldPos, .5f);
        }

        private Vector3 LocalToWorld(Vector3 localPos)
        {
            Vector3 worldPos = transform.position;
            worldPos += transform.right * localPos.x;
            worldPos += transform.up * localPos.y;
            worldPos += transform.forward * localPos.z;
            return worldPos;
        }
    }
}
