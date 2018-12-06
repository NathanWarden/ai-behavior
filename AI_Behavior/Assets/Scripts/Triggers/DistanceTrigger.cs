using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AIBehavior
{
	public abstract class DistanceTrigger : BaseTrigger
	{
		public float distanceThreshold = 1.0f;
		public DistanceNegotiation negotiationMode = DistanceNegotiation.Default;


		protected override bool Evaluate(AIBehaviors fsm)
		{
			Transform[] tfms = objectFinder.GetTransforms();
			bool result = false;

			if ( tfms.Length > 0 )
			{
				Vector3 thisTFMPos = fsm.aiTransform.position;
				float sqrDistanceThreshold = distanceThreshold * distanceThreshold;
				DistanceNegotiation mode = GetNegotiationMode();

				for ( int i = 0; i < tfms.Length; i++ )
				{
					Vector3 targetDir = tfms[i].position - thisTFMPos;
				
					if ( Compare(targetDir.sqrMagnitude, sqrDistanceThreshold) )
					{
						if ( mode == DistanceNegotiation.Any )
						{
							return true;
						}

						result = true;
					}
					else
					{
						if ( mode == DistanceNegotiation.All )
						{
							return false;
						}
					}
				}
			}
			else
			{
				result = ResultForNoTaggedObjectsFound();
			}

			return result;
		}


		public DistanceNegotiation GetNegotiationMode()
		{
			if ( negotiationMode == DistanceNegotiation.Default )
			{
				return GetDefaultNegotiationMode();
			}

			return negotiationMode;
		}


		protected abstract bool ResultForNoTaggedObjectsFound();
		protected abstract DistanceNegotiation GetDefaultNegotiationMode();
		protected abstract bool Compare(float sqrMagnitude, float sqrThreshold);


#if UNITY_EDITOR
		public override void DrawInspectorProperties(AIBehaviors fsm, SerializedObject sObject)
		{
			SerializedProperty distanceProperty = sObject.FindProperty("distanceThreshold");
			SerializedProperty negotiationModeProperty = sObject.FindProperty("negotiationMode");

			EditorGUILayout.PropertyField(distanceProperty, new GUIContent("Distance"));
			EditorGUILayout.PropertyField(negotiationModeProperty, new GUIContent("Check Mode"));
		}


		protected virtual Color GetGizmoColor()
		{
			return Color.red;
		}


		public override void DrawGizmos(AIBehaviors fsm)
		{
			Transform tfm = fsm.aiTransform;

			Handles.color = GetGizmoColor();

			distanceThreshold = Handles.RadiusHandle(tfm.rotation, tfm.position, distanceThreshold);
		}
#endif
	}
}