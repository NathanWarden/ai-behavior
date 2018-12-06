using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif


namespace AIBehavior
{
	[ExcludeFromSubTriggersList]
	public class GoToRandomStateTrigger : BaseTrigger
	{
		public BaseState[] states;
		private BaseState result = null;
		public float[] weights;


		protected override bool Evaluate(AIBehaviors fsm)
		{
			return GetRandomResult(fsm, out result, Random.value);
		}


		public bool GetRandomResult(AIBehaviors fsm, out BaseState resultState, float randomValue)
		{
			resultState = null;

			if ( states.Length > 0 )
			{
				float totalWeight = 1.0f;

				for ( int i = 0; i < states.Length; i++ )
				{
					if ( randomValue > totalWeight - weights[i] )
					{
						resultState = states[i];
						return true;
					}

					totalWeight -= weights[i];
				}
			}
			else
			{
				Debug.LogWarning("The 'Go To Random State Trigger' requires at least 1 possible state!");
			}

			resultState = transitionState;
			return true;
		}


		protected override void ChangeToTransitionState (AIBehaviors fsm)
		{
			fsm.ChangeActiveState(result);
		}


		public override bool CanInvertResult ()
		{
			return false;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Go To Random State";
		}

		
#if UNITY_EDITOR
		public override void DrawInspectorProperties(AIBehaviors fsm, SerializedObject sObject)
		{
			SerializedProperty statesArray = sObject.FindProperty("states");
			SerializedProperty weightsArray = sObject.FindProperty("weights");
			int arraySize = statesArray.arraySize;
			float total = 0.0f;
			string transitionName = transitionState != null ? transitionState.name : "";

			arraySize = EditorGUILayout.IntField("Number of possible states", arraySize);
			statesArray.arraySize = arraySize;
			weightsArray.arraySize = arraySize;

			for ( int i = 0; i < arraySize; i++ )
			{
				EditorGUILayout.BeginHorizontal();
				{
					SerializedProperty stateProperty = statesArray.GetArrayElementAtIndex(i);
					SerializedProperty weightsProperty = weightsArray.GetArrayElementAtIndex(i);

					total += weightsProperty.floatValue;
					stateProperty.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(fsm, stateProperty.objectReferenceValue as BaseState);

					EditorGUILayout.Slider(weightsProperty, 0.0f, 1.0f, GUIContent.none);
				}
				EditorGUILayout.EndHorizontal();
			}
			
			if ( total > 1.0f )
			{
				Color oldColor = GUI.color;
				GUI.color = Color.red;
				GUILayout.Label("All possibilities combined must be less than 1");
				GUI.color = oldColor;
			}

			GUILayout.Label("If the total of the numbers above are less than 1,");
			GUILayout.Label("the remaining possiblity will go to the");
			GUILayout.Label("'Change to State' below which is: " + transitionName);

			sObject.ApplyModifiedProperties();
		}
#endif
	}
}