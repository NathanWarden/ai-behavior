using UnityEngine;

namespace AIBehavior
{
	public struct AttackData
	{
		public AIBehaviors fsm;
		public AttackState attackState;
		public Transform target;
		public float damage { get { return attackState.attackDamage; } }
		public float plusOrMinusDamage { get { return attackState.plusOrMinusDamage; } }


		public AttackData(AIBehaviors aiBehaviors, AttackState attackState, Transform target)
		{
			this.fsm = aiBehaviors;
			this.attackState = attackState;
			this.target = target;
		}
	}
}