using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterType charType;
    public Agent Agent;
    // [SerializeField] NavMeshAgent agent;
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
        switch (charType)
        {
            case CharacterType.normie:
            case CharacterType.champion:
                if (path.TryGetNextPoint(pathPointIndex, out Vector3 destinationMob))
                {
                    Agent.SetDestination(destinationMob);
                }
                break;
            case CharacterType.enemyNormie:
            case CharacterType.enemyBig:
                if (path.TryGetNextPoint(pathPointIndex, out Vector3 destinationEnemy))
                {
                    Agent.SetDestination(destinationEnemy);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        health = charType switch
        {
            CharacterType.normie or CharacterType.enemyNormie => GameManager.normieHealth,
            CharacterType.champion => GameManager.instance.championHealth,
            CharacterType.enemyBig => GameManager.instance.bigNormieHealth,
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
        while (!Agent.isStopped)
        {
            if (Agent.hasReached)
            {
                if (isEnemy)
                {
                    pathPointIndex++;
                    if (path.TryGetNextPoint(pathPointIndex, out Vector3 nextPoint))
                    {
                        Agent.SetDestination(nextPoint, targetWidth);
                    }
                    else
                    {
                        Agent.Stop();
                        GameManager.instance.EnemyReachedCannon();   
                    }
                }
                else
                {
                    pathPointIndex++;
                    if (path.TryGetNextPoint(pathPointIndex, out Vector3 nextPoint))
                    {
                        Debug.Log(nextPoint);
                        Agent.SetDestination(nextPoint, targetWidth);
                    }
                    else
                    {
                        Agent.Stop();
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

    public void GotDamage(int point)
    {
        blinkEffect.TriggerBlink();
        health -= point;
        if (health <= 0)
        {
            myCollider.enabled = false;
            Agent.Stop();
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
