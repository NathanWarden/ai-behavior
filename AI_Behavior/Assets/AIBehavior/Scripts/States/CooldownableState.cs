using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif

namespace AIBehavior
{
	public abstract class CooldownableState : BaseState
	{
		// === Cooldown Properties === //
		
		public float cooldownTime = 0.0f;
		public bool startCooldownOnStateEnd = true;

		private float cooledDownTime = 0.0f;
		

		public override void InitState(AIBehaviors fsm)
		{
			base.InitState(fsm);

			if (!startCooldownOnStateEnd) 
			{
				TriggerCooldown ();
			}
		}


		protected override void StateEnded (AIBehaviors fsm)
		{
			if (startCooldownOnStateEnd) 
			{
				TriggerCooldown ();
			}
		}
		

		protected void TriggerCooldown()
		{
			cooledDownTime = Time.time + cooldownTime;
		}
		
		
		public bool CoolDownFinished ()
		{
			return cooledDownTime < Time.time;
		}


		public override bool CanSwitchToState ()
		{
			return CoolDownFinished ();
		}


		public override void HandleAction (AIBehaviors fsm)
		{
			base.HandleAction (fsm);
		}


		public float GetRemainingCooldownTime()
		{
			float time = cooledDownTime - Time.time;

			if (time < 0)
				time = 0;
			
			return time;
		}
		

		#if UNITY_EDITOR
		protected override void DrawFoldouts (UnityEditor.SerializedObject m_Object, AIBehaviors fsm)
		{
			base.DrawFoldouts (m_Object, fsm);

			if ( DrawFoldout("cooldownFoldout", "Cooldown Properties:") )
			{
				DrawCooldownProperties(m_Object, fsm);
			}
			
			EditorGUILayout.Separator();
		}
		
		
		void DrawCooldownProperties(SerializedObject m_State, AIBehaviors fsm)
		{
			SerializedProperty m_Property;

			GUILayout.BeginVertical(GUI.skin.box);

			m_Property = m_State.FindProperty("cooldownTime");
			EditorGUILayout.PropertyField(m_Property);

			m_Property = m_State.FindProperty("startCooldownOnStateEnd");
			EditorGUILayout.PropertyField(m_Property);

			float remainingTime = GetRemainingCooldownTime ();
			if (remainingTime > 0)
				EditorGUILayout.LabelField ("Remaining cooldown time: " + remainingTime.ToString("F2") + " seconds");

			m_State.ApplyModifiedProperties();

			GUILayout.EndVertical();
		}
		#endif
	}
}