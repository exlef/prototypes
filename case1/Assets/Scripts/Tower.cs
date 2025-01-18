using UnityEngine;

public class Tower : MonoBehaviour
{
    private int health;

    public void Init(int _health)
    {
        health = _health;
    }
    
    public void GotDamage(int damagePoint)
    {
        health -= damagePoint;
        if (health <= 0)
        {
            GameManager.instance.OnTowerDefeated();
        }
    }
}
