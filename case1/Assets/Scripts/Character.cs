using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterType charType;
    public Agent agent;
    [SerializeField] float radius = 1;
    [SerializeField] float stoppingDistance = 1;
    [SerializeField] Collider myCollider;
    [SerializeField] BlinkEffect blinkEffect;
    
    public LevelPath path { get; private set; }
    public int pathPointIndex { get; private set; }
    [HideInInspector] public MultiplierDoor door;
    int health = 1;
    private bool isEnemy;
    private float targetWidth = 1; // this is a hack. this will prevent characters to line up when moving towards a target. later, better solution will replace this.

    readonly WaitForSecondsRealtime targetReachedCheckWait = new(0.2f);
    readonly WaitForSeconds deathWait = new(0.2f);
    
    public void Init(LevelPath _path, int _pathPointIndex, CharacterType _charType, MultiplierDoor _door)
    {
        path = _path;
        pathPointIndex = _pathPointIndex;
        charType = _charType;
        float speed = charType switch
        {
            CharacterType.normie => GameManager.instance.mobNormieSpeed,
            CharacterType.champion => GameManager.instance.championSpeed,
            CharacterType.enemyNormie => GameManager.instance.enemyNormieSpeed,
            CharacterType.enemyBig => GameManager.instance.bigEnemySpeed,
            _ => throw new ArgumentOutOfRangeException()
        };
        agent = new Agent(transform, radius, stoppingDistance, speed);
        switch (charType)
        {
            case CharacterType.normie:
            case CharacterType.champion:
                if (path.TryGetNextPoint(pathPointIndex, out Vector3 destinationMob))
                {
                    agent.SetDestination(destinationMob, 0);
                }
                break;
            case CharacterType.enemyNormie:
            case CharacterType.enemyBig:
                if (path.TryGetNextPoint(pathPointIndex, out Vector3 destinationEnemy))
                {
                    agent.SetDestination(destinationEnemy, targetWidth);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        health = charType switch
        {
            CharacterType.normie or CharacterType.enemyNormie => GameManager.normieHealth,
            CharacterType.champion => GameManager.instance.championHealth,
            CharacterType.enemyBig => GameManager.instance.bigEnemyHealth,
            _ => throw new ArgumentOutOfRangeException()
        };
        

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
        while (!agent.isStopped)
        {
            if (agent.hasReached)
            {
                if (isEnemy)
                {
                    pathPointIndex++;
                    if (path.TryGetNextPoint(pathPointIndex, out Vector3 nextPoint))
                    {
                        // if this is the last waypoint for enemy then set targetWidth to zero to make enemies go exactly to point
                        if (pathPointIndex == path.points.Count - 1)
                        {
                            targetWidth = 0;
                        }
                        agent.SetDestination(nextPoint, targetWidth);
                    }
                    else
                    {
                        agent.Stop();
                        GameManager.instance.EnemyReachedCannon();   
                    }
                }
                else
                {
                    pathPointIndex++;
                    if (path.TryGetNextPoint(pathPointIndex, out Vector3 nextPoint))
                    {
                        agent.SetDestination(nextPoint, targetWidth);
                    }
                    else
                    {
                        agent.Stop();
                        GameManager.instance.MobReachedTower(this);
                    }
                }
            }
            yield return targetReachedCheckWait;
        }
    }

    // IEnumerator CheckHasTargetReached()
    // {
    //     while (agent.enabled)
    //     {
    //         if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
    //         {
    //             if (isEnemy)
    //             {
    //                 pathPointIndex++;
    //                 if (path.TryGetNextPoint(pathPointIndex, out Vector3 nextPoint))
    //                 {
    //                     agent.SetDestination(nextPoint);
    //                 }
    //                 else
    //                 {
    //                     agent.enabled = false;
    //                     GameManager.instance.EnemyReachedCannon();   
    //                 }
    //             }
    //             else
    //             {
    //                 pathPointIndex++;
    //                 if (path.TryGetNextPoint(pathPointIndex, out Vector3 nextPoint))
    //                 {
    //                     agent.SetDestination(nextPoint);
    //                 }
    //                 else
    //                 {
    //                     agent.enabled = false;
    //                     GameManager.instance.MobReachedTower(this);
    //                 }
    //             }
    //         }
    //         yield return targetReachedCheckWait;
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character c))
        {
            if (c.isEnemy)
            {
                
                GotDamage(GameManager.normieHealth);
                c.GotDamage(GameManager.normieHealth);
            }
        }
    }

    private void OnDrawGizmos()
    {
        var tr = transform;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(tr.position, radius);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(tr.position, tr.position + (tr.forward * stoppingDistance));
    }

    public void GotDamage(int point)
    {
        blinkEffect.TriggerBlink();
        health -= point;
        if (health <= 0)
        {
            myCollider.enabled = false;
            agent.Stop();
            StartCoroutine(DeathRoutine());
        }
    }

    IEnumerator DeathRoutine()
    {
        yield return deathWait;
        GameManager.instance.OnCharactersDeath(this);
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
