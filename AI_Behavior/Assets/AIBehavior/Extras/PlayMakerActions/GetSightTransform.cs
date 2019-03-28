#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class GetSightTransform : AIPlayMakerActionBase
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
			sightTransform.Value = ai.sightTransform.gameObject;
		}
	}
}
#endif