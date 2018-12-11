using UnityEngine;
using System.Collections.Generic;


namespace AIBehavior
{
	public class AIAnimationStates : MonoBehaviour
	{
		public AnimationType animationType = AnimationType.Auto;

		public AIAnimationState[] states = new AIAnimationState[1];

		public GameObject animationStatesGameObject = null;

		private Dictionary<string, AIAnimationState> statesDictionary = new Dictionary<string, AIAnimationState>();


		public AIAnimationState GetStateWithName(string stateName)
		{
			if ( statesDictionary.ContainsKey(stateName) )
			{
				return statesDictionary[stateName];
			}

			for ( int i = 0; i < states.Length; i++ )
			{
				if ( states[i].name == stateName )
				{
					statesDictionary[stateName] = states[i];
					return states[i];
				}
			}

			return null;
		}
	}
}