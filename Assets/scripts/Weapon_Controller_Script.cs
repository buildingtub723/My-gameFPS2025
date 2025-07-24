using UnityEngine;
public enum FireMode
{
    SemiAuto,
    FullAuto
}

public class Weapon_Controller_Script : MonoBehaviour
{
    [Header("Firing Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 0.1f;
    public float spreadAngle = 1f;
    public int pelletCount = 1;

    [Header("Ammo Settings")]
    public int magazineSize = 30;
    public int ammoInMagazine;
    public int ammoReserve = 90;
    public float reloadTime = 2f;

    [Header("Animation")]
    public Animator animator;

    public string weaponType;

    private float nextFireTime;
    private bool isReloading = false;

    public FireMode fireMode = FireMode.SemiAuto; // Add this line at the top
    private bool isFiring = false;

    public WeaponAudioHandler weaponAudio;

    private void Awake()
    {
        weaponAudio = GetComponent<WeaponAudioHandler>();
    }
    private void Start()
    {
        ammoInMagazine = magazineSize;
    }
    private void Update()
    {
        if (fireMode == FireMode.FullAuto && isFiring && !isReloading && ammoInMagazine > 0)
        {
            if (Time.time >= nextFireTime)
            {
                Fire(); // Call fire internally for full auto
            }
        }
    }
    public void StartFiring()
    {
        isFiring = true;
    }

    public void StopFiring()
    {
        isFiring = false;
    }
    public void Fire()
    {
        if (Time.time < nextFireTime || isReloading)
            return;

        if (ammoInMagazine <= 0)
        {
            weaponAudio?.PlayEmpty();
            return;
        }

        nextFireTime = Time.time + fireRate;
        ammoInMagazine--;

        weaponAudio?.PlayFire();

        if (animator != null)
            animator.SetTrigger("Shoot");

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 spreadDir = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            ) * firePoint.forward;

            Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(spreadDir));
        }
    }

    public void Reload()
    {
        if (isReloading || ammoInMagazine == magazineSize || ammoReserve <= 0)
            return;

        weaponAudio?.PlayReload();

        MonoBehaviour root = GetComponentInParent<MonoBehaviour>();
        if (root != null)
            root.StartCoroutine(ReloadRoutine());

        Debug.Log("Reloading...");
    }

    private System.Collections.IEnumerator ReloadRoutine()
    {
        isReloading = true;
        if (animator != null) animator.SetTrigger("Reload");
        yield return new WaitForSeconds(reloadTime);

        int bulletsToReload = magazineSize - ammoInMagazine;
        int bulletsAvailable = Mathf.Min(bulletsToReload, ammoReserve);

        ammoInMagazine += bulletsAvailable;
        ammoReserve -= bulletsAvailable;

        isReloading = false;
    }
}
