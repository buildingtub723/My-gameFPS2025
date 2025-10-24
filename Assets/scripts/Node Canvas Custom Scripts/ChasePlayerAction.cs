using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace LoneWolf.AI
{
    [Category("AI/Movement")]
    public class ChasePlayerAction : ActionTask<NavMeshAgent>
    {
        public BBParameter<GameObject> Player;
        public BBParameter<float> stoppingDistance = 3f;
        public BBParameter<float> updateInterval = 1f;
        public BBParameter<float> chaseSpeed = 4f;

        private float nextUpdateTime = 0f;

        protected override void OnExecute()
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed.value;
        }

        protected override void OnUpdate()
        {
            if (Player == null || Player.value == null) return;

            if (Time.time >= nextUpdateTime)
            {
                nextUpdateTime = Time.time + updateInterval.value;
                Vector3 pos = Player.value.transform.position;
                agent.SetDestination(pos);
            }

            if (Vector3.Distance(agent.transform.position, Player.value.transform.position) <= stoppingDistance.value)
            {
                EndAction(true); // close enough
            }
        }

        protected override void OnStop()
        {
            if (agent != null && agent.isOnNavMesh)
                agent.isStopped = true;
        }
    }
}