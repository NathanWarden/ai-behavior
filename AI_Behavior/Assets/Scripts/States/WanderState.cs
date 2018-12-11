using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AIBehavior
{
	public class WanderState : BaseState
	{
		public LayerMask floorLayers;
		public bool stayCloseToPoint;
		public Transform anchorPoint;
		public float maxDistanceFromAnchor;
		Vector3 targetPosition = Vector3.zero;

		const float sqrDistanceToTargetThreshold = 1.0f;

		Vector3 previousPosition = Vector3.zero;
		float previousNoMovementDistance = -1;
		float nextCheckHasMovement;
		const float checkHasMovementFrequency = 1.0f;

		protected override void Init(AIBehaviors fsm)
		{
			fsm.PlayAudio();

			previousNoMovementDistance = -1;

			targetPosition = GetTargetPoint(fsm);

			nextCheckHasMovement = Time.time + checkHasMovementFrequency;
		}


		protected override void StateEnded(AIBehaviors fsm)
		{

		}


		protected override bool Reason(AIBehaviors fsm)
		{
			return true;
		}


		void HandleTargetReached(AIBehaviors fsm)
		{
			targetPosition = GetTargetPoint(fsm);
		}


		protected override void Action(AIBehaviors fsm)
		{
			targetPosition = GetNextMovement(fsm);

			fsm.MoveAgent(targetPosition, movementSpeed, rotationSpeed);

			if (Time.time > nextCheckHasMovement && !HasMovement (fsm.aiTransform.position)) 
			{
				targetPosition = GetTargetPoint (fsm);
			}

			// Target reached
			if (Vector3.SqrMagnitude (transform.position - targetPosition) < sqrDistanceToTargetThreshold) 
			{
				HandleTargetReached (fsm);
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
			return targetPosition;
		}
			

		Vector3 GetTargetPoint(AIBehaviors fsm)
		{
			Vector3 sightPoint = fsm.sightTransform.position;
		
			Vector3 newTarget = transform.position;

			int tries = 10;
			float distance = 5.0f;

			// First trying to find a point to a bigger distance (5m) and if not finding any trying with 3m and finally 1m
			for (int i = 3; i > 0; i--) 
			{
				for (int j = tries; j > 0; j--) 
				{
					if (FindAPoint (sightPoint, distance, out newTarget)) 
					{
						return newTarget;
					}
				}
				distance -= 2;
			}
			// If finally not found a point rotate to another direction
			transform.Rotate(0, 90, 0);

			return newTarget;
		}

		bool FindAPoint(Vector3 start, float distance, out Vector3 newPoint)
		{
			Vector3 sightdirection = transform.forward * distance + Vector3.down;
			sightdirection += transform.right*Random.Range(-10,10);

			//Debug.DrawRay (start, sightdirection*10, Color.red, 5);
			RaycastHit hit;
			if(Physics.Raycast(start, sightdirection, out hit, 100))
			{
				if (floorLayers == (floorLayers | (1 << hit.transform.gameObject.layer))) 
				{
					// If exceeding the maxDistance from the anchorPoint, turn to another rotation
					if (Vector3.Distance(hit.point, anchorPoint.position) > maxDistanceFromAnchor) 
					{
						transform.Rotate(0, 90, 0);
						newPoint = transform.position;
						return false;
					}

					newPoint = hit.point;
					return true;
				} 
			}

			newPoint = transform.position;
			return false;
		}

		public override string DefaultDisplayName()
		{
			return "WanderState";
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

			GUILayout.Label ("Wander Properties:", EditorStyles.boldLabel);

			GUILayout.BeginVertical(GUI.skin.box);

			// Properties
			property = stateObject.FindProperty("floorLayers");
			EditorGUILayout.PropertyField (property);

			property = stateObject.FindProperty("stayCloseToPoint");
			EditorGUILayout.PropertyField (property);

			if (property.boolValue) 
			{
				property = stateObject.FindProperty("anchorPoint");
				EditorGUILayout.PropertyField (property);

				property = stateObject.FindProperty("maxDistanceFromAnchor");
				EditorGUILayout.PropertyField (property);
			}

			GUILayout.EndVertical();

			stateObject.ApplyModifiedProperties();

			if ( Application.isPlaying )
			{
				GUILayout.Label ("Debug:", EditorStyles.boldLabel);
				GUILayout.BeginVertical(GUI.skin.box);
				GUILayout.Label("Distance to target: " + (transform.position - targetPosition).magnitude.ToString(), EditorStyles.boldLabel);
				GUILayout.EndVertical();
			}
		}
		#endif
	}
}