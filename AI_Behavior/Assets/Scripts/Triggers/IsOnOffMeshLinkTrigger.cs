using UnityEngine;
using AIBehavior;
using System.Collections;
using UnityEngine.AI;


namespace AIBehavior
{
	public class IsOnOffMeshLinkTrigger : BaseTrigger
	{
		NavMeshAgent navMeshAgent = null;
		bool hasNavMeshAgent = false;


		protected override void Init (AIBehaviors fsm)
		{
			if (navMeshAgent == null)
			{
				navMeshAgent = fsm.GetComponent<NavMeshAgent>();
			}

			hasNavMeshAgent = navMeshAgent != null;
		}


		protected override bool Evaluate (AIBehaviors fsm)
		{
			return hasNavMeshAgent && navMeshAgent.isOnOffMeshLink;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Is On Off Mesh Link";
		}
	}
}