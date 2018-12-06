#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetSightPosition : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmVector3)]
		public FsmVector3 sightPosition;


		public override void Reset()
		{
			base.Reset();
			sightPosition = null;
		}


		protected override void DoAIAction ()
		{
			sightPosition.Value = ai.GetSightPosition();
		}
	}
}
#endif