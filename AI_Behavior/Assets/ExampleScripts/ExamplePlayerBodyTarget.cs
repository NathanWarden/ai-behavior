using UnityEngine;


namespace AIBehaviorExamples
{
	public class ExamplePlayerBodyTarget : MonoBehaviour
	{
		public float multiplier = 1.0f;


		public void Damage(float damage)
		{
			Transform parent = transform.parent;

			damage *= multiplier;
			Debug.Log("Got hit with " + damage + " damage");

			if ( parent != null )
			{
				parent.SendMessageUpwards("Damage", damage);
			}
		}
	}
}