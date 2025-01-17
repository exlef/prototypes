using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] [Min(1f)] float cannonMoveSpeed = 1;
    [Tooltip("how far cannon can move left and right")]
    [SerializeField] float cannonMoveLimit = 3f; // TODO: make it range(0,1) this will 1 will let cannon moves all the way to edge of the screen
    [SerializeField] float cannonShootInterval = 1f;

    [Header("References")]
    [SerializeField] Cannon cannon;
    [SerializeField] Mob mobPrefab;

    public static GameManager instance;
    private bool playerTouching;
    private float cannonShootTimer;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            playerTouching = true;
            var inputDelta= Input.mousePositionDelta;
            var inputNormalized = inputDelta.x / Screen.width;
            cannon.Move(inputNormalized * cannonMoveSpeed, cannonMoveLimit);
        }
        else
        {
            playerTouching = false;
        }

        if (playerTouching) cannonShootTimer += Time.deltaTime;

        if (playerTouching && cannonShootTimer >= cannonShootInterval)
        {
            cannonShootTimer -= cannonShootTimer;
            cannon.Shoot();
        }
    }

    public void SpawnMobOnCannonFire()
    {
        Mob mob = Instantiate(mobPrefab, cannon.spawnPos, Quaternion.identity);
        mob.Init(Vector3.forward * 10);
    }
}