using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [Tooltip("What this enemy will drop on death (can be ammo box, weapon, etc.)")]
    public GameObject[] lootPrefabs;

    [Tooltip("Drop chance for each loot (0 to 1)")]
    public float[] dropChances;
}