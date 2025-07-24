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

    [Header("References")]
    public Health playerHealth;
    public Weapon_Controller_Script weapon;
    public ThirdPersonCharacterController playerController;

    void Update()
    {
        UpdateHealthUI();
        UpdateAmmoUI();
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
        if (playerController.currentWeapon != null)
        {
            ammoText.text = $"Ammo: {playerController.currentWeapon.ammoInMagazine}/{playerController.currentWeapon.ammoReserve}";
        }
        else
        {
            ammoText.text = "Ammo: - / -";
        }
    }

    // You could use this later to hide/show crosshair
    public void SetCrosshairVisible(bool visible)
    {
        if (crosshair != null)
            crosshair.SetActive(visible);
    }
}
