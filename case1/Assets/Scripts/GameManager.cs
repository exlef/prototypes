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
    [SerializeField] Character mobNormie;
    [SerializeField] Character enemyNormie;
    [SerializeField] GameObject victoryScreen;
    [SerializeField] GameObject failedScreen;

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
        Character mob = Instantiate(mobNormie, cannon.spawnPos, Quaternion.identity);
        mob.Init(tower.transform.position, false, null);
    }

    public void SpawnMobOnDoorCollision(int multiplier, MultiplierDoor door)
    {
        for (int i = 0; i < multiplier -1 ; i++)
        {
            Character mob = Instantiate(mobNormie, door.transform.position, Quaternion.identity);
            mob.Init(tower.transform.position, false, door);
        }
    }

    public void MobReachedTower(Character mob)
    {
        tower.GotDamage(1);
        Destroy(mob.gameObject); 
    }

    public void OnTowerDefeated()
    {
        pause = true;
        victoryScreen.gameObject.SetActive(true);
    }

    IEnumerator EnemySpawnRoutine()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            yield return new WaitForSeconds(waves[i].timeout);
            SpawnEnemyAtTower(waves[i].enemyCount);
        }
    }

    void SpawnEnemyAtTower(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Character character = Instantiate(enemyNormie, tower.spawnPoint.position, tower.spawnPoint.rotation);
            character.Init(cannon.transform.position, true, null);    
        }    
    }

    public void EnemyReachedCannon()
    {
        pause = true;
        failedScreen.SetActive(true);
    }
}

[Serializable]
public struct Wave
{
    public float timeout;
    public int enemyCount;
}