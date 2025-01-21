using System.Collections;
using TMPro;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform spawnPoint;
    [SerializeField] Transform towerVisual;
    [SerializeField] AnimationCurve enemySpawnShakeCurve;
    [SerializeField] Vector3 defeatAnimEndPos;
    [SerializeField] TextMeshPro healthText; 
    private int health;
    private Vector3 originalScale;
    private bool isDamageShaking;

    public void Init(int _health)
    {
        health = _health;
        healthText.text = $"{health}";
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
        if (isDamageShaking) yield break;

        isDamageShaking = true;

        float shakeDuration = 0.2f;
        float shakeMagnitude = 0.1f;
        float elapsed = 0f;

        Vector3 targetScale = new Vector3(originalScale.x + shakeMagnitude, originalScale.y, originalScale.z);

        while (elapsed < shakeDuration)
        {
            float t = elapsed / shakeDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t*t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;

        isDamageShaking = false;
    }
    
    public void EnemySpawnShake()
    {
        transform.localScale = originalScale;
        transform.localScale += Vector3.right * Random.Range(-0.1f, 0.1f);
    }

    public IEnumerator DefeatAnim()
    {
        Vector3 originalPos = transform.position;
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            transform.position = Vector3.Lerp(originalPos, transform.position + defeatAnimEndPos, t);
            EnemySpawnShake();
            yield return null;
        }
    }
}
