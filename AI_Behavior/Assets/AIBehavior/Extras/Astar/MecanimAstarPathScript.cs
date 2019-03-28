#if USE_ASTAR
using UnityEngine;
using Pathfinding;
using System.Collections.Generic;


namespace AIBehavior
{
	public class MecanimAstarPathScript : MonoBehaviour
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
		private float movementSpeed, rotationSpeed;
		private float h, v;

		private Seeker seeker;
		List<Vector3> corners = new List<Vector3>();


		// Use this for initialization
		void Start ()
		{
			AIBehaviors ai = GetComponent<AIBehaviors>();

			seeker = GetComponent<Seeker>();

			ai.externalMove = OnNewDestination;

			if ( mecanimAnimator == null )
			{
				mecanimAnimator = GetComponentInChildren<Animator>();
			}

			mecanimTransform = mecanimAnimator.transform;
		}


		void OnNewDestination(Vector3 targetPoint, float movementSpeed, float rotationSpeed)
		{
			this.movementSpeed = movementSpeed;
			this.rotationSpeed = rotationSpeed;

			Debug.Log(targetPoint);

			seeker.StartPath(mecanimTransform.position, targetPoint, GotAstarPath);
		}


		void GotAstarPath(Path p)
		{
			corners = p.vectorPath;
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
			if ( corners.Count > 1 )
			{
				curPoint = 1;
				return mecanimTransform.InverseTransformPoint(corners[1]);
			}

			if ( corners.Count > 0 )
			{
				curPoint = 0;
				return mecanimTransform.InverseTransformPoint(corners[0]);
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
			for ( int i = 0; i < corners.Count; i++ )
			{
				Gizmos.color = curPoint == i ? Color.red : Color.white;
				Gizmos.DrawCube(corners[i], Vector3.one);
			}
		}
	}
}
#endif