#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetSightDistance : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmFloat)]
		public FsmFloat sightDistance;


		public override void Reset()
		{
			base.Reset();
			sightDistance = null;
		}


		protected override void DoAIAction ()
		{
			sightDistance.Value = ai.sightDistance;
		}
	}
}
#endif