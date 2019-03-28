using UnityEngine;

namespace AIBehaviorDemo
{
	public class RotationOscillator : MonoBehaviour
	{
		public Vector3 rotationVector;
		public float speed = 1.0f;
		public float timeOffset = 0.0f;

		private Quaternion initialRotation;


		void Awake()
		{
			initialRotation = transform.rotation;
		}


		void LateUpdate ()
		{
			transform.rotation = initialRotation;
			transform.Rotate(rotationVector * Mathf.Sin((Time.time + timeOffset) * speed));
		}
	}
}