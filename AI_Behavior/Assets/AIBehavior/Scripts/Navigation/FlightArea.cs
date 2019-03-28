using UnityEngine;
using System.Collections;

namespace AIBehavior
{
	public class FlightArea : MonoBehaviour
	{
		private const float defaultSize = 10.0f;
		public Vector3 size = new Vector3(defaultSize, defaultSize, defaultSize);


		public void OnDrawGizmosSelected()
		{
			Color color = Color.cyan;

			color.a = 0.25f;
			Gizmos.color = color;

			Gizmos.DrawCube (transform.position, size);
		}
	}
}