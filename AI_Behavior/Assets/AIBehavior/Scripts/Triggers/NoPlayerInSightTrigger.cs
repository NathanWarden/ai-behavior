using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AIBehavior
{
	public class NoPlayerInSightTrigger : BaseTrigger
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
		
		
		public override string DefaultDisplayName()
		{
			return "No Player In Sight";
		}
	}
}