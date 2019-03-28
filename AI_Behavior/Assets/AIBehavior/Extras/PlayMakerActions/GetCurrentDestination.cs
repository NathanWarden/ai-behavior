#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetCurrentDestination : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmVector3)]
		public FsmVector3 destination;


		public override void Reset()
		{
			base.Reset();
			destination = null;
		}


		protected override void DoAIAction ()
		{
			destination.Value = ai.currentDestination;
		}
	}
}
#endif