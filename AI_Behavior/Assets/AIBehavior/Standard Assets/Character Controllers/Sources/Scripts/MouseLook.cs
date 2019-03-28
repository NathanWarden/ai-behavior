using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
namespace AIBehaviorExamples
{
	[AddComponentMenu("Camera-Control/Mouse Look")]
	public class MouseLook : MonoBehaviour
	{
		public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
		public RotationAxes axes = RotationAxes.MouseXAndY;
		public bool useJoysticks = false;
		public float sensitivityX = 15F;
		public float sensitivityY = 15F;

		public float minimumX = -360F;
		public float maximumX = 360F;

		public float minimumY = -60F;
		public float maximumY = 60F;

		float rotationY = 0F;
		float joyX = 0;

		void Awake()
		{
	#if !UNITY_EDITOR
			Cursor.lockState = CursorLockMode.Locked;
	#endif
		}


		void Update ()
		{
			joyX = GetJoystickX() * 2;

			if (axes == RotationAxes.MouseXAndY)
			{
				float rotationX = transform.localEulerAngles.y + (Input.GetAxis("Mouse X") + GetJoystickX()) * sensitivityX;

				rotationY += Input.GetAxis("Mouse Y") * sensitivityY + GetJoystickY();
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
				
				transform.localEulerAngles = new Vector3(-rotationY, rotationX + joyX, 0);
			}
			else if (axes == RotationAxes.MouseX)
			{
				transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX + joyX, 0);
			}
			else
			{
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY + GetJoystickY();
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

				transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
			}
		}
		
		
		private float GetJoystickX()
		{
			return GetJoystick("Joystick X");
		}
		
		
		private float GetJoystickY()
		{
			return GetJoystick("Joystick Y");
		}

		
		private float GetJoystick(string axisName)
		{
			if ( useJoysticks )
			{
				return Input.GetAxis(axisName);
			}
			
			return 0.0f;
		}

		
		void Start ()
		{
			Rigidbody rb = GetComponent<Rigidbody>();

			// Make the rigid body not change rotation
			if (rb != null)
				rb.freezeRotation = true;
		}
	}
}