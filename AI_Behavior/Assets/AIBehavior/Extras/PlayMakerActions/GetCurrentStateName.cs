#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetCurrentStateName : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		public FsmString stateName;


		public override void Reset()
		{
			base.Reset();
			stateName = null;
		}


		protected override void DoAIAction ()
		{
			if (ai.currentState == null) return;

			stateName.Value = ai.currentState.name;
		}
	}
}
#endif