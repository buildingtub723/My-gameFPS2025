using UnityEngine;

public class UniversalBullet : MonoBehaviour
{
    public float damage = 25f;
    public float lifeTime = 5f;
    public float speed = 20f;
    public Team shooterTeam;

    void Start()
    {
        // Set bullet forward velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed; // Unity 6 correct usage
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