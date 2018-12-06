using UnityEngine;


namespace AIBehaviorExamples
{
	public class PlayerStats : MonoBehaviour
	{
		public float health = 100.0f;


		public void SubtractHealth(float amount)
		{
			health -= amount;

			if ( health <= 0.0f )
			{
				health = 0.0f;
				Debug.LogWarning("You're Dead!");
			}
			else
			{
				Debug.Log("Health is now: " + health);
			}
		}

		public void Damage(float damage)
		{
			Debug.Log("Got hit with " + damage + " damage points");
			SubtractHealth(damage);
		}
	}
}