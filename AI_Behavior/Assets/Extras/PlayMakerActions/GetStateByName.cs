#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetStateByName : GetState
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

			state.Value = ai.GetStateByName(stateName.Value);
		}
	}
}
#endif