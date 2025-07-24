using UnityEngine;

public class WeaponAudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [Header("Clips")]
    public AudioClip fireClip;
    public AudioClip reloadClip;
    public AudioClip emptyClip;

    public void PlayFire() => audioSource.PlayOneShot(fireClip);
    public void PlayReload() => audioSource.PlayOneShot(reloadClip);
    public void PlayEmpty() => audioSource.PlayOneShot(emptyClip);
}
