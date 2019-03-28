#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using AIBehavior;

using System.Collections.Generic;


namespace AIBehaviorEditor
{
	public abstract class AIBehaviorsAnimationEditorGUI
	{
		protected const string kArraySize = "animationStates.Array.size";
		protected const string kArrayData = "animationStates.Array.data[{0}]";

		static bool foldoutValue = false;
		static bool gotFoldoutValue = false;


		public static void OnInspectorEnabled(SerializedObject m_ParentObject, SerializedObject m_StateObject)
		{
			AssignAnimationStatesComponent(m_StateObject);
		}


		public static void DrawAnimationFields(SerializedObject m_StateObject, bool usesMultipleAnimations)
		{
			SerializedProperty m_animationStates = m_StateObject.FindProperty("animationStatesComponent");
			SerializedProperty statesProperty = m_StateObject.FindProperty("animationStates");
			int arraySize = statesProperty.arraySize;
			AIAnimationStates animStatesComponent;
			AIBehaviorsStyles styles = new AIBehaviorsStyles();
			bool newFoldoutValue;
			bool hadNullAnimation = false;

			const string foldoutValueKey = "AIBehaviors_AnimationsFoldout";

			AssignAnimationStatesComponent(m_StateObject);
			animStatesComponent = m_animationStates.objectReferenceValue as AIAnimationStates;

			if ( !gotFoldoutValue )
			{
				if ( EditorPrefs.HasKey(foldoutValueKey) )
					foldoutValue = EditorPrefs.GetBool(foldoutValueKey);

				gotFoldoutValue = true;
			}

			newFoldoutValue = foldoutValue;
			newFoldoutValue = EditorGUILayout.Foldout(foldoutValue, "Animations:", EditorStyles.foldoutPreDrop);

			if ( foldoutValue != newFoldoutValue )
			{
				foldoutValue = newFoldoutValue;
				EditorPrefs.SetBool(foldoutValueKey, foldoutValue);
			}

			// Check the array for null animations
			for ( int i = 0; i < arraySize; i++ )
			{
				if ( statesProperty.GetArrayElementAtIndex(i).objectReferenceValue == null )
				{
					if ( animStatesComponent != null )
					{
						statesProperty.GetArrayElementAtIndex(i).objectReferenceValue = animStatesComponent.states[0];
						hadNullAnimation = true;
					}
				}
			}

			if ( !foldoutValue || hadNullAnimation )
			{
				m_StateObject.ApplyModifiedProperties();
				return;
			}

			// Is the component assigned?
			if ( m_animationStates.objectReferenceValue != null && animStatesComponent != null )
			{
				GUILayout.BeginVertical(GUI.skin.box);

				AIAnimationState[] states = new AIAnimationState[arraySize];
				string[] animationStateNames = GetAnimationStateNames(m_StateObject);

				if ( animStatesComponent.states.Length == 0 )
				{
					Color oldColor = GUI.color;
					GUI.color = Color.yellow;
					GUILayout.Label("No states have been created\nfor the AIAnimationStates component.");
					GUI.color = oldColor;

					return;
				}

				for ( int i = 0; i < arraySize; i++ )
				{
					SerializedProperty prop = statesProperty.GetArrayElementAtIndex(i);
					bool oldEnabled = GUI.enabled;

					if ( prop != null )
					{
						Object obj = prop.objectReferenceValue;
						int curIndex;

						if ( obj != null )
						{
							states[i] = obj as AIAnimationState;
							curIndex = GetCurrentStateIndex(states[i], animStatesComponent.states);

							if ( curIndex == -1 )
							{
								if ( animStatesComponent.states.Length > 0 )
								{
									curIndex = 0;
									states[i] = animStatesComponent.states[0];
								}
							}

							GUILayout.BeginHorizontal();
							{
								curIndex = EditorGUILayout.Popup(curIndex, animationStateNames, EditorStyles.popup);
								m_StateObject.FindProperty(string.Format(kArrayData, i)).objectReferenceValue = animStatesComponent.states[curIndex];

								if ( usesMultipleAnimations )
								{
									GUI.enabled = i > 0;
									if ( GUILayout.Button(styles.blankContent, styles.upStyle, GUILayout.MaxWidth(styles.arrowButtonWidths)) )
									{
										statesProperty.MoveArrayElement(i, i-1);
									}

									GUI.enabled = i < arraySize-1;
									if ( GUILayout.Button(styles.blankContent, styles.downStyle, GUILayout.MaxWidth(styles.arrowButtonWidths)) )
									{
										statesProperty.MoveArrayElement(i, i+1);
									}

									GUI.enabled = true;
									if ( GUILayout.Button(styles.blankContent, styles.addStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
									{
										statesProperty.InsertArrayElementAtIndex(i);
									}

									GUI.enabled = arraySize > 1;
									if ( GUILayout.Button(styles.blankContent, styles.removeStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
									{
										AIBehaviorsAssignableObjectArray.RemoveObjectAtIndex(m_StateObject, i, "animationStates");
										GUILayout.EndHorizontal();
										break;
									}
									GUI.enabled = oldEnabled;
								}
							}
							GUILayout.EndHorizontal();
						}
						else
						{
							statesProperty.GetArrayElementAtIndex(i).objectReferenceValue = animStatesComponent.states[0];
						}
					}

					GUI.enabled = oldEnabled;
				}

				GUILayout.EndVertical();
			}

			m_StateObject.ApplyModifiedProperties();
		}


		public static void AssignAnimationStatesComponent(SerializedObject m_StateObject)
		{
			SerializedProperty prop = m_StateObject.FindProperty("animationStatesComponent");
			GameObject parentObject = (m_StateObject.targetObject as BaseState).transform.parent.gameObject;

			if ( prop.objectReferenceValue == null )
			{
				if ( parentObject != null )
				{
					prop.objectReferenceValue = parentObject.GetComponent<AIAnimationStates>();
					m_StateObject.ApplyModifiedProperties();
				}
			}
		}


		public static string[] GetAnimationStateNames(SerializedObject m_StateObject)
		{
			SerializedProperty m_animationStates = m_StateObject.FindProperty("animationStatesComponent");
			AIAnimationStates animStatesComponent = m_animationStates.objectReferenceValue as AIAnimationStates;
			bool animStatesNull = animStatesComponent == null;
			int animNamesSize = animStatesNull ? 0 : animStatesComponent.states.Length;
			string[] animationStateNames = new string[animNamesSize];

			for ( int i = 0; i < animationStateNames.Length; i++ )
			{
				if ( animStatesComponent.states[i] != null )
					animationStateNames[i] = animStatesComponent.states[i].name;
				else
					animationStateNames[i] = "";
			}

			return animationStateNames;
		}


		static int GetCurrentStateIndex(AIAnimationState curState, AIAnimationState[] states)
		{
			for ( int i = 0; i < states.Length; i++ )
			{
				if ( curState == states[i] )
					return i;
			}

			return -1;
		}
	}
}
#endif