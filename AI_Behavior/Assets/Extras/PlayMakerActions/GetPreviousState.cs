#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetPreviousState : GetState
	{
		protected override void DoAIAction ()
		{
			if (ai.currentState == null) return;

			state.Value = ai.previousState;
		}
	}
}
#endif