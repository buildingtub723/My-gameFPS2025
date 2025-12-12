using UnityEngine;

public class WeaponSwitchingScript : MonoBehaviour
{
    public Transform weaponHolder;
    private int currentWeaponIndex = 0;

    private ThirdPersonCharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<ThirdPersonCharacterController>();
        SelectWeapon(currentWeaponIndex);
    }

    private void Update()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll > 0f)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weaponHolder.childCount;
            SelectWeapon(currentWeaponIndex);
        }
        else if (scroll < 0f)
        {
            currentWeaponIndex = (currentWeaponIndex - 1 + weaponHolder.childCount) % weaponHolder.childCount;
            SelectWeapon(currentWeaponIndex);
        }
    }

    void SelectWeapon(int index)
    {
        for (int i = 0; i < weaponHolder.childCount; i++)
        {
            Transform weapon = weaponHolder.GetChild(i);
            weapon.gameObject.SetActive(i == index);

            if (i == index)
            {
                // Update reference in character controller
                characterController.currentWeapon = weapon.GetComponent<Weapon_Controller_Script>();
            }
        }
    }
}
