using UnityEngine;
using AIBehavior;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif


namespace AIBehaviorDemo
{
	public class DemoDoneState : BaseState
	{
		public GameManager gameManager;


		protected override void Init(AIBehaviors fsm)
		{
			if ( gameManager == null )
			{
				gameManager = Object.FindObjectOfType(typeof(GameManager)) as GameManager;
			}

			gameManager.GameOver();
		}


		protected override void StateEnded(AIBehaviors fsm)
		{
		}


		protected override bool Reason(AIBehaviors fsm)
		{
			return true;
		}


		protected override void Action(AIBehaviors fsm)
		{
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Demo Done";
		}


		#if UNITY_EDITOR
		// === Editor Methods === //

		public override void OnStateInspectorEnabled(SerializedObject m_ParentObject)
		{
		}


		protected override void DrawStateInspectorEditor(SerializedObject m_Object, AIBehaviors stateMachine)
		{
			InspectorHelper.DrawInspector(m_Object);
			m_Object.ApplyModifiedProperties();
		}
		#endif
	}
}