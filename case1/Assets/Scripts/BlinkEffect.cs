using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class BlinkEffect : MonoBehaviour
{
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    
    private Renderer rendererComponent;
    private MaterialPropertyBlock[] propertyBlocks;
    private Color[] originalColors;
    private Color[] originalEmissions;
    private bool isBlinking = false;

    [SerializeField]
    private Color blinkColor = Color.white;
    [SerializeField]
    private float blinkDuration = 0.1f;
    [SerializeField]
    private int blinkCount = 2;

    private void Awake()
    {
        rendererComponent = GetComponent<Renderer>();
        Material[] materials = rendererComponent.materials;
        
        // Initialize arrays for each material
        propertyBlocks = new MaterialPropertyBlock[materials.Length];
        originalColors = new Color[materials.Length];
        originalEmissions = new Color[materials.Length];

        // Setup property blocks for each material
        for (int i = 0; i < materials.Length; i++)
        {
            propertyBlocks[i] = new MaterialPropertyBlock();
            
            // Store original colors from each material
            originalColors[i] = materials[i].GetColor(BaseColorId);
            originalEmissions[i] = materials[i].GetColor(EmissionColorId);

            // Initialize the property block with these colors
            propertyBlocks[i].SetColor(BaseColorId, originalColors[i]);
            propertyBlocks[i].SetColor(EmissionColorId, originalEmissions[i]);
            rendererComponent.SetPropertyBlock(propertyBlocks[i], i);
        }
    }

    private void OnDestroy()
    {
        // Clean up materials array if it was created
        if (rendererComponent != null)
        {
            Material[] materials = rendererComponent.materials;
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(materials[i]);
                    }
                }
            }
        }
    }
    
    public void TriggerBlink()
    {
        if (!isBlinking)
        {
            StartCoroutine(BlinkCoroutine());
        }
    }

    private IEnumerator BlinkCoroutine()
    {
        isBlinking = true;
        float blinkInterval = blinkDuration / (blinkCount * 2);

        for (int i = 0; i < blinkCount; i++)
        {
            // Set blink color for all materials
            for (int matIndex = 0; matIndex < propertyBlocks.Length; matIndex++)
            {
                rendererComponent.GetPropertyBlock(propertyBlocks[matIndex], matIndex);
                propertyBlocks[matIndex].SetColor(BaseColorId, blinkColor);
                propertyBlocks[matIndex].SetColor(EmissionColorId, blinkColor);
                rendererComponent.SetPropertyBlock(propertyBlocks[matIndex], matIndex);
            }

            yield return new WaitForSeconds(blinkInterval);

            // Restore original colors for all materials
            for (int matIndex = 0; matIndex < propertyBlocks.Length; matIndex++)
            {
                rendererComponent.GetPropertyBlock(propertyBlocks[matIndex], matIndex);
                propertyBlocks[matIndex].SetColor(BaseColorId, originalColors[matIndex]);
                propertyBlocks[matIndex].SetColor(EmissionColorId, originalEmissions[matIndex]);
                rendererComponent.SetPropertyBlock(propertyBlocks[matIndex], matIndex);
            }

            yield return new WaitForSeconds(blinkInterval);
        }

        isBlinking = false;
    }
}