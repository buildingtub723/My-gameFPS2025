using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Header = UnityEngine.HeaderAttribute;

[Category("Combat")]
[Description("Faces target horizontally and fires controlled bursts.")]
public class AttackTargetBurstAction : ActionTask
{
    [Header("Blackboard")]
    public BBParameter<GameObject> target;
    public BBParameter<GameObject> weaponObject;

    [Header("Burst Settings")]
    public int burstCount = 3;
    public float burstDelay = 0.2f;
    public float recoveryTime = 2f;

    [Header("Body Rotation")]
    public float bodyTurnSpeed = 6f;

    private Weapon_Controller_Script weapon;
    private Transform body;

    private int shotsFired;
    private float nextShotTime;
    private float burstEndTime;
    private bool isBursting;

    protected override string info => "Attack Target (Burst)";

    protected override void OnExecute()
    {
        if (target.value == null || weaponObject.value == null)
        {
            EndAction(false);
            return;
        }

        weapon = weaponObject.value.GetComponent<Weapon_Controller_Script>();
        if (weapon == null)
        {
            Debug.LogError("AttackTargetBurstAction: Weapon_Controller_Script missing!");
            EndAction(false);
            return;
        }

        body = agent.transform;

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

        RotateBodyTowardsTarget();

        if (isBursting)
        {
            HandleBurstFire();
        }

        if (Time.time >= burstEndTime)
        {
            EndAction(true);
        }
    }

    private void RotateBodyTowardsTarget()
    {
        Vector3 toTarget = target.value.transform.position - body.position;
        toTarget.y = 0f; // HARD Y LOCK — NO VERTICAL AIM

        if (toTarget.sqrMagnitude < 0.001f)
            return;

        Quaternion lookRot = Quaternion.LookRotation(toTarget.normalized);
        body.rotation = Quaternion.Slerp(
            body.rotation,
            lookRot,
            Time.deltaTime * bodyTurnSpeed
        );
    }

    private void HandleBurstFire()
    {
        if (Time.time >= nextShotTime && shotsFired < burstCount)
        {
            weapon.Fire();
            shotsFired++;
            nextShotTime = Time.time + burstDelay;
        }

        if (shotsFired >= burstCount)
        {
            isBursting = false;
        }
    }
}