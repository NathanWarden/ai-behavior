#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GotHit : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmFloat)]
		public FsmFloat damage;


		public override void Reset()
		{
			base.Reset();
			damage = null;
		}


		protected override void DoAIAction ()
		{
			ai.GotHit(damage.Value);
		}
	}
}
#endif