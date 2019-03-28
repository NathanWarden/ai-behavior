#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using AIBehavior;


namespace AIBehaviorEditor
{
	public static class AIBehaviorsTriggersGUI
	{
		const string kArraySize = "triggers.Array.size";
		const string kArrayData = "triggers";

		const int subTriggersInsetAmount = 20;


		public static void Draw(SerializedObject m_Object, AIBehaviors fsm, string foldoutLabel, string foldoutValueKey)
		{
			string[] triggerTypeNames = AIBehaviorsComponentInfoHelper.GetTriggerTypeNames();
			string[] subTriggerTypeNames = AIBehaviorsComponentInfoHelper.GetTriggerTypeNames(true);
			SerializedProperty triggersProperty = m_Object.FindProperty("triggers");
			AIBehaviorsStyles styles = new AIBehaviorsStyles();
			bool foldoutValue = false;
			bool newFoldoutValue = false;
			
			EditorGUILayout.Separator();

			if ( EditorPrefs.HasKey(foldoutValueKey) )
			{
				foldoutValue = EditorPrefs.GetBool(foldoutValueKey);
			}

			newFoldoutValue = EditorGUILayout.Foldout(foldoutValue, foldoutLabel, EditorStyles.foldoutPreDrop);

			if ( foldoutValue != newFoldoutValue )
			{
				foldoutValue = newFoldoutValue;
				EditorPrefs.SetBool(foldoutValueKey, foldoutValue);
			}

			if ( !foldoutValue )
			{
				return;
			}

			int arraySize = triggersProperty.arraySize;
			bool arrayIndexRemoved;

			for ( int i = 0; i < arraySize; i++ )
			{
				GUILayout.BeginVertical(GUI.skin.box);
				{
					GUILayout.BeginHorizontal();
					{
						arrayIndexRemoved = DrawTriggerSelectionPopup(triggerTypeNames, ref triggersProperty, i);

						if ( !arrayIndexRemoved )
						{
							arrayIndexRemoved = DrawArrowsPlusAndMinus(i, arraySize, ref m_Object, ref triggersProperty, styles, "triggers", triggerTypeNames[0]);
						}
					}
					GUILayout.EndHorizontal();

					if ( arrayIndexRemoved )
					{
						arraySize--;
					}
					else
					{
						BaseTrigger baseTrigger = triggersProperty.GetArrayElementAtIndex(i).objectReferenceValue as BaseTrigger;

						if ( baseTrigger == null )
						{
							Debug.LogError("Null trigger deleted!");
							triggersProperty.DeleteArrayElementAtIndex(i);
						}
						else
						{
							baseTrigger.DrawInspectorGUI(fsm);

							DrawSubTriggers(triggersProperty.GetArrayElementAtIndex(i), styles, subTriggersInsetAmount, fsm, triggerTypeNames, subTriggerTypeNames);
							baseTrigger.DrawTransitionState(fsm);
						}
					}
				}
				GUILayout.EndVertical();
			}

			if ( GUILayout.Button(styles.blankContent, styles.addStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
			{
				Component result = null;

				for ( int i = 0; i < triggerTypeNames.Length; i++ )
				{
					result = ComponentHelper.AddComponentByName(fsm.statesGameObject, triggerTypeNames[i]);

					if ( result != null )
					{
						break;
					}
				}

				triggersProperty.arraySize++;
				triggersProperty.GetArrayElementAtIndex(arraySize).objectReferenceValue = result;
			}

			m_Object.ApplyModifiedProperties();
		}


		private static bool DrawTriggerSelectionPopup (string[] triggerTypeNames, ref SerializedProperty triggersProperty, int index)
		{
			BaseTrigger trigger = triggersProperty.GetArrayElementAtIndex(index).objectReferenceValue as BaseTrigger;
			int initialSelection = 0;
			int newSelection = 0;

			for ( int i = 0; i < triggerTypeNames.Length; i++ )
			{
				if ( trigger != null )
				{
					if ( trigger.GetType().Name == triggerTypeNames[i] )
					{
						initialSelection = i;
						break;
					}
				}
			}

			newSelection = EditorGUILayout.Popup(initialSelection, triggerTypeNames, EditorStyles.popup);

			if ( newSelection != initialSelection )
			{
				GameObject triggerObject = trigger.gameObject;
				BaseTrigger[] triggers = trigger.subTriggers;
				BaseTrigger newTrigger;

				#if UNITY_4_3
				Undo.RegisterFullObjectHierarchyUndo(triggerObject.transform.root.gameObject);
				#else
				Undo.RegisterFullObjectHierarchyUndo(triggerObject.transform.root.gameObject, "AddTrigger");
				#endif

				newTrigger = ComponentHelper.AddComponentByName(triggerObject, triggerTypeNames[newSelection]) as BaseTrigger;
				Object.DestroyImmediate(trigger, true);
				newTrigger.subTriggers = triggers;
				triggersProperty.GetArrayElementAtIndex(index).objectReferenceValue = newTrigger;

				return true;
			}

			return false;
		}


		private static bool DrawArrowsPlusAndMinus(int index, int arraySize, ref SerializedObject m_Object, ref SerializedProperty triggersProperty, AIBehaviorsStyles styles, string propertyName, string firstTriggerType)
		{
			SerializedProperty prop = triggersProperty.GetArrayElementAtIndex(index);

			if ( prop != null )
			{
				Object obj = prop.objectReferenceValue;

				if ( obj != null )
				{
					bool oldEnabled = GUI.enabled;

					GUI.enabled = index > 0;
					if ( GUILayout.Button(styles.blankContent, styles.upStyle, GUILayout.MaxWidth(styles.arrowButtonWidths)) )
					{
						triggersProperty.MoveArrayElement(index, index-1);
					}

					GUI.enabled = index < arraySize-1;
					if ( GUILayout.Button(styles.blankContent, styles.downStyle, GUILayout.MaxWidth(styles.arrowButtonWidths)) )
					{
						triggersProperty.MoveArrayElement(index, index+1);
					}

					GUI.enabled = true;
					if ( GUILayout.Button(styles.blankContent, styles.addStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
					{
						GameObject go = (m_Object.targetObject as Component).gameObject;

						triggersProperty.InsertArrayElementAtIndex(index);
						triggersProperty.GetArrayElementAtIndex(index+1).objectReferenceValue = ComponentHelper.AddComponentByName(go, firstTriggerType);
					}

					if ( GUILayout.Button(styles.blankContent, styles.removeStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
					{
						BaseTrigger baseTrigger = prop.objectReferenceValue as BaseTrigger;

						#if UNITY_4_3
						Undo.RegisterFullObjectHierarchyUndo(baseTrigger);
						#else
						Undo.RegisterFullObjectHierarchyUndo(baseTrigger, "RemovedTrigger");
						#endif

						AIBehaviorsAssignableObjectArray.RemoveObjectAtIndex(m_Object, index, propertyName);
						DestroyTriggers(baseTrigger.subTriggers);
						Object.DestroyImmediate(baseTrigger, true);

						return true;
					}

					GUI.enabled = oldEnabled;
				}
			}

			return false;
		}


		private static void DestroyTriggers(BaseTrigger[] subTriggers)
		{
			foreach ( BaseTrigger trigger in subTriggers )
			{
				DestroyTriggers(trigger.subTriggers);
				Object.DestroyImmediate(trigger, true);
			}
		}


		private static void DrawSubTriggers(SerializedProperty triggerProperty, AIBehaviorsStyles styles, int insetSpace, AIBehaviors fsm, string[] triggerTypeNames, string[] subTriggerTypeNames)
		{
			BaseTrigger trigger = triggerProperty.objectReferenceValue as BaseTrigger;
			SerializedObject triggerObject = new SerializedObject(triggerProperty.objectReferenceValue);
			SerializedProperty subTriggersProperty = triggerObject.FindProperty("subTriggers");

			if ( trigger.subTriggers.Length > 0 )
			{
				GUILayout.BeginVertical(GUI.skin.box);
				{
					int arraySize = trigger.subTriggers.Length;

					for ( int i = 0; i < arraySize; i++ )
					{
						bool removedAnElement = false;

						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(insetSpace);
							removedAnElement = DrawTriggerSelectionPopup(subTriggerTypeNames, ref subTriggersProperty, i);

							if ( !removedAnElement )
							{
								removedAnElement = DrawArrowsPlusAndMinus(i, arraySize, ref triggerObject, ref subTriggersProperty, styles, "subTriggers", triggerTypeNames[0]);
							}
						}
						GUILayout.EndHorizontal();

						if ( removedAnElement )
						{
							break;
						}

						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(insetSpace);

							GUILayout.BeginVertical();
							{
								trigger.subTriggers[i].DrawInspectorGUI(fsm);

								DrawSubTriggers(subTriggersProperty.GetArrayElementAtIndex(i), styles, insetSpace + subTriggersInsetAmount, fsm, triggerTypeNames, subTriggerTypeNames);
							}
							GUILayout.EndVertical();
						}
						GUILayout.EndHorizontal();
					}
				}
				GUILayout.EndVertical();
			}

			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(insetSpace);
				if ( GUILayout.Button(styles.blankContent, styles.addStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
				{
					GameObject go = trigger.gameObject;
					int arraySize = subTriggersProperty.arraySize;

					#if UNITY_4_3
					Undo.RegisterFullObjectHierarchyUndo(fsm.gameObject);
					#else
					Undo.RegisterFullObjectHierarchyUndo(fsm.gameObject, "AddSubTrigger");
					#endif

					subTriggersProperty.arraySize++;
					subTriggersProperty.GetArrayElementAtIndex(arraySize).objectReferenceValue = ComponentHelper.AddComponentByName(go, triggerTypeNames[0]);
				}
			}
			GUILayout.EndHorizontal();

			triggerObject.ApplyModifiedProperties();
		}
	}
}
#endif