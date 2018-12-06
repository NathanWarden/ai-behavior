#if USE_PLAYMAKER
using HutongGames.PlayMaker;

namespace AIBehavior
{
	public class PlayCurrentStateAudio : AIPlayMakerActionBase
	{
		protected override void DoAIAction ()
		{
			ai.PlayAudio();
		}
	}
}
#endif