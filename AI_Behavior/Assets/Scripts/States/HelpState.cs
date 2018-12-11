using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif


namespace AIBehavior
{
	public class HelpState : BaseState
	{
		public HelpCondition helpCondition;
		public BaseState[] helpStates;	// When the AI is in one of this states it will help
		public float withinHelpPointDistance = 1.0f;
		public Vector3 helpPoint;
		public BaseState helpPointReachedState = null;

		public enum HelpCondition
		{
			AlwaysHelp,
			WhenInSpecificStates,
		}


		protected override void Init(AIBehaviors fsm)
		{
			fsm.PlayAudio();
		}


		protected override void StateEnded(AIBehaviors fsm)
		{
		}


		protected override bool Reason(AIBehaviors fsm)
		{
			if ( Vector3.Distance(fsm.aiTransform.position, helpPoint) < withinHelpPointDistance )
			{
				if ( helpPointReachedState != null )
				{
					fsm.ChangeActiveState(helpPointReachedState);
					return false;
				}
			}

			return true;
		}


		protected override void Action(AIBehaviors fsm)
		{
			fsm.MoveAgent(helpPoint, movementSpeed, rotationSpeed);
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Help";
		}

		public bool CanHelp(AIBehaviors fsm)
		{
			List<BaseState> statesList = new List<BaseState> ();
			statesList.AddRange(helpStates);

			if (helpCondition == HelpCondition.AlwaysHelp)
				return true;
			else if (statesList.Contains (fsm.currentState))
				return true;

			return false;
		}


		#if UNITY_EDITOR
		// === Editor Methods === //

		public override void OnStateInspectorEnabled(SerializedObject m_ParentObject)
		{
		}


		protected override void DrawStateInspectorEditor(SerializedObject stateObject, AIBehaviors fsm)
		{
			SerializedProperty property;

			GUILayout.Label ("Help Properties:", EditorStyles.boldLabel);
			
			GUILayout.BeginVertical(GUI.skin.box);

			property = stateObject.FindProperty("helpCondition");
			EditorGUILayout.PropertyField(property);
			if (helpCondition == HelpCondition.WhenInSpecificStates) 
			{
				property = stateObject.FindProperty("helpStates");

				int arraySize = property.arraySize;
				arraySize = EditorGUILayout.IntField("Number Of States", arraySize);
				property.arraySize = arraySize;

				SerializedProperty stateProperty;
				for (int i = 0; i < arraySize; i++) 
				{
					stateProperty = property.GetArrayElementAtIndex(i);
					stateProperty.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(fsm, stateProperty.objectReferenceValue as BaseState);
				}
				//InspectorHelper.DrawArray (stateObject, "helpStates");
			}

			property = stateObject.FindProperty("withinHelpPointDistance");
			EditorGUILayout.PropertyField(property);

			EditorGUILayout.Separator();

			GUILayout.BeginHorizontal ();
			GUILayout.Label("State When Help Point Reached");
			property = stateObject.FindProperty("helpPointReachedState");
			property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(fsm, property.objectReferenceValue as BaseState);
			GUILayout.EndHorizontal ();

			GUILayout.EndVertical();

			stateObject.ApplyModifiedProperties();

			if ( Application.isPlaying )
			{
				GUILayout.Label ("Debug:", EditorStyles.boldLabel);
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.Label("Distance to target: " + (transform.position - helpPoint).magnitude.ToString(), EditorStyles.boldLabel);
				GUILayout.EndVertical();
			}
		}
		#endif
	}
}