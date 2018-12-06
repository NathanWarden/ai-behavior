using AIBehavior;


namespace AIBehavior
{
	public class CurrentStateTrigger : BaseTrigger
	{
		public BaseState checkState;


		protected override bool Evaluate(AIBehaviors fsm)
		{
			return fsm.currentState == checkState;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Current State";
		}
	}
}