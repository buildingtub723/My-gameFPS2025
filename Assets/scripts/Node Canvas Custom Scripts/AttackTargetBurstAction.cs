using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Header = UnityEngine.HeaderAttribute;

[Category("Combat")]
[Description("AI aims horizontally and vertically at the target, firing in controlled bursts.")]
public class AttackTargetBurstAction : ActionTask
{
    [Header("Blackboard References")]
    public BBParameter<GameObject> target;
    public BBParameter<GameObject> weaponObject;
    public BBParameter<GameObject> weaponHolder; // pivot for vertical aiming

    [Header("Burst Settings")]
    public int burstCount = 3;         // number of shots per burst
    public float burstDelay = 0.2f;    // time between each shot
    public float recoveryTime = 2f;    // cooldown before the next burst can start

    [Header("Aiming Settings")]
    public float bodyTurnSpeed = 5f;
    public float aimSpeed = 8f;

    private Weapon_Controller_Script weaponScript;
    private Transform body;
    private Transform pivot;

    private int shotsFired;
    private float nextShotTime;
    private float burstEndTime;

    private bool isBursting;

    protected override string info => "Attack Target (Burst)";

    protected override void OnExecute()
    {
        // validate
        if (target.value == null || weaponObject.value == null)
        {
            EndAction(false);
            return;
        }

        // cache
        weaponScript = weaponObject.value.GetComponent<Weapon_Controller_Script>();
        if (weaponScript == null)
        {
            Debug.LogError($"{weaponObject.value.name} missing Weapon_Controller_Script!");
            EndAction(false);
            return;
        }

        body = agent != null ? agent.transform : weaponObject.value.transform.root;
        pivot = weaponHolder.value != null ? weaponHolder.value.transform : weaponObject.value.transform;

        // start burst
        shotsFired = 0;
        isBursting = true;
        nextShotTime = Time.time;
        burstEndTime = Time.time + (burstCount * burstDelay) + recoveryTime;
    }

    protected override void OnUpdate()
    {
        if (target.value == null)
        {
            EndAction(false);
            return;
        }

        AimAtTarget();

        if (isBursting)
        {
            HandleBurstFire();
        }

        // once the burst + cooldown are done, finish
        if (Time.time >= burstEndTime)
        {
            EndAction(true);
        }
    }

    private void AimAtTarget()
    {
        Vector3 toTarget = target.value.transform.position - body.position;

        // --- horizontal rotation ---
        Vector3 flatDir = toTarget;
        flatDir.y = 0;
        if (flatDir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(flatDir.normalized);
            body.rotation = Quaternion.Lerp(body.rotation, lookRot, Time.deltaTime * bodyTurnSpeed);
        }

        // --- vertical aiming (pitch only) ---
        if (pivot != null)
        {
            Vector3 aimDir = (target.value.transform.position - pivot.position).normalized;
            Quaternion lookPitch = Quaternion.LookRotation(aimDir);
            Quaternion verticalOnly = Quaternion.Euler(lookPitch.eulerAngles.x, pivot.rotation.eulerAngles.y, pivot.rotation.eulerAngles.z);
            pivot.rotation = Quaternion.Lerp(pivot.rotation, verticalOnly, Time.deltaTime * aimSpeed);
        }
    }

    private void HandleBurstFire()
    {
        if (Time.time >= nextShotTime && shotsFired < burstCount)
        {
            weaponScript.Fire(); // trigger one shot
            shotsFired++;
            nextShotTime = Time.time + burstDelay;
        }

        if (shotsFired >= burstCount)
        {
            isBursting = false; // stop firing, wait for cooldown
        }
    }
}