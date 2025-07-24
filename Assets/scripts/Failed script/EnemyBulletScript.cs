using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    public float lifetime = 5f;
    public float damage = 25f;
    public Team shooterTeam; // Assigned when fired

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet")) return; // prevent self-collision

        TeamIdentity targetTeam = other.GetComponent<TeamIdentity>();
        if (targetTeam != null)
        {
            if (targetTeam.team == shooterTeam)
            {
                Debug.Log("Ignored friendly fire on: " + other.name);
                return;
            }
        }

        Enemy_Health_Script health = other.GetComponent<Enemy_Health_Script>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log("Enemy hit: " + other.name);
        }

        if (other.CompareTag("Player") && (targetTeam == null || targetTeam.team != shooterTeam))
        {
            Debug.Log("Player hit!");
            // Add player damage logic here
        }

        Destroy(gameObject);
    }
}
