using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AIBehavior
{
	public class ChangeTagState : BaseState
	{
		public string newTag = "Untagged";


		protected override void Init(AIBehaviors fsm)
		{
			fsm.tag = newTag;
		}


		protected override void StateEnded(AIBehaviors fsm)
		{
		}


		protected override bool Reason(AIBehaviors fsm)
		{
			return true;
		}


		protected override void Action(AIBehaviors fsm)
		{
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Change Tag";
		}


	#if UNITY_EDITOR
		// === Editor Methods === //

		public override void OnStateInspectorEnabled(SerializedObject m_ParentObject)
		{
		}


		protected override void DrawStateInspectorEditor(SerializedObject m_Object, AIBehaviors stateMachine)
		{
			SerializedProperty m_property = m_Object.FindProperty("newTag");
			string resultTag;

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Change To Tag:");
				resultTag = EditorGUILayout.TagField(newTag);
			}
			GUILayout.EndHorizontal();

			if ( resultTag != newTag )
			{
				m_property.stringValue = resultTag;
			}

			m_Object.ApplyModifiedProperties();
		}
	#endif
	}
}