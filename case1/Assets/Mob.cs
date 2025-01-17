using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Mob : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    public MultiplierDoor door;
    readonly WaitForSecondsRealtime targetReachedCheckWait = new(0.2f);
    public void Init(Vector3 destination, MultiplierDoor _door = null)
    {
        agent.destination = destination;
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
