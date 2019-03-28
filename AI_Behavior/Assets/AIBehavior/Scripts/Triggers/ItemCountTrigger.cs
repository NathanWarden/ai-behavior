namespace AIBehavior
{
	public class ItemCountTrigger : BaseTrigger
	{
		public enum ComparisonCondition
		{
			LessThan,
			GreaterThan,
			EqualTo
		}
		public ComparisonCondition comparisonCondition = ComparisonCondition.LessThan;
		public int itemCount = 1;


		protected override bool Evaluate (AIBehaviors fsm)
		{
			return CheckCount();
		}


		public bool CheckCount()
		{
			int currentCount = objectFinder.GetTransforms().Length;

			switch (comparisonCondition)
			{
			case ComparisonCondition.LessThan:
				return currentCount < itemCount;
			case ComparisonCondition.GreaterThan:
				return currentCount > itemCount;
			case ComparisonCondition.EqualTo:
				return currentCount == itemCount;
			default:
				return false;
			}
		}
	}
}