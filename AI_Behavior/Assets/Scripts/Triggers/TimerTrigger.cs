using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AIBehavior
{
	public class TimerTrigger : BaseTrigger
	{
		public float duration = 1.0f;
		public float plusOrMinusDuration = 0.0f;
		public ResetMode timerResetMode = ResetMode.OnStateEnter;
		private float timerExpiration = 0.0f;


		protected override void Awake()
		{
			ResetTimer();
		}


		protected override void Init(AIBehaviors fsm)
		{
			if ( timerResetMode == ResetMode.OnStateEnter )
			{
				ResetTimer();
			}
		}


		protected override bool Evaluate(AIBehaviors fsm)
		{
			return DidTimeExpire(Time.time);
		}


		protected override void OnTriggered()
		{
			if ( timerResetMode == ResetMode.WhenTriggered )
			{
				ResetTimer();
			}
		}


		public void ResetTimer()
		{
			ResetTimer(Time.time, duration + (Random.value * plusOrMinusDuration - Random.value * plusOrMinusDuration));
		}


		public void ResetTimer(float currentTime, float duration)
		{
			timerExpiration = currentTime + duration;
		}


		public bool DidTimeExpire(float currentTime)
		{
			return currentTime > timerExpiration;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Timer";
		}
	}
}