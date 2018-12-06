using UnityEngine;
using AIBehavior;


namespace AIBehaviorExamples
{
	public class ExampleCallbacks : MonoBehaviour
	{
		void OnEnable()
		{
			AIBehaviors aiBehaviors = GetComponent<AIBehaviors>();

			aiBehaviors.onStateChanged += OnStateChanged;
			aiBehaviors.onPlayAnimation += OnPlayAnimation;
		}


		void OnDisable()
		{
			AIBehaviors aiBehaviors = GetComponent<AIBehaviors>();

			aiBehaviors.onStateChanged -= OnStateChanged;
			aiBehaviors.onPlayAnimation -= OnPlayAnimation;
		}


		void OnStateChanged(BaseState newState, BaseState previousState)
		{
			Debug.Log("Changed from " + previousState.name + " to " + newState.name);
		}


		void OnPlayAnimation(AIAnimationState animationState)
		{
			Debug.Log("Play " + animationState.name);
		}
	}
}