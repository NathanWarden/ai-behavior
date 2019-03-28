using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AIBehavior
{
	public class CustomNoPlayerInSightTrigger : BaseTrigger
	{
		public float triggerAfterTime = 0.0f;
		private float triggerTime = 0.0f;
		private bool timerStarted = false;


		protected override void Init(AIBehaviors fsm)
		{
		}


		protected override bool Evaluate(AIBehaviors fsm)
		{
			bool cantSeePlayer = fsm.GetClosestPlayerWithinSight(objectFinder.GetTransforms(), false) == null;

			if ( cantSeePlayer )
			{
				if ( timerStarted )
				{
					if ( triggerTime < Time.time )
					{
						return true;
					}
				}
				else
				{
					triggerTime = Time.time + triggerAfterTime;
					timerStarted = true;
				}
			}
			else
			{
				timerStarted = false;
			}

			return false;
		}

		protected override void ChangeToTransitionState(AIBehaviors fsm)
		{
			if (transitionState != null && transitionState.CanSwitchToState()) 
			{
				if(fsm.previousState is AttackState && transitionState is SeekState)
				{
					(transitionState as SeekState).specifySeekTarget = true;
					(transitionState as SeekState).seekTarget = (fsm.previousState as AttackState).GetLastKnownTarget();
				}
					
				fsm.ChangeActiveState(transitionState);
			}
		}
		
		
		public override string DefaultDisplayName()
		{
			return "No Player In Sight";
		}
	}
}