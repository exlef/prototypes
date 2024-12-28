using System;
using UnityEngine;

namespace NumbersVectorsDot_Product
{
    public class Solution : MonoBehaviour
    {   
        [SerializeField] Vector3 localPos;
        [SerializeField] Transform child;

        [SerializeField] Transform xMarker;
        [SerializeField] Transform yMarker;
        [SerializeField] Transform zMarker;

        void OnDrawGizmos()
        {
            Vector3 worldPos = LocalToWorld(localPos);
            Gizmos.DrawWireSphere(worldPos, .5f);

            child.localPosition = WorldToLocal(worldPos);

            Debug.DrawRay(Vector3.zero, worldPos);
        }

        Vector3 LocalToWorld(Vector3 localPos)
        {
            Vector3 worldPos = transform.position;
            worldPos += transform.right * localPos.x;
            worldPos += transform.up * localPos.y;
            worldPos += transform.forward * localPos.z;
            return worldPos;
        }

        Vector3 WorldToLocal(Vector3 worldPos)
        {
            Vector3 localPos = Vector3.zero;

            var x = Vector3.Dot(transform.right, worldPos) * transform.right;
            var y = Vector3.Dot(transform.up, worldPos) * transform.up;
            var z = Vector3.Dot(transform.forward, worldPos) * transform.forward;

            xMarker.position = x;
            yMarker.position = y;
            zMarker.position = z;

            return localPos;
        }
    }
}
