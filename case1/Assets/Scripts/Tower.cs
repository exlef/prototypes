using TMPro;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform spawnPoint;
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
}
