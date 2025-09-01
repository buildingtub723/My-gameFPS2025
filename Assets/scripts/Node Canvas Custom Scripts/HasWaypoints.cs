using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace LoneWolf.AI
{
    [Category("AI/Conditions")]
    public class HasWaypoints : ConditionTask
    {
        // Assign your waypoint array/list in the Blackboard
        public BBParameter<Transform[]> Waypoints;

        protected override bool OnCheck()
        {
            return Waypoints != null && Waypoints.value.Length > 0;
        }
    }
}
