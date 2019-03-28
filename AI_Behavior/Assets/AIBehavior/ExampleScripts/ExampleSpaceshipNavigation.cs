using UnityEngine;
using System.Collections;
using AIBehavior;


namespace AIBehaviorExamples
{
	[RequireComponent(typeof(Rigidbody))]
	public class ExampleSpaceshipNavigation : MonoBehaviour
	{
		public float forceMultiplier = 10.0f;
		private float rotationMultiplier = 2.0f;

		AIBehaviors ai;
		Vector3 targetPoint = Vector3.forward;
		float targetSpeed;
		float rotationSpeed;


		// Use this for initialization
		void Start ()
		{
			ai = GetComponent<AIBehaviors>();
			ai.externalMove += OnGotNewDestination;

			targetPoint = transform.position;
		}


		void OnGotNewDestination(Vector3 targetPoint, float targetSpeed, float rotationSpeed)
		{
			this.targetPoint = targetPoint;
			this.targetSpeed = targetSpeed;
			this.rotationSpeed = rotationSpeed;
		}


		void Update()
		{
			Vector3 D = targetPoint - transform.position;
			Quaternion rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(D), (rotationSpeed / 360.0f) * rotationMultiplier * Time.deltaTime);
			transform.rotation = rot;
		}


		// Update is called once per frame
		void FixedUpdate ()
		{
			GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * forceMultiplier * targetSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
		}
	}
}