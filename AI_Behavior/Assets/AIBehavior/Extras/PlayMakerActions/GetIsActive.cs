#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetIsActive : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmBool)]
		public FsmBool isActive;


		public override void Reset()
		{
			base.Reset();
			isActive = null;
		}


		protected override void DoAIAction ()
		{
			isActive.Value = ai.isActive;
		}
	}
}
#endif