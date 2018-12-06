#if USE_PLAYMAKER
using HutongGames.PlayMaker;


namespace AIBehavior
{
	[ActionCategory("AI Behavior")]
	public abstract class AIPlayMakerActionBase : FsmStateAction
	{
		[UIHint(UIHint.FsmObject)]
		public FsmObject aiObject;
		protected AIBehaviors ai { get { return aiObject.Value as AIBehaviors; } }
		public bool everyFrame;


		public override void Reset()
		{
			everyFrame = false;
		}


		public override void Awake ()
		{
			if ( aiObject.Value == null )
			{
				aiObject.Value = Owner.GetComponent<AIBehaviors>();
			}
		}


		public override void OnEnter()
		{
			if ( ai != null )
			{
				DoAIAction();
			}

			if (!everyFrame)
				Finish();
		}

		public override void OnUpdate()
		{
			if ( ai == null ) return;
			DoAIAction();
		}


		protected abstract void DoAIAction();
	}
}
#endif