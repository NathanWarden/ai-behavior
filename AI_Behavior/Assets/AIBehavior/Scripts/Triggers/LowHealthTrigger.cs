namespace AIBehavior
{
	public class LowHealthTrigger : HealthTrigger
	{
		public override bool IsThresholdCrossed(AIBehaviors fsm)
		{
			return fsm.GetHealthValue() <= healthThreshold;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Low Health";
		}


		#if UNITY_EDITOR
		protected override string GetDescriptionText ()
		{
			return "Below Health";
		}


		protected override string GetToolTipText ()
		{
			return "Triggered if the health is less than or equal to this value.";
		}
		#endif
	}
}