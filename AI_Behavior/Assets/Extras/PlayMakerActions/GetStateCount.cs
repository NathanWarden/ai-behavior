#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetStateCount : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmInt)]
		public FsmInt stateCount;


		public override void Reset()
		{
			base.Reset();
			stateCount = null;
		}


		protected override void DoAIAction ()
		{
			stateCount.Value = ai.stateCount;
		}
	}
}
#endif