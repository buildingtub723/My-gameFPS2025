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
        public LayerMask obstacleMask = Physics.DefaultRaycastLayers;

        protected override bool OnCheck()
        {
            if (Player == null || Player.value == null)
                return false;

            Vector3 dir = Player.value.transform.position - agent.transform.position;
            float dist = dir.magnitude;

            if (dist > DetectionRange.value)
                return false;

            // Raycast from AI's "eyes" toward the player
            Vector3 origin = agent.transform.position + Vector3.up * 1.5f;
            if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist, obstacleMask))
            {
                if (hit.collider.gameObject == Player.value)
                    return true; // sees the player
            }

            return false; // blocked
        }
    }
}
