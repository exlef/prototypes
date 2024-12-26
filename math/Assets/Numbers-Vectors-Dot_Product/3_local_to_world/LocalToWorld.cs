using UnityEngine;

namespace NumbersVectorsDot_Product
{

    public class LocalToWorld : MonoBehaviour
    {

        [SerializeField] Transform parent;
        [SerializeField] Transform child;
        [SerializeField] Vector3 childWorldPostion;

        void OnDrawGizmos()
        {
            // var d = child.position - parent.position;
            var d = child.localPosition;

            var x = Vector3.Dot(parent.right, d);
            var y = Vector3.Dot(parent.up, d);
            var z = Vector3.Dot(parent.forward, d);
            childWorldPostion = new Vector3(x, y, z) + parent.position;

            // childWorldPostion = d;

            child.gameObject.name = child.position.ToString();
        }
    }

}