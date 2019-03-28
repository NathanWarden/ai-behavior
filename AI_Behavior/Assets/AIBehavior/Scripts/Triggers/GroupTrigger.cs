using UnityEngine;


namespace AIBehavior
{
	public class GroupTrigger : BaseTrigger
	{
		protected override bool Evaluate(AIBehaviors fsm)
		{
			return true;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Group";
		}
	}
}