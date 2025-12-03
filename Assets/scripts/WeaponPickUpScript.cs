using TMPro;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public GameObject weaponPrefab;

    public TMP_Text interactionText;
    public void Interact(GameObject interactor)
    {
        var controller = interactor.GetComponent<ThirdPersonCharacterController>();
        if (controller != null && weaponPrefab != null)
        {
            controller.PickupWeapon(weaponPrefab);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && interactionText != null)
        {
            var nameText = weaponPrefab != null ? weaponPrefab.name : "weapon";
            interactionText.text = $"Press E to pick up {nameText}";
            interactionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && interactionText != null)
            interactionText.gameObject.SetActive(false);
    }
}