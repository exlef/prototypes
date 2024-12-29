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

        Vector3 WorldToLocalDebug(Vector3 worldPos)
        {
            Vector3 localPos = Vector3.zero;

            var parentToChild = worldPos - transform.position;
            var parentPos = transform.position;

            Debug.DrawRay(transform.position, parentToChild);

            xMarker.position = Vector3.Dot(transform.right , parentToChild) * transform.right + parentPos;
            yMarker.position = Vector3.Dot(transform.up, parentToChild) * transform.up + parentPos;
            zMarker.position = Vector3.Dot(transform.forward, parentToChild) * transform.forward + parentPos;


            localPos.x = Vector3.Dot(transform.right, parentToChild);
            localPos.y = Vector3.Dot(transform.up, parentToChild);
            localPos.z = Vector3.Dot(transform.forward, parentToChild);

            return localPos;
        }

        Vector3 WorldToLocal(Vector3 worldPos)
        {
            Vector3 localPos = Vector3.zero;

            var parentToChild = worldPos - transform.position;

            localPos.x = Vector3.Dot(transform.right, parentToChild);
            localPos.y = Vector3.Dot(transform.up, parentToChild);
            localPos.z = Vector3.Dot(transform.forward, parentToChild);

            return localPos;
        }
    }
}
