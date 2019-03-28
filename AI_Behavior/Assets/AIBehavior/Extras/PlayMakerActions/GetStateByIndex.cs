#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetStateByIndex : GetState
	{
		[RequiredField]
		[UIHint(UIHint.FsmInt)]
		public FsmInt stateIndex;


		public override void Reset()
		{
			base.Reset();
			stateIndex = null;
		}


		protected override void DoAIAction ()
		{
			if (ai.currentState == null) return;

			state.Value = ai.GetStateByIndex(stateIndex.Value);
		}
	}
}
#endif