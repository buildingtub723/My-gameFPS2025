using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace LoneWolf.AI
{
    [Category("AI/Conditions")]
    public class CanSeePlayerCondition : ConditionTask<NavMeshAgent>
    {
        public BBParameter<GameObject> Player;
        public BBParameter<float> DetectionRange = 25f;
        // optional: keep an obstacle mask if you want to ignore layers (leave default to -1 to include all)
        public LayerMask obstacleMask = ~0; // all layers

        protected override bool OnCheck()
        {
            if (Player == null || Player.value == null)
            {
                Debug.Log($"CanSeePlayer: Player is null on {agent.name}");
                return false;
            }

            Vector3 origin = agent.transform.position + Vector3.up * 1.5f;
            Vector3 targetPos = Player.value.transform.position;
            Vector3 dir = targetPos - origin;
            float dist = dir.magnitude;

            Debug.DrawRay(origin, dir.normalized * Mathf.Min(dist, 100f), Color.red, 0.1f);
            Debug.Log($"CanSeePlayer: Distance to player: {dist} (Agent:{agent.name})");

            // distance check
            if (dist > DetectionRange.value)
            {
                return false;
            }

            // Perform a raycast that respects obstacleMask, but still detect colliders on child objects.
            // Use Physics.Raycast with the obstacleMask so you can still configure blocking layers if desired.
            if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist, obstacleMask))
            {
                // If the ray hit *any* collider that belongs to the Player (root or child), that counts as seeing the player.
                var hitGO = hit.collider.gameObject;
                if (hitGO == Player.value || hit.collider.transform.IsChildOf(Player.value.transform))
                {
                    return true;
                }

                // Ray hit something else first (an obstacle)
                return false;
            }
            else
            {
                // No collider was hit along the ray — unusual if player is there and has a collider.
                // As a fallback, check distance only (but usually you'd want a collider to detect).
                Debug.Log($"CanSeePlayer: Raycast found no hit from {agent.name} to Player. Check layers/colliders.");
                return false;
            }
        }
    }
}

