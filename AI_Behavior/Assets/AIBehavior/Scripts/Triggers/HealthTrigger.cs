using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AIBehavior
{
	public abstract class HealthTrigger : BaseTrigger
	{
		public float healthThreshold = 50.0f;
		

		protected override bool Evaluate(AIBehaviors fsm)
		{
			return IsThresholdCrossed(fsm);
		}


		public abstract bool IsThresholdCrossed(AIBehaviors fsm);


		#if UNITY_EDITOR
		public override void DrawInspectorProperties(AIBehaviors fsm, SerializedObject sObject)
		{
			SerializedProperty prop = sObject.FindProperty("healthThreshold");
			EditorGUILayout.PropertyField(prop, new GUIContent(GetDescriptionText(), GetToolTipText()));
		}


		protected abstract string GetDescriptionText();
		protected abstract string GetToolTipText();
		#endif
	}
}