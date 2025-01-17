using UnityEngine;
using UnityEngine.AI;

public class Mob : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    public MultiplierDoor door;
    public void Init(Vector3 destination, MultiplierDoor _door = null)
    {
        agent.destination = destination;
        door = _door;
    }
}
