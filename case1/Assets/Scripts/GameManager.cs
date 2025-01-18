using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("Cannon")]
    [SerializeField] [Min(1f)] float cannonMoveSpeed = 1;
    [Tooltip("how far cannon can move left and right")]
    [SerializeField] float cannonMoveLimit = 3f; // TODO: make it range(0,1) this will 1 will let cannon moves all the way to edge of the screen
    [SerializeField] float cannonShootInterval = 1f;

    [Space]
    [SerializeField] [Min(1)] int towerHealth = 1;

    [Space]
    [SerializeField] Wave[] waves;

    [Header("References")]
    [SerializeField] Cannon cannon;
    [SerializeField] Tower tower;
    [SerializeField] Mob mobNormie;
    [SerializeField] Mob enemyNormie;

    public static GameManager instance;
    private bool playerTouching;
    private float cannonShootTimer;
    private bool pause;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        tower.Init(towerHealth);
        StartCoroutine(EnemySpawnRoutine());
    }

    void Update()
    {
        if (pause) return;
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
        Mob mob = Instantiate(mobNormie, cannon.spawnPos, Quaternion.identity);
        mob.Init(tower.transform.position);
    }

    public void SpawnMobOnDoorCollision(int multiplier, MultiplierDoor door)
    {
        for (int i = 0; i < multiplier; i++)
        {
            Mob mob = Instantiate(mobNormie, door.transform.position, Quaternion.identity);
            mob.Init(tower.transform.position, door);
        }
    }

    public void MobReachedTower(Mob mob)
    {
        tower.GotDamage(1);
        Destroy(mob.gameObject); 
    }

    public void OnTowerDefeated()
    {
        Debug.Log("defeat");
        pause = true;
    }

    IEnumerator EnemySpawnRoutine()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            yield return new WaitForSeconds(waves[i].timeout);
            SpawnEnemyMobAtTower(waves[i].enemyCount);
        }
    }

    void SpawnEnemyMobAtTower(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Mob mob = Instantiate(enemyNormie, tower.SpawnPos, Quaternion.identity);
            mob.Init(cannon.transform.position, true);    
        }    
    }
}

[Serializable]
public struct Wave
{
    public float timeout;
    public int enemyCount;
}