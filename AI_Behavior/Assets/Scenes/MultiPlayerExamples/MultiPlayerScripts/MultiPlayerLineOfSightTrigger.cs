using UnityEngine;

#if UNITY_EDITOR
using AIBehaviorEditor;
#endif


namespace AIBehavior
{
	public class MultiPlayerLineOfSightTrigger : BaseTrigger
	{
		public Transform sightTransformOverride;
		Transform seenPlayer;


		protected override bool Evaluate(AIBehaviors fsm)
		{
			seenPlayer = fsm.GetClosestPlayerWithinSight(objectFinder.GetTransforms(), true, sightTransformOverride);
			return seenPlayer != null;
		}
		
		protected override void ChangeToTransitionState(AIBehaviors fsm)
		{
			if (transitionState != null && transitionState.CanSwitchToState()) 
			{
				(transitionState as SeekState).specifySeekTarget = true;
				(transitionState as SeekState).seekTarget = seenPlayer;
				fsm.ChangeActiveState(transitionState);
			}
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