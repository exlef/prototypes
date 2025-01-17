using System.Collections;
using UnityEngine;

class Cannon : MonoBehaviour
{
    public Vector3 spawnPos => spawnPoint.position;

    [SerializeField] Animator animator;
    [SerializeField] Transform spawnPoint;
    
    static readonly int shootTriggerIndex = Animator.StringToHash("shoot");
    
    public void Move(float x, float limit)
    {
        Vector3 pos = transform.position + new Vector3(x, 0, 0);
        // this assumes cannon is at Vector3.zero at start TODO: fix this.
        pos.x = Mathf.Clamp(pos.x, -limit, limit);
        transform.position = pos;
    }

    public void Shoot()
    {
        animator.CrossFadeInFixedTime("idle", 0f);
        animator.CrossFadeInFixedTime(shootTriggerIndex, 0f);
    }
}