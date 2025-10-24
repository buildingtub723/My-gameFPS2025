using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace LoneWolf.AI
{
    [Category("AI/Movement")]
    public class ChaseTargetAction : ActionTask<NavMeshAgent>
    {
        public BBParameter<GameObject> Target;               // generic
        public BBParameter<float> stoppingDistance = 3f;
        public BBParameter<float> updateInterval = 0.5f;
        public BBParameter<float> chaseSpeed = 4f;

        private float nextUpdateTime = 0f;

        protected override void OnExecute()
        {
            if (Target == null || Target.value == null)
            {
                EndAction(false);
                return;
            }
            agent.isStopped = false;
            agent.speed = chaseSpeed.value;
            nextUpdateTime = 0f;
        }

        protected override void OnUpdate()
        {
            if (Target == null || Target.value == null)
            {
                EndAction(false);
                return;
            }

            if (Time.time >= nextUpdateTime)
            {
                nextUpdateTime = Time.time + updateInterval.value;
                Vector3 pos = Target.value.transform.position;
                // reset stale path safety
                agent.ResetPath();
                agent.SetDestination(pos);
            }

            if (!agent.pathPending && Vector3.Distance(agent.transform.position, Target.value.transform.position) <= stoppingDistance.value)
            {
                // reached — leave it up to BT to transition
                EndAction(true);
            }
        }

        protected override void OnStop()
        {
            if (agent != null) agent.isStopped = true;
        }
    }
}