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
		public bool destroyGameObjects;
		public GameObject[] gameObjectsToDestroy;
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
				transform.parent.tag = deadTag;
			}

			if (destroyComponents)
			{
				DestroyObjects (componentsToDestroy);
			}

			if (destroyGameObjects)
			{
				DestroyObjects (gameObjectsToDestroy);
			}

			if (destroyColliders)
			{
				DestroyObjects (gameObject.transform.parent.GetComponentsInChildren<Collider>());
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

		void DestroyObjects (UnityEngine.Object[] objects)
		{
			for (int i = 0; i < objects.Length; i++)
			{
				Destroy(objects [i]);
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

			property = m_Object.FindProperty("destroyGameObject");
			EditorGUILayout.PropertyField (property, new GUIContent("Destroy This Object"));
			if (property.boolValue) 
			{
				property = m_Object.FindProperty("destroyAfterTime");
				EditorGUILayout.PropertyField (property);
			}

			property = m_Object.FindProperty("destroyColliders");
			EditorGUILayout.PropertyField (property);

			property = m_Object.FindProperty("destroyGameObjects");
			EditorGUILayout.PropertyField (property);
			if (property.boolValue)
			{
				InspectorHelper.DrawArray (m_Object, "gameObjectsToDestroy");
			}

			property = m_Object.FindProperty("destroyComponents");
			EditorGUILayout.PropertyField (property);
			if (property.boolValue)
			{
				InspectorHelper.DrawArray (m_Object, "componentsToDestroy");
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