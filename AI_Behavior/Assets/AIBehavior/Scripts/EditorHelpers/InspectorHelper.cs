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
	public static class InspectorHelper
	{
		public static void DrawInspector(SerializedObject sObject)
		{
			Object unityObject = sObject.targetObject as Object;
			Type type = unityObject.GetType();
			BindingFlags bindingFlags = AIBehaviorsComponentInfoHelper.standardBindingFlags;
			List<FieldInfo> fields = new List<FieldInfo>(type.GetFields(bindingFlags));
			Type baseStateType = typeof(BaseState);
			SerializedProperty property;
			bool hasFields;

			for (int i = 0; i < fields.Count; i++)
			{
				object[] hideInInspector = fields[i].GetCustomAttributes(typeof(HideInInspector), true);

				if (fields[i].DeclaringType != type || sObject.FindProperty(fields[i].Name) == null || hideInInspector.Length > 0)
				{
					fields.RemoveAt(i);
					i--;
				}
			}

			hasFields = fields.Count > 0;

			if (hasFields)
			{
				GUILayout.Label("Properties: ", EditorStyles.boldLabel);

				GUILayout.BeginVertical(GUI.skin.box);
			}

			for (int i = 0; i < fields.Count; i++)
			{
				FieldInfo field = fields[i];

				property = sObject.FindProperty(field.Name);

				if (property.isArray && property.propertyType != SerializedPropertyType.String)
				{
					DrawArray(sObject, field.Name);
				}
				else
				{
					bool isBaseState = field.FieldType == baseStateType;

					if (isBaseState)
					{
						GameObject targetObject = (sObject.targetObject as Component).gameObject;
						AIBehaviors fsm = targetObject.transform.parent.GetComponent<AIBehaviors>();

						GUILayout.BeginHorizontal();
						GUILayout.Label(field.Name + ": ");
						property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(fsm, property.objectReferenceValue as BaseState) as BaseState;
						GUILayout.EndHorizontal();
					}
					else
					{
						EditorGUILayout.PropertyField(property);
					}
				}
			}

			if (hasFields)
			{
				GUILayout.EndVertical();
			}
		}


		public static void DrawArray(SerializedObject sObject, string fieldName)
		{
			SerializedProperty arraySizeProperty = sObject.FindProperty(fieldName + ".Array.size");
			SerializedProperty arrayDataProperty;
			SerializedProperty prop;
			string arrayDataPropertyName = fieldName + ".Array.data[{0}]";
			string baseStateTypeString = (typeof(BaseState)).ToString();
			AIBehaviorsStyles styles = new AIBehaviorsStyles();

			prop = sObject.FindProperty(fieldName);

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label(fieldName.ToUpper() + ": ");
				EditorGUILayout.PropertyField(arraySizeProperty);
			}
			GUILayout.EndHorizontal();

			for ( int i = 0; i < prop.arraySize; i++ )
			{
				bool isBaseState;
				bool oldEnabled = GUI.enabled;

				GUILayout.BeginHorizontal();
				{
					arrayDataProperty = sObject.FindProperty(string.Format(arrayDataPropertyName, i));
					isBaseState = arrayDataProperty.type.Contains(baseStateTypeString);

					if ( isBaseState )
					{
						GameObject targetObject = (sObject.targetObject as Component).gameObject;
						AIBehaviors fsm = targetObject.transform.parent.GetComponent<AIBehaviors>();
						BaseState curState = arrayDataProperty.objectReferenceValue as BaseState;

						arrayDataProperty.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(fsm, curState) as BaseState;
					}
					else
					{
						EditorGUILayout.PropertyField(arrayDataProperty);
					}

					GUI.enabled = i > 0;
					if ( GUILayout.Button(styles.blankContent, styles.upStyle, GUILayout.MaxWidth(styles.arrowButtonWidths)) )
					{
						prop.MoveArrayElement(i, i-1);
					}
					GUI.enabled = oldEnabled;

					GUI.enabled = i < prop.arraySize - 1;
					if ( GUILayout.Button(styles.blankContent, styles.downStyle, GUILayout.MaxWidth(styles.arrowButtonWidths)) )
					{
						prop.MoveArrayElement(i, i+1);
					}
					GUI.enabled = oldEnabled;

					if ( GUILayout.Button(styles.blankContent, styles.addStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
					{
						prop.InsertArrayElementAtIndex(i);
					}
					GUI.enabled = oldEnabled;

					if ( GUILayout.Button(styles.blankContent, styles.removeStyle, GUILayout.MaxWidth(styles.addRemoveButtonWidths)) )
					{
						prop.DeleteArrayElementAtIndex(i);
					}
					GUI.enabled = oldEnabled;
				}
				GUILayout.EndHorizontal();
			}
		}
	}
}
#endif