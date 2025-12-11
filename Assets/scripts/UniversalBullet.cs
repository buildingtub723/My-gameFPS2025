using UnityEngine;

public class UniversalBullet : MonoBehaviour
{
    public float damage = 25f;
    public float lifeTime = 5f;
    public float speed = 20f;
    public Team shooterTeam;
    public string weaponType;

    void Start()
    {
        // Set bullet forward velocity
        Rigidbody rb = GetComponent<Rigidbody>();

        
        if (rb != null && (weaponType == "Shotgun" || weaponType == "Rifle"))
        {
            rb.linearVelocity = transform.forward * speed; // Unity 6 correct usage
        }
        else if (rb != null && weaponType == "Grenade")
        {
            Vector3 shootDir = (transform.forward + Vector3.up * 0.25f).normalized;
            rb.linearVelocity = shootDir * (speed / 2f);
        }

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet")) return;

        var damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}