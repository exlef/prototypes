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
    [SerializeField] Transform cannonModelParentTr;
    
    static readonly int shootTriggerIndex = Animator.StringToHash("shoot");
    
    [Header("Rotation Settings")]
    [SerializeField] private float maxRotationAngle = 15f; // Maximum rotation angle when moving
    [SerializeField] private float rotationSpeed = 5f; // How fast the rotation occurs
    [SerializeField] private float returnSpeed = 3f; // How fast it returns to neutral position

    private float targetRotation = 0f;
    private float currentVelocity = 0f; // Used for SmoothDamp
    private float lastMoveInput = 0f;

    public void Move(float x, float limit)
    {
        lastMoveInput = x; // Store the input for rotation calculation
        
        Vector3 pos = transform.position + new Vector3(x, 0, 0);
        pos.x = Mathf.Clamp(pos.x, pos.x - limit, pos.x + limit);
        transform.position = pos;
        
        targetRotation = -x * maxRotationAngle; // Negative to rotate in the correct direction
    }
    
    private void Update()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        // Get current rotation and normalize it
        float currentRotation = cannonModelParentTr.localEulerAngles.y;
        if (currentRotation > 180f) 
            currentRotation -= 360f;

        float targetAngle;
        float smoothTime;

        // If we have movement input
        if (!Mathf.Approximately(lastMoveInput, 0f))
        {
            targetAngle = targetRotation;
            smoothTime = 1f / rotationSpeed;
        }
        else
        {
            targetAngle = 0f;
            smoothTime = 1f / returnSpeed;
        }

        // Calculate new rotation
        float newRotation = Mathf.SmoothDamp(currentRotation, targetAngle, ref currentVelocity, smoothTime);
        
        // Apply rotation
        cannonModelParentTr.localEulerAngles = new Vector3(0f, newRotation, 0f);


        // Only reset lastMoveInput, keep targetRotation for smooth transitions
        lastMoveInput = 0f;
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