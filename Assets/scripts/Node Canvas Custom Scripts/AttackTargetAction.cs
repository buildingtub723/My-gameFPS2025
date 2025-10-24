using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace LoneWolf.AI
{
    [Category("AI/Combat")]
    public class AttackTargetAction : ActionTask<UnityEngine.AI.NavMeshAgent>
    {
        public BBParameter<GameObject> Target;
        public BBParameter<float> maxAttackRange = 25f;
        public BBParameter<float> fireRate = 0.2f;

        private Weapon_Controller_Script weapon;
        private float lastFireTime;

        protected override void OnExecute()
        {
            // Cache the weapon component on the AI
            weapon = agent.GetComponentInChildren<Weapon_Controller_Script>();
            lastFireTime = -999f;

            if (weapon == null)
            {
                Debug.LogWarning($"{agent.name} has no Weapon_Controller_Script — AttackTargetAction will do nothing!");
                EndAction(false);
                return;
            }

            // Stop moving if currently navigating
            agent.isStopped = true;
        }

        protected override void OnUpdate()
        {
            if (Target == null || Target.value == null || weapon == null)
            {
                EndAction(false);
                return;
            }

            Vector3 targetPos = Target.value.transform.position;
            float distance = Vector3.Distance(agent.transform.position, targetPos);

            // Face the target
            Vector3 dir = (targetPos - agent.transform.position);
            dir.y = 0;
            if (dir.sqrMagnitude > 0.001f)
                agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, Quaternion.LookRotation(dir.normalized), Time.deltaTime * 8f);

            // Out of range? Stop attacking so BT can switch to chase
            if (distance > maxAttackRange.value)
            {
                EndAction(false);
                return;
            }

            // Fire if cooldown expired
            if (Time.time >= lastFireTime + fireRate.value)
            {
                lastFireTime = Time.time;
                weapon.Fire();
            }
        }

        protected override void OnStop()
        {
            if (agent != null)
                agent.isStopped = false;
        }
    }
}