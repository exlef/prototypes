using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class BlinkEffect : MonoBehaviour
{
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    
    private Renderer rendererComponent;
    private MaterialPropertyBlock propertyBlock;
    private Color originalColor;
    private Color originalEmission;
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
        propertyBlock = new MaterialPropertyBlock();
        
        // Get original colors from the material itself
        originalColor = rendererComponent.sharedMaterial.GetColor(BaseColorId);
        originalEmission = rendererComponent.sharedMaterial.GetColor(EmissionColorId);

        // Initialize the property block with these colors
        propertyBlock.SetColor(BaseColorId, originalColor);
        propertyBlock.SetColor(EmissionColorId, originalEmission);
        rendererComponent.SetPropertyBlock(propertyBlock);
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
            // Set blink color
            rendererComponent.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColorId, blinkColor);
            propertyBlock.SetColor(EmissionColorId, blinkColor);
            rendererComponent.SetPropertyBlock(propertyBlock);

            yield return new WaitForSeconds(blinkInterval);

            // Restore original color
            rendererComponent.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColorId, originalColor);
            propertyBlock.SetColor(EmissionColorId, originalEmission);
            rendererComponent.SetPropertyBlock(propertyBlock);

            yield return new WaitForSeconds(blinkInterval);
        }

        isBlinking = false;
    }
}