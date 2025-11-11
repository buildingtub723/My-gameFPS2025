using UnityEngine;
using UnityEngine.AI;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Header = UnityEngine.HeaderAttribute;

namespace LoneWolf.AI
{
    [Category("Movement")]
    [Description("Randomly strafe left/right relative to the current position while looking at the target.")]
    public class StrafeInCombatAction : ActionTask<NavMeshAgent>
    {
        [Header("Blackboard")]
        public BBParameter<GameObject> target;

        [Header("Strafe Settings")]
        [Range(0f, 1f)]
        public BBParameter<float> strafeChance = 1f;         // chance to strafe at all
        public BBParameter<float> strafeDistanceMin = 2f;   // min lateral distance
        public BBParameter<float> strafeDistanceMax = 4f;   // max lateral distance
        public BBParameter<float> strafeDuration = 1.2f;    // max strafe time
        public BBParameter<float> minRunTime = 0.4f;        // prevent instant success
        public BBParameter<float> strafeSpeedMultiplier = 1.0f;

        private Vector3 strafeTarget;
        private float strafeEndTime;
        private float strafeStartTime;
        private float originalSpeed;
        private bool isStrafing;

        protected override void OnExecute()
        {
            if (target == null || target.value == null)
            {
                EndAction(false);
                return;
            }

            // chance to skip strafe entirely
            if (Random.value > strafeChance.value)
            {
                EndAction(true);
                return;
            }

            Vector3 agentPos = agent.transform.position;

            // pick left or right
            Vector3 right = agent.transform.right;
            if (Random.value > 0.5f) right = -right;

            // random lateral distance
            float lateral = Random.Range(strafeDistanceMin.value, strafeDistanceMax.value);
            Vector3 candidate = agentPos + right * lateral;

            // find valid NavMesh position
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                strafeTarget = hit.position;
            }
            else
            {
                // fallback: stay in place if we can't find a valid spot
                strafeTarget = agentPos;
            }

            // start strafing
            isStrafing = true;
            originalSpeed = agent.speed;
            agent.speed = originalSpeed * strafeSpeedMultiplier.value;
            agent.isStopped = false;
            agent.SetDestination(strafeTarget);
            strafeStartTime = Time.time;
            strafeEndTime = Time.time + strafeDuration.value;
        }

        protected override void OnUpdate()
        {
            if (!isStrafing)
            {
                EndAction(true);
                return;
            }

            // always look at player
            if (target != null && target.value != null)
            {
                Vector3 lookDir = target.value.transform.position - agent.transform.position;
                lookDir.y = 0f;
                if (lookDir.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized);
                    agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRot, Time.deltaTime * 8f);
                }
            }

            bool reached = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f;
            bool timedOut = Time.time >= strafeEndTime;
            bool ranEnough = Time.time - strafeStartTime >= minRunTime.value;

            if (ranEnough && (reached || timedOut))
            {
                EndAction(true);
            }
        }

        protected override void OnStop()
        {
            if (isStrafing)
            {
                isStrafing = false;
                agent.speed = originalSpeed;
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.ResetPath();
                    agent.isStopped = true;
                }
            }
        }
    }
}
