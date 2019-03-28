using UnityEngine;

#if UNITY_EDITOR
using AIBehaviorEditor;
#endif


namespace AIBehavior
{
	public class LineOfSightTrigger : BaseTrigger
	{
		public Transform sightTransformOverride;


		protected override bool Evaluate(AIBehaviors fsm)
		{
			return fsm.GetClosestPlayerWithinSight(objectFinder.GetTransforms(), true, sightTransformOverride) != null;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Line Of Sight";
		}


#if UNITY_EDITOR
		public override void DrawGizmos(AIBehaviors fsm)
		{
			if (sightTransformOverride != null )
			{
				AIBehaviorsTriggerGizmos.DrawVisionCone(sightTransformOverride.position, sightTransformOverride.forward, fsm.sightDistance, fsm.sightFOV / 2.0f);
			}
		}
#endif
	}
}