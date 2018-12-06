#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class SetUseSightFalloff : AIPlayMakerActionBase
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
			ai.useSightFalloff = useSightFalloff.Value;
		}
	}
}
#endif