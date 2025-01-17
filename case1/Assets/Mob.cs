using UnityEngine;
using UnityEngine.AI;

public class Mob : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    public void Init(Vector3 destination)
    {
        agent.destination = destination;
    }
}
