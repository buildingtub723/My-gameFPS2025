using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace LoneWolf.AI
{
    [Category("LoneWolf/Combat")]
    [Description("Check if the current weapon's magazine is empty.")]
    public class IsMagazineEmptyCondition : ConditionTask
    {
        public BBParameter<GameObject> weaponObject;

        protected override bool OnCheck()
        {
            if (weaponObject == null || weaponObject.value == null)
            {
                Debug.LogWarning("IsMagazineEmptyCondition: WeaponObject is not assigned.");
                return false;
            }

            var weapon = weaponObject.value.GetComponent<Weapon_Controller_Script>();
            if (weapon == null)
            {
                Debug.LogWarning("IsMagazineEmptyCondition: No Weapon_Controller_Script found on weaponObject.");
                return false;
            }

            return weapon.ammoInMagazine <= 0;
        }
    }
}