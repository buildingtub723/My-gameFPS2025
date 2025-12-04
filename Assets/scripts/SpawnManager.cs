using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Threading;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Spawner Setup")]
    public List<EnemySpawner> spawners = new List<EnemySpawner>();
    public GameObject enemyPrefab;

    [Header("Wave Settings")]
    public int startingEnemiesPerWave = 5;
    public float spawnDelayBetweenEnemies = 1f;
    public float timeBetweenWaves = 5f;
    public float difficultyIncreaseRate = 1.2f;

    [Header("Level Timer")]
    public float levelDuration = 180f; // 3 minutes in seconds

    public TMP_Text timerText;

    private int currentWave = 0;
    private bool isSpawning = false;
    private int activeEnemies = 0;
    private float levelTimeRemaining;

    public int CurrentWave => currentWave;
    public float TimeUntilNextWave { get; private set; } = 0f;
    public float LevelTimeRemaining => Mathf.Max(0f, levelTimeRemaining);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        RegisterAllSpawners();
        levelTimeRemaining = levelDuration;
        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        // Countdown the level timer
        if (levelTimeRemaining > 0f)
        {
            levelTimeRemaining -= Time.deltaTime;

            timerText.text = $"Time Left: {Mathf.CeilToInt(levelTimeRemaining)}s";
        }
    }

    public void RegisterSpawner(EnemySpawner spawner)
    {
        if (!spawners.Contains(spawner))
            spawners.Add(spawner);
    }

    public void UnregisterSpawner(EnemySpawner spawner)
    {
        spawners.Remove(spawner);
    }

    public void RegisterAllSpawners()
    {
        spawners.Clear();
        spawners.AddRange(FindObjectsOfType<EnemySpawner>());
    }

    private IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(2f);

        while (levelTimeRemaining > 0f) // Stop when level timer runs out
        {
            currentWave++;
            int enemiesThisWave = Mathf.RoundToInt(startingEnemiesPerWave * Mathf.Pow(difficultyIncreaseRate, currentWave - 1));
            Debug.Log($"Starting Wave {currentWave} with {enemiesThisWave} enemies");

            yield return StartCoroutine(SpawnWave(enemiesThisWave));

            // Wait until all enemies are destroyed before next wave
            while (activeEnemies > 0 && levelTimeRemaining > 0f)
                yield return null;

            // Countdown to next wave
            for (float t = timeBetweenWaves; t > 0 && levelTimeRemaining > 0f; t -= Time.deltaTime)
            {
                TimeUntilNextWave = t;
                yield return null;
            }
            TimeUntilNextWave = 0f;
        }

        Debug.Log("Level complete! No more waves.");
    }

    private IEnumerator SpawnWave(int enemyCount)
    {
        isSpawning = true;

        for (int i = 0; i < enemyCount && levelTimeRemaining > 0f; i++)
        {
            SpawnFromRandomSpawner();
            yield return new WaitForSeconds(spawnDelayBetweenEnemies);
        }

        isSpawning = false;
    }

    private void SpawnFromRandomSpawner()
    {
        if (spawners.Count == 0 || enemyPrefab == null)
        {
            Debug.LogError("No spawners or enemy prefab assigned!");
            return;
        }

        EnemySpawner chosenSpawner = spawners[Random.Range(0, spawners.Count)];
        chosenSpawner.SpawnEnemy(enemyPrefab);
        activeEnemies++;
    }

    public void EnemyDestroyed()
    {
        activeEnemies = Mathf.Max(0, activeEnemies - 1);
    }
}
