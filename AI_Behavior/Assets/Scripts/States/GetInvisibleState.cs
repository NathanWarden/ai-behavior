using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif


namespace AIBehavior
{
	public class GetInvisibleState : CooldownableState
	{
		public bool returnToPreviousState = false;
		public BaseState goToState;
		public float duration = 1.0f;
		public bool disableColliders = false;

		protected Renderer[] aiRenderers;
		protected Collider[] aiColliders;


		protected override void Init(AIBehaviors fsm)
		{
			fsm.PlayAudio();

			aiRenderers = transform.root.GetComponentsInChildren<Renderer> ();
			aiColliders = transform.root.GetComponentsInChildren<Collider> ();

			foreach (Renderer aiRenderer in aiRenderers) 
			{
				aiRenderer.enabled = false;
			}

			if (disableColliders) 
			{
				foreach (Collider col in aiColliders) 
				{
					col.enabled = false;
				}
			}

			if (returnToPreviousState && fsm.previousState != null)
				fsm.ChangeActiveState (fsm.previousState);
			else 
				fsm.ChangeActiveState (goToState);

			StartCoroutine(EndInvisibility(fsm));
		}


		protected override void StateEnded(AIBehaviors fsm)
		{
			base.StateEnded (fsm);
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
			return "Invisible";
		}


		protected IEnumerator EndInvisibility(AIBehaviors fsm)
		{
			yield return new WaitForSeconds (duration);

			foreach (Renderer aiRenderer in aiRenderers) 
			{
				aiRenderer.enabled = true;
			}

			if (disableColliders) 
			{
				foreach (Collider col in aiColliders) 
				{
					col.enabled = true;
				}
			}

			StateEnded (fsm);
		}


	#if UNITY_EDITOR
		// === Editor Methods === //

		public override void OnStateInspectorEnabled(SerializedObject m_ParentObject)
		{
		}


		protected override void DrawStateInspectorEditor(SerializedObject stateObject, AIBehaviors fsm)
		{
			SerializedProperty property;

			GUILayout.Label ("Invisibility Properties:", EditorStyles.boldLabel);

			GUILayout.BeginVertical(GUI.skin.box);

			property = stateObject.FindProperty("duration");
			EditorGUILayout.PropertyField(property);

			property = stateObject.FindProperty("disableColliders");
			EditorGUILayout.PropertyField(property);

			property = stateObject.FindProperty("returnToPreviousState");
			EditorGUILayout.PropertyField(property);

			if (!property.boolValue) 
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Go to state:");
				property = stateObject.FindProperty("goToState");
				property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(fsm, property.objectReferenceValue as BaseState);
				GUILayout.EndHorizontal();
			}

			GUILayout.EndVertical();

			stateObject.ApplyModifiedProperties();
		}
	#endif
	}
}