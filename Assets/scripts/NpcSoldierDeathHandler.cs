using UnityEngine;
using UnityEngine.AI;

public class NpcSoldierDeathHandler : MonoBehaviour, IDeathHandler
{
    private bool hasDied = false;
    private Animator animator;
    private NavMeshAgent agent;
    private Collider[] colliders;
    private AudioSource audioSource;

    [Header("Death Settings")]
    public AudioClip deathClip;
    public GameObject ragdollPrefab; // optional — if you want ragdoll spawning
    public float destroyDelay = 5f;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        colliders = GetComponentsInChildren<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    public void HandleDeath(GameObject instigator)
    {
        if (hasDied) return;
        hasDied = true;

        Debug.Log($"{gameObject.name} died (killed by {instigator.name})");

        // Stop movement + disable navigation
        if (agent != null) agent.enabled = false;

        // Disable colliders (optional — prevents physics issues)
        foreach (var col in colliders) col.enabled = false;

        // Play death animation if available
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Play death sound
        if (audioSource != null && deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
        }

        // Optionally spawn ragdoll
        if (ragdollPrefab != null)
        {
            Instantiate(ragdollPrefab, transform.position, transform.rotation);
        }

        // Finally destroy object after delay
        Destroy(gameObject, destroyDelay);
    }
}
