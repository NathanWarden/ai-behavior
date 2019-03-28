#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class SetUseSightTransform : AIPlayMakerActionBase
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
			ai.useSightTransform = useSightTransform.Value;
		}
	}
}
#endif