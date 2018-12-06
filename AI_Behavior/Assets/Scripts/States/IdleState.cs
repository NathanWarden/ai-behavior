using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AIBehavior
{
	public class IdleState : BaseState
	{
		public bool rotatesTowardTarget = false;
		private GameObject targetRotationObject = null;


		protected override void Init(AIBehaviors fsm)
		{
			Transform fsmTFM = fsm.aiTransform;
			Transform targetRotationTFM;

			targetRotationObject = new GameObject("RotationTarget");
			targetRotationTFM = targetRotationObject.transform;
			targetRotationTFM.position = fsmTFM.position + fsmTFM.forward;
			fsm.RotateAgent(targetRotationTFM, rotationSpeed);

			targetRotationTFM.parent = fsmTFM;

			fsm.PlayAudio();
		}


		protected override void StateEnded(AIBehaviors fsm)
		{
			if ( Application.isPlaying )
			{
				Destroy (targetRotationObject);
			}
			else
			{
				DestroyImmediate (targetRotationObject);
			}
		}


		protected override bool Reason(AIBehaviors fsm)
		{
			return true;
		}


		protected override void Action(AIBehaviors fsm)
		{
			if ( !rotatesTowardTarget )
			{
				fsm.MoveAgent(fsm.aiTransform, 0.0f, rotationSpeed);
			}
			else
			{
				Transform target = fsm.GetClosestPlayer(objectFinder.GetTransforms());

				if ( target != null )
				{
					fsm.MoveAgent(target, 0.0f, rotationSpeed);
				}
			}
		}


		public override bool RotatesTowardTarget()
		{
			return rotatesTowardTarget;
		}
		
		
		public override string DefaultDisplayName()
		{
			return "Idle";
		}


	#if UNITY_EDITOR
		// === Editor Methods === //

		public override void OnStateInspectorEnabled(SerializedObject m_ParentObject)
		{
		}


		protected override void DrawStateInspectorEditor(SerializedObject m_Object, AIBehaviors stateMachine)
		{
			GUILayout.BeginVertical(GUI.skin.box);
			AIBehaviorEditor.InspectorHelper.DrawInspector(m_Object);
			GUILayout.EndVertical();

			m_Object.ApplyModifiedProperties();
		}
	#endif
	}
}