using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("Cannon")]
    [SerializeField] [Min(1f)] float cannonMoveSpeed = 1;
    [Tooltip("how far cannon can move left and right")]
    [SerializeField] float cannonMoveLimit = 3f;
    [SerializeField] float cannonShootInterval = 1f;
    [Space] 
    [Header("Champion")]
    [Tooltip("the number of normie mobs that needs to be spawn from cannon to be able to release a champion")]
    [SerializeField] int spawnCountToReleaseChampion = 4;
    [SerializeField] [Min(1)] int championTowerDamageCount = 4;
    [Tooltip("the time in seconds that champion will wait before damage the tower again.")]
    [SerializeField] [Min(0f)] float championTowerDamageInterval = 0.2f;

    [Space]
    [Header("Heath values")]
    [Tooltip("the health values are the number of normie(s) that takes to be dead")]
    [Min(1)] public int towerHealth = 1;
    [Tooltip("the health values are the number of normie(s) that takes to be dead")]
    [Min(1)] public int bigEnemyHealth = 4;
    [Tooltip("the health values are the number of normie(s) that takes to be dead")]
    [Min(1)] public int championHealth = 4;
    
    [Space]
    [Header("Speed values")]
    public float mobNormieSpeed = 3f;
    public float championSpeed = 3f;
    public float enemyNormieSpeed = 3f;
    public float bigEnemySpeed = 3f;
    
    [Space]
    [Header("Weight values")]
    public float mobNormieWeight = 1f;
    public float championWeight = 2f;
    public float enemyNormieWeight = 1f;
    public float bigEnemyWeight = 4f;

    [Space] public int pushBoxWeight = 100;

    [Space]
    [SerializeField] Wave[] waves;

    [Header("References")]
    [SerializeField] Cannon cannon;
    [SerializeField] Tower tower;
    public Character mobNormiePrefab;
    public Character mobChampionPrefab;
    public Character enemyNormiePrefab;
    public Character enemyBigPrefab;
    [SerializeField] GameObject victoryScreen;
    [SerializeField] GameObject failedScreen;
    [SerializeField] ChampionSlider championSlider;
    [SerializeField] Transform mobLevelPathsParentTr;
    [SerializeField] Transform enemyLevelPathsParentTr;
    [SerializeField] Transform staticLevelWallsParentTr;
    public PushBox pushBox;
    [SerializeField] PhysicsHandler physicsHandler;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip cannonShootSfx;
    [SerializeField] AudioClip championReleasedSfx;
    [SerializeField] CharPooler charPooler;
    
    public static GameManager instance;
    public List<Character> mobs = new List<Character>();
    public List<Character> enemies = new List<Character>();
    public StaticWall[] staticLevelWalls;
    public const int normieHealth = 1;
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
        charPooler.Init();
        tower.Init(towerHealth);
        mobLevelPaths = mobLevelPathsParentTr.GetComponentsInChildren<LevelPath>(false);
        staticLevelWalls = staticLevelWallsParentTr.GetComponentsInChildren<StaticWall>(false);
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
            PlaySound(cannonShootSfx);
            cannon.Shoot();
        }

        if (spawnCounter.IsFull()) cannon.ShowChampionReleaseIndicators();
        else cannon.HideChampionReleaseIndicators();
        
        physicsHandler.Tick();
    }

    public void SpawnNormieMobOnCannonFire()
    {
        spawnCounter.Set(spawnCounter.Get() + 1, championSlider);
        // Character mob = Instantiate(mobNormiePrefab, cannon.spawnPos, Quaternion.identity);
        Character mob = charPooler.GetChar(mobNormiePrefab, cannon.spawnPos, Quaternion.identity);
        mob.Init(GetClosestPath(cannon.transform.position), 0, CharacterType.normie, null);
        mobs.Add(mob);
    }

    public void SpawnMobOnDoorCollision(int multiplier, Character originalMob, MultiplierDoor door)
    {
        for (int i = 0; i < multiplier -1 ; i++)
        {
            Character prefab = originalMob.charType switch
            {
                CharacterType.champion => mobChampionPrefab,
                CharacterType.normie => mobNormiePrefab,
                _ => null
            };
            if (prefab == null)
            {
                Debug.LogError("There shouldn't be any other character types that try to spawn at doors.");
                return;
            }
            // Character mob = Instantiate(prefab, originalMob.transform.position, originalMob.transform.rotation);
            Character mob = charPooler.GetChar(prefab, originalMob.transform.position, originalMob.transform.rotation);
            mob.Init(originalMob.path, originalMob.pathPointIndex, prefab.charType, door);
            mobs.Add(mob);
        }
    }

    public void MobReachedTower(Character mob)
    {
        switch (mob.charType)
        {
            case CharacterType.normie:
                tower.TryDamage(normieHealth);
                mob.GotDamage(normieHealth);
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
        Time.timeScale = 0.1f;
        failedScreen.SetActive(true);
    }

    public void OnTowerDefeated()
    {
        pause = true;
        victoryScreen.gameObject.SetActive(true);
    }

    public void OnCharactersDeath(Character c)
    {
        switch (c.charType)
        {
            case CharacterType.normie:
            case CharacterType.champion:
                mobs.Remove(c);
                break;
            case CharacterType.enemyNormie:
            case CharacterType.enemyBig:
                enemies.Remove(c);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        charPooler.DestroyChar(c);
    }

    public float GetWeightValue(Character c)
    {
        return c.charType switch
        {
            CharacterType.normie => mobNormieWeight,
            CharacterType.champion => championWeight,
            CharacterType.enemyNormie => enemyNormieWeight,
            CharacterType.enemyBig => bigEnemyWeight,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    void TrySpawnChampionFromCannon()
    {
        if (spawnCounter.Get() < spawnCountToReleaseChampion) return;
        spawnCounter.Set(0, championSlider);
        // Character mob = Instantiate(mobChampionPrefab, cannon.spawnPos, Quaternion.identity);
        Character mob = charPooler.GetChar(mobChampionPrefab, cannon.spawnPos, Quaternion.identity);
        mob.Init(GetClosestPath(cannon.transform.position), 0, CharacterType.champion, null);
        mobs.Add(mob);
        PlaySound(championReleasedSfx);
    }
    
    IEnumerator SpawnEnemyAtTowerCo(int count, LevelPath levelPath, Character prefab)
    {
        for (int i = 0; i < count; i++)
        {
            // Character enemy = Instantiate(prefab, tower.spawnPoint.position, tower.spawnPoint.rotation);
            Character enemy = charPooler.GetChar(prefab, tower.spawnPoint.position, tower.spawnPoint.rotation);
            enemy.Init(levelPath, 0, prefab.charType, null);
            enemies.Add(enemy);
            tower.Shake();
            yield return new WaitForEndOfFrame();
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

    void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    IEnumerator EnemySpawnRoutine()
    {
        while (true)
        {
            for (int i = 0; i < waves.Length; i++)
            {
                yield return new WaitForSeconds(waves[i].timeout);
                StartCoroutine(SpawnEnemyAtTowerCo(waves[i].normieEnemyCount, waves[i].levelPath, enemyNormiePrefab));
                StartCoroutine(SpawnEnemyAtTowerCo(waves[i].bigEnemyCount, waves[i].levelPath, enemyBigPrefab));
            }
        }
    }
    
    IEnumerator MobChampionTowerDamageRoutine(Character champion)
    {
        for (int i = 0; i < championTowerDamageCount; i++)
        {
            tower.TryDamage(1);
            yield return championTowerDamageWait;
            if(champion.gameObject.activeInHierarchy) yield break; // champion can be killed by other causes while damaging the tower so we need to check. 
        }
        // if(champion.gameObject.activeInHierarchy) yield break;
        champion.GotDamage(championHealth);
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

        public bool IsFull()
        {
            return (float)counter / maxNumber >= 1f;
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