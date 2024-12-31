using UnityEngine;

namespace Matrix_DotProduct
{
    public class TurretDemo : MonoBehaviour
    {
        [SerializeField] Transform turret;

        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            var result = Physics.Raycast(ray, out RaycastHit hit);
            if(!result) return;
            turret.position = hit.point;
        }
    }
}
