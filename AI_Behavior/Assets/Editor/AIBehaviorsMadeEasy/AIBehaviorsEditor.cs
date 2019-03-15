#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using AIBehavior;
using System;
using System.Reflection;
using System.Collections.Generic;

using Object = UnityEngine.Object;


namespace AIBehaviorEditor
{
	[CustomEditor(typeof(AIBehaviors))]
	public class AIBehaviorsEditor : Editor
	{
		const string selectedStateKey = "AIBehaviors_SelectedState";

		const string kStatesArraySize = "states.Array.size";
		const string kStatesArrayData = "states.Array.data[{0}]";

		const int arrowButtonWidths = 18;
		const int addRemoveButtonWidths = 16;

		SerializedObject m_Object;
		Transform transform;
		GameObject statesGameObject;

		private int curStateSelection = 0;
		private int prevStateSelection = -1;
		AIBehaviors fsm = null;
		bool inittedSuccessfully = true;

		string[] derivedStateNames;

		AIBehaviorsStyles styles;


		void OnEnable()
		{
			BaseState[] states;

			styles = new AIBehaviorsStyles();

			m_Object = new SerializedObject(target);
			fsm = m_Object.targetObject as AIBehaviors;
			transform = fsm.transform;

			derivedStateNames = AIBehaviorsComponentInfoHelper.GetStateTypeNames();

			curStateSelection = 0;
			prevStateSelection = 0;

			// Sorts old states and initializes new states
			InitStates();
			List<BaseState> statesList = new List<BaseState>(fsm.GetAllStates());

			for ( int i = 0; i < statesList.Count; i++ )
			{
				if ( statesList[i] == null )
				{
					statesList.RemoveAt(i);
					i--;
				}
			}

			states = statesList.ToArray();
			fsm.ReplaceAllStates(states);
			
			for ( int i = 0; i < states.Length; i++ )
			{
				states[i].OnInspectorEnabled(m_Object);
			}
			
			if ( EditorPrefs.HasKey(selectedStateKey) )
			{
				string stateName = EditorPrefs.GetString(selectedStateKey);
				
				for ( int i = 0; i < states.Length; i++ )
				{
					if ( stateName == states[i].name )
					{
						curStateSelection = i;
						prevStateSelection = i;
					}
				}
			}
		}


