#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class SetSightDistance : AIPlayMakerActionBase
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
			ai.sightDistance = sightDistance.Value;
		}
	}
}
#endif