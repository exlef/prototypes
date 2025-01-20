using System;
using System.Collections;
using UnityEngine;

class Cannon : MonoBehaviour
{
    public Vector3 spawnPos => spawnPoint.position;

    [SerializeField] Animator animator;
    [SerializeField] Transform spawnPoint;
    [SerializeField] private float rotationAnimationSpeed = 5f;
    [SerializeField] ParticleSystem championIndicatorParticle;
    [SerializeField] Material originalMat;
    [SerializeField] Material cannonReadyToShootChampionMaterial;
    [SerializeField] SkinnedMeshRenderer cannonRenderer;
    [SerializeField] Transform cannonModelParentTr;
    [SerializeField] Transform cannonProceduralAnimHelper;
    
    static readonly int shootTriggerIndex = Animator.StringToHash("shoot");
    
    [Header("Rotation Settings")]
    [SerializeField] private float maxRotationAngle = 15f; // Maximum rotation angle when moving
    [SerializeField] private float rotationSpeed = 5f; // How fast the rotation occurs
    [SerializeField] private float returnSpeed = 3f; // How fast it returns to neutral position

    private float lastMoveInput = 0f;

    public void Move(float x, float limit)
    {
        lastMoveInput = x; // Store the input for rotation calculation
        
        Vector3 pos = transform.position + new Vector3(x, 0, 0);
        pos.x = Mathf.Clamp(pos.x,  -limit,  limit);
        transform.position = pos;
        
        // targetRotation = -x * maxRotationAngle; // Negative to rotate in the correct direction
        Vector3 dir = Quaternion.Euler(0, -x * 100, 0) * cannonModelParentTr.forward;
        cannonModelParentTr.transform.forward = Vector3.Slerp(cannonModelParentTr.transform.forward, dir, Time.deltaTime);
    }
    
    private void Update()
    {
        cannonProceduralAnimHelper.position = Vector3.Lerp(cannonProceduralAnimHelper.position,
            transform.position + Vector3.forward * 2, (Time.deltaTime * rotationAnimationSpeed * rotationAnimationSpeed));
        
        Vector3 ParentToHelperVec = (cannonProceduralAnimHelper.position - cannonModelParentTr.position).normalized;
        ParentToHelperVec.y = 0;
        Debug.DrawRay(cannonModelParentTr.position, ParentToHelperVec, Color.magenta);
        cannonModelParentTr.forward = ParentToHelperVec.normalized;

        // cannonModelParentTr.forward = dir;
        // UpdateRotation();

        // Vector3 dir = Quaternion.Euler(0, -lastMoveInput * 100, 0) * cannonModelParentTr.forward;
        // cannonModelParentTr.transform.forward = Vector3.Slerp(cannonModelParentTr.transform.forward, dir, Time.deltaTime);
        // lastMoveInput = 0;
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