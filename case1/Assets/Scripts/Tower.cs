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

    public void Init(int _health)
    {
        health = _health;
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
        StartCoroutine(TransformShaker.ShakeTransform(transform, enemySpawnShakeCurve, enemySpawnShakeDuration, Vector3.right,
            TransformShaker.ShakeType.Scale));
    }
}
