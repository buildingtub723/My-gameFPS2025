using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    void OnEnable()
    {
        if (SpawnManager.Instance != null)
            SpawnManager.Instance.RegisterSpawner(this);
    }

    void OnDisable()
    {
        if (SpawnManager.Instance != null)
            SpawnManager.Instance.UnregisterSpawner(this);
    }

    public void SpawnEnemy(GameObject prefab)
    {
        if (prefab != null)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
            Debug.Log("Enemy spawned at: " + transform.position);
        }
        else
        {
            Debug.LogError("No prefab provided to EnemySpawner!");
        }
    }
}
