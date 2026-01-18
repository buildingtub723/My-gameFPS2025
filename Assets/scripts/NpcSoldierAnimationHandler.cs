using UnityEngine;
using UnityEngine.AI;

public class NpcSoldierAnimationHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Weapon_Controller_Script weapon;
    [SerializeField] private Health health;

    [Header("Hit Reaction")]
    [SerializeField] private float hitCooldown = 0.4f;

    private float lastHitTime;
    private bool isDead;

    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!agent) agent = GetComponentInParent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        BindWeaponEvents();
        BindHealthEvents();
    }

    private void OnDisable()
    {
        UnbindWeaponEvents();
        UnbindHealthEvents();
    }

    private void Update()
    {
        if (isDead || agent == null) return;
        UpdateMovement();
    }

    // --------------------------------------------------------
    // MOVEMENT
    // --------------------------------------------------------

    private void UpdateMovement()
    {
        Vector3 localVel = transform.InverseTransformDirection(agent.velocity);
        Vector3 normalized = localVel / Mathf.Max(agent.speed, 0.01f);

        animator.SetFloat("MoveX", Mathf.Clamp(normalized.x, -1f, 1f));
        animator.SetFloat("MoveY", Mathf.Clamp(normalized.z, -1f, 1f));
    }

    // --------------------------------------------------------
    // WEAPON EVENTS
    // --------------------------------------------------------

    private void BindWeaponEvents()
    {
        if (!weapon) return;

        weapon.OnShoot += OnWeaponFire;
        weapon.OnReloadStart += OnWeaponReload;
    }

    private void UnbindWeaponEvents()
    {
        if (!weapon) return;

        weapon.OnShoot -= OnWeaponFire;
        weapon.OnReloadStart -= OnWeaponReload;
    }

    private void OnWeaponFire()
    {
        if (isDead) return;
        animator.SetTrigger("Fire");
    }

    private void OnWeaponReload()
    {
        if (isDead) return;
        animator.SetTrigger("Reload");
    }

    // --------------------------------------------------------
    // HEALTH EVENTS
    // --------------------------------------------------------

    private void BindHealthEvents()
    {
        if (!health) return;

        health.OnDamageTaken += OnDamaged;
        health.OnDeath += OnDeath;
    }

    private void UnbindHealthEvents()
    {
        if (!health) return;

        health.OnDamageTaken -= OnDamaged;
        health.OnDeath -= OnDeath;
    }

    private void OnDamaged(float amount)
    {
        if (isDead) return;
        if (Time.time - lastHitTime < hitCooldown) return;

        lastHitTime = Time.time;
        animator.SetTrigger("Hit");
    }

    private void OnDeath()
    {
        if (isDead) return;

        isDead = true;
        animator.SetBool("IsDead", true);
        animator.SetTrigger("Die");
    }
}
