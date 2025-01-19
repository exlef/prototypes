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
    [SerializeField] [Min(1)] int championTowerDamageCount = 4;
    [Tooltip("the time in seconds that champion will wait before damage the tower again.")]
    [SerializeField] [Min(0f)] float championTowerDamageInterval = 0.2f;

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
    [SerializeField] ChampionSlider championSlider;
    [FormerlySerializedAs("levelPathsParentTr")] [SerializeField] Transform mobLevelPathsParentTr;
    [SerializeField] Transform enemyLevelPathsParentTr;
    
    public static GameManager instance;
    bool playerTouching;
    float cannonShootTimer;
    bool pause;
    private SpawnCounter spawnCounter; 
    WaitForSeconds  championTowerDamageWait;
    LevelPath[] mobLevelPaths;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        tower.Init(towerHealth);
        mobLevelPaths = mobLevelPathsParentTr.GetComponentsInChildren<LevelPath>(false);
        StartCoroutine(EnemySpawnRoutine());
        championTowerDamageWait = new WaitForSeconds(championTowerDamageInterval);
        championSlider.Init(Camera.main, cannon.transform);
        spawnCounter = new SpawnCounter(0, spawnCountToReleaseChampion);
    }

    void Update()
    {
        if (pause) return;
        if (Input.GetMouseButton(0))
        {
            playerTouching = true;
            cannonShootTimer += Time.deltaTime;
            championSlider.Show();
            var inputDelta= Input.mousePositionDelta;
            var inputNormalized = inputDelta.x / Screen.width;
            cannon.Move(inputNormalized * cannonMoveSpeed, cannonMoveLimit);
        }
        else
        {
            playerTouching = false;
            cannonShootTimer -= cannonShootTimer;
            championSlider.Hide();
            TrySpawnChampionFromCannon();
        }

        if (playerTouching && cannonShootTimer >= cannonShootInterval)
        {
            cannonShootTimer -= cannonShootTimer;
            cannon.Shoot();
        }
    }

    public void SpawnNormieMobOnCannonFire()
    {
        spawnCounter.Set(spawnCounter.Get() + 1, championSlider);
        Character mob = Instantiate(mobNormiePrefab, cannon.spawnPos, Quaternion.identity);
        mob.Init(GetClosestPath(cannon.transform.position), 0, CharacterType.normie, null);
    }

    public void SpawnMobOnDoorCollision(int multiplier, Character originalMob, MultiplierDoor door)
    {
        for (int i = 0; i < multiplier -1 ; i++)
        {
            if (originalMob.charType == CharacterType.champion)
            {
                Character mob = Instantiate(mobChampionPrefab, door.transform.position, Quaternion.identity); // TODO: mobs should spawn at the original mobs position not the door
                mob.Init(originalMob.path, originalMob.pathPointIndex, CharacterType.champion, door); // TODO: char type should be original mob.charType 

            }
            else if (originalMob.charType == CharacterType.normie)
            {
                Character mob = Instantiate(mobNormiePrefab, door.transform.position, Quaternion.identity);
                mob.Init(originalMob.path, originalMob.pathPointIndex, CharacterType.normie, door);

            }
        }
    }

    public void MobReachedTower(Character mob)
    {
        switch (mob.charType)
        {
            case CharacterType.normie:
                tower.TryDamage(1);
                Destroy(mob.gameObject); 
                break;
            case CharacterType.champion:
                StartCoroutine(MobChampionTowerDamageRoutine(mob));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mob.charType), mob.charType, null);
        }
    }

    public void EnemyReachedCannon()
    {
        pause = true;
        failedScreen.SetActive(true);
    }

    public void OnTowerDefeated()
    {
        pause = true;
        victoryScreen.gameObject.SetActive(true);
    }

    void TrySpawnChampionFromCannon()
    {
        if (spawnCounter.Get() < spawnCountToReleaseChampion) return;
        spawnCounter.Set(0, championSlider);
        Character mob = Instantiate(mobChampionPrefab, cannon.spawnPos, Quaternion.identity);
        mob.Init(GetClosestPath(cannon.transform.position), 0, CharacterType.champion, null);
    }
    
    void SpawnEnemyAtTower(int count, LevelPath levelPath, Character prefab)
    {
        for (int i = 0; i < count; i++)
        {
            Character character = Instantiate(prefab, tower.spawnPoint.position, tower.spawnPoint.rotation);
            character.Init(levelPath, 0, prefab.charType, null); // todo: path will be determined in waves by designer
        }    
    }
    
    LevelPath GetClosestPath(Vector3 cannonPos)
    {
        LevelPath closestPath = mobLevelPaths[0];
        float closestDistance = float.MaxValue;

        foreach (LevelPath path in mobLevelPaths)
        {
            if (path.points.Count > 0)
            {
                // Compare only the x-axis of the first point
                float distance = Mathf.Abs(path.points[0].position.x - cannonPos.x);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPath = path;
                }
            }
        }

        return closestPath;
    }

    IEnumerator EnemySpawnRoutine()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            yield return new WaitForSeconds(waves[i].timeout);
            SpawnEnemyAtTower(waves[i].normieEnemyCount, waves[i].levelPath, enemyNormiePrefab);
            SpawnEnemyAtTower(waves[i].bigEnemyCount, waves[i].levelPath, enemyBigPrefab);
        }
    }
    
    IEnumerator MobChampionTowerDamageRoutine(Character champion)
    {
        for (int i = 0; i < championTowerDamageCount; i++)
        {
            tower.TryDamage(1);
            yield return championTowerDamageWait;
        }

        Destroy(champion.gameObject);
    }
    
    struct SpawnCounter
    {
        private int counter;
        private readonly int maxNumber;
        
        public SpawnCounter(int counter, int maxNumber)
        {
            this.counter = counter;
            this.maxNumber = maxNumber;
        }
        
        public void Set(int _counter, ChampionSlider slider)
        {
            counter = _counter;
            var value = (float)counter / maxNumber;
            slider.UpdateSlider(value);
        }

        public int Get()
        {
            return counter;
        }
    }
}

[Serializable]
public struct Wave
{
    public float timeout;
    public int normieEnemyCount;
    public int bigEnemyCount;
    public LevelPath levelPath;
}