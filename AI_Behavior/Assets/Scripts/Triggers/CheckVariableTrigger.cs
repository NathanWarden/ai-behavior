using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif

namespace AIBehavior
{
	public class CheckVariableTrigger : BaseTrigger
	{
		public enum CheckCondition
		{
			LessThan,
			GreaterThan,
			EqualTo
		}
			
		public int varIndex = 0;
		public CheckCondition checkCondition = CheckCondition.LessThan;
		public int numValue = 1;
		public bool boolValue = false;


		protected override bool Evaluate (AIBehaviors fsm)
		{
			bool result = false;

			AiBehaviorVariable selectedVar = fsm.userVariables[varIndex];
			if(selectedVar != null)
			{
				if(selectedVar.IsFloat() || selectedVar.IsInteger())
				{
					result = CheckCount(selectedVar);
				}
				else if (selectedVar.IsBoolean())
				{
					result = selectedVar.GetBoolValue() == boolValue;
				}
			}

			return result;
		}
			
		public bool CheckCount(AiBehaviorVariable selectedVar)
		{
			float varValue = selectedVar.GetFloatValue(); // Using a float for ints also
			switch (checkCondition)
			{
			case CheckCondition.LessThan:
				return varValue < numValue;
			case CheckCondition.GreaterThan:
				return varValue > numValue;
			case CheckCondition.EqualTo:
				return varValue == numValue;
			default:
				return false;
			}
		}

		#if UNITY_EDITOR
		public override void DrawInspectorProperties(AIBehaviors fsm, SerializedObject sObject)
		{
			if (fsm.userVariables.Length == 0)
			{
				GUILayout.Box("No variables defined!");
				return;
			}

			EditorGUILayout.Separator();

			GUILayout.Label("Properties: ", EditorStyles.boldLabel);
			GUILayout.BeginVertical(GUI.skin.box);

			string[] varNames = fsm.GetVariableNames();
			varIndex = EditorGUILayout.Popup(varIndex, varNames);
			AiBehaviorVariable selectedVar = fsm.userVariables[varIndex];

			SerializedProperty property;

			// Float and Int
			if(selectedVar.IsFloat() || selectedVar.IsInteger())
			{
				property = sObject.FindProperty("checkCondition");
				EditorGUILayout.PropertyField(property);
				property = sObject.FindProperty("numValue");
				EditorGUILayout.PropertyField(property, new GUIContent("Value"));
			}
			else if(selectedVar.IsBoolean()) // Bool
			{
				property = sObject.FindProperty("boolValue");
				property.boolValue = EditorGUILayout.Toggle("Is: ", property.boolValue);
			}

			GUILayout.EndVertical();

			sObject.ApplyModifiedProperties();
		}
		#endif
	}
}