using UnityEngine;
using UnityEngine.AI;

public class NpcSoldierAnimationHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Weapon_Controller_Script weapon;
    [SerializeField] private Health health;

    [Header("Aiming (Vertical)")]
    [Tooltip("Bone used for vertical aiming (Spine / Chest)")]
    [SerializeField] private Transform aimBone;

    [Tooltip("Maximum up/down angle in degrees")]
    [SerializeField] private float maxAimAngle = 45f;

    [Tooltip("How fast the aim bone follows target")]
    [SerializeField] private float aimSmoothSpeed = 10f;

    [Header("Hit Reaction")]
    [SerializeField] private float hitCooldown = 0.4f;

    private float lastHitTime;
    private bool isDead;

    // Runtime aiming data (set by AI)
    private float targetAimPitch;   // -1 to +1
    private float currentAimPitch;

    private Quaternion initialBoneLocalRotation;

    // --------------------------------------------------------------------

    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!agent) agent = GetComponentInParent<NavMeshAgent>();

        if (aimBone != null)
        {
            initialBoneLocalRotation = aimBone.localRotation;
        }
        else
        {
            Debug.LogWarning($"{name}: No Aim Bone assigned!");
        }
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

    private void LateUpdate()
    {
        if (isDead) return;
        ApplyVerticalAiming();
    }

    // --------------------------------------------------------------------
    // MOVEMENT (BASE LAYER)
    // --------------------------------------------------------------------

    private void UpdateMovement()
    {
        Vector3 localVel = transform.InverseTransformDirection(agent.velocity);
        Vector3 normalized = localVel / Mathf.Max(agent.speed, 0.01f);

        animator.SetFloat("MoveX", Mathf.Clamp(normalized.x, -1f, 1f));
        animator.SetFloat("MoveY", Mathf.Clamp(normalized.z, -1f, 1f));
    }

    // --------------------------------------------------------------------
    // AIMING API (CALLED BY AI)
    // --------------------------------------------------------------------

    /// <summary>
    /// Called by AI / Behavior Tree.
    /// Normalized pitch value: -1 (down) to +1 (up)
    /// </summary>
    public void SetAimPitch(float normalizedPitch)
    {
        targetAimPitch = Mathf.Clamp(normalizedPitch, -1f, 1f);
    }

    private void ApplyVerticalAiming()
    {
        if (aimBone == null) return;

        // Smooth pitch
        currentAimPitch = Mathf.Lerp(
            currentAimPitch,
            targetAimPitch,
            Time.deltaTime * aimSmoothSpeed
        );

        float angle = currentAimPitch * maxAimAngle;

        Quaternion pitchRotation = Quaternion.Euler(angle, 0f, 0f);

        // Apply additive rotation AFTER animation
        aimBone.localRotation = initialBoneLocalRotation * pitchRotation;
    }

    // --------------------------------------------------------------------
    // WEAPON ANIMATOR
    // --------------------------------------------------------------------

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

    // --------------------------------------------------------------------
    // HEALTH ANIMATOR
    // --------------------------------------------------------------------

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
