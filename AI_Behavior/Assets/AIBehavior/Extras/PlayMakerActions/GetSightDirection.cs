#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetSightDirection : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmVector3)]
		public FsmVector3 sightDirection;


		public override void Reset()
		{
			base.Reset();
			sightDirection = null;
		}


		protected override void DoAIAction ()
		{
			sightDirection.Value = ai.GetSightDirection();
		}
	}
}
#endif