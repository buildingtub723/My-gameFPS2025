using UnityEngine;

public class HealthRegen : MonoBehaviour
{
    public float regenRate = 5f;
    public float regenCooldown = 3f;

    private Health health;
    private float lastDamageTime;

    void Awake()
    {
        health = GetComponent<Health>();
        if (health != null)
        {
            health.OnDamageTaken += (_) => lastDamageTime = Time.time;
        }
    }

    void Update()
    {
        if (health == null || health.currentHealth >= health.maxHealth) return;

        if (Time.time >= lastDamageTime + regenCooldown)
        {
            health.Heal(regenRate * Time.deltaTime);
        }
    }
}
