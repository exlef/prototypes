using UnityEngine;

namespace NumbersVectorsDot_Product
{

    [ExecuteInEditMode]
    public class LocalToWorld : MonoBehaviour
    {

        [SerializeField] Transform parent;
        [SerializeField] Transform child;
        [SerializeField] Vector3 childWorldPostion;
        [SerializeField] Transform yMarker;
        [SerializeField] Transform xMarker;
        [SerializeField] Transform xPlusy;

        void Update()
        {
            Debug.DrawRay(parent.position, child.position - parent.position);
            var dir = child.position - parent.position;
            var x = Vector3.Dot(parent.right, dir);
            var y = Vector3.Dot(parent.up, dir);

            xMarker.position = x * parent.right + parent.position;
            yMarker.position = y * parent.up + parent.position;
            xPlusy.position = xMarker.position + yMarker.position - parent.position;

            childWorldPostion = xMarker.position + yMarker.position - parent.position;

            child.gameObject.name = child.position.ToString(); 
        }
    }

}