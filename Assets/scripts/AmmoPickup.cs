using UnityEngine;

public class AmmoPickup : MonoBehaviour, IInteractable
{
    public string weaponType; // e.g., "Shotgun", "Rifle"
    public int ammoAmount = 30;

    public void Interact(GameObject interactor)
    {
        var controller = interactor.GetComponent<ThirdPersonCharacterController>();
        if (controller == null || controller.availableWeapons == null) return;

        bool gaveAmmo = false;

        foreach (var weapon in controller.availableWeapons)
        {
            if (weapon != null && weapon.weaponType == weaponType)
            {
                weapon.ammoReserve += ammoAmount;
                gaveAmmo = true;
            }
        }

        if (gaveAmmo)
        {
            Debug.Log("Picked up " + ammoAmount + " ammo for " + weaponType);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("No matching weapon for ammo pickup.");
        }
    }
}