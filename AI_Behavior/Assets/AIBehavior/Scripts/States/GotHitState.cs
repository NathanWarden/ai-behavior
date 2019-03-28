using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif


namespace AIBehavior
{
	public class GotHitState : CooldownableState
	{
		public float hitStateDuration = 1.0f;
		public bool returnToPreviousState = false;
		public BaseState changeToState;
		float endStateTime = 0.0f;


		protected override void Init(AIBehaviors fsm)
		{
			fsm.PlayAudio();

			endStateTime = Time.time + hitStateDuration;
		}


		protected override void StateEnded(AIBehaviors fsm) {}


		protected override bool Reason(AIBehaviors fsm)
		{
			if (Time.time > endStateTime) 
			{
				if (returnToPreviousState) 
				{
					fsm.ChangeActiveState (fsm.previousState);
				} 
				else if (changeToState != null) 
				{
					fsm.ChangeActiveState (changeToState);
				} 
				else 
				{
					Debug.LogWarning ("No state set to change into");
				}

				return false;
			}
			
			return true;
		}


		protected override void Action(AIBehaviors fsm)
		{
			fsm.MoveAgent(fsm.currentDestination, movementSpeed, rotationSpeed);
		}


		new public bool CoolDownFinished()
		{
			return base.CoolDownFinished();
		}


		public virtual bool CanGetHit(AIBehaviors fsm)
		{
			return !(fsm.currentState is DeadState);
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Got Hit";
		}

		
	#if UNITY_EDITOR
		// === Editor Methods === //

		public override void OnStateInspectorEnabled(SerializedObject m_ParentObject)
		{
		}


		protected override void DrawStateInspectorEditor(SerializedObject stateObject, AIBehaviors fsm)
		{
			SerializedProperty property;

			GUILayout.Label ("Got Hit Properties:", EditorStyles.boldLabel);
			GUILayout.BeginVertical(GUI.skin.box);

			property = stateObject.FindProperty("hitStateDuration");
			EditorGUILayout.PropertyField(property);

			property = stateObject.FindProperty("returnToPreviousState");
			EditorGUILayout.PropertyField(property);

			if (!property.boolValue) 
			{
				GUILayout.BeginHorizontal ();
				{
					GUILayout.Label ("Change To State:");
					property = stateObject.FindProperty ("changeToState");
					property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup (fsm, property.objectReferenceValue as BaseState);
				}
				GUILayout.EndHorizontal ();
			}

			GUILayout.EndVertical();
			stateObject.ApplyModifiedProperties();
		}
	#endif
	}
}