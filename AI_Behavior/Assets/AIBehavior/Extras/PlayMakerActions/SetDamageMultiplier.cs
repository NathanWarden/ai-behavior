#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class SetDamageMultiplier : AIPlayMakerActionBase
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
			ai.SetDamageMultiplier(multiplierAmount.Value);
		}
	}
}
#endif