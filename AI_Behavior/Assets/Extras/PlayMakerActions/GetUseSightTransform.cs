#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetUseSightTransform : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmBool)]
		public FsmBool useSightTransform;


		public override void Reset()
		{
			base.Reset();
			useSightTransform = null;
		}


		protected override void DoAIAction ()
		{
			useSightTransform.Value = ai.useSightTransform;
		}
	}
}
#endif