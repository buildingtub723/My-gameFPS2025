using UnityEngine;
using System;
using System.Collections;

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

    [Header("Weapon Info")]
    public string weaponType;
    public FireMode fireMode = FireMode.SemiAuto;

    [Header("Audio")]
    public WeaponAudioHandler weaponAudio;

    // ─────────────────────────────
    // EVENTS (Animation Relay)
    // ─────────────────────────────
    public event Action OnShoot;
    public event Action OnReloadStart;
    public event Action OnReloadEnd;

    private float nextFireTime;
    private bool isReloading = false;
    private bool isFiring = false;

    private void Awake()
    {
        weaponAudio = GetComponent<WeaponAudioHandler>();
    }

    private void Update()
    {
        if (fireMode == FireMode.FullAuto && isFiring && !isReloading && ammoInMagazine > 0)
        {
            if (Time.time >= nextFireTime)
            {
                Fire();
            }
        }
    }

    // ─────────────────────────────
    // FIRE CONTROL
    // ─────────────────────────────
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
        OnShoot?.Invoke();

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 spreadDir = Quaternion.Euler(
                UnityEngine.Random.Range(-spreadAngle, spreadAngle),
                UnityEngine.Random.Range(-spreadAngle, spreadAngle),
                0f
            ) * firePoint.forward;

            Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.LookRotation(spreadDir)
            );
        }
    }

    // ─────────────────────────────
    // RELOAD
    // ─────────────────────────────
    public void Reload(Component coroutineRunner)
    {
        if (isReloading || ammoInMagazine == magazineSize || ammoReserve <= 0)
            return;

        var runner = coroutineRunner as MonoBehaviour;
        if (runner == null)
        {
            Debug.LogError("WeaponController.Reload: coroutineRunner is not a MonoBehaviour.");
            return;
        }

        weaponAudio?.PlayReload();
        runner.StartCoroutine(ReloadRoutine());
    }


    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        OnReloadStart?.Invoke();

        yield return new WaitForSeconds(reloadTime);

        int bulletsNeeded = magazineSize - ammoInMagazine;
        int bulletsAvailable = Mathf.Min(bulletsNeeded, ammoReserve);

        ammoInMagazine += bulletsAvailable;
        ammoReserve -= bulletsAvailable;

        isReloading = false;
        OnReloadEnd?.Invoke();
    }

    // ─────────────────────────────
    // STATUS (OPTIONAL HELPERS)
    // ─────────────────────────────
    public bool IsReloading() => isReloading;
    public bool HasAmmo() => ammoInMagazine > 0;
}

