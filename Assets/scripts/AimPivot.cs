using UnityEngine;

public class AimPivot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform body; // capsule / root facing target

    [Header("Pitch Limits")]
    public float minPitch = -45f;
    public float maxPitch = 45f;

    [Header("Settings")]
    public float aimSpeed = 12f;

    private Transform target;
    private float currentPitch;

    void LateUpdate()
    {
        if (!target || !firePoint || !body)
            return;

        // Direction from FIRE POINT to target (WORLD)
        Vector3 dir = target.position - firePoint.position;

        // Remove body yaw influence
        Quaternion yawOnly = Quaternion.Euler(0f, body.eulerAngles.y, 0f);
        Vector3 localDir = Quaternion.Inverse(yawOnly) * dir;

        // Compute pitch safely
        float desiredPitch = Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;
        desiredPitch = Mathf.Clamp(desiredPitch, minPitch, maxPitch);

        currentPitch = Mathf.Lerp(
            currentPitch,
            desiredPitch,
            Time.deltaTime * aimSpeed
        );

        transform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ClearTarget()
    {
        target = null;
        currentPitch = 0f;
        transform.localRotation = Quaternion.identity;
    }
}