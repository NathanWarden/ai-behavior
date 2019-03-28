using UnityEngine;
using AIBehavior;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using AIBehaviorEditor;
#endif


namespace AIBehavior
{
	public class SpeedExceededTrigger : BaseTrigger
	{
		public float speedThreshold = 1.0f;
		public float maxDistanceFromAI = 10.0f;
		public bool setTransformAsSeekTarget = false;
		public SeekState seekState = null;

		Dictionary<Transform, Vector3> transformPreviousPositions = new Dictionary<Transform, Vector3>();
		float previousCheckTime = 0.0f;


		float timeTotal = 0.0f;


		protected override void Init(AIBehaviors fsm)
		{
			transformPreviousPositions = new Dictionary<Transform, Vector3>();
			previousCheckTime = Time.time;
		}


		protected override bool Evaluate(AIBehaviors fsm)
		{
			float time = Time.time;
			Transform target;

			if ( SpeedExceededForTarget(out target, time) )
			{
				// Set target to seek state if bool checked
				if ( setTransformAsSeekTarget )
				{
					seekState.specifySeekTarget = true;
					seekState.seekTarget = target;
				}
				return true;
			}

			timeTotal += time - previousCheckTime;
			previousCheckTime = time;

			return false;
		}


		public bool SpeedExceededForTarget(out Transform target, float time)
		{
			Transform[] tfms = objectFinder.GetTransforms();
			float sqrSpeedThreshold = speedThreshold * speedThreshold;
			float sqrMaxDistanceFromAI = maxDistanceFromAI * maxDistanceFromAI;
			float timeDiff = time - previousCheckTime;
			float sqrTimeDiff = timeDiff * timeDiff;

			foreach ( Transform tfm in tfms )
			{
				Vector3 curPosition = tfm.position;

				if ( transformPreviousPositions.ContainsKey(tfm) )
				{

					// Is the object within the max distanceFromAI?
					if ( (curPosition - transform.position).sqrMagnitude < sqrMaxDistanceFromAI )
					{
						Vector3 diffVector = curPosition - transformPreviousPositions[tfm];

						// Is the objects speed greater than the minimum speed
						if ( diffVector.sqrMagnitude / sqrTimeDiff > sqrSpeedThreshold )
						{
							target = tfm;
							return true;
						}
					}
				}

				transformPreviousPositions[tfm] = curPosition;
			}

			target = null;
			return false;
		}		
		
		public override string DefaultDisplayName()
		{
			return "Speed Exceeded";
		}


	#if UNITY_EDITOR
		public override void DrawInspectorProperties(AIBehaviors fsm, SerializedObject triggerObject)
		{
			SerializedProperty property;

			property = triggerObject.FindProperty("speedThreshold");
			EditorGUILayout.PropertyField(property);

			GUILayout.BeginHorizontal();
			{
				property = triggerObject.FindProperty("maxDistanceFromAI");
				EditorGUILayout.PropertyField(property);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				property = triggerObject.FindProperty("setTransformAsSeekTarget");
				EditorGUILayout.PropertyField(property);

				if ( property.boolValue )
				{
					property = triggerObject.FindProperty("seekState");
					property.objectReferenceValue = AIBehaviorsStatePopups.DrawEnabledStatePopup<SeekState>(fsm, property.objectReferenceValue as SeekState);
				}
			}
			GUILayout.EndHorizontal();
		}
	#endif
	}
}