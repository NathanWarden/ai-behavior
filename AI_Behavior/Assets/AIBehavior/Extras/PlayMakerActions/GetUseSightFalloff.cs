#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetUseSightFalloff : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmBool)]
		public FsmBool useSightFalloff;


		public override void Reset()
		{
			base.Reset();
			useSightFalloff = null;
		}


		protected override void DoAIAction ()
		{
			useSightFalloff.Value = ai.useSightFalloff;
		}
	}
}
#endif