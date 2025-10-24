using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("LoneWolf/Combat")]
[Description("Finds the weapon object attached to this enemy and stores it in the blackboard.")]
public class FindWeaponObjectAction : ActionTask
{
    [BlackboardOnly]
    public BBParameter<GameObject> WeaponObject;

    [Tooltip("Optional: search by tag instead of child name or component.")]
    public string weaponTag = "";

    [Tooltip("If true, the task will also search in child objects for a Weapon_Controller_Script component.")]
    public bool searchInChildren = true;

    protected override void OnExecute()
    {
        GameObject foundWeapon = null;

        // 1. Try find by tag (if user specified one)
        if (!string.IsNullOrEmpty(weaponTag))
        {
            GameObject taggedWeapon = GameObject.FindGameObjectWithTag(weaponTag);
            if (taggedWeapon != null)
            {
                foundWeapon = taggedWeapon;
            }
        }

        // 2. If not found yet, try to find weapon by component
        if (foundWeapon == null && searchInChildren)
        {
            Weapon_Controller_Script weapon = agent.GetComponentInChildren<Weapon_Controller_Script>();
            if (weapon != null)
                foundWeapon = weapon.gameObject;
        }

        // 3. As fallback, try to find by transform name (common name like "Weapon" or "Gun")
        if (foundWeapon == null)
        {
            Transform weaponTransform = agent.transform.Find("Weapon") ??
                                        agent.transform.Find("Gun") ??
                                        agent.transform.Find("WeaponHolder");
            if (weaponTransform != null)
                foundWeapon = weaponTransform.gameObject;
        }

        // 4. If weapon found, assign it to Blackboard
        if (foundWeapon != null)
        {
            WeaponObject.value = foundWeapon;
            Debug.Log(agent.name + " found weapon: " + foundWeapon.name);
            EndAction(true);
        }
        else
        {
            Debug.LogWarning(agent.name + " could not find weapon in FindWeaponObjectAction!");
            EndAction(false);
        }
    }
}