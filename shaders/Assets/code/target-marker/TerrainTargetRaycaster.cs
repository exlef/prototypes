using UnityEngine;

public class TerrainTargetRaycaster : MonoBehaviour
{
    void Update()
    {
        if (/*Input.GetMouseButton(0)*/ true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                MeshRenderer meshRenderer = hit.collider.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    // Get UV coordinate of the hit point
                    Vector2 hitUV = hit.textureCoord;
                    // Debug.Log(hitUV);
                    Material material = meshRenderer.material;
                    material.SetFloat("_TargetX", hitUV.x);
                    material.SetFloat("_TargetY", hitUV.y);

                    // Update shader with new offset value
                    /*
                    Material material = meshRenderer.material;

                    Vector4 decalTexST = material.GetVector("_DecalTex_ST");
                    Vector2 tiling = new(decalTexST.x, decalTexST.y);

                    hitUV *= tiling;
                    hitUV -= new Vector2(0.5f, 0.5f);
                    hitUV = -hitUV;


                    material.SetTextureOffset("_DecalTex", hitUV);
                    */
                }
            }
        }
    }
}
