using UnityEngine;
using AIBehavior;


namespace AIBehaviorExamples
{
	public class ExampleProjectile : MonoBehaviour
	{
		public GameObject explosionPrefab;
		public AttackData attackData;
		public float playerShootingDamage;

		void Awake()
		{
			Destroy(gameObject, 10.0f);
		}


		void OnTriggerEnter(Collider col)
		{
			SpawnFlames();
			if (col.tag == "Player")
				col.SendMessage ("Damage", CalculateDamage(attackData)); // Only taking the attackData.damage value if the shooter was an AI
			else
				col.SendMessage ("Damage", playerShootingDamage); // Other case take the playerShootingDamage.
		}


		void OnCollisionEnter(Collision col)
		{
			SpawnFlames();
		}


		void SpawnFlames()
		{
			Instantiate(explosionPrefab, transform.position, Quaternion.identity);
			Destroy(gameObject);
		}

		float CalculateDamage(AttackData attackData)
		{
			float minDamage = attackData.damage - attackData.plusOrMinusDamage;
			float maxDamage = attackData.damage + attackData.plusOrMinusDamage;

			return Random.Range (minDamage, maxDamage);
		}
	}
}