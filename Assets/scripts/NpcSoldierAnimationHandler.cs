using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class NpcSoldierAnimationHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    [Header("Hit Reaction")]
    [SerializeField] private float hitCooldown = 0.4f;
    private float lastHitTime = -999f;

    private bool isDead;

    private void Awake()
    {
        //if (!animator) animator = GetComponent<Animator>();
        //if (!agent) agent = GetComponentInParent<NavMeshAgent>();
    }

    private void Update()
    {
        if (isDead || agent == null) return;

        UpdateMovementParameters();
    }

    // -------------------------
    // MOVEMENT
    // -------------------------
    private void UpdateMovementParameters()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        Vector3 normalized = localVelocity / Mathf.Max(agent.speed, 0.01f);

        animator.SetFloat("MoveX", Mathf.Clamp(normalized.x, -1f, 1f));
        animator.SetFloat("MoveY", Mathf.Clamp(normalized.z, -1f, 1f));
    }

    // -------------------------
    // COMBAT (CALLED BY BT / WEAPON)
    // -------------------------
    public void PlayFire()
    {
        if (isDead) return;
        animator.SetTrigger("Fire");
    }

    public void PlayReload()
    {
        if (isDead) return;
        animator.SetTrigger("Reload");
    }

    // -------------------------
    // DAMAGE / HIT
    // -------------------------
    public void PlayHit(float damage)
    {
        if (isDead) return;
        if (Time.time - lastHitTime < hitCooldown) return;

        lastHitTime = Time.time;
        animator.SetTrigger("Hit");
    }

    // -------------------------
    // DEATH
    // -------------------------
    public void PlayDeath()
    {
        if (isDead) return;

        isDead = true;
        animator.SetBool("IsDead", true);
        animator.SetTrigger("Die");
    }
}

