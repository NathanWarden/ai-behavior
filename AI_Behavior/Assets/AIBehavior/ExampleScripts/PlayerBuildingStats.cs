using UnityEngine;
using System.Collections;

namespace AIBehaviorExamples
{
	public class PlayerBuildingStats : MonoBehaviour
	{
		public float health = 100.0f;


		public void SubtractHealth(float amount)
		{
			health -= amount;

			if ( health <= 0.0f )
			{
				health = 0.0f;
				Debug.LogWarning("Building destroyed!");
				StartCoroutine (DestroyBuilding ());
			}
			else
			{
				Debug.Log("Building health is now: " + health);
			}
		}

		public void Damage(float damage)
		{
			Debug.Log("Building hit with " + damage + " damage points");
			SubtractHealth(damage);
		}

		IEnumerator DestroyBuilding()
		{
			transform.tag = "Untagged";
			for (int i = 0; i < 100; i++) 
			{
				Shake (0.2f);
				yield return null;
				Shake (-0.2f);
				yield return null;
				GoDown (0.03f);
				yield return null;
			}
			Destroy (gameObject);
		}

		void Shake(float direction)
		{
			transform.position += Vector3.right * direction;
		}

		void GoDown(float speed)
		{
			transform.position += Vector3.down * speed;
		}
	}
}