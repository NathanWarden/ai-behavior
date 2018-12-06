#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetFOV : AIPlayMakerActionBase
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
			fov.Value = ai.sightFOV;
		}
	}
}
#endif