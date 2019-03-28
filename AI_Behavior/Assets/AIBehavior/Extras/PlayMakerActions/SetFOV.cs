#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class SetFOV : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmFloat)]
		public FsmFloat fov;


		public override void Reset()
		{
			base.Reset();
			fov = null;
		}


		protected override void DoAIAction ()
		{
			ai.sightFOV = fov.Value;
		}
	}
}
#endif