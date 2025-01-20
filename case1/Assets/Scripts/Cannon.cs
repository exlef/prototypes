using System;
using System.Collections;
using UnityEngine;

class Cannon : MonoBehaviour
{
    public Vector3 spawnPos => spawnPoint.position;

    [SerializeField] Animator animator;
    [SerializeField] Transform spawnPoint;

    [SerializeField] private ParticleSystem championIndicatorParticle;
    [SerializeField] Material originalMat;
    [SerializeField] Material cannonReadyToShootChampionMaterial;
    [SerializeField] SkinnedMeshRenderer cannonRenderer;
    
    static readonly int shootTriggerIndex = Animator.StringToHash("shoot");

    public void Move(float x, float limit)
    {
        Vector3 pos = transform.position + new Vector3(x, 0, 0);
        pos.x = Mathf.Clamp(pos.x, pos.x - limit, pos.x + limit);
        transform.position = pos;
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