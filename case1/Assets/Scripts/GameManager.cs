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
    [Tooltip("the number of normie mobs that needs to be spawn from cannon to be able to release a champion")]
    [SerializeField] int spawnCountToReleaseChampion = 4;

    [Space]
    [SerializeField] [Min(1)] int towerHealth = 1;

    [Space]
    [SerializeField] Wave[] waves;

    [Header("References")]
    [SerializeField] Cannon cannon;
    [SerializeField] Tower tower;
    [SerializeField] Character mobNormiePrefab;
    [FormerlySerializedAs("mobBigPrefab")] [SerializeField] Character mobChampionPrefab;
    [SerializeField] Character enemyNormiePrefab;
    [SerializeField] Character enemyBigPrefab;
    [SerializeField] GameObject victoryScreen;
    [SerializeField] GameObject failedScreen;

    public static GameManager instance;
    bool playerTouching;
    float cannonShootTimer;
    bool pause; 
    int spawnCounter; 

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
            TrySpawnChampion();
        }

        if (playerTouching) cannonShootTimer += Time.deltaTime;

        if (playerTouching && cannonShootTimer >= cannonShootInterval)
        {
            cannonShootTimer -= cannonShootTimer;
            cannon.Shoot();
        }
    }

    void TrySpawnChampion()
    {
        if (spawnCounter < spawnCountToReleaseChampion) return;
        spawnCounter = 0;
        Character mob = Instantiate(mobChampionPrefab, cannon.spawnPos, Quaternion.identity);
        mob.Init(tower.transform.position, false, null);
    }

    public void SpawnMobOnCannonFire()
    {
        spawnCounter++;
        Character mob = Instantiate(mobNormiePrefab, cannon.spawnPos, Quaternion.identity);
        mob.Init(tower.transform.position, false, null);
    }

    public void SpawnMobOnDoorCollision(int multiplier, MultiplierDoor door)
    {
        for (int i = 0; i < multiplier -1 ; i++)
        {
            Character mob = Instantiate(mobNormiePrefab, door.transform.position, Quaternion.identity);
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
            SpawnEnemyAtTower(waves[i].normieEnemyCount, enemyNormiePrefab);
            SpawnEnemyAtTower(waves[i].bigEnemyCount, enemyBigPrefab);
        }
    }

    void SpawnEnemyAtTower(int count, Character prefab)
    {
        for (int i = 0; i < count; i++)
        {
            Character character = Instantiate(prefab, tower.spawnPoint.position, tower.spawnPoint.rotation);
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
    public int normieEnemyCount;
    public int bigEnemyCount;
}