using UnityEngine;

public class AimPivotController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Empty transform that rotates up/down (inside capsule)")]
    public Transform aimPivot;

    [Header("Settings")]
    public float aimSpeed = 10f;

    private bool isAiming;
    private Vector3 targetPosition;

    // Called by AttackTargetBurstAction
    public void BeginAim()
    {
        isAiming = true;
    }

    // Called every update while attacking
    public void SetTargetPosition(Vector3 worldPos)
    {
        targetPosition = worldPos;
    }

    // Called when attack action exits
    public void EndAim()
    {
        isAiming = false;
    }

    private void LateUpdate()
    {
        if (!isAiming || aimPivot == null)
            return;

        Vector3 dir = targetPosition - aimPivot.position;
        if (dir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        aimPivot.rotation = Quaternion.Slerp(
            aimPivot.rotation,
            targetRot,
            Time.deltaTime * aimSpeed
        );
    }
}
