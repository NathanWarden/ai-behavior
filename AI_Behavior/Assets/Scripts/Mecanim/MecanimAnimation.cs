using UnityEngine;


namespace AIBehavior
{
	public class MecanimAnimation : MonoBehaviour
	{
		Animator animator;
		AIAnimationState prevAnim = null;


		void Awake()
		{
			animator = GetComponentInChildren<Animator>();
			if(animator == null)
				animator = GetComponentInParent<Animator>();
		}


		public void OnAnimationState(AIAnimationState animData)
		{
			string animName = animData.name;

			if ( animData != prevAnim )
			{
				if ( animData.crossFadeIn || (prevAnim != null && prevAnim.crossFadeOut) )
				{
					animator.CrossFade(animName, animData.transitionTime);
				}

				prevAnim = animData;
			}
		}
	}
}