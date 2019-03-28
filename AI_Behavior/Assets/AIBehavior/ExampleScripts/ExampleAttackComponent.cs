using UnityEngine;
using AIBehavior;


namespace AIBehaviorExamples
{
	public class ExampleAttackComponent : MonoBehaviour
	{
		public GameObject projectilePrefab;
		public Transform launchPointWeapon;
		public float aimMetersAboveTarget = 1.5f;


		public void MeleeAttack(AttackData attackData)
		{
			Debug.Log ("Melee attack");
			attackData.target.SendMessage("Damage", CalculateDamage(attackData));
		}


		public void RangedAttack(AttackData attackData)
		{
			if ( attackData.target != null )
			{
				launchPointWeapon.LookAt(attackData.target);

				GameObject projectile = GameObject.Instantiate(projectilePrefab, launchPointWeapon.position, launchPointWeapon.rotation) as GameObject;
				projectile.transform.LookAt (attackData.target.position + Vector3.up * aimMetersAboveTarget);
				ExampleProjectile projectileComponent = projectile.GetComponent<ExampleProjectile>();
				projectileComponent.attackData = attackData;
				Rigidbody rb = projectile.GetComponent<Rigidbody>();
				rb.AddForce(projectile.transform.forward * 1000);

				Debug.Log ("Attacked target '" + attackData.target.name + "' with attack state named '" + attackData.attackState.name + "' with damage " + attackData.damage);
			}
			else
			{
				Debug.LogWarning("attackData.target is null, you may want to have a NoPlayerInSight trigger on the AI '" + attackData.attackState.transform.parent.name + "'");
			}
		}

		float CalculateDamage(AttackData attackData)
		{
			float minDamage = attackData.damage - attackData.plusOrMinusDamage;
			float maxDamage = attackData.damage + attackData.plusOrMinusDamage;

			return Random.Range (minDamage, maxDamage);
		}
	}
}