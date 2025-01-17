using System.Collections;
using UnityEngine;

class Cannon : MonoBehaviour
{
    static readonly int shootTriggerIndex = Animator.StringToHash("shoot");
    [SerializeField] Animator animator;
    
    public void Move(float x, float limit)
    {
        Vector3 pos = transform.position + new Vector3(x, 0, 0);
        // this assumes cannon is at Vector3.zero at start TODO: fix this.
        pos.x = Mathf.Clamp(pos.x, -limit, limit);
        transform.position = pos;
    }

    public void Shoot(float animTimeDuration)
    {
        animator.CrossFade(shootTriggerIndex, animTimeDuration);
    }
}