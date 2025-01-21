using System.Collections;
using TMPro;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform spawnPoint;
    [SerializeField] Transform towerVisual;
    [SerializeField] AnimationCurve enemySpawnShakeCurve;
    [SerializeField] TextMeshPro healthText; 
    private int health;
    private Vector3 originalScale;
    private bool isDamageShaking;

    public void Init(int _health)
    {
        health = _health;
        originalScale = transform.localScale;
    }
    
    public void TryDamage(int damagePoint)
    {
        if (health <= 0) return;
        health -= damagePoint;
        
        if (health <= 0)
        {
            GameManager.instance.OnTowerDefeated();
        }

        if (health < 0) health = 0;
        healthText.text = $"{health}";

        StartCoroutine(DamageShake());
    }
    
    IEnumerator DamageShake()
    {
        if (isDamageShaking) yield break; // Exit if already shaking

        isDamageShaking = true;

        // Shake parameters
        float shakeDuration = 0.2f; // Duration of the shake
        float shakeMagnitude = 0.1f; // Magnitude of the shake
        float elapsed = 0f;

        Vector3 targetScale = new Vector3(originalScale.x + shakeMagnitude, originalScale.y, originalScale.z);

        while (elapsed < shakeDuration)
        {
            // Interpolate between the original scale and the target scale
            float t = elapsed / shakeDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.Sin(t * Mathf.PI * 2)); // Sinusoidal shake effect

            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Reset scale back to original
        transform.localScale = originalScale;

        isDamageShaking = false; // Reset shaking state
    }
    
    public void EnemySpawnShake()
    {
        transform.localScale = originalScale;
        transform.localScale += Vector3.right * Random.Range(-0.1f, 0.1f);
    }
}
