using UnityEngine;

public class BarrelExplosionHandler : MonoBehaviour, IDeathHandler
{
    public GameObject explosionPrefab;
    public float explosionRadius = 5f;
    public float explosionDamage = 50f;

    public void HandleDeath(GameObject owner)
    {
        // Instantiate explosion effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, owner.transform.position, Quaternion.identity);
        }

        // Deal damage in radius
        Collider[] hits = Physics.OverlapSphere(owner.transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            IDamageable dmg = hit.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(explosionDamage);
            }
        }

        Destroy(owner);
    }
}