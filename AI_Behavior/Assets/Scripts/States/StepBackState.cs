using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif


namespace AIBehavior
{
	public class StepBackState : BaseState
	{
		public BaseState nextState;
		private NavMeshAgent navMeshAgent = null;
		public float distance = 3.0f;
		public float stopThreshold = 0.1f;
		Vector3 targetPosition;
		Transform player;
		Vector3 direction;

		float nextCheckHasMovement;
		const float checkHasMovementFrequency = 1.0f;
		Vector3 previousPosition = Vector3.zero;
		float previousNoMovementDistance = -1;

		protected override void Init(AIBehaviors fsm)
		{
			navMeshAgent = fsm.GetComponentInChildren<NavMeshAgent>();
			fsm.PlayAudio();

			nextCheckHasMovement = Time.time + checkHasMovementFrequency;

			player = fsm.GetClosestPlayer(objectFinder.GetTransforms());

			targetPosition = Vector3.zero;
		}


		protected override void StateEnded(AIBehaviors fsm)
		{
			
		}


		protected override bool Reason(AIBehaviors fsm)
		{
			if ((fsm.aiTransform.position - targetPosition).magnitude < stopThreshold) 
			{
				fsm.ChangeActiveState (nextState);
				return false;
			}
			return true;
		}


		protected override void Action(AIBehaviors fsm)
		{
			if ( player != null )
			{
				// Rotate towards the target
				fsm.aiTransform.LookAt (new Vector3(player.position.x, transform.position.y, player.position.z));

				direction = (fsm.aiTransform.position - player.position).normalized;
				direction.y = 0.0f;
				targetPosition = player.position + (direction * distance);

				// Move back in the opposite direction
				if (navMeshAgent != null) 
				{
					navMeshAgent.updateRotation = false; // disable the automatic rotation
					fsm.MoveAgent (targetPosition, movementSpeed, 0.0f);
					navMeshAgent.updateRotation = true; //when no longer need to step back then go to normal
				} 
				else 
				{
					fsm.aiTransform.Translate(direction * Time.deltaTime * movementSpeed, Space.World);
				}
			}

			// No movement check
			if (Time.time > nextCheckHasMovement && !HasMovement (fsm.aiTransform.position)) 
			{
				fsm.ChangeActiveState (fsm.previousState);
			}

			previousPosition = fsm.aiTransform.position;
		}


		bool HasMovement(Vector3 currentPosition)
		{
			nextCheckHasMovement = Time.time + checkHasMovementFrequency;

			float movementDistance = (previousPosition - currentPosition).sqrMagnitude;

			if ( movementDistance < 0.001f * 0.001f )
			{
				if ( movementDistance <= previousNoMovementDistance )
				{
					return false;
				}

				previousNoMovementDistance = movementDistance;
			}

			return true;
		}
	

		public override string DefaultDisplayName()
		{
			return "StepBackState";
		}


		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(targetPosition, 0.5f);
		}


		#if UNITY_EDITOR
		// === Editor Methods === //

		public override void OnStateInspectorEnabled(SerializedObject m_ParentObject)
		{
		}


		protected override void DrawStateInspectorEditor(SerializedObject stateObject, AIBehaviors fsm)
		{
			SerializedProperty property;

			GUILayout.Label ("Step Back Properties:", EditorStyles.boldLabel);

			GUILayout.BeginVertical(GUI.skin.box);

			// Properties
			property = stateObject.FindProperty("distance");
			EditorGUILayout.PropertyField (property);

			property = stateObject.FindProperty("stopThreshold");
			EditorGUILayout.PropertyField (property);

			GUILayout.BeginHorizontal ();
			{
				GUILayout.Label ("Next State:");
				property = stateObject.FindProperty ("nextState");
				property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup (fsm, property.objectReferenceValue as BaseState);
			}
			GUILayout.EndHorizontal ();

			GUILayout.EndVertical();

			stateObject.ApplyModifiedProperties();

			if ( Application.isPlaying )
			{
				GUILayout.Label ("Debug:", EditorStyles.boldLabel);
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.Label("Distance to target: " + (fsm.aiTransform.position - targetPosition).magnitude.ToString(), EditorStyles.boldLabel);
				GUILayout.EndVertical();
			}
		}
		#endif
	}
}