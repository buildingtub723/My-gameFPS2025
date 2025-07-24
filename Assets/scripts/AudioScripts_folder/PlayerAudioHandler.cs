using UnityEngine;

public class PlayerAudioHandler : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Clips")]
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;

    public void PlayFootstep()
    {
        if (footstepClips.Length == 0) return;
        audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
    }

    public void PlayJump() => audioSource.PlayOneShot(jumpClip);
    public void PlayLand() => audioSource.PlayOneShot(landClip);
    public void PlayHurt() => audioSource.PlayOneShot(hurtClip);
    public void PlayDeath() => audioSource.PlayOneShot(deathClip);
}
