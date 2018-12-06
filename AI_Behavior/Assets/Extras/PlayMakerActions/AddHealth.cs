#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class AddHealth : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmFloat)]
		public FsmFloat healthAmount;


		public override void Reset()
		{
			base.Reset();
			healthAmount = null;
		}


		protected override void DoAIAction ()
		{
			ai.AddHealthValue(healthAmount.Value);
		}
	}
}
#endif