#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public abstract class GetState : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmObject state;


		public override void Reset()
		{
			base.Reset();
			state = null;
		}
	}
}
#endif