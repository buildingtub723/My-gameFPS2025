using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace LoneWolf.AI
{
    [Category("AI/Movement")]
    public class MoveToVector3 : ActionTask<NavMeshAgent>
    {
        public BBParameter<Vector3> targetPosition;
        public BBParameter<float> stoppingDistance = 0.5f;
        public BBParameter<float> waitAfterArrival = 2f;

        private bool arrived = false;
        private float waitTimer = 0f;

        protected override void OnExecute()
        {
            if (!agent.isOnNavMesh)
            {
                EndAction(false);
                return;
            }

            agent.ResetPath(); // prevent stale path
            agent.stoppingDistance = stoppingDistance.value;
            agent.SetDestination(targetPosition.value);
            arrived = false;
            waitTimer = 0f;
        }

        protected override void OnUpdate()
        {
            if (!agent.isOnNavMesh)
            {
                EndAction(false);
                return;
            }

            if (!arrived)
            {
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
                {
                    arrived = true;
                    waitTimer = waitAfterArrival.value;
                }
            }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    EndAction(true);
                }
            }
        }

        protected override void OnStop()
        {
            if (agent.isOnNavMesh)
                agent.ResetPath(); // clear when switching to chase
        }
    }
}
