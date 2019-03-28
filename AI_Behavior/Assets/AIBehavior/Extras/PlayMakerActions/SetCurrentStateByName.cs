#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class SetCurrentStateByName : AIPlayMakerActionBase
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
			ai.ChangeActiveStateByName(stateName.Value);
		}
	}
}
#endif