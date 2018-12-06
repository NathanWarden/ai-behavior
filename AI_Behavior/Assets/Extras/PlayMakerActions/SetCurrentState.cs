#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class SetCurrentState : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmObject)]
		public FsmObject state;


		public override void Reset()
		{
			base.Reset();
			state = null;
		}


		protected override void DoAIAction ()
		{
			ai.ChangeActiveState(state.Value as BaseState);
		}
	}
}
#endif