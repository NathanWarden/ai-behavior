using UnityEngine;


namespace AIBehaviorExamples
{
	public class ProjectileCollider : MonoBehaviour
	{
		public float damage;
		bool shouldDestroy = false;

		void Update()
		{
			if ( shouldDestroy )
			{
				Destroy(gameObject);
			}
		}


		void OnTriggerEnter(Collider col)
		{
			if ( col.tag != "Player" )
				shouldDestroy = true;
		}


		void OnCollisionEnter()
		{
			shouldDestroy = true;
		}
	}
}