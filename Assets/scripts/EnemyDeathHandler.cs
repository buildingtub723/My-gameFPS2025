using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour, IDeathHandler
{
    public void HandleDeath(GameObject owner)
    {
        Debug.Log($"{gameObject.name} was killed by {owner.name}");

        TryDropLoot();

        // Destroy this enemy object
        Destroy(gameObject);
    }

    private void TryDropLoot()
    {
        EnemyLoot loot = GetComponent<EnemyLoot>();
        if (loot == null || loot.lootPrefabs == null) return;

        for (int i = 0; i < loot.lootPrefabs.Length; i++)
        {
            if (loot.lootPrefabs[i] == null) continue;

            float chance = (i < loot.dropChances.Length) ? loot.dropChances[i] : 1f;
            if (Random.value <= chance)
            {
                Instantiate(loot.lootPrefabs[i], transform.position + Vector3.up * 0.5f, Quaternion.identity);
            }
        }
    }
}

