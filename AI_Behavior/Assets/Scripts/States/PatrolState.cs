using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif


namespace AIBehavior
{
	public class PatrolState : BaseState
	{
		Transform rotationHelper = null;

		public Transform patrolPointsGroup;
		protected Transform[] patrolPoints = new Transform[0];
		protected int currentPatrolPoint = 0;
		protected int patrolDirection = 1;

		public float pointDistanceThreshold = 0.5f;
		protected Vector3 previousPosition;

		public bool useAccurateCornering;
		public int corneringError = 10;

		public PatrolMode patrolMode = PatrolMode.Loop;
		public ContinuePatrolMode continuePatrolMode = ContinuePatrolMode.NearestNextNode;
		public BaseState patrolEndedState;


		public enum PatrolMode
		{
			Once,
			Loop,
			PingPong,
			Random
		}


		public enum ContinuePatrolMode
		{
			Reset,
			ContinuePrevious,
			NearestNode,
			NearestNextNode,
			Random
		}


		public PatrolState()
		{
			currentPatrolPoint = 0;
			patrolDirection = 1;
		}


		protected override void Awake()
		{
			base.Awake();

			previousPosition = transform.position;

			if ( isEnabled )
			{
				SortPatrolPoints();
			}
		}


		// === Public Methods === //

		public virtual void SetPatrolPoints(Transform patrolPointsGroup)
		{
			if ( patrolPointsGroup != null )
			{
				this.patrolPointsGroup = patrolPointsGroup;
				SortPatrolPoints();
				ResetPatrol();
			}
			else
			{
				patrolPoints = null;
			}
		}


		public virtual void SetPatrolPoints(Transform[] newPatrolPoints)
		{
			if ( newPatrolPoints.Length >= 2 )
			{
				this.patrolPoints = newPatrolPoints;
				ResetPatrol();
			}
			else
			{
				Debug.LogError("In order to set patrol points, the array must contain at least two points");
			}
		}


		public Transform GetCurrentPatrolTarget()
		{
			if ( currentPatrolPoint < 0 || currentPatrolPoint >= patrolPoints.Length )
			{
				return null;
			}

			return patrolPoints[currentPatrolPoint];
		}


		// === Private Methods === //

		void SortPatrolPoints()
		{
		    if ( patrolPointsGroup != null )
            {
    			Transform[] tfms = patrolPointsGroup.GetComponentsInChildren<Transform>();
    			int curIndex = 0;

    			patrolPoints = new Transform[tfms.Length - 1];

    			for ( int i = 0; i < tfms.Length; i++ )
    			{
    				if ( tfms[i] != patrolPointsGroup )
    				{
    					patrolPoints[curIndex] = tfms[i];
    					curIndex++;
    				}
    			}

    			for ( int i = 0; i < patrolPoints.Length; i++ )
    			{
    				for ( int j = i+1; j < patrolPoints.Length; j++ )
    				{
    					if ( patrolPoints[i].name.CompareTo(patrolPoints[j].name) > 0 )
    					{
    						Transform temp = patrolPoints[i];

    						patrolPoints[i] = patrolPoints[j];
    						patrolPoints[j] = temp;
    					}
    				}
    			}
    		}
		}


		protected override void Init(AIBehaviors fsm)
		{
			if ( patrolPointsGroup == null && (patrolPoints == null || patrolPoints.Length == 0) )
			{
				Debug.LogWarning("The variable 'patrolPointsGroup' is unassigned for the 'Patrol' state on " + fsm.name);
				patrolPoints = new Transform[] { fsm.aiTransform, fsm.aiTransform };
			}

			if ( patrolPoints == null )
			{
				SortPatrolPoints();
			}

			if ( rotationHelper == null )
			{
				rotationHelper = (new GameObject("RotationHelper")).transform;
				rotationHelper.parent = fsm.aiTransform;
				rotationHelper.localPosition = Vector3.forward;
			}

			switch ( continuePatrolMode )
			{
				case ContinuePatrolMode.Reset:
					patrolDirection = 1;
					currentPatrolPoint = 0;
					break;

				case ContinuePatrolMode.NearestNode:
				case ContinuePatrolMode.NearestNextNode:
					Vector3 thisPos = fsm.aiTransform.position;
					int nearestNode = 0;
					float nearestSqrMagnitude = Mathf.Infinity;

					for ( int i = 0; i < patrolPoints.Length; i++ )
					{
						float thisSqrMagnitude = (thisPos - patrolPoints[i].position).sqrMagnitude;

						if ( CheckIfWithinThreshold(thisPos, patrolPoints[i].position, nearestSqrMagnitude) )
						{
							nearestSqrMagnitude = thisSqrMagnitude;
							nearestNode = i;
						}
					}

					if ( continuePatrolMode == ContinuePatrolMode.NearestNode )
						currentPatrolPoint = nearestNode;
					else if ( patrolPoints.Length != 0 )
						currentPatrolPoint = (nearestNode + 1) % patrolPoints.Length;

					break;

				case ContinuePatrolMode.ContinuePrevious:
					break;

				case ContinuePatrolMode.Random:
					currentPatrolPoint = Random.Range(0, patrolPoints.Length);
					break;
			}

			fsm.RotateAgent(rotationHelper, rotationSpeed);

			fsm.PlayAudio();
		}


		protected override void StateEnded(AIBehaviors fsm)
		{
		}


