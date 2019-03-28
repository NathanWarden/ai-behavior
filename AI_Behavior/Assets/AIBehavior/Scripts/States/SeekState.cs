using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif


namespace AIBehavior
{
	public class SeekState : BaseState
	{
		public bool specifySeekTarget = false;
		public Transform seekTarget = null;
		Vector3 targetPosition = Vector3.zero;

		public BaseState seekTargetReachedState;
		public BaseState noSeekTargetFoundState;
		public float distanceToTargetThreshold = 0.25f;
		private float sqrDistanceToTargetThreshold = 1.0f;
		public bool encircleTarget = false;
		Vector3 encircleOffset;

		public bool destroyTargetWhenReached = false;

		public bool getAsCloseAsPossible = false;
		public BaseState noMovementState;
		private NavMeshAgent navMeshAgent = null;
		private NavMeshPath navMeshPath = null;

		Vector3 previousPosition = Vector3.zero;
		float previousNoMovementDistance = -1;

		float nextCheckHasMovement;
		const float checkHasMovementFrequency = 1.0f;


		protected override void Init(AIBehaviors fsm)
		{
			navMeshAgent = fsm.GetComponentInChildren<NavMeshAgent>();
			sqrDistanceToTargetThreshold = GetSquareDistanceThreshold();
			fsm.PlayAudio();

			getAsCloseAsPossible &= navMeshAgent != null;
			previousNoMovementDistance = -1;

			if ( getAsCloseAsPossible )
			{
				previousPosition = Vector3.one * Mathf.Infinity;
				navMeshPath = new NavMeshPath();
			}

			if ( seekTarget != null )
			{
				targetPosition = GetTargetPosition();
			}

			nextCheckHasMovement = Time.time + checkHasMovementFrequency;

			if ( encircleTarget )
			{
				float radians = Random.value * Mathf.PI * 2;

				encircleOffset = new Vector3(Mathf.Cos(radians), 0.0f, Mathf.Sin(radians)) * distanceToTargetThreshold;
			}
		}


		protected override void StateEnded(AIBehaviors fsm)
		{
			if ( !specifySeekTarget )
			{
				seekTarget = null;
			}
		}


		protected override bool Reason(AIBehaviors fsm)
		{
			if ( seekTarget == null )
			{
				Transform[] seekTfms = objectFinder.GetTransforms();
				Vector3 pos = fsm.aiTransform.position;
				Vector3 diff;
				float nearestItem = Mathf.Infinity;

				if ( seekTfms == null )
				{
					fsm.ChangeActiveState(noSeekTargetFoundState);
				}

				foreach ( Transform tfm in seekTfms )
				{
					float sqrMagnitude;

					diff = tfm.position - pos;
					sqrMagnitude = diff.sqrMagnitude;

					if ( sqrMagnitude < nearestItem )
					{
						seekTarget = tfm;
						nearestItem = sqrMagnitude;
						targetPosition = seekTarget.position + GetEncircleOffset();
					}
				}
			}
			else
			{
				float sqrDist = (fsm.aiTransform.position - targetPosition).sqrMagnitude;

				if ( sqrDist < sqrDistanceToTargetThreshold )
				{
					HandleTargetReached(fsm);
					return false;
				}
			}

			return true;
		}


		Vector3 GetTargetPosition()
		{
			return seekTarget.position + GetEncircleOffset();
		}


		Vector3 GetEncircleOffset()
		{
			return encircleTarget ? encircleOffset : Vector3.zero;
		}


		void HandleTargetReached(AIBehaviors fsm)
		{
			if ( destroyTargetWhenReached )
			{
				Destroy(seekTarget.gameObject);
			}

			fsm.ChangeActiveState(seekTargetReachedState);
		}


		protected override void Action(AIBehaviors fsm)
		{
			if ( seekTarget != null )
			{
				targetPosition = GetNextMovement(fsm) + GetEncircleOffset();

				fsm.MoveAgent(targetPosition, movementSpeed, rotationSpeed);

				if ( Time.time > nextCheckHasMovement && !HasMovement(fsm.aiTransform.position) )
				{
					fsm.ChangeActiveState(noMovementState);
				}
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


		public override Vector3 GetNextMovement (AIBehaviors fsm)
		{
			if ( seekTarget != null )
			{
				if ( getAsCloseAsPossible )
				{
					if ( navMeshPath.status == NavMeshPathStatus.PathInvalid )
					{
						RecalculatePath();
						return seekTarget.position;
					}
					else
					{
						Vector3[] corners = navMeshPath.corners;

						if ( corners.Length > 0 )
						{
							Vector3 target = corners[corners.Length-1];
							navMeshAgent.CalculatePath(seekTarget.position, navMeshPath);

							return target;
						}
						else
						{
							RecalculatePath();
							return seekTarget.position;
						}
					}
				}
				else
				{
					return GetTargetPosition();
				}
			}

			return base.GetNextMovement (fsm);
		}


		float checkInterval = 1f;
		float nextCheck = 0.0f;

		void RecalculatePath()
		{
			if ( Time.realtimeSinceStartup > nextCheck )
			{
				navMeshAgent.CalculatePath(seekTarget.position, navMeshPath);
				nextCheck = Time.realtimeSinceStartup + checkInterval;
			}
		}


		protected virtual float GetSquareDistanceThreshold ()
		{
			return distanceToTargetThreshold * distanceToTargetThreshold;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Seek";
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

			GUILayout.Label ("Seek Properties:", EditorStyles.boldLabel);
			
			GUILayout.BeginVertical(GUI.skin.box);

			property = stateObject.FindProperty("specifySeekTarget");
			EditorGUILayout.PropertyField(property);

			if ( property.boolValue )
			{
				property = stateObject.FindProperty("seekTarget");
				EditorGUILayout.PropertyField(property);
			}

			EditorGUILayout.Separator();
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("No Seek Target Transition:");
				property = stateObject.FindProperty("noSeekTargetFoundState");
				property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(fsm, property.objectReferenceValue as BaseState);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("No Movement Transition:");
				property = stateObject.FindProperty("noMovementState");
				property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(fsm, property.objectReferenceValue as BaseState);
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.Separator();
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Seek Target Reached Transition:");
				property = stateObject.FindProperty("seekTargetReachedState");
				property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(fsm, property.objectReferenceValue as BaseState);
			}
			GUILayout.EndHorizontal();

			property = stateObject.FindProperty("distanceToTargetThreshold");
			float prevValue = property.floatValue;
			EditorGUILayout.PropertyField(property);

			if ( property.floatValue <= 0.0f )
				property.floatValue = prevValue;

			property = stateObject.FindProperty("encircleTarget");
			EditorGUILayout.PropertyField(property);

			property = stateObject.FindProperty("destroyTargetWhenReached");
			EditorGUILayout.PropertyField(property);

			property = stateObject.FindProperty("getAsCloseAsPossible");
			EditorGUILayout.PropertyField(property);

			if ( property.boolValue )
			{
				if ( fsm.GetComponentInChildren<NavMeshAgent>() == null )
				{
					EditorGUILayout.HelpBox("This trigger only works with an AI that uses a NavMeshAgent", MessageType.Warning);
				}
			}

			GUILayout.EndVertical();

			stateObject.ApplyModifiedProperties();

			if ( Application.isPlaying )
			{
				GUILayout.Label ("Debug:", EditorStyles.boldLabel);

				GUILayout.BeginVertical(GUI.skin.box);
				if ( seekTarget != null )
				{
					GUILayout.Label("Distance to target: " + (transform.position - targetPosition).magnitude.ToString(), EditorStyles.boldLabel);
				}
				else
				{
					GUILayout.Label("No Seek Target Found", EditorStyles.boldLabel);
				}
				GUILayout.EndVertical();
			}
		}
	#endif
	}
}