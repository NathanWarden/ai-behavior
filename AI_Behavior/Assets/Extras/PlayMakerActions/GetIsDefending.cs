#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetIsDefending : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmBool)]
		public FsmBool isDefending;


		public override void Reset()
		{
			base.Reset();
			isDefending = null;
		}


		protected override void DoAIAction ()
		{
			isDefending.Value = ai.isDefending;
		}
	}
}
#endif