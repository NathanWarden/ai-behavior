using UnityEngine;
using System.Collections;

namespace AIBehaviorExamples
{
	public class CharacterMover : MonoBehaviour
	{
		public float gravity = 20.0F;
		private Vector3 moveDirection = Vector3.zero;
		private CharacterController controller;


		void Start ()
		{	
			controller = GetComponent<CharacterController>();
			if (controller == null)
				controller = GetComponentInParent<CharacterController> ();
		}


		void Update()
		{
			moveDirection.y -= gravity * Time.deltaTime;

			controller.Move(moveDirection * Time.deltaTime);
		}
	}
}