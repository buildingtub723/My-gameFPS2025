using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Header = UnityEngine.HeaderAttribute;

[Category("Combat")]
[Description("Aims at target and fires controlled bursts.")]
public class AttackTargetBurstAction : ActionTask
{
    [Header("Blackboard")]
    public BBParameter<GameObject> target;
    public BBParameter<GameObject> weaponObject;
    public BBParameter<float> aimPitch;   // NEW Blackboard variable

    [Header("Burst Settings")]
    public int burstCount = 3;
    public float burstDelay = 0.2f;
    public float recoveryTime = 2f;

    [Header("Aiming Settings")]
    public float bodyTurnSpeed = 6f;
    public float maxAimAngle = 45f; // degrees

    private Weapon_Controller_Script weapon;
    private Transform body;
    private NpcSoldierAnimationHandler animationHandler;

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

        animationHandler = agent.GetComponent<NpcSoldierAnimationHandler>();
        if (animationHandler == null)
        {
            Debug.LogError("AttackTargetBurstAction: NpcSoldierAnimationHandler missing!");
            EndAction(false);
            return;
        }

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

        if (Time.time >= burstEndTime)
        {
            EndAction(true);
        }
    }

    private void AimAtTarget()
    {
        Vector3 toTarget = target.value.transform.position - body.position;

        //  BODY YAW (horizontal) 
        Vector3 flatDir = new Vector3(toTarget.x, 0f, toTarget.z);
        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(flatDir.normalized);
            body.rotation = Quaternion.Slerp(
                body.rotation,
                lookRot,
                Time.deltaTime * bodyTurnSpeed
            );
        }

        //  VERTICAL AIM (pitch)
        float pitchAngle = Vector3.SignedAngle(
            body.forward,
            toTarget.normalized,
            body.right
        );

        float normalizedPitch = Mathf.Clamp(
            pitchAngle / maxAimAngle,
            -1f,
            1f
        );

        // Write to Blackboard
        aimPitch.value = normalizedPitch;

        // Push to animation handler
        animationHandler.SetAimPitch(normalizedPitch);
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