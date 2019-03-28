#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class SetCurrentStateByIndex : AIPlayMakerActionBase
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
			ai.ChangeActiveStateByIndex(stateIndex.Value);
		}
	}
}
#endif