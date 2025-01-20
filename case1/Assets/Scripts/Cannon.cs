using System;
using System.Collections;
using UnityEngine;

class Cannon : MonoBehaviour
{
    public Vector3 spawnPos => spawnPoint.position;

    [SerializeField] Animator animator;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float rotationAnimationSpeed = 5f;
    [SerializeField] ParticleSystem championIndicatorParticle;
    [SerializeField] Material originalMat;
    [SerializeField] Material cannonReadyToShootChampionMaterial;
    [SerializeField] SkinnedMeshRenderer cannonRenderer;
    [SerializeField] Transform cannonModelParentTr;
    [SerializeField] Transform cannonProceduralAnimHelper;
    
    static readonly int shootTriggerIndex = Animator.StringToHash("shoot");
    
    public void Move(float x, float limit)
    {
        Vector3 pos = transform.position + new Vector3(x, 0, 0);
        pos.x = Mathf.Clamp(pos.x,  -limit,  limit);
        transform.position = pos;
    }
    
    private void Update()
    {
        RotateCannonAnim();
    }

    void RotateCannonAnim()
    {
        cannonProceduralAnimHelper.position = Vector3.Lerp(cannonProceduralAnimHelper.position,
            transform.position + Vector3.forward * 2, Time.deltaTime * rotationAnimationSpeed * Time.deltaTime * rotationAnimationSpeed);
        
        Vector3 ParentToHelperVec = (cannonProceduralAnimHelper.position - cannonModelParentTr.position).normalized;
        ParentToHelperVec.y = 0;
        Debug.DrawRay(cannonModelParentTr.position, ParentToHelperVec, Color.magenta);
        cannonModelParentTr.forward = ParentToHelperVec.normalized;
    }

    public void ShowChampionReleaseIndicators()
    {
        if(championIndicatorParticle.isPlaying == false)
            championIndicatorParticle.Play();
        cannonRenderer.sharedMaterial = cannonReadyToShootChampionMaterial;
    }

    public void HideChampionReleaseIndicators()
    {
        championIndicatorParticle.Stop();
        cannonRenderer.sharedMaterial = originalMat;
    }

    public void Shoot()
    {
        animator.CrossFadeInFixedTime("idle", 0f);
        animator.CrossFadeInFixedTime(shootTriggerIndex, 0f);
    }

    private void OnValidate()
    {
        cannonRenderer.sharedMaterial = originalMat;
    }
}