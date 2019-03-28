namespace AIBehavior
{
	public class HighHealthTrigger : HealthTrigger
	{
		public override bool IsThresholdCrossed(AIBehaviors fsm)
		{
			return fsm.GetHealthValue() > healthThreshold;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "High Health";
		}


		#if UNITY_EDITOR
		protected override string GetDescriptionText ()
		{
			return "Above Health";
		}


		protected override string GetToolTipText ()
		{
			return "Triggered if the health is greater than (but not equal to) this value.";
		}
		#endif
	}
}