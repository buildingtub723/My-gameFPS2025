using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public GameObject weaponPrefab;

    public void Interact(GameObject interactor)
    {
        var controller = interactor.GetComponent<ThirdPersonCharacterController>();
        if (controller != null && weaponPrefab != null)
        {
            controller.PickupWeapon(weaponPrefab);
            Destroy(gameObject);
        }
    }
}