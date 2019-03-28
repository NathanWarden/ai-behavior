using UnityEngine;
using AIBehavior;
using System.Collections;

namespace AIBehaviorExamples
{
	public class ExampleDefendScript : MonoBehaviour
	{
		public void OnStartDefending(DefendState defendState)
		{
			Debug.Log ("Start Defending");
			GetComponent<AIBehaviors>().SetDamageMultiplier(defendState.defensiveBonus);
		}


		public void OnStopDefending(DefendState defendState)
		{
			Debug.Log ("Stop Defending");
			GetComponent<AIBehaviors>().SetDamageMultiplier(1.0f);
		}
	}
}
