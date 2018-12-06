#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class SetSightTransform : AIPlayMakerActionBase
	{
		[RequiredField]
		[UIHint(UIHint.FsmGameObject)]
		public FsmGameObject sightTransform;


		public override void Reset()
		{
			base.Reset();
			sightTransform = null;
		}


		protected override void DoAIAction ()
		{
			ai.sightTransform = sightTransform.Value.transform;
		}
	}
}
#endif