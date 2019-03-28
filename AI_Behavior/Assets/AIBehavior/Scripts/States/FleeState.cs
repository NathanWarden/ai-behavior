using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif


namespace AIBehavior
{
	public class FleeState : BaseState
	{
		public FleeMode fleeMode = FleeMode.AwayFromNearestTaggedObject;

		public string fleeTargetTag = "Untagged";
		public Transform fleeToTarget;
		public Vector3 fleeDirection;

		public BaseState flightEndedState;
		public float distanceToTargetThreshold = 1.0f;
		public float stopFleeDistance = 5.0f;

		private Transform currentTarget;
		private float sqrDistanceToTargetThreshold = 1.0f;
		private float sqrStopFleeDistance = 5.0f;
		private GameObject[] fleeToObjects = null;


		public enum FleeMode
		{
			NearestTaggedObject,
			FixedTarget,
			Direction,
			AwayFromNearestTaggedObject
		}


		protected override void Init(AIBehaviors fsm)
		{
			sqrDistanceToTargetThreshold = distanceToTargetThreshold * distanceToTargetThreshold;
			sqrStopFleeDistance = stopFleeDistance * stopFleeDistance;
			fsm.PlayAudio();
			if(fleeMode == FleeMode.NearestTaggedObject)
				fleeToObjects = GameObject.FindGameObjectsWithTag(fleeTargetTag);
		}

		protected override void StateEnded(AIBehaviors fsm) {}

		protected override bool Reason(AIBehaviors fsm)
		{
			float sqrDist;

			// Checks if the flight ended
			switch (fleeMode) 
			{
			case FleeMode.NearestTaggedObject:
			case FleeMode.FixedTarget:
				if (currentTarget != null) 
				{
					sqrDist = (fsm.aiTransform.position - currentTarget.position).sqrMagnitude;

					if ( sqrDist < sqrDistanceToTargetThreshold )
					{
						fsm.ChangeActiveState(flightEndedState);
						return false;
					}
				}

				break;

			case FleeMode.Direction:
			case FleeMode.AwayFromNearestTaggedObject:
				Vector3 nearestObjectPosition = fsm.GetClosestPlayer (objectFinder.GetTransforms ()).position;

				sqrDist = (fsm.aiTransform.position - nearestObjectPosition).sqrMagnitude;

				if ( sqrDist > sqrStopFleeDistance )
				{
					fsm.ChangeActiveState(flightEndedState);
					return false;
				}

				break;

			}

			return true;
		}


		protected override void Action(AIBehaviors fsm)
		{
			fsm.MoveAgent(GetNextMovement(fsm), movementSpeed, rotationSpeed);
		}


		public override Vector3 GetNextMovement (AIBehaviors fsm)
		{
			Vector3 result;

			switch ( fleeMode )
			{
			case FleeMode.NearestTaggedObject:
				float nearestSqrDistance = Mathf.Infinity;
				int targetIndex = -1;

				for ( int i = 0; i < fleeToObjects.Length; i++ )
				{
					Vector3 dist = fleeToObjects[i].transform.position - this.transform.position;

					if ( dist.sqrMagnitude < nearestSqrDistance )
					{
						nearestSqrDistance = dist.sqrMagnitude;
						targetIndex = i;
					}
				}

				if ( targetIndex != -1 )
				{
					currentTarget = fleeToObjects[targetIndex].transform;
					result = currentTarget.position;
				}
				else
				{
					result = base.GetNextMovement(fsm);
				}

				break;

			case FleeMode.FixedTarget:
				if ( fleeToTarget != null )
				{
					currentTarget = fleeToTarget;
					result = fleeToTarget.position;
				}
				else
				{
					Debug.LogWarning("Flee To Target isn't set for FleeState");
					result = base.GetNextMovement(fsm);
				}

				break;

			case FleeMode.Direction:
				result = fsm.aiTransform.position + fleeDirection * stopFleeDistance;
				break;

			case FleeMode.AwayFromNearestTaggedObject:
				Vector3 nearestObjectPosition = fsm.GetClosestPlayer(objectFinder.GetTransforms()).position;
				Vector3 fsmPosition = fsm.aiTransform.position;
				Vector3 direction = (fsmPosition - nearestObjectPosition).normalized * stopFleeDistance;
				
				result = fsmPosition + direction;
				break;

			default:
				result = base.GetNextMovement(fsm);
				break;
			}
			
			return result;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Flee";
		}


	#if UNITY_EDITOR
		// === Editor Methods === //

		public override void OnStateInspectorEnabled(SerializedObject m_ParentObject)
		{
		}


		protected override void DrawStateInspectorEditor(SerializedObject m_Object, AIBehaviors stateMachine)
		{
			SerializedObject m_State = new SerializedObject(this);
			SerializedProperty m_property;
			GUIContent distanceToTargetThresholdContent = new GUIContent("Distance to target threshold", "This distance should be greater than the stopping distance of the Nav Mesh Agent. Default=1");

			m_State.Update();

			GUILayout.Label("Flee Properties:", EditorStyles.boldLabel);
			GUILayout.BeginVertical(GUI.skin.box);

			m_property = m_State.FindProperty("fleeMode");
			EditorGUILayout.PropertyField(m_property);

			FleeMode fleeMode = (FleeMode)m_property.enumValueIndex;

			switch ( fleeMode )
			{
			case FleeMode.NearestTaggedObject:
				GUILayout.BeginHorizontal();
				{
					GUILayout.Label("Use nearest object with tag:");
					m_property = m_State.FindProperty("fleeTargetTag");
					m_property.stringValue = EditorGUILayout.TagField(m_property.stringValue);
				}
				GUILayout.EndHorizontal();
				m_property = m_State.FindProperty("distanceToTargetThreshold");
				EditorGUILayout.PropertyField(m_property, distanceToTargetThresholdContent);

				break;

			case FleeMode.FixedTarget:
				m_property = m_State.FindProperty("fleeToTarget");
				EditorGUILayout.PropertyField(m_property);
				m_property = m_State.FindProperty("distanceToTargetThreshold");
				EditorGUILayout.PropertyField(m_property, distanceToTargetThresholdContent);

				break;
				
			case FleeMode.Direction:
				m_property = m_State.FindProperty("fleeDirection");
				EditorGUILayout.PropertyField(m_property);
				m_property = m_State.FindProperty("stopFleeDistance");
				EditorGUILayout.PropertyField(m_property);

				break;

			case FleeMode.AwayFromNearestTaggedObject:
				m_property = m_State.FindProperty("stopFleeDistance");
				EditorGUILayout.PropertyField(m_property);
				
				break;
			}

			EditorGUILayout.Separator();
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Flight Ended Transition:");
				m_property = m_State.FindProperty("flightEndedState");
				m_property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(stateMachine, m_property.objectReferenceValue as BaseState);
			}
			GUILayout.EndHorizontal();

			// "distanceToTargetThreshold" and "stopFleeDistance" can't be less than 0
			m_property = m_State.FindProperty("distanceToTargetThreshold");
			if (m_property.floatValue < 0.0f)
				m_property.floatValue = 0.0f;
			m_property = m_State.FindProperty("stopFleeDistance");
			if (m_property.floatValue < 0.0f)
				m_property.floatValue = 0.0f;

			GUILayout.EndVertical();

			m_State.ApplyModifiedProperties();
		}
	#endif
	}
}