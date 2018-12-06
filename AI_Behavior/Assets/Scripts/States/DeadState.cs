using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif


namespace AIBehavior
{
	public class DeadState : BaseState
	{
		public bool destroyGameObject;
		public float destroyAfterTime = 0.0f;
		public bool destroyColliders;
		public bool destroyComponents;
		public Component[] componentsToDestroy;
		public bool changeTag;
		public string deadTag;

		private float destroyTime = 0.0f;


		protected override void Init(AIBehaviors fsm)
		{
			fsm.PlayAudio();
			fsm.MoveAgent(fsm.aiTransform, 0.0f, 0.0f);

			destroyTime = Time.time + destroyAfterTime;

			if (changeTag) 
			{
				fsm.tag = deadTag;
			}

			if (componentsToDestroy.Length > 0) 
			{
				DestroyComponents ();
			}

			if (destroyColliders) 
			{
				DestroyColliders (fsm);
			}
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
			if ( destroyGameObject && Time.time > destroyTime )
			{
				Destroy (fsm.gameObject);
			}
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Dead";
		}

		void DestroyColliders (AIBehaviors fsm)
		{
			Collider[] colliders = fsm.GetComponentsInChildren<Collider>();

			foreach ( Collider collider in colliders )
			{
				Destroy(collider);
			}
		}

		void DestroyComponents ()
		{
			for (int i = 0; i < componentsToDestroy.Length; i++) 
			{
				Destroy(componentsToDestroy [i]);
			}
		}

	#if UNITY_EDITOR
		// === Editor Methods === //

		public override void OnStateInspectorEnabled(SerializedObject m_ParentObject)
		{
		}


		protected override void DrawStateInspectorEditor(SerializedObject m_Object, AIBehaviors stateMachine)
		{
			SerializedProperty property;

			GUILayout.Label ("Dead Properties:", EditorStyles.boldLabel);
			
			GUILayout.BeginVertical(GUI.skin.box);

			//InspectorHelper.DrawInspector(m_Object);
			property = m_Object.FindProperty("destroyGameObject");
			EditorGUILayout.PropertyField (property);
			if (property.boolValue) 
			{
				property = m_Object.FindProperty("destroyAfterTime");
				EditorGUILayout.PropertyField (property);
			}

			property = m_Object.FindProperty("destroyColliders");
			EditorGUILayout.PropertyField (property);

			property = m_Object.FindProperty("destroyComponents");
			EditorGUILayout.PropertyField (property);
			if (property.boolValue) 
			{
				//property = m_Object.FindProperty("componentsToDestroy");
				InspectorHelper.DrawArray (m_Object, "componentsToDestroy");
				//EditorGUILayout.PropertyField (property);
			}

			property = m_Object.FindProperty("changeTag");
			EditorGUILayout.PropertyField (property);
			if (property.boolValue) 
			{
				property = m_Object.FindProperty("deadTag");
				EditorGUILayout.PropertyField (property);
			}
				
			GUILayout.EndVertical();

			m_Object.ApplyModifiedProperties();
		}
	#endif
	}
}