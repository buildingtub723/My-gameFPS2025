using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace LoneWolf.AI
{
    [Category("LoneWolf/Combat")]
    [Description("Triggers reload on the assigned weapon.")]
    public class ReloadWeaponAction : ActionTask
    {
        public BBParameter<GameObject> weaponObject;

        protected override void OnExecute()
        {
            if (weaponObject == null || weaponObject.value == null)
            {
                Debug.LogWarning("ReloadWeaponAction: WeaponObject is not assigned.");
                EndAction(false);
                return;
            }

            var weapon = weaponObject.value.GetComponent<Weapon_Controller_Script>();
            if (weapon == null)
            {
                Debug.LogWarning("ReloadWeaponAction: No Weapon_Controller_Script found on weaponObject.");
                EndAction(false);
                return;
            }

            // Only reload if not already reloading and magazine not full
            if (!weapon.Equals(null) && !weaponObject.value.GetComponent<Weapon_Controller_Script>().Equals(null))
            {
                if (weapon.ammoInMagazine < weapon.magazineSize && !weaponObject.value.GetComponent<Weapon_Controller_Script>().Equals(null))
                {
                    weapon.Reload(agent);
                    Debug.Log("ReloadWeaponAction: Reloading weapon.");
                }
            }

            EndAction(true); // Always end action successfully
        }
    }
}
