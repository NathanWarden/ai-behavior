using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AIBehavior
{
	public class ChangeState : BaseState
	{
		public GameObject changeInto;


		protected override void Init(AIBehaviors fsm)
		{
			fsm.PlayAudio();
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
			Transform tfm = fsm.aiTransform;
			Vector3 position = tfm.position;
			Quaternion rotation = tfm.rotation;

			Instantiate(changeInto, position, rotation);
			Destroy(fsm.gameObject);
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Change";
		}


	#if UNITY_EDITOR
		// === Editor Methods === //

		public override void OnStateInspectorEnabled(SerializedObject m_ParentObject)
		{
		}


		protected override void DrawStateInspectorEditor(SerializedObject m_Object, AIBehaviors stateMachine)
		{
			SerializedObject m_State = new SerializedObject(this);
			SerializedProperty m_Prop;

			GUILayout.Label ("Change Properties:", EditorStyles.boldLabel);
			
			GUILayout.BeginVertical(GUI.skin.box);

			m_Prop = m_State.FindProperty("changeInto");
			EditorGUILayout.PropertyField(m_Prop);

			GUILayout.EndVertical();

			m_State.ApplyModifiedProperties();
		}
	#endif
	}
}