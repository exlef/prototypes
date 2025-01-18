using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    [SerializeField] [Min(1)] int health = 1;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Collider myCollider;
    [SerializeField] BlinkEffect blinkEffect;

    public CharacterType charType { get; private set; }
    [HideInInspector] public MultiplierDoor door;
    private bool isEnemy;

    readonly WaitForSecondsRealtime targetReachedCheckWait = new(0.2f);
    readonly WaitForSeconds deathWait = new(0.2f);
    
    public void Init(Vector3 destination, CharacterType _charType, MultiplierDoor _door)
    {
        agent.SetDestination(destination);
        charType = _charType;
        isEnemy = charType switch
        {
            CharacterType.normie or CharacterType.champion => false,
            CharacterType.enemyNormie or CharacterType.enemyBig => true,
            _ => throw new ArgumentOutOfRangeException()
        };
        door = _door;
        StartCoroutine(CheckHasTargetReached());
    }

    IEnumerator CheckHasTargetReached()
    {
        while (agent.enabled)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (isEnemy)
                {
                    GameManager.instance.EnemyReachedCannon();                    
                }
                else
                {
                    GameManager.instance.MobReachedTower(this);
                }
            }
            yield return targetReachedCheckWait;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character c))
        {
            if (c.isEnemy)
            {
                GotDamage(1);
                c.GotDamage(1);
            }
        }
    }

    void GotDamage(int point)
    {
        blinkEffect.TriggerBlink();
        health -= point;
        if (health <= 0)
        {
            myCollider.enabled = false;
            agent.enabled = false;
            StartCoroutine(DeathRoutine());
        }
    }

    IEnumerator DeathRoutine()
    {
        yield return deathWait;
        Destroy(gameObject);
    }
}

public enum CharacterType
{
    normie = 0,
    champion = 10,
    enemyNormie = 20,
    enemyBig =40,
}
