using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace AIBehavior
{
	public class MecanimNavMeshPathScript : MonoBehaviour
	{
		/// <summary>
		/// This is a reference to the Animator component you wish to target. If none is specified, it will automatically try and find one within this MonoBehaviour or its' children.
		/// </summary>
		public Animator mecanimAnimator = null;

		/// <summary>
		/// This needs to be the name as the Speed variable within your mecanim AnimationController.
		/// </summary>
		public string speedVariable = "Speed";

		/// <summary>
		/// This needs to be the name as the Direction variable within your mecanim AnimationController.
		/// </summary>
		public string directionVariable = "Direction";

		/// <summary>
		/// This is how fast the mecanim Direction variable will be lerped.
		/// </summary>
		public float rotationLerpRate = 5.0f;

		/// <summary>
		/// This is the minimum distance the agent needs to be from its' destination before it starts moving.
		/// </summary>
		public float minDistanceMoveThreshold = 3.0f;

		private Transform mecanimTransform;
		private NavMeshPath navMeshPath;
		private float movementSpeed, rotationSpeed;
		private float h, v;


		// Use this for initialization
		void Start ()
		{
			AIBehaviors ai = GetComponent<AIBehaviors>();

			ai.externalMove = OnNewDestination;
			navMeshPath = new NavMeshPath();

			if ( mecanimAnimator == null )
			{
				mecanimAnimator = GetComponentInChildren<Animator>();
			}

			mecanimTransform = ai.aiTransform;
		}


		void OnNewDestination(Vector3 targetPoint, float movementSpeed, float rotationSpeed)
		{
			this.movementSpeed = movementSpeed;
			this.rotationSpeed = rotationSpeed;

			NavMesh.CalculatePath(mecanimTransform.position, targetPoint, 255, navMeshPath);
		}


		void Update()
		{
			UpdateVerticalAndHorizontalMovement();

			mecanimAnimator.SetFloat(speedVariable, v * movementSpeed);
			mecanimAnimator.SetFloat(directionVariable, h * rotationSpeed);
		}


		void UpdateVerticalAndHorizontalMovement()
		{
			Vector3 offsetVector = GetCharacterOffsetVector();
			bool shouldTurn = ShouldTurn(offsetVector);
			float targetLerpH = 0.0f;

			if ( shouldTurn )
			{
				targetLerpH = GetTurnValue(offsetVector);
			}

			h = Mathf.SmoothStep(h, targetLerpH, Time.deltaTime * rotationLerpRate);
			v = Mathf.Lerp(0.2f, 1.0f, 1.0f - Mathf.Abs (h));
		}


		Vector3 GetCharacterOffsetVector()
		{
			if ( navMeshPath.corners.Length > 1 )
			{
				curPoint = 1;
				return mecanimTransform.InverseTransformPoint(navMeshPath.corners[1]);
			}

			if ( navMeshPath.corners.Length > 0 )
			{
				curPoint = 0;
				return mecanimTransform.InverseTransformPoint(navMeshPath.corners[0]);
			}

			return mecanimTransform.position;
		}


		bool ShouldMoveForward(Vector3 offsetVector)
		{
			return offsetVector.sqrMagnitude > minDistanceMoveThreshold * minDistanceMoveThreshold;
		}


		bool ShouldTurn(Vector3 offsetVector)
		{
			return offsetVector.normalized.z < 0.99f;
		}


		float GetTurnValue (Vector3 offsetVector)
		{
			return offsetVector.normalized.x;
		}


		int curPoint = 0;

		void OnDrawGizmos()
		{
			if ( navMeshPath != null )
			{
				for ( int i = 0; i < navMeshPath.corners.Length; i++ )
				{
					Gizmos.color = curPoint == i ? Color.red : Color.white;
					Gizmos.DrawCube(navMeshPath.corners[i], Vector3.one);
				}
			}
		}
	}
}