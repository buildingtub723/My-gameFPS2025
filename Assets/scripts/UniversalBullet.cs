using Unity.VisualScripting;
using UnityEngine;

public class UniversalBullet : MonoBehaviour
{
    public float damage = 25f;
    public float lifeTime = 5f;
    public float speed = 20f;
    public Team shooterTeam;
    public string weaponType;


 [Header("Melee Settings")]
    public GameObject attackObject;   // Assign in Inspector
    public float forwardDistance = 1f;
    public float thrustSpeed = 10f;
    public float retractSpeed = 10f;

    private Vector3 startLocalPos;
    private bool attacking = false;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // store original local position
        startLocalPos = transform.localPosition;

        if (rb != null && (weaponType == "Shotgun" || weaponType == "Rifle"))
        {
            rb.linearVelocity = transform.forward * speed;
        }
        else if (rb != null && weaponType == "Grenade")
        {
            Vector3 shootDir = (transform.forward + Vector3.up * 0.25f).normalized;
            rb.linearVelocity = shootDir * (speed / 2f);
        }
        else if (weaponType == "Melee")
        {
            // disable physics so movement is smooth
            if (rb != null) rb.isKinematic = true;

            StartCoroutine(DoMeleeThrust());
        }

        // lifeTimer only applies to ranged weapons
        if (weaponType != "Melee")
            Destroy(gameObject, lifeTime);
    }



    private System.Collections.IEnumerator DoMeleeThrust()
    {
        attacking = true;

        Vector3 targetPos = startLocalPos + transform.forward * forwardDistance;

        // Move forward
        while (Vector3.Distance(transform.localPosition, targetPos) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                targetPos,
                thrustSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Move backward
        while (Vector3.Distance(transform.localPosition, startLocalPos) > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                startLocalPos,
                retractSpeed * Time.deltaTime
            );
            yield return null;
        }

        // Snap back to original position
        transform.localPosition = startLocalPos;

        // KEEP THE ATTACK OBJECT ENABLED — do NOT disable it
        // (You asked to let it stay protruding)

        attacking = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (weaponType == "Melee")
            return; // melee shouldn't collide like a projectile

        if (collision.gameObject.CompareTag("Bullet"))
            return;

        var damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.TakeDamage(damage);

        Destroy(gameObject);
    }

}