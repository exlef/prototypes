using System;
using UnityEngine;
using TMPro;

public class MultiplierDoor : MonoBehaviour
{
    [SerializeField] [Min(1)] int multiplier = 2;
    [SerializeField] [Min(0)] float leftMoveAmount;
    [SerializeField] [Min(0)] float rightMoveAmount;
    [SerializeField] [Min(0)] float speed = 1f;
    [SerializeField] TextMeshPro multiplierText;
    private Vector3 aPos;
    private Vector3 bPos;
    private float t;
    private int direction = 1;

    private void Start()
    {
        t = Mathf.InverseLerp(-leftMoveAmount, rightMoveAmount, transform.position.x);
        aPos = transform.position + Vector3.left * leftMoveAmount;
        bPos = transform.position + Vector3.right * rightMoveAmount;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(aPos, bPos, t);
        t += Time.deltaTime * speed * direction;
        // to prevent t exceeds valid 01 range otherwise we will have issues since unity's lerp function is clamped  
        t = Mathf.Clamp01(t);

        if (t >= 1f || t <= 0f)
        {
            direction *= -1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character mob))
        {
            switch (mob.charType)
            {
                case CharacterType.normie:
                case CharacterType.champion:
                    if (mob.door == this) return; // because this mob already passed or has spawned by this door
                    mob.door = this;
                    GameManager.instance.SpawnMobOnDoorCollision(multiplier, mob, this);
                    break;
                case CharacterType.enemyNormie:
                case CharacterType.enemyBig:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // if(mob.isEnemy) return;
            // if (mob.door == this) return; // because this mob already passed or has spawned by this door
            // mob.door = this;
            // GameManager.instance.SpawnMobOnDoorCollision(multiplier, mob, this);
        }
    }

    private void OnValidate()
    {
        multiplierText.text = $"x{multiplier}";
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;
        var aPosGizmo = transform.position + Vector3.left * leftMoveAmount;
        var bPosGizmo = transform.position + Vector3.right * rightMoveAmount;
        
        Gizmos.DrawSphere(aPosGizmo, 0.2f);
        Gizmos.DrawSphere(bPosGizmo, 0.2f);
        Gizmos.DrawLine(aPosGizmo, bPosGizmo);
    }
}
