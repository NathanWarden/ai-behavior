using UnityEngine;

namespace AIBehaviorDemo
{
	public class Rotator : MonoBehaviour
	{
		public Vector3 rotationVector;


		void LateUpdate ()
		{
			transform.Rotate(rotationVector * Time.deltaTime);
		}
	}
}