using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth;

    private IDeathHandler deathHandler;

    public delegate void OnDamaged(float amount);
    public event OnDamaged OnDamageTaken;

    private void Awake()
    {
        currentHealth = maxHealth;
        deathHandler = GetComponent<IDeathHandler>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        OnDamageTaken?.Invoke(amount);

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            deathHandler?.HandleDeath(gameObject);
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
}
