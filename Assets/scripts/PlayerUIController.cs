using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIController : MonoBehaviour
{
    [Header("Health UI")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    [Header("Ammo UI")]
    public TextMeshProUGUI ammoText;

    [Header("Crosshair")]
    public GameObject crosshair;

    [Header("Wave UI")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI waveTimerText;
    public TextMeshProUGUI levelTimerText;

    [Header("References")]
    public Health playerHealth;
    public Weapon_Controller_Script weapon;
    public ThirdPersonCharacterController playerController;
    public SpawnManager spawnManager;

    void Update()
    {
        UpdateHealthUI();
        UpdateAmmoUI();
        UpdateWaveUI();
        UpdateLevelTimerUI();
    }

    void UpdateHealthUI()
    {
        if (playerHealth != null)
        {
            healthSlider.value = playerHealth.currentHealth;
            healthText.text = $"HP: {playerHealth.currentHealth} / {playerHealth.maxHealth}";
        }
    }

    void UpdateAmmoUI()
    {
        if (playerController != null && playerController.currentWeapon != null)
        {
            ammoText.text = $"Ammo: {playerController.currentWeapon.ammoInMagazine}/{playerController.currentWeapon.ammoReserve}";
        }
        else
        {
            ammoText.text = "Ammo: - / -";
        }
    }

    void UpdateWaveUI()
    {
        if (spawnManager != null)
        {
            waveText.text = $"Wave: {spawnManager.CurrentWave}";
            waveTimerText.text = $"Next Wave In: {spawnManager.TimeUntilNextWave:0.0}s";
        }
    }

    void UpdateLevelTimerUI()
    {
        if (spawnManager != null)
        {
            float timeLeft = spawnManager.LevelTimeRemaining;
            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            levelTimerText.text = $"Time Left: {minutes:00}:{seconds:00}";
        }
    }

    public void SetCrosshairVisible(bool visible)
    {
        if (crosshair != null)
            crosshair.SetActive(visible);
    }
}
