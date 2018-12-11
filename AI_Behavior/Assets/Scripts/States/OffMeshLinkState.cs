using UnityEngine;
using System.Collections;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif

namespace AIBehavior
{
	public class OffMeshLinkState : BaseState
	{
		public float jumpDelay = 0.0f;
		public bool returnToPreviousState = false;
		public BaseState transitionState = null;

		float jumpTime = 0.0f;
		float jumpHeight = 0.0f;
		float jumpDuration = 0.0f;
		float fallDistance = 0.0f;
		float fallDuration = 0.0f;

		private bool hasLinkData = false;
		private bool isJumping = false;

		private Vector3 startPosition;
		private Vector3 endPosition;

		Transform fsmTransform = null;
		NavMeshAgent navMeshAgent = null;
		bool navMeshAgentEnabledState = false;

		private float gravity { get { return Physics.gravity.y; } }


		protected override void Init (AIBehaviors fsm)
		{
			fsmTransform = fsm.aiTransform;

			if (navMeshAgent == null)
			{
				navMeshAgent = fsm.GetComponent<NavMeshAgent>();
			}

			jumpTime = Time.time + jumpDelay;
			hasLinkData = false;
			isJumping = false;
		}


		protected override bool Reason(AIBehaviors fsm)
		{
			if (navMeshAgent == null)
			{
				if (returnToPreviousState) 
				{
					fsm.ChangeActiveState (fsm.previousState);
				} 
				else 
				{
					fsm.ChangeActiveState (transitionState);
				}
				return false;
			}
			else if ( !hasLinkData )
			{
				OffMeshLinkData offMeshLinkData = navMeshAgent.currentOffMeshLinkData;

				startPosition = fsmTransform.position;
				endPosition = offMeshLinkData.endPos + AddDistanceToEndPosition(offMeshLinkData);

				hasLinkData = true;

				navMeshAgentEnabledState = navMeshAgent.enabled;
				navMeshAgent.enabled = false;
			}

			return true;
		}


		Vector3 AddDistanceToEndPosition(OffMeshLinkData linkData)
		{
			Vector3 linkStartPosition = linkData.startPos;
			Vector3 linkEndPosition = linkData.endPos;
			Vector3 direction = (linkEndPosition-linkStartPosition);

			direction.y = 0.0f;
			direction = direction.normalized;

			return direction * (fsmTransform.position-linkStartPosition).magnitude;
		}


		protected override void Action(AIBehaviors fsm)
		{
			if (!isJumping && Time.time > jumpTime)
			{
				isJumping = true;
				StartCoroutine(HandleJump(startPosition, endPosition, fsm));
			}
		}


		IEnumerator HandleJump (Vector3 startLocation, Vector3 endLocation, AIBehaviors fsm)
		{
			jumpHeight = GetJumpHeight(startLocation, endLocation);
			jumpDuration = GetHeightDuration(jumpHeight);
			fallDistance = GetFallDistance(startLocation, endLocation);
			fallDuration = GetHeightDuration(fallDistance);

			transform.position = startLocation;

			StartCoroutine(DoHorizontal(startLocation, endLocation, jumpDuration + fallDuration));
			yield return StartCoroutine(DoJump(startLocation, endLocation, jumpDuration));
			yield return StartCoroutine(DoFall(startLocation.y + jumpHeight, endLocation, fallDuration));

			fsmTransform.position = endPosition;
			if (returnToPreviousState) 
			{
				fsm.ChangeActiveState (fsm.previousState);
			} 
			else 
			{
				fsm.ChangeActiveState (transitionState);
			}
		}


		float GetJumpHeight(Vector3 startLocation, Vector3 endLocation)
		{
			return GetPeakDistance(startLocation, endLocation) - startLocation.y;
		}


		float GetFallDistance(Vector3 startLocation, Vector3 endLocation)
		{
			return GetPeakDistance(startLocation, endLocation) - endLocation.y;
		}


		float GetPeakDistance(Vector3 startLocation, Vector3 endLocation)
		{
			float peakDistance;

			if ( startLocation.y > endLocation.y )
			{
				peakDistance = startLocation.y;
			}
			else
			{
				peakDistance = endLocation.y;
			}

			return peakDistance + 1;
		}


		float GetHeightDuration(float distance)
		{
			return Mathf.Sqrt(Mathf.Abs(distance/gravity));
		}


		IEnumerator DoHorizontal(Vector3 startLocation, Vector3 endLocation, float duration)
		{
			float startTime = Time.time;
			float endTime = startTime + duration;

			while ( Time.time < endTime )
			{
				float percentage = Mathf.InverseLerp(startTime, endTime, Time.time);
				Vector3 currentPosition = Vector3.Lerp(startLocation, endLocation, percentage);

				currentPosition.y = fsmTransform.position.y;
				fsmTransform.position = currentPosition;

				yield return null;
			}

			fsmTransform.position = endLocation;
		}


		IEnumerator DoJump(Vector3 startLocation, Vector3 endLocation, float duration)
		{
			float startTime = Time.time;
			float endTime = startTime + duration;
			float endY = -gravity * (duration * duration);

			while ( Time.time < endTime )
			{
				float currentTime = endTime - Time.time;
				Vector3 position = fsmTransform.position;

				position.y = startLocation.y + endY + gravity * (currentTime * currentTime);

				fsmTransform.position = position;

				yield return null;
			}
		}


		IEnumerator DoFall(float startY, Vector3 endLocation, float duration)
		{
			float startTime = Time.time;
			float endTime = startTime + duration;

			while ( Time.time < endTime )
			{
				float currentTime = Time.time - startTime;
				Vector3 position = fsmTransform.position;

				position.y = startY + gravity * (currentTime * currentTime);

				fsmTransform.position = position;

				yield return null;
			}

			fsmTransform.position = endLocation;
		}


		protected override void StateEnded (AIBehaviors fsm)
		{
			navMeshAgent.enabled = navMeshAgentEnabledState;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Off Mesh Link";
		}


#if UNITY_EDITOR
		protected override void DrawStateInspectorEditor (SerializedObject stateObject, AIBehaviors fsm)
		{
			SerializedProperty property;

			GUILayout.Label("Off Mesh Link State Properties: ", EditorStyles.boldLabel);

			GUILayout.BeginVertical(GUI.skin.box);

			//GameObject targetObject = (stateObject.targetObject as Component).gameObject;
			//AIBehaviors fsm = targetObject.transform.parent.GetComponent<AIBehaviors>();

			property = stateObject.FindProperty("jumpDelay");
			EditorGUILayout.PropertyField(property);

			property = stateObject.FindProperty("returnToPreviousState");
			EditorGUILayout.PropertyField(property);

			if (!property.boolValue) 
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Transition State: ");
				property = stateObject.FindProperty ("transitionState");
				property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup (fsm, property.objectReferenceValue as BaseState) as BaseState;
				GUILayout.EndHorizontal ();
			}

			GUILayout.EndVertical();
		}
#endif
	}
}