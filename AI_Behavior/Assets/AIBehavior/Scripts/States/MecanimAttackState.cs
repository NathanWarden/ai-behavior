using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AIBehavior
{
	public class MecanimAttackState : AttackState
	{
		public Animator animator;
		public int mecanimLayerIndex = 0;
		float prevNormalizedTime = 0.0f;


		protected override void Awake()
		{
			base.Awake();

			if ( animator == null )
			{
				animator = transform.parent.GetComponentInChildren<Animator>();

				if ( animator == null )
				{
					Debug.LogWarning("An Animator component must be attached when using the " + this.GetType());
				}
			}
		}


		protected override void Init (AIBehaviors fsm)
		{
			prevNormalizedTime = 0.0f;
			base.Init(fsm);
		}


		protected override void HandleAnimationAttackMode (AIBehaviors fsm, Transform target)
		{
			if ( animator != null && scriptWithAttackMethod != null && !string.IsNullOrEmpty(methodName) )
			{
				AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(mecanimLayerIndex);
				int hash = Animator.StringToHash(animationStates[0].name);

				if ( hash.Equals(stateInfo.fullPathHash) || hash.Equals(stateInfo.shortNameHash) )
				{
					float curNormalizedTime = stateInfo.normalizedTime % 1.0f;

					if ( curNormalizedTime > attackPoint && prevNormalizedTime < attackPoint )
					{
						Attack(fsm, target);
					}

					prevNormalizedTime = curNormalizedTime;
				}
			}
		}


		protected override IEnumerator ChangeStateWhenAnimationFinished (AIBehaviors fsm)
		{
			BaseState currentAttackState = fsm.currentState;
			AnimatorStateInfo stateInfo;
			float curNormalizedTime;

			do 
			{
				yield return null;
				stateInfo = animator.GetCurrentAnimatorStateInfo (mecanimLayerIndex);
				curNormalizedTime = stateInfo.normalizedTime % 1.0f;
			} 
			while (curNormalizedTime < 0.95f || curNormalizedTime < attackPoint);
			
			if (fsm.currentState == currentAttackState) // Check if the state hasn't changed for some reason
				fsm.ChangeActiveState(reloadState);
		}


		protected override void StateEnded (AIBehaviors fsm)
		{
			base.StateEnded(fsm);
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Mecanim Attack";
		}


#if UNITY_EDITOR
		// === Editor Methods === //
		protected override void DrawStateInspectorEditor(SerializedObject m_State, AIBehaviors fsm)
		{
			SerializedProperty m_property = m_State.FindProperty("animator");

			EditorGUILayout.PropertyField(m_property);

			m_property = m_State.FindProperty("mecanimLayerIndex");
			EditorGUILayout.PropertyField(m_property);

			base.DrawStateInspectorEditor(m_State, fsm);
		}
#endif
	}
}