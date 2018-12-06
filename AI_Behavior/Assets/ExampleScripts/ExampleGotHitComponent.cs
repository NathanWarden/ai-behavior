using UnityEngine;
using AIBehavior;


namespace AIBehaviorExamples
{
	public class ExampleGotHitComponent : MonoBehaviour
	{
		AIBehaviors fsm = null;


		void Awake()
		{
			fsm = GetComponent<AIBehaviors>();
		}

		public void OnStartDefending(DefendState defendState)
		{
			// Code here for when the defend state begins
			//    Use defendState.defensiveBonus to get the defensive bonus value
			fsm.SetDamageMultiplier (defendState.defensiveBonus);
			Debug.Log ("Defending, now damage multiplier is " + defendState.defensiveBonus);
		}


		public void OnStopDefending(DefendState defendState)
		{
			// Code here for when the defend state ends
			fsm.SetDamageMultiplier (1.0f);
			Debug.Log ("Stop defending, now damage multiplier is 1");
		}

		public void Damage(float damage)
		{
			fsm.Damage(damage);
			BaseState hitState = fsm.GetStateByName ("Got Hit");
			if (hitState != null)
				fsm.ChangeActiveState (hitState);
		}
	}
}