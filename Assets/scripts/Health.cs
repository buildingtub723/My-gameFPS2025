using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    private IDeathHandler deathHandler;

    // Damage event
    public delegate void OnDamaged(float amount);
    public event OnDamaged OnDamageTaken;

    // Death event
    public delegate void OnDied();
    public event OnDied OnDeath;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        deathHandler = GetComponent<IDeathHandler>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        OnDamageTaken?.Invoke(amount);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        currentHealth = 0f;
        OnDeath?.Invoke(); // notify listeners (audio, fx, etc.)
        deathHandler?.HandleDeath(gameObject);
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
}
