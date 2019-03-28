using UnityEngine;
using System.Collections;

namespace AIBehaviorExamples
{
	public class MoveUpAndDown : MonoBehaviour
	{
		Transform thisTfm;
		Vector3 initialPosition;


		void Start ()
		{
			thisTfm = transform;
			initialPosition = thisTfm.position;
		}

		
		void Update ()
		{
			Vector3 position = initialPosition;

			position.y += Mathf.Sin(Time.time);

			thisTfm.position = position;
		}
	}
}