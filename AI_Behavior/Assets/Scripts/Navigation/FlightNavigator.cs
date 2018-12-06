using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AIBehavior
{
	public class FlightNavigator : MonoBehaviour
	{
		public FlightNavigation navigation;
		public AIBehaviors ai;
		public float speed = 1.0f;
		public float rotationSpeed = 90.0f;
		public Vector3 center = Vector3.zero;
		public float height = 0.0f;
		public float radius = 0.5f;
		public Axis axis = Axis.y;
		public float stoppingDistance = 0.25f;
		public bool flying = false;

		Vector3[] path = new Vector3[0];
		Vector3[] rotationTargetPoints = new Vector3[0];
		int pathIndex = 0;
		Vector3 previousDestination;
		Quaternion startRotation;
		Quaternion endRotation;
		Vector3 currentRotationTarget = Vector3.zero;

		public int maxFailureCount = 10;
		protected int failureCount = 0;

		float startMoveTime = 0.0f;
		float endMoveTime = 0.0f;
		float startRotationTime = 0.0f;
		float endRotationTime = 0.0f;

		public enum Axis { x, y, z }

		Transform checkTransform;


		void Awake ()
		{
			checkTransform = new GameObject ("CheckTransform").transform;
			previousDestination = transform.position;
			Setup ();
		}


		void Setup()
		{
			if (ai == null)
			{
				ai = GetComponent<AIBehaviors> ();
			}

			if (navigation == null)
			{
				navigation = Object.FindObjectOfType<FlightNavigation> ();
			}

			if (ai != null && navigation != null)
			{
				ai.externalMove = OnPathChanged;
			}
		}


		void Update()
		{
			if ( flying && CheckIfStillFlying () )
			{
				float weight = Mathf.InverseLerp (startMoveTime, endMoveTime, Time.time);
				transform.position = Vector3.Lerp (path[pathIndex-1], path[pathIndex], weight);
				RotateTowardDestination ();
			}
		}


		bool CheckIfStillFlying()
		{
			if ( pathIndex < path.Length )
			{
				float weight = Mathf.InverseLerp (startMoveTime, endMoveTime, Time.time);

				if ( weight + Mathf.Epsilon >= 1.0f )
				{
					pathIndex++;
					flying = pathIndex < path.Length;

					if ( flying )
					{
						UpdateMoveTimes ();
						UpdateRotationTimes ();
					}
				}
			}
			else
			{
				flying = false;
			}

			return flying;
		}


		void UpdateMoveTimes()
		{
			if ( Time.time > endMoveTime )
			{
				Vector3 previousPoint = path [pathIndex - 1];
				Vector3 currentPoint = path [pathIndex];
				float distance = (previousPoint - currentPoint).magnitude;
				float travelTime = distance / speed;

				startMoveTime = Time.time;
				endMoveTime = startMoveTime + travelTime;
			}
		}


		void UpdateRotationTimes()
		{
			float angle, rotateTime;
			float percentage = Mathf.InverseLerp (0, path.Length, pathIndex);
			int index = Mathf.Clamp(Mathf.FloorToInt (percentage * rotationTargetPoints.Length), 1, rotationTargetPoints.Length);
			Vector3 newRotationTarget = rotationTargetPoints [index];

			if ( !currentRotationTarget.Equals(newRotationTarget) )
			{
				currentRotationTarget = newRotationTarget;

				startRotation = transform.rotation;
				transform.LookAt (rotationTargetPoints[index]);
				endRotation = transform.rotation;
				transform.rotation = startRotation;

				angle = Quaternion.Angle (startRotation, endRotation);
				rotateTime = angle / rotationSpeed;

				startRotationTime = Time.time;
				endRotationTime = startRotationTime + rotateTime;
			}
		}


		void RotateTowardDestination()
		{
			float weight = Mathf.InverseLerp (startRotationTime, endRotationTime, Time.time);
			transform.rotation = Quaternion.Lerp (startRotation, endRotation, weight);
		}


		void OnPathChanged(Vector3 targetPosition, float targetSpeed, float rotationSpeed)
		{
			speed = targetSpeed;
			this.rotationSpeed = rotationSpeed;
			SetDestination (targetPosition);
		}


		public void SetDestination(Vector3 targetPosition)
		{
			if ( (targetPosition-previousDestination).sqrMagnitude > Mathf.Epsilon )
			{
				previousDestination = targetPosition;
				navigation.GetPath (transform.position, targetPosition, OnGotPath, OnGotPathFailed);
			}
		}


		void OnGotPath(Vector3[] newPath)
		{
			if ( CheckPath(newPath) )
			{
				SetPath (newPath);
			}
			else
			{
				failureCount++;
				StartCoroutine (GetNewPath(previousDestination));
			}
		}


		public void SetPath(Vector3[] newPath)
		{
			path = newPath;
			rotationTargetPoints = GetRotationPath(newPath);
			pathIndex = 0;
			flying = true;
			failureCount = 0;
			endMoveTime = 0.0f;
			startMoveTime = -1.0f;
		}


		bool CheckPath(Vector3[] newPath)
		{
			for ( int i = 0; i < newPath.Length-1; i++ )
			{
				Vector3 pointA = newPath [i];
				Vector3 pointB = newPath [i + 1];

				checkTransform.position = pointA;
				checkTransform.LookAt (pointB);

				if ( Physics.CapsuleCast(GetCap(checkTransform, false), GetCap(checkTransform, true), radius, checkTransform.forward, (pointA-pointB).magnitude) )
				{
					return false;
				}
			}

			return true;
		}


		IEnumerator GetNewPath(Vector3 target)
		{
			if ( failureCount > maxFailureCount )
			{
				failureCount = 0;
				yield return null;
			}

			if ( !flying )
			{
				previousDestination = previousDestination + Vector3.up;
				SetDestination (target);
			}
		}


		Vector3[] GetRotationPath(Vector3[] newPath)
		{
			List<Vector3> path = new List<Vector3> ();
			int index = Mathf.RoundToInt(Mathf.Lerp (0, newPath.Length, 0.5f));

			path.Add (newPath[0]);
			path.Add (newPath[index]);
			path.Add (newPath[newPath.Length-1]);

			return path.ToArray();
		}


		void OnGotPathFailed(Vector3 attemptedDestination)
		{
		}


		Vector3 GetCap(Transform positionTfm, bool reverseSide)
		{
			Vector3 capPosition = positionTfm.TransformPoint(center);
			Vector3 axisVector;
			float multiplier = reverseSide ? -1 : 1;

			switch ( axis )
			{
			case Axis.x:
				axisVector = positionTfm.right;
				break;
			case Axis.y:
				axisVector = positionTfm.up;
				break;
			default:
				axisVector = positionTfm.forward;
				break;
			}

			return capPosition + axisVector * (height / 2.0f) * multiplier;
		}


		public float GetFlightProgress()
		{
			return path.Length > 0 ? (float)pathIndex / path.Length : 0;
		}


		void OnDrawGizmosSelected()
		{
			if ( enabled )
			{
				Color color = Color.red;

				color.a = 0.25f;
				Gizmos.color = color;

				Gizmos.DrawSphere (GetCap(transform, false), radius);
				Gizmos.DrawSphere (GetCap(transform, true), radius);
			}
		}
	}
}