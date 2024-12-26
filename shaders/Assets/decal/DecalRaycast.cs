using UnityEngine;

public class DecalRaycast : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                MeshRenderer meshRenderer = hit.collider.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    // Get UV coordinate of the hit point
                    Vector2 hitUV = hit.textureCoord;

                    // Update shader with new offset value
                    Material material = meshRenderer.sharedMaterial;

                    hitUV -= new Vector2(0.5f, 0.5f);
                    hitUV = -hitUV;

                    material.SetTextureOffset("_DecalTex", hitUV);
                }
            }
        }
    }
}
