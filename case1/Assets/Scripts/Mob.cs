using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Mob : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    public bool isEnemy { get; private set; }
    [HideInInspector] public MultiplierDoor door;
    readonly WaitForSecondsRealtime targetReachedCheckWait = new(0.2f);
    
    public void Init(Vector3 destination, bool _isEnemy = false, MultiplierDoor _door = null)
    {
        agent.destination = destination;
        isEnemy = _isEnemy;
        door = _door;
        StartCoroutine(CheckHasTargetReached());
    }

    IEnumerator CheckHasTargetReached()
    {
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return targetReachedCheckWait;
        }
        GameManager.instance.MobReachedTower(this);
    }
}
