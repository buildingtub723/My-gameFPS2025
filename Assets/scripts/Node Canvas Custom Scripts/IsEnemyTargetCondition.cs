using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace LoneWolf.AI
{
    [Category("AI/Conditions")]
    public class IsEnemyTargetCondition : ConditionTask
    {
        public BBParameter<GameObject> Target;

        protected override bool OnCheck()
        {
            if (Target == null || Target.value == null) return false;
            var myTeam = agent.GetComponent<TeamIdentity>();
            var otherTeam = Target.value.GetComponent<TeamIdentity>();
            if (myTeam == null || otherTeam == null) return true; // no data: assume enemy (or choose false)
            return myTeam.team != otherTeam.team;
        }
    }
}
