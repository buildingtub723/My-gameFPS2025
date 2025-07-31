using UnityEngine;

public class EnemyAudioHandler : MonoBehaviour
{
    public AudioClip hurtClip;
    public AudioClip deathClip;
    public float volume = 1f;

    private AudioSource audioSource;
    private Health health;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        health = GetComponent<Health>();

        if (health != null)
        {
            health.OnDamageTaken += PlayHurtSound;
        }
    }

    private void PlayHurtSound(float damage)
    {
        if (hurtClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(hurtClip, volume);
        }
    }
    public bool TryDetachAudio()
    {
        if (audioSource == null) return false;

        GameObject audioGO = new GameObject("TempDeathAudio");
        audioGO.transform.position = transform.position;

        AudioSource tempSource = audioGO.AddComponent<AudioSource>();
        tempSource.clip = deathClip;
        tempSource.volume = volume;
        tempSource.Play();

        Destroy(audioGO, deathClip.length + 0.5f);
        return true;
    }
    public void PlayDeathSound()
    {
        if (deathClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathClip, volume);
        }
    }
}
