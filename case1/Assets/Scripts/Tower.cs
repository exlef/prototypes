using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform spawnPoint;
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
    }
}
