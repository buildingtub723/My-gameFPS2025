using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // The prefab of the enemy GameObject to spawn.
    // Assign this in the Inspector.
    public GameObject enemyPrefab;

    // The time in seconds between each enemy spawn.
    public float spawnInterval = 3f;

    // The position where enemies will be spawned.
    // If not set, enemies will spawn at the spawner's GameObject position.
    public Vector3 spawnPositionOffset = Vector3.zero;

    void Start()
    {
        // Check if an enemy prefab is assigned to prevent errors.
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned to the EnemySpawner! Please assign a GameObject prefab in the Inspector.");
            return;
        }

        // Begin spawning enemies repeatedly after an initial delay equal to the interval.
        InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
    }

    /// <summary>
    /// Instantiates an enemy at the calculated spawn position.
    /// </summary>
    void SpawnEnemy()
    {
        // Calculate the world position where the enemy will spawn.
        // This is the spawner's position plus the defined offset.
        Vector3 spawnPoint = transform.position + spawnPositionOffset;

        // Instantiate the enemy prefab at the calculated spawn point with no rotation.
        Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
        Debug.Log($"Spawned enemy at {spawnPoint}");
    }
}
