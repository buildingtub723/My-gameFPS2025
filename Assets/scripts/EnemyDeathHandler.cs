using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour, IDeathHandler
{
    private bool hasDied = false;

    public void HandleDeath(GameObject instigator)
    {
        if (hasDied) return;
        hasDied = true;

        Debug.Log($"{gameObject.name} was killed by {instigator.name}");

        // Stop AI behavior if needed
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.Die(); // You can define this to stop movement/shooting/etc.
        }

        // Play death animation
        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Play death sound
        EnemyAudioHandler audioHandler = GetComponent<EnemyAudioHandler>();
        if (audioHandler != null)
        {
            audioHandler.PlayDeathSound();
            audioHandler.TryDetachAudio(); // Detaches audio source so it can finish playing
        }

        // Drop loot once
        TryDropLoot();

        // Destroy the whole enemy after a delay (give time for audio/animation)
        Destroy(gameObject, 3f);
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

