using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Health))]
public class NpcSoldierAudioManager : MonoBehaviour
{
    [Header("Audio Sources & Clips")]
    public AudioSource audioSource;
    public AudioClip[] hurtClips;
    public AudioClip deathClip;
    public AudioClip spottedClip;

    [Header("Hurt Settings")]
    public float hurtCooldown = 0.6f;
    public bool playOnFirstHit = true;

    private Health health;
    private float lastHurtTime = -Mathf.Infinity;
    private bool firstHitPlayed = false;
    private bool isDead = false;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (health != null)
        {
            health.OnDamageTaken += HandleHurt;
            health.OnDeath += HandleDeath;
        }
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDamageTaken -= HandleHurt;
            health.OnDeath -= HandleDeath;
        }
    }

    private void HandleHurt(float amount)
    {
        // Do nothing if dead
        if (isDead) return;

        // First hit override
        if (!firstHitPlayed && playOnFirstHit)
        {
            PlayRandomHurt();
            firstHitPlayed = true;
            lastHurtTime = Time.time;
            return;
        }

        // Cooldown check
        if (Time.time - lastHurtTime >= hurtCooldown)
        {
            PlayRandomHurt();
            lastHurtTime = Time.time;
        }
    }

    private void HandleDeath()
    {
        isDead = true;
        PlayDeathSound();
    }

    private void PlayRandomHurt()
    {
        if (hurtClips == null || hurtClips.Length == 0) return;

        AudioClip clip = hurtClips[Random.Range(0, hurtClips.Length)];
        audioSource.pitch = Random.Range(0.95f, 1.05f); // subtle pitch variation
        audioSource.PlayOneShot(clip);
    }

    public void PlayDeathSound()
    {
        if (deathClip != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(deathClip);
        }
    }

    public void PlaySpottedSound()
    {
        if (isDead) return;

        if (spottedClip != null)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(spottedClip);
        }
    }
}