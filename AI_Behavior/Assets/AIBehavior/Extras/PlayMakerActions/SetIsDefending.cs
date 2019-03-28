#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class SetIsDefending : AIPlayMakerActionBase
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
			ai.isDefending = isDefending.Value;
		}
	}
}
#endif