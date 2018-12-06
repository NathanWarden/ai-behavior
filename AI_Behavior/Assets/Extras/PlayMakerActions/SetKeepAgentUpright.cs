#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class SetKeepAgentUpright : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmBool)]
		public FsmBool keepAgentUpright;


		public override void Reset()
		{
			base.Reset();
			keepAgentUpright = null;
		}


		protected override void DoAIAction ()
		{
			ai.keepAgentUpright = keepAgentUpright.Value;
		}
	}
}
#endif