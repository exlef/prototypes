using System.Collections;
using TMPro;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform spawnPoint;
    [SerializeField] Transform towerVisual;
    [SerializeField] float enemySpawnShakeDuration = 0.2f;
    [SerializeField] AnimationCurve enemySpawnShakeCurve;
    [SerializeField] TextMeshPro healthText; 
    private int health;
    private Vector3 originalScale;

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
    }
    
    public void EnemySpawnShake()
    {
        transform.localScale = originalScale;
        transform.localScale += Vector3.right * Random.Range(-0.1f, 0.1f);
    }
}
