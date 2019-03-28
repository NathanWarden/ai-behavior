#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetDamageMultiplier : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmFloat)]
		public FsmFloat multiplierAmount;


		public override void Reset()
		{
			base.Reset();
			multiplierAmount = null;
		}


		protected override void DoAIAction ()
		{
			multiplierAmount.Value = ai.GetDamageMultiplier();
		}
	}
}
#endif