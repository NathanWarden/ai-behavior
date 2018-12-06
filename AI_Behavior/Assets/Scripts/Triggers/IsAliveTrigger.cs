using UnityEngine;
using AIBehavior;
using System.Collections;

namespace AIBehavior
{
	/// <summary>
	/// Returns true if this AI has health above zero.
	/// </summary>
	public class IsAliveTrigger : BaseTrigger
	{
		protected override bool Evaluate(AIBehaviors fsm)
		{
			return fsm.health > 0;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Is Alive";
		}
	}
}