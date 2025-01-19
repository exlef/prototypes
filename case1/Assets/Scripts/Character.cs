using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    public CharacterType charType;
    [SerializeField] [Min(1)] int health = 1;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Collider myCollider;
    [SerializeField] BlinkEffect blinkEffect;
    
    public LevelPath path { get; private set; }
    public int pathPointIndex { get; private set; }
    [HideInInspector] public MultiplierDoor door;
    private bool isEnemy;

    readonly WaitForSecondsRealtime targetReachedCheckWait = new(0.2f);
    readonly WaitForSeconds deathWait = new(0.2f);
    
    public void Init(LevelPath _path, int _pathPointIndex, CharacterType _charType, MultiplierDoor _door)
    {
        path = _path;
        pathPointIndex = _pathPointIndex;
        charType = _charType;
        switch (charType)
        {
            case CharacterType.normie:
            case CharacterType.champion:
                if (path.TryGetNextPoint(pathPointIndex, out Vector3 destinationMob))
                {
                    agent.SetDestination(destinationMob);
                }
                break;
            case CharacterType.enemyNormie:
            case CharacterType.enemyBig:
                if (path.TryGetNextPoint(pathPointIndex, out Vector3 destinationEnemy))
                {
                    agent.SetDestination(destinationEnemy);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        

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
                    pathPointIndex++;
                    if (path.TryGetNextPoint(pathPointIndex, out Vector3 nextPoint))
                    {
                        agent.SetDestination(nextPoint);
                    }
                    else
                    {
                        agent.enabled = false;
                        GameManager.instance.EnemyReachedCannon();   
                    }
                }
                else
                {
                    pathPointIndex++;
                    if (path.TryGetNextPoint(pathPointIndex, out Vector3 nextPoint))
                    {
                        agent.SetDestination(nextPoint);
                    }
                    else
                    {
                        agent.enabled = false;
                        GameManager.instance.MobReachedTower(this);
                    }
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
