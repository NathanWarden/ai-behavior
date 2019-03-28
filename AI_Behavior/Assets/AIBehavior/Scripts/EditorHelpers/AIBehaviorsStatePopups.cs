#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using AIBehavior;

using System.Collections.Generic;


namespace AIBehaviorEditor
{
	public static class AIBehaviorsStatePopups
	{
		public static BaseState DrawEnabledStatePopup (AIBehaviors fsm, BaseState curState)
		{
			return DrawEnabledStatePopup<BaseState>(fsm, curState);
		}


		public static T DrawEnabledStatePopup<T> (AIBehaviors fsm, T curState) where T : BaseState
		{
			List<string> statesList = new List<string>();
			Dictionary<string, T> statesDictionary = new Dictionary<string, T>();
			T[] states = fsm.GetStates<T>();
			string[] stateSelections;
			int selection = 0;

			for ( int i = 0; i < states.Length; i++ )
			{
				if ( states[i].isEnabled )
				{
					string stateName = states[i].name;

					statesList.Add(stateName);
					statesDictionary[stateName] = states[i] as T;

					if ( states[i] == curState )
					{
						selection = statesList.Count-1;
					}
				}
			}

			stateSelections = statesList.ToArray();
			selection = EditorGUILayout.Popup(selection, stateSelections, GUILayout.MaxWidth(75));

			return statesDictionary[stateSelections[selection]];
		}
	}
}
#endif