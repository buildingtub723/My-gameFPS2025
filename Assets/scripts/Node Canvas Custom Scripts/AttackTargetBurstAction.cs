using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Header = UnityEngine.HeaderAttribute;

[Category("Combat")]
[Description("AI aims vertically with WeaponHolder and rotates body horizontally toward the target before firing bursts.")]
public class AttackTargetBurstAction : ActionTask
{
    [Header("Blackboard References")]
    public BBParameter<GameObject> target;
    public BBParameter<GameObject> weaponObject;
    public BBParameter<GameObject> weaponHolder; // Pivot for vertical aiming

    [Header("Settings")]
    public int burstCount = 3;
    public float burstDelay = 0.2f;
    public float bodyTurnSpeed = 5f;
    public float aimSpeed = 8f;

    private Weapon_Controller_Script weaponScript;
    private int shotsFired;
    private float nextShotTime;

    protected override string info => "Attack Target (Burst)";

    protected override void OnExecute()
    {
        if (target.value == null || weaponObject.value == null)
        {
            EndAction(false);
            return;
        }

        weaponScript = weaponObject.value.GetComponent<Weapon_Controller_Script>();
        if (weaponScript == null)
        {
            Debug.LogError($"{weaponObject.value.name} missing Weapon_Controller_Script!");
            EndAction(false);
            return;
        }

        shotsFired = 0;
        nextShotTime = Time.time;
    }

    protected override void OnUpdate()
    {
        if (target.value == null)
        {
            EndAction(false);
            return;
        }

        Transform body = agent != null ? agent.transform : weaponObject.value.transform.root;
        Transform weaponPivot = weaponHolder.value?.transform;

        // --- Rotate the body horizontally toward the target ---
        Vector3 flatTargetDir = (target.value.transform.position - body.position);
        flatTargetDir.y = 0f; // ignore vertical
        if (flatTargetDir.sqrMagnitude > 0.001f)
        {
            Quaternion flatLook = Quaternion.LookRotation(flatTargetDir.normalized);
            body.rotation = Quaternion.Lerp(body.rotation, flatLook, Time.deltaTime * bodyTurnSpeed);
        }

        // --- Rotate weapon vertically for aiming ---
        if (weaponPivot != null)
        {
            Vector3 aimDir = (target.value.transform.position - weaponPivot.position).normalized;
            // Only affect pitch (vertical rotation)
            Quaternion fullLook = Quaternion.LookRotation(aimDir);
            Quaternion verticalOnly = Quaternion.Euler(fullLook.eulerAngles.x, weaponPivot.rotation.eulerAngles.y, weaponPivot.rotation.eulerAngles.z);
            weaponPivot.rotation = Quaternion.Lerp(weaponPivot.rotation, verticalOnly, Time.deltaTime * aimSpeed);
        }

        // --- Shooting logic ---
        if (Time.time >= nextShotTime && shotsFired < burstCount)
        {
            weaponScript.Fire();
            shotsFired++;
            nextShotTime = Time.time + burstDelay;
        }

        if (shotsFired >= burstCount)
        {
            EndAction(true);
        }
    }
}