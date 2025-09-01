using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace LoneWolf.AI
{
    [Category("AI/Movement")]
    public class RandomNavMeshPointAction : ActionTask<NavMeshAgent>
    {
        public BBParameter<float> radius = 10f;
        public BBParameter<Vector3> randomPoint;

        protected override void OnExecute()
        {
            Vector3 randomDir = Random.insideUnitSphere * radius.value;
            randomDir += agent.transform.position;

            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, radius.value, NavMesh.AllAreas))
            {
                randomPoint.value = hit.position;
                EndAction(true); // success
            }
            else
            {
                EndAction(false); // failed to find point
            }
        }
    }
}

