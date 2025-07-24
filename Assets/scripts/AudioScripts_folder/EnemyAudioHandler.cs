using UnityEngine;

public class EnemyAudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [Header("Clips")]
    public AudioClip[] idleSounds;
    public AudioClip aggroSound;
    public AudioClip shootSound;
    public AudioClip deathSound;

    public void PlayIdle()
    {
        if (idleSounds.Length > 0)
            audioSource.PlayOneShot(idleSounds[Random.Range(0, idleSounds.Length)]);
    }

    public void PlayAggro() => audioSource.PlayOneShot(aggroSound);
    public void PlayShoot() => audioSource.PlayOneShot(shootSound);
    public void PlayDeath() => audioSource.PlayOneShot(deathSound);
}
