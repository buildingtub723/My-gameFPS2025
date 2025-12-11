using UnityEngine;

public class WeaponSwitchingScript : MonoBehaviour
{
    public Transform weaponHolder;
    public Weapon_Controller_Script defaultMeleeWeapon; // Assign your melee weapon prefab or script
    private int currentWeaponIndex = 0;

    private ThirdPersonCharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<ThirdPersonCharacterController>();

        if (weaponHolder.childCount > 0)
        {
            SelectWeapon(currentWeaponIndex);
        }
        else
        {
            EquipDefaultMelee();
        }
    }

    private void Update()
    {
        // If no weapons picked, always use melee
        if (weaponHolder.childCount == 0)
        {
            EquipDefaultMelee();
            return;
        }

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
                characterController.currentWeapon = weapon.GetComponent<Weapon_Controller_Script>();
            }
        }
    }

    void EquipDefaultMelee()
    {
        // No weapons, always fall back to melee
        characterController.currentWeapon = defaultMeleeWeapon;
    }
}