		protected override bool Reason(AIBehaviors fsm)
		{
			if ( patrolMode == PatrolMode.Once )
			{
				// Is the patrol ended?
				if ( currentPatrolPoint >= patrolPoints.Length )
				{
					currentPatrolPoint = 0;
					fsm.ChangeActiveState(patrolEndedState);

					return false;
				}
			}

			return true;
		}


		protected override void Action(AIBehaviors fsm)
		{
			fsm.MoveAgent(GetNextMovement(fsm), movementSpeed, rotationSpeed);
		}


		public override Vector3 GetNextMovement (AIBehaviors fsm)
		{
			Vector3 targetPoint = patrolPoints[currentPatrolPoint].position;
			Vector3 thisPos = fsm.aiTransform.position;
			
			if ( CheckIfWithinThreshold(thisPos, targetPoint, GetSquarePointDistanceThreshold(pointDistanceThreshold) ) )
			{
				GetNextPatrolPoint();
			}

			return targetPoint;
		}


		protected virtual float GetSquarePointDistanceThreshold (float pointDistanceThreshold)
		{
			return pointDistanceThreshold * pointDistanceThreshold;
		}


		protected virtual bool CheckIfWithinThreshold (Vector3 currentPosition, Vector3 destination, float sqrDistanceThreshold)
		{
			float distance = (currentPosition - destination).sqrMagnitude;
			bool isWithinDistance = distance < sqrDistanceThreshold;
			bool result = isWithinDistance;

			if ( useAccurateCornering )
			{
				Vector3 velocity = transform.forward * Time.deltaTime * movementSpeed * corneringError;
				float nextDistance = (currentPosition + velocity - destination).sqrMagnitude;
				bool isPastPoint = distance < nextDistance;

				result &= isPastPoint;
			}

			return result;
		}


		protected void GetNextPatrolPoint ()
		{
			currentPatrolPoint += patrolDirection;
			
			// Check for once, loop, ping pong, clamp forever
			
			if ( patrolMode == PatrolMode.Loop )
			{
				currentPatrolPoint %= patrolPoints.Length;
			}
			else if ( patrolMode == PatrolMode.PingPong )
			{
				if ( patrolDirection == 1 && currentPatrolPoint == patrolPoints.Length )
				{
					currentPatrolPoint = patrolPoints.Length - 1;
					patrolDirection = -1;
				}
				else if ( currentPatrolPoint == 0 )
				{
					currentPatrolPoint = 0;
					patrolDirection = 1;
				}
			}
			else if ( patrolMode == PatrolMode.Random )
			{
				if ( patrolPoints.Length > 1 )
				{
					int newPoint = currentPatrolPoint;
					
					while ( newPoint == currentPatrolPoint )
					{
						newPoint = Random.Range(0, patrolPoints.Length);
					}
					
					currentPatrolPoint = newPoint;
				}
			}
		}


		public void ResetPatrol ()
		{
			currentPatrolPoint = -1;
			patrolDirection = 1;
			GetNextPatrolPoint();
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Patrol";
		}


	#if UNITY_EDITOR
		// === Editor Functions === //

		public override void OnStateInspectorEnabled(SerializedObject m_ParentObject)
		{
		}


		protected override void DrawStateInspectorEditor(SerializedObject m_Object, AIBehaviors stateMachine)
		{
			SerializedObject m_State = new SerializedObject(this);
			SerializedProperty m_property;

			m_State.Update();

			// === Handle other properties === //

			EditorGUILayout.Separator();

			GUILayout.Label ("Patrol Properties:", EditorStyles.boldLabel);
			
			GUILayout.BeginVertical(GUI.skin.box);
			GUILayout.Label("Movement Options:", EditorStyles.boldLabel);

			PatrolMode patrolMode;

			m_property = m_State.FindProperty("patrolMode");
			EditorGUILayout.PropertyField(m_property);
			patrolMode = (PatrolMode)m_property.enumValueIndex;

			m_property = m_State.FindProperty("continuePatrolMode");
			EditorGUILayout.PropertyField(m_property, new GUIContent("Continue Previous Patrol", "When this state is switched back to, will it continue where it left off?"));

			if ( patrolMode == PatrolState.PatrolMode.Once )
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Label("End Patrol Transition:");
					m_property = m_State.FindProperty("patrolEndedState");
					m_property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup(stateMachine, m_property.objectReferenceValue as BaseState);
				}
				GUILayout.EndHorizontal();
			}

			// Handle the patrol points

			if ( patrolPoints == null )
				patrolPoints = new Transform[0];

			EditorGUILayout.Separator();

			m_property = m_State.FindProperty("patrolPointsGroup");
			EditorGUILayout.PropertyField(m_property);

			m_property = m_State.FindProperty("pointDistanceThreshold");
			EditorGUILayout.PropertyField(m_property, new GUIContent("Distance Threshold"));
			
			if ( m_property.floatValue < 0.0f )
				m_property.floatValue = 0.0f;

			m_property = m_State.FindProperty("useAccurateCornering");
			EditorGUILayout.PropertyField(m_property, new GUIContent("Accurate Cornering"));

			if ( m_property.boolValue )
			{
				m_property = m_State.FindProperty("corneringError");
				EditorGUILayout.PropertyField(m_property, new GUIContent("Cornering Error"));

				if ( m_property.intValue <= 0 )
				{
					m_property.intValue = 1;
				}
			}

			GUILayout.EndVertical();

			m_State.ApplyModifiedProperties();
		}
	#endif
	}
}
