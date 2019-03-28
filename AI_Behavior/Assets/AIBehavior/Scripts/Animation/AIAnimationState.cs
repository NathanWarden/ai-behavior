using UnityEngine;


namespace AIBehavior
{
	public class AIAnimationState : MonoBehaviour
	{
		public new string name = "Untitled";

		public int startFrame = 0;
		public int endFrame = 0;

		public float speed = 1.0f;

		public WrapMode animationWrapMode = WrapMode.Loop;

		public bool crossFadeIn = false;
		public bool crossFadeOut = false;
		public float transitionTime = 0.2f;

		public bool foldoutOpen = false;
	}
}