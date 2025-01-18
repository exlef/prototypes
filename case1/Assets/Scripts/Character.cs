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
    public bool isEnemy { get; private set; }
    [HideInInspector] public MultiplierDoor door;
    readonly WaitForSecondsRealtime targetReachedCheckWait = new(0.2f);
    readonly WaitForSeconds deathWait = new(0.2f);
    
    public void Init(Vector3 destination, bool _isEnemy, MultiplierDoor _door)
    {
        agent.destination = destination;
        isEnemy = _isEnemy;
        door = _door;
        StartCoroutine(CheckHasTargetReached());
    }

    IEnumerator CheckHasTargetReached()
    {
        while (agent.enabled)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                GameManager.instance.MobReachedTower(this);
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