		public override void OnInspectorGUI()
		{
			BaseState[] states = fsm.GetAllStates();

			if ( prevStateSelection != curStateSelection )
			{
				prevStateSelection = curStateSelection;
				EditorUtility.SetDirty(fsm.GetStateByIndex(curStateSelection));
			}

			if ( inittedSuccessfully )
			{
				m_Object.Update();

				if ( curStateSelection >= states.Length )
				{
					curStateSelection = 0;
				}

				GUILayout.Space(10);
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.Space(10);

				for ( int i = 0; i < fsm.stateCount; i++ )
				{
					string stateDisplayName;

					if ( states[i] == null || states[i].name == null )
					{
						Undo.RecordObject(fsm, "Removed null state");
						AIBehaviorsAssignableObjectArray.RemoveObjectAtIndex(m_Object, i, "states");
						m_Object.ApplyModifiedProperties();
						return;
					}

					stateDisplayName = states[i].name;

					GUILayout.BeginHorizontal();
					{
						const int guiWidths = 90;

						if ( GUILayout.Button("Edit", GUILayout.MaxWidth(50)) )
						{
							curStateSelection = i;

							EditorPrefs.SetString(selectedStateKey, stateDisplayName);
						}

						GUILayout.Space(10);

						Color oldColor = GUI.color;
						string updatedName;

						if ( Application.isPlaying && states[i] == fsm.currentState )
						{
							GUI.color = Color.green;
						}
						else if ( !Application.isPlaying && states[i] == fsm.initialState )
						{
							GUI.color = Color.green;
						}

						updatedName = GUILayout.TextField(states[i].name, GUILayout.MaxWidth(guiWidths));

						GUI.color = oldColor;

						UpdateStateName(updatedName, states[i]);

						int curState = 0;
						int newIndex = 0;

						for ( int j = 0; j < derivedStateNames.Length; j++ )
						{
							if ( states[i].GetType().Name == derivedStateNames[j] )
							{
								curState = j;
								break;
							}
						}

						newIndex = EditorGUILayout.Popup(curState, derivedStateNames, GUILayout.MaxWidth(guiWidths));

						if ( curState != newIndex )
						{
							SerializedProperty m_Prop = m_Object.FindProperty(string.Format(kStatesArrayData, i));
							string newName;

							DestroyImmediate(m_Prop.objectReferenceValue);
							string typeName = derivedStateNames[newIndex];
							m_Prop.objectReferenceValue = ComponentHelper.AddComponentByName(fsm.statesGameObject, typeName);
							newName = AIBehaviorsComponentInfoHelper.GetNameFromType(typeName);
							UpdateStateName(newName, (m_Prop.objectReferenceValue as BaseState));
						}

						// Draw Up, Down, and Remove
						bool oldEnabled = GUI.enabled;
						GUI.enabled = i != 0;
						if ( GUILayout.Button(styles.blankContent, styles.upStyle, GUILayout.MaxWidth(styles.arrowButtonWidths)) )
						{
							AIBehaviorsAssignableObjectArray.Swap(m_Object, i, i-1, kStatesArrayData);
						}

						GUI.enabled = i < states.Length-1;
						if ( GUILayout.Button(styles.blankContent, styles.downStyle, GUILayout.MaxWidth(styles.arrowButtonWidths)) )
						{
							AIBehaviorsAssignableObjectArray.Swap(m_Object, i, i+1, kStatesArrayData);
						}

						GUI.enabled = true;


						if ( GUILayout.Button(styles.blankContent, styles.addStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
						{
							SerializedProperty prop = m_Object.FindProperty("states");
							prop.InsertArrayElementAtIndex(i);
							BaseState prevState = m_Object.FindProperty(string.Format(kStatesArrayData, i)).objectReferenceValue as BaseState;
							string typeName = prevState.GetType().Name;
							BaseState newState = ComponentHelper.AddComponentByName(statesGameObject, typeName) as BaseState;
							newState.name = prevState.name;
							UpdateStateName(prevState.name, newState);
							m_Object.FindProperty(string.Format(kStatesArrayData, i+1)).objectReferenceValue = newState;
							Undo.RegisterCreatedObjectUndo(statesGameObject, "Added New State");
						}

						GUI.enabled = states.Length > 1;
						if ( GUILayout.Button(styles.blankContent, styles.removeStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
						{
							BaseState state = m_Object.FindProperty(string.Format(kStatesArrayData, i)).objectReferenceValue as BaseState;

							Undo.RecordObject(state, "Remove a State");

							foreach ( BaseTrigger trigger in state.triggers )
							{
								DestroyImmediate(trigger, true);
							}

							DestroyImmediate(state, true);

							AIBehaviorsAssignableObjectArray.RemoveObjectAtIndex(m_Object, i, "states");
							m_Object.ApplyModifiedProperties();
							return;
						}

						GUI.enabled = oldEnabled;

						if ( EditorGUILayout.Toggle(styles.blankContent, states[i].isEnabled, GUILayout.MaxWidth(25)) != states[i].isEnabled )
						{
							states[i].isEnabled = !states[i].isEnabled;
						}
					}
					GUILayout.EndHorizontal();
				}

				EditorGUILayout.Separator();
				DrawInitialStatePopup();

				GUILayout.EndVertical();

				EditorGUILayout.Separator();
				DrawGeneralAgentProperties();
				
				EditorGUILayout.Separator();
				fsm.objectFinder.DrawPlayerTagsSelection(fsm, m_Object, "objectFinder", true);

				AIBehaviorsTriggersGUI.Draw(m_Object, fsm, "Global Triggers:", "AIBehaviors_GlobalTriggersFoldout");		

				EditorGUILayout.Separator();
				DrawAnimationCallbackSelection();

				m_Object.ApplyModifiedProperties();

				// Draw Individual states below

				AIBehaviorsGeneralEditorGUI.Separator();
				GUILayout.BeginHorizontal();
				GUILayout.Label("");
				GUILayout.Label("-- " + states[curStateSelection].name + " (" + states[curStateSelection].GetType().ToString() + ") --", EditorStyles.boldLabel);
				GUILayout.Label("");
				GUILayout.EndHorizontal();
				AIBehaviorsGeneralEditorGUI.Separator();
				states[curStateSelection].DrawInspectorEditor(fsm);

				EditorGUILayout.Separator();
			}
			else
			{
				GUI.contentColor = Color.red;
				GUILayout.Label("You must fix your errors before editting.");
			}
		}


		void UpdateStateName(string updatedName, BaseState state)
		{
			if ( state != null && updatedName != state.name )
			{
				SerializedObject serializedObject = new SerializedObject(state);
				serializedObject.FindProperty("name").stringValue = updatedName;
				serializedObject.ApplyModifiedProperties();
			}
		}


		void InitStates()
		{
			SerializedProperty m_Prop = m_Object.FindProperty("statesGameObject");

			if ( m_Prop.objectReferenceValue == null )
			{
				statesGameObject = new GameObject("States");
				m_Prop.objectReferenceValue = statesGameObject;

				statesGameObject.transform.parent = transform;
				statesGameObject.transform.localPosition = Vector3.zero;
				statesGameObject.transform.localRotation = Quaternion.identity;
				statesGameObject.transform.localScale = Vector3.one;

				m_Object.ApplyModifiedProperties();
			}
			else
			{
				statesGameObject = m_Prop.objectReferenceValue as GameObject;
			}

			GetExistingStates();

			if ( fsm.states.Length == 0 )
			{
				InitNewStates();
			}
		}


		void InitNewStates()
		{
			Dictionary<string, Type> statesDictionary = new Dictionary<string, Type>();
			List<BaseState> statesList = new List<BaseState>();
			bool isMecanim = fsm.GetComponent<Animator>() != null;

			// Setup a dictionary of the default states
			statesDictionary["Idle"] = typeof(IdleState);
			statesDictionary["Patrol"] = typeof(PatrolState);
			statesDictionary["Seek"] = typeof(SeekState);
			statesDictionary["Flee"] = typeof(FleeState);

			if ( isMecanim )
			{
				statesDictionary["Attack"] = typeof(MecanimAttackState);
			}
			else
			{
				statesDictionary["Attack"] = typeof(AttackState);
			}

			statesDictionary["Defend"] = typeof(DefendState);
			statesDictionary["Got Hit"] = typeof(GotHitState);
			statesDictionary["Change"] = typeof(ChangeState);
			statesDictionary["Help"] = typeof(HelpState);
			statesDictionary["Get Help"] = typeof(GetHelpState);
			statesDictionary["Dead"] = typeof(DeadState);

			foreach ( string stateName in statesDictionary.Keys )
			{
				string stateClassName = statesDictionary[stateName].Name;

				try
				{
					BaseState baseState = ComponentHelper.AddComponentByName(statesGameObject, stateClassName) as BaseState;
					baseState.name = stateName;

					statesList.Add(baseState);
				}
				catch
				{
					Debug.LogError("Type \"" + stateClassName + "\" does not exist.  You must have a class named \"" + stateClassName + "\" that derives from \"StateBase\".");
					inittedSuccessfully = false;
				}
			}

			fsm.ReplaceAllStates(statesList.ToArray());
		}


		void GetExistingStates()
		{
			BaseState[] states = fsm.GetAllStates();

			foreach ( BaseState state in states )
			{
				if ( state != null && string.IsNullOrEmpty(state.name) )
				{
					SerializedObject sObject = new SerializedObject(state);
					SerializedProperty mProp = sObject.FindProperty("name");
					string stateTypeName = state.GetType().ToString();

					mProp.stringValue = AIBehaviorsComponentInfoHelper.GetNameFromType(stateTypeName);
					sObject.ApplyModifiedProperties();
				}
			}
		}


		void DrawInitialStatePopup()
		{
			SerializedProperty m_InitialState = m_Object.FindProperty("initialState");
			BaseState state = m_InitialState.objectReferenceValue as BaseState;
			BaseState updatedState;

			GUILayout.Label("Initial State:", EditorStyles.boldLabel);
			updatedState = AIBehaviorsStatePopups.DrawEnabledStatePopup(fsm, state);
			if ( updatedState != state )
			{
				m_InitialState.objectReferenceValue = updatedState;
			}
		}


		void DrawGeneralAgentProperties()
		{
			SerializedProperty m_property;

			GUILayout.Label("General AI Properties:", EditorStyles.boldLabel);

			GUILayout.BeginVertical(GUI.skin.box);
			{
				const string aiTransformTooltip = "Root transform for the AI. If unassigned it will take this transform";
				m_property = m_Object.FindProperty("aiTransform");
				EditorGUILayout.PropertyField (m_property, new GUIContent ("AI Transform", aiTransformTooltip));
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical(GUI.skin.box);
			{
				m_property = m_Object.FindProperty("showDebugMessages");
				EditorGUILayout.PropertyField(m_property);
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical(GUI.skin.box);
			{
				m_property = m_Object.FindProperty("keepAgentUpright");
				EditorGUILayout.PropertyField(m_property);
			}
			GUILayout.EndVertical();
			
			EditorGUILayout.Separator();

			GUILayout.BeginVertical(GUI.skin.box);
			{
				const string raycastLayersText = "Visual Obstruction Layers";
				const string raycastLayersTooltip = "These are the layers that block the AI's view between them and the player. This would include items such as the general level geometry, walls, boxes or any other objects with colliders that are included in this layer mask.";

				m_property = m_Object.FindProperty("raycastLayers");
				EditorGUILayout.PropertyField(m_property, new GUIContent(raycastLayersText, raycastLayersTooltip));

				// Sight Falloff

				m_property = m_Object.FindProperty("useSightFalloff");
				EditorGUILayout.PropertyField(m_property);

				// Sight Distance

				m_property = m_Object.FindProperty("sightDistance");
				EditorGUILayout.PropertyField(m_property);

				if ( m_property.floatValue < 0.0f )
					m_property.floatValue = 0.0f;

				// Sight FOV

				m_property = m_Object.FindProperty("sightFOV");
				EditorGUILayout.PropertyField(m_property);

				if ( m_property.floatValue < 0.0f )
					m_property.floatValue = 0.0f;

				// Eye Transform
			
				m_property = m_Object.FindProperty("useSightTransform");
				EditorGUILayout.PropertyField(m_property);

				if ( m_property.boolValue )
				{
					m_property = m_Object.FindProperty("sightTransform");
					EditorGUILayout.PropertyField(m_property);
				}
				else
				{
					m_property = m_Object.FindProperty("eyePosition");
					EditorGUILayout.PropertyField(m_property);
				}

				EditorGUILayout.Separator();
			}
			GUILayout.EndVertical();

			EditorGUILayout.Separator();

			// Health
			
			GUILayout.BeginVertical(GUI.skin.box);
			{
				m_property = m_Object.FindProperty("health");
				EditorGUILayout.PropertyField(m_property);
			
				m_property = m_Object.FindProperty("maxHealth");
				EditorGUILayout.PropertyField(m_property);
			}
			GUILayout.EndVertical();

			EditorGUILayout.Separator();

			// Custom variables

			GUILayout.BeginVertical(GUI.skin.box);
			{
				GUILayout.Label("AI Behavior Variables");

				m_property = m_Object.FindProperty("userVariables");

				// Add button
				if ( GUILayout.Button(styles.blankContent, styles.addStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
				{
					m_property.arraySize ++;
				}

				// Show variables
				SerializedProperty property;
				for (int i = 0; i < m_property.arraySize; i++)
				{
					GUILayout.BeginVertical(GUI.skin.box);
					{
						property = m_property.GetArrayElementAtIndex(i);
						EditorGUILayout.PropertyField(property.FindPropertyRelative("name"), new GUIContent("Name:"));
						SerializedProperty typeProp = property.FindPropertyRelative("varType");
						EditorGUILayout.PropertyField(typeProp, new GUIContent("Type:"));
						SerializedProperty prop = property.FindPropertyRelative("value");
						if(typeProp.enumValueIndex == 0) // Float
						{
							float val = 0.0f;
							float.TryParse(prop.stringValue, out val);
							prop.stringValue = EditorGUILayout.FloatField("Value:", val).ToString();
						}
						else if (typeProp.enumValueIndex == 1) // Int
						{
							int val = 0;
							int.TryParse(prop.stringValue, out val);
							prop.stringValue = EditorGUILayout.IntField("Value:", val).ToString();
						}
						else if (typeProp.enumValueIndex == 2) // Bool
						{
							prop.stringValue = EditorGUILayout.Toggle("Value:", prop.stringValue.Equals("True")).ToString();
						}

						// Remove button
						if ( GUILayout.Button(styles.blankContent, styles.removeStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
						{
							m_property.DeleteArrayElementAtIndex(i);
						}
					}
					GUILayout.EndVertical();
				}
			}
			GUILayout.EndVertical();
		}


		void DrawAnimationCallbackSelection()
		{
			Component[] components = AIBehaviorsComponentInfoHelper.GetNonFSMComponents(fsm.gameObject);
			List<Component> filteredComponents = new List<Component>();
			Component animCallbackComp = m_Object.FindProperty("animationCallbackComponent").objectReferenceValue as Component;
			string currentMethodName = m_Object.FindProperty("animationCallbackMethodName").stringValue;
			List<string> componentNames = new List<string>();
			Dictionary<string, List<string>> methodsList = new Dictionary<string, List<string>>();
			int curComponentIndex = 0, newComponentIndex = 0;
			int curMethodIndex = 0, newMethodIndex = 0;

			GUILayout.BeginVertical (GUI.skin.box);

			GUILayout.Label("Animation Component Callback: ", EditorStyles.boldLabel);

			// Find the current component and potential components that can be a callback

			for ( int i = 0; i < components.Length; i++ )
			{
				Component comp = components[i];
				Type compType = comp.GetType();
				MethodInfo[] methods = compType.GetMethods();

				for ( int m = 0; m < methods.Length; m++ )
				{
					MethodInfo mi = methods[m];
					ParameterInfo[] parms = mi.GetParameters();

					if ( parms.Length == 1 && parms[0].ParameterType == typeof(AIAnimationState) )
					{
						string componentName = compType.ToString();

						if ( animCallbackComp != null && compType == animCallbackComp.GetType() )
						{
							curComponentIndex = componentNames.Count;
						}

						if ( !methodsList.ContainsKey(componentName) )
						{
							methodsList[componentName] = new List<string>();
						}

						if ( currentMethodName == mi.Name )
						{
							curMethodIndex = methodsList[componentName].Count;
						}

						filteredComponents.Add(comp);
						componentNames.Add(componentName);
						methodsList[componentName].Add(mi.Name);
					}
					else if ( animCallbackComp == comp )
					{
						animCallbackComp = null;
					}
				}
			}

			// If no component was found, show a code sample
			if ( componentNames.Count == 0 )
			{
				AIBehaviorsCodeSampleGUI.Draw(typeof(AIAnimationState), "animData", "OnAnimationState");
			}
			else
			{
				GUILayoutOption widthOption = GUILayout.MinWidth(75);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Component: ", widthOption);
				newComponentIndex = EditorGUILayout.Popup(curComponentIndex, componentNames.ToArray());
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Method: ", widthOption);
				newMethodIndex = EditorGUILayout.Popup(curMethodIndex, methodsList[componentNames[curComponentIndex]].ToArray());
				GUILayout.EndHorizontal();

				if ( curComponentIndex != newComponentIndex || animCallbackComp == null )
				{
					m_Object.FindProperty("animationCallbackComponent").objectReferenceValue = filteredComponents[newComponentIndex];
				}

				string curComponentName = componentNames[curComponentIndex];
				string curMethodName = m_Object.FindProperty("animationCallbackMethodName").stringValue;
				string methodListIndexName = methodsList[curComponentName][newMethodIndex];

				if ( curMethodName != methodListIndexName || currentMethodName == "" || currentMethodName == null )
				{
					m_Object.FindProperty("animationCallbackMethodName").stringValue = methodListIndexName;
				}
			}

			GUILayout.EndVertical();
		}


		void OnSceneGUI()
		{
			if ( curStateSelection != prevStateSelection )
				EditorUtility.SetDirty(fsm.GetStateByIndex(curStateSelection));

			AIBehaviorsTriggerGizmos.DrawVisionCone(fsm.GetSightPosition(), fsm.GetSightDirection(), fsm.sightDistance, fsm.sightFOV / 2.0f);
			AIBehaviorsTriggerGizmos.DrawGizmos(fsm, curStateSelection);
		}
	}
}
#endif